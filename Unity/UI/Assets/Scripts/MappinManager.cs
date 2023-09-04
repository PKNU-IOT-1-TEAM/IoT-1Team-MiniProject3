using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MappinManager : MonoBehaviour
{
    private void Awake()
    {
        // EventManager.Instance가 null인지 체크하여 안전하게 이벤트를 구독
        if (EventManager.Instance == null)
        {
            Debug.Log("MapPinBtnManager : Waiting for EventManager.Instance to be initialized...");
            return; // EventManager.Instance가 null인 경우에는 이후 코드를 실행하지 않고 종료
        }

        // 이제 EventManager.Instance가 null이 아님을 보장하고 이벤트를 구독
        EventManager.Instance.OnPanelDestructionResponsed += DestructPanel; // 해당 맵핀 다시 활성화.
        Debug.Log("MapPinBtnManager : 이제 EventManager.Instance가 null이 아님을 보장하고 이벤트를 구독");             
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼을 클릭한 경우
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast를 통해 클릭한 위치의 오브젝트를 감지합니다.
            if (Physics.Raycast(ray, out hit))
            {
                // Raycast로 감지한 오브젝트가 pinmap 오브젝트인 경우
                if (hit.transform.CompareTag("camera1") || hit.transform.CompareTag("MapPin"))
                {
                    // 클릭된 pinmap 오브젝트의 정보를 받아서 CreatePanel 함수 실행
                    MapPinInfo mapPinInfo = hit.transform.GetComponent<MapPinInfo>();
                    CreatePanel(mapPinInfo);
                }
            }
        }
    }


    public void CreatePanel(MapPinInfo mapPinInfo)
    {
        Debug.Log("버튼 클릭 이벤트 처리 함수");
        // 패널 생성 요청을 EventManager를 통해 보냄
        EventManager.Instance.RequestPanelCreation(mapPinInfo);
        
        // 패널을 열고 Collider를 비활성화하여 클릭을 막음
        mapPinInfo.gameObject.GetComponent<Collider>().enabled = false;
    }

    // 패널이 닫혔을 때 호출되는 함수
    public void DestructPanel(MapPinInfo mapPinInfo)
    {
        // 패널이 닫힌 후에 Collider를 다시 활성화하여 클릭 가능하게 함
        mapPinInfo.gameObject.GetComponent<Collider>().enabled = true ;
        
    }
}


