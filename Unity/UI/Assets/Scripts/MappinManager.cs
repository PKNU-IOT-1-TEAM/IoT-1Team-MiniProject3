using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MappinManager : MonoBehaviour
{
    private void Awake()
    {
        // EventManager.Instance�� null���� üũ�Ͽ� �����ϰ� �̺�Ʈ�� ����
        if (EventManager.Instance == null)
        {
            Debug.Log("MapPinBtnManager : Waiting for EventManager.Instance to be initialized...");
            return; // EventManager.Instance�� null�� ��쿡�� ���� �ڵ带 �������� �ʰ� ����
        }

        // ���� EventManager.Instance�� null�� �ƴ��� �����ϰ� �̺�Ʈ�� ����
        EventManager.Instance.OnPanelDestructionResponsed += DestructPanel; // �ش� ���� �ٽ� Ȱ��ȭ.
        Debug.Log("MapPinBtnManager : ���� EventManager.Instance�� null�� �ƴ��� �����ϰ� �̺�Ʈ�� ����");             
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư�� Ŭ���� ���
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast�� ���� Ŭ���� ��ġ�� ������Ʈ�� �����մϴ�.
            if (Physics.Raycast(ray, out hit))
            {
                // Raycast�� ������ ������Ʈ�� pinmap ������Ʈ�� ���
                if (hit.transform.CompareTag("camera1") || hit.transform.CompareTag("MapPin"))
                {
                    // Ŭ���� pinmap ������Ʈ�� ������ �޾Ƽ� CreatePanel �Լ� ����
                    MapPinInfo mapPinInfo = hit.transform.GetComponent<MapPinInfo>();
                    CreatePanel(mapPinInfo);
                }
            }
        }
    }


    public void CreatePanel(MapPinInfo mapPinInfo)
    {
        Debug.Log("��ư Ŭ�� �̺�Ʈ ó�� �Լ�");
        // �г� ���� ��û�� EventManager�� ���� ����
        EventManager.Instance.RequestPanelCreation(mapPinInfo);
        
        // �г��� ���� Collider�� ��Ȱ��ȭ�Ͽ� Ŭ���� ����
        mapPinInfo.gameObject.GetComponent<Collider>().enabled = false;
    }

    // �г��� ������ �� ȣ��Ǵ� �Լ�
    public void DestructPanel(MapPinInfo mapPinInfo)
    {
        // �г��� ���� �Ŀ� Collider�� �ٽ� Ȱ��ȭ�Ͽ� Ŭ�� �����ϰ� ��
        mapPinInfo.gameObject.GetComponent<Collider>().enabled = true ;
        
    }
}


