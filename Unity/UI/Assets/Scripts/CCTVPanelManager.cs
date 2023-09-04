using UnityEngine;

public class CCTVPanelManager : MonoBehaviour
{
    public GameObject panelPrefab; // 패널 프리팹

    private void Awake()
    {
        // EventManager.Instance가 null인지 체크하여 안전하게 이벤트를 구독
        if (EventManager.Instance == null)
        {
            Debug.Log("UIPanelManager : Waiting for EventManager.Instance to be initialized...");
            return; // EventManager.Instance가 null인 경우에는 이후 코드를 실행하지 않고 종료
        }

        // 이제 EventManager.Instance가 null이 아님을 보장하고 이벤트를 구독
        EventManager.Instance.OnPanelCreationRequested += CreatePanel;
        Debug.Log("UIPanelManager : 이제 EventManager.Instance가 null이 아님을 보장하고 이벤트를 구독");
    }

    // 패널을 동적으로 생성하는 함수
    private void CreatePanel(MapPinInfo mapPinInfo)
    {
        Debug.Log("패널을 동적으로 생성하는 함수");
        VLC_Player vlc_Player = panelPrefab.GetComponent<VLC_Player>();
        vlc_Player.mapPinInfo = mapPinInfo;

        // 패널 프리팹을 동적으로 생성하여 UI 씬에 추가
        GameObject panelInstance =Instantiate(panelPrefab);

        // UI 씬의 캔버스 아래에 생성된 패널을 자식으로 배치
        panelInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

      
        // 패널을 활성화하여 화면에 나타남
        panelInstance.SetActive(true);


    }
}
