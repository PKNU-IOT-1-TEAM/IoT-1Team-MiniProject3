using UnityEngine;
using LibVLCSharp;
using UnityEngine.UI;
using System;

public class VLC_Player : MonoBehaviour
{
    public static LibVLC libVLC;
    public MediaPlayer mediaPlayer;

    //Assign a Canvas RawImage to render on a GUI object
    public RawImage screen;
    public AspectRatioFitter screenAspectRatioFitter;
    public Button playButton;
    public Button pauseButton;
    public Button closeButton;
    public Text title;

    Texture2D _vlcTexture = null;   //This is the texture libVLC writes to directly.
    public RenderTexture texture = null;    //We copy it into this texture which we actually use in unity.

    public MapPinInfo mapPinInfo;   // cctv 정보  // UIPanelManager.cs에서 설정함.

    public bool flipTextureX = true; //No particular reason you'd need this but it is sometimes useful
    public bool flipTextureY = true; //Set to false on Android, to true on Windows

    public bool playOnAwake = true;
    private bool _isPlaying = false;
    public bool logToConsole = false; //Log function calls and LibVLC logs to Unity console

    //Unity Awake, OnDestroy, and Update functions
    #region unity
    private void Awake()
    {
        //Setup LibVLC
        if (libVLC == null)
            CreateLibVLC();

        //Setup Screen
        if(screen == null)
            screen = GetComponent<RawImage>();

        //Setup Media Player
        CreateMediaPlayer();


        //VLC Event Handlers
        mediaPlayer.Playing += (object sender, EventArgs e) =>
        {
            try
            {
                _isPlaying = true;//Switch to the Pause button next update
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex.ToString());
            }
        };
        mediaPlayer.Paused += (object sender, EventArgs e) => {
            try
            {
                _isPlaying = false;//Switch to the Play button next update
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex.ToString());
            }
        };

        //Buttons
        pauseButton.onClick.AddListener(() => { Pause(); });
        playButton.onClick.AddListener(() => { Play(); });
        closeButton.onClick.AddListener(() => { close(); });

        if(mapPinInfo != null)
        {
            title.text = mapPinInfo.locationName;
            // Play On Start
            Open(mapPinInfo.cctvURL);
            // pause/playButton 초기 설정
            UpdatePlayPauseButton(playOnAwake); 
        }
        else
        {
            Debug.Log("mapPinInfo 못받음");
        }
    }

    private void OnDestroy()
    {
        //Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
        DestroyMediaPlayer();
    }

    void Update()
    {

        //Get size every frame
        uint height = 0;
        uint width = 0;
        mediaPlayer.Size(0, ref width, ref height); // 가로, 세로 크기 가져옴.

        //Automatically resize output textures if size changes
        if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
        {
            ResizeOutputTextures(width, height);
        }

        if (_vlcTexture != null)
        {
            //Update the vlc texture (tex)
            var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
            if (updated)
            {
                _vlcTexture.UpdateExternalTexture(texptr);

                //Copy the vlc texture into the output texture, flipped over
                var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
                Graphics.Blit(_vlcTexture, texture, flip, Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
            }
        }

        if (texture != null)
            screenAspectRatioFitter.aspectRatio = (float)texture.width / texture.height;

        UpdatePlayPauseButton(_isPlaying);
    }

    #endregion

    // UI관련 함수
    #region
    
    // 패널창 닫기
    private void close()
    {
        Debug.Log("닫기 버튼 클릭");
        // 패널 을 EventManager를 통해 보냄
        EventManager.Instance.ResponsePanelDestruction(mapPinInfo);
        Destroy(gameObject);
    }

    // 재생/일시정지 버튼 활성, 비활성
    private void UpdatePlayPauseButton(bool playing)
    {
        pauseButton.gameObject.SetActive(playing);
        playButton.gameObject.SetActive(!playing); ;
    }

    #endregion


    //Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.
    #region vlc

    private void Open(string cctvURL)
    {
        Log("VLCPlayer Open");
        if(mediaPlayer.Media != null)
            mediaPlayer.Media.Dispose();

        mediaPlayer.Media = new Media(new Uri(cctvURL));
        Play();
    }

    private void Play()
    {
        Log("VLCPlayerExample Play");

        mediaPlayer.Play();
    }

    public void Pause()
    {
        Log("VLCPlayer Pause");
        mediaPlayer.Pause();
    }

    public void Stop()
    {
        Log("VLCPlayer Stop");
        mediaPlayer?.Stop();

        _vlcTexture = null;
        texture = null;
    }

    public bool IsPlaying
    {
        get
        {
            if(mediaPlayer == null) 
                return false;
            return mediaPlayer.IsPlaying;
        }
    }
    
    // 필요한가??
    public long Duration
    {
        get
        {
            if (mediaPlayer == null || mediaPlayer.Media == null)
                return 0;
            return mediaPlayer.Media.Duration;
        }
    }
    // 필요한가??
    public long Time
    {
        get
        {
            if (mediaPlayer == null)
                return 0;
            return mediaPlayer.Time;
        }
    }

    // 필요할까?
    public VideoOrientation? GetVideoOrientation()
    {
        var tracks = mediaPlayer?.Tracks(TrackType.Video);

        if (tracks == null || tracks.Count == 0)
            return null;

        var orientation = tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

        return orientation; ;
    }

    #endregion


    //Private functions create and destroy VLC objects and textures
    #region
    //Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
    private void CreateLibVLC()
    {
        Log("VLCPlayerExample CreateLibVLC");
        //Dispose of the old libVLC if necessary
        if (libVLC != null)
        {
            libVLC.Dispose();
            libVLC = null;
        }

        Core.Initialize(Application.dataPath);  // Load VLC dells
        libVLC = new LibVLC(enableDebugLogs: true); // false로 바꾸자

        //Setup Error Logging
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        libVLC.Log += (s, e) =>
        {
            //Always use try/catch in LibVLC events.
            //LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
            try
            {
                if (logToConsole)
                {
                    Log(e.FormattedLog);
                }
            }
            catch (Exception ex)
            {
                Log("Exception caught in libVLC.Log: \n" + ex.ToString());
            }

        };
    }

    //Create a new MediaPlayer object and dispose of the old one. 
    private void CreateMediaPlayer()
    {
        Log("VLCPlayer CreateMediaPlayer");
        if (mediaPlayer != null)
        {
            DestroyMediaPlayer();
        }
        mediaPlayer = new MediaPlayer(libVLC);
    }

    //Dispose of the MediaPlayer object. 
    private void DestroyMediaPlayer()
    {
        Log("VLCPlayer DestroyMediaPlayer");
        mediaPlayer?.Stop();
        mediaPlayer?.Dispose();
        mediaPlayer = null;
    }

    //Resize the output textures to the size of the video
    void ResizeOutputTextures(uint px, uint py)
    {
        // mediaPlayer가 현재 재생 중인 프래임을 텍스처로 가져옴. 
        var texptr = mediaPlayer.GetTexture(px, py, out bool updated);  // updated: 프래임이 갱신되었는지 여부
        if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
        {
            //If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
            if (GetVideoOrientation() == VideoOrientation.BottomRight)
            {
                uint swap = px;
                px = py;
                py = swap;
            }

            //Make a texture of the proper size for the video to output to
            _vlcTexture = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr);
            //Make a renderTexture the same size as vlctex
            texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0, RenderTextureFormat.ARGB32);

            if (screen != null)
                screen.texture = texture;
        }
    }

    void Log(object message)
    {
        if (logToConsole)
            Debug.Log(message);
    }
    #endregion
}
