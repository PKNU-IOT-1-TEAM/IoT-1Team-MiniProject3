using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    // 메뉴 버튼
    public Button Btn_Weather;
    public Button Btn_Sensor;
    public Button Btn_Control;
    public Button Btn_Parking;
    public Button Btn_Water;

	// 팝업창 변수
	public GameObject Popup_Weather;
    public GameObject Popup_Sensor;
    public GameObject Popup_Control;
    public GameObject Popup_Warning;
    public GameObject Popup_Parking;
    public GameObject Popup_Water;

	// 팝업창 닫기 버튼
	public Button Btn_Close_Weather;
    public Button Btn_Close_Sensor;
    public Button Btn_Close_Control;
    public Button Btn_Close_Warning;
    public Button Btn_Close_Parking;
    public Button Btn_Close_Water;

	// Start is called before the first frame update
	void Awake()
    {
        // 팝업창 비활성화 -> 메인 화면에서 팝업창 보이면 안되니까
        Close_Popup_Weather();
        Close_Popup_Sensor();
        Close_Popup_Control();
		Init_Close_Popup_Warning();
        Close_Popup_Parking();
		Close_Popup_Water();

		Btn_Weather.onClick.AddListener(() => { Open_Popup_Weather(); });
        Btn_Sensor.onClick.AddListener(() => { Open_Popup_Sensor(); });
        Btn_Control.onClick.AddListener(() => { Open_Popup_Control(); });
        Btn_Parking.onClick.AddListener(() => { Open_Popup_Parking(); });
		Btn_Water.onClick.AddListener(() => { Open_Popup_Water(); });

		Btn_Close_Weather.onClick.AddListener(() => { Close_Popup_Weather(); });
        Btn_Close_Sensor.onClick.AddListener(() => { Close_Popup_Sensor(); });
        Btn_Close_Control.onClick.AddListener(() => { Close_Popup_Control(); });
        Btn_Close_Warning.onClick.AddListener(() => { Close_Popup_Warning(); });
        Btn_Close_Parking.onClick.AddListener(() => { Close_Popup_Parking(); });
		Btn_Close_Water.onClick.AddListener(() => { Close_Popup_Water(); });
	}

    // 날씨 버튼 클릭시 팝업창 활성화 
    public void Open_Popup_Weather()
    {
        // 팝업창 활성화 이벤트 
        Popup_Weather.SetActive(true);
        Popup_Weather.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        Popup_Weather.transform.SetAsLastSibling();
    }
    public void Close_Popup_Weather()
    {
        Popup_Weather.SetActive(false);
    }

    // 센서 버튼 클릭시 팝업창 활성화 
    public void Open_Popup_Sensor()
    {
        // 팝업창 활성화 이벤트
        Popup_Sensor.SetActive(true);
        Popup_Sensor.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        Popup_Sensor.transform.SetAsLastSibling();
    }
	public void Open_Popup_Water()
	{
		// 팝업창 활성화 이벤트
		Popup_Water.SetActive(true);
		Popup_Water.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
		Popup_Water.transform.SetAsLastSibling();
	}
	public void Close_Popup_Sensor()
    {
        Popup_Sensor.SetActive(false);
    }
    // 제어 버튼 클릭시 팝업창 활성화
    public void Open_Popup_Control()
    {
        // 팝업창 활성화 이벤트
        Popup_Control.SetActive(true);
        Popup_Control.GetComponent<GraphicRaycaster>().enabled = false;
        Popup_Warning.SetActive(true);
        Popup_Warning.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        Popup_Warning.transform.SetAsLastSibling();
        
    }
    public void Close_Popup_Warning() 
    {
        Debug.Log("경고창 닫기 클릭");;
        Popup_Warning.SetActive(false);
        Popup_Control.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        Popup_Control.transform.SetAsLastSibling();
        Popup_Control.GetComponent<GraphicRaycaster>().enabled = true;

    }
    public void Init_Close_Popup_Warning()
    {
        Popup_Warning.SetActive(false);
    }
    public void Close_Popup_Control()
    {
        Popup_Control.SetActive(false);
    }
	public void Close_Popup_Water()
	{
		Popup_Water.SetActive(false);
	}
	// 주차예약 버튼 클릭시 팝업창 활성화
	public void Open_Popup_Parking()
    {
        // 팝업창 활성화 이벤트
        Popup_Parking.SetActive(true);
        Popup_Parking.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
        Popup_Parking.transform.SetAsLastSibling();
    }
    public void Close_Popup_Parking()
    {
        Popup_Parking.SetActive(false);
    }

}
