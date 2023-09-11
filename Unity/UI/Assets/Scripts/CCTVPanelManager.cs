using UnityEngine;

public class CCTVPanelManager : MonoBehaviour
{
    public GameObject panelPrefab; // �г� ������

    private void Awake()
    {
        // EventManager.Instance�� null���� üũ�Ͽ� �����ϰ� �̺�Ʈ�� ����
        if (EventManager.Instance == null)
        {
            Debug.Log("UIPanelManager : Waiting for EventManager.Instance to be initialized...");
            return; // EventManager.Instance�� null�� ��쿡�� ���� �ڵ带 �������� �ʰ� ����
        }

        // ���� EventManager.Instance�� null�� �ƴ��� �����ϰ� �̺�Ʈ�� ����
        EventManager.Instance.OnPanelCreationRequested += CreatePanel;
        Debug.Log("UIPanelManager : ���� EventManager.Instance�� null�� �ƴ��� �����ϰ� �̺�Ʈ�� ����");
    }

    // �г��� �������� �����ϴ� �Լ�
    private void CreatePanel(MapPinInfo mapPinInfo)
    {
        Debug.Log("�г��� �������� �����ϴ� �Լ�");
        VLC_Player vlc_Player = panelPrefab.GetComponent<VLC_Player>();
        vlc_Player.mapPinInfo = mapPinInfo;

        // �г� �������� �������� �����Ͽ� UI ���� �߰�
        GameObject panelInstance =Instantiate(panelPrefab);

        // UI ���� ĵ���� �Ʒ��� ������ �г��� �ڽ����� ��ġ
        panelInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

      
        // �г��� Ȱ��ȭ�Ͽ� ȭ�鿡 ��Ÿ��
        panelInstance.SetActive(true);


    }
}
