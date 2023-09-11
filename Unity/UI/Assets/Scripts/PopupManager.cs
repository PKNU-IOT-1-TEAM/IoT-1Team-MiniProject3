using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    // �޴� ��ư
    public Button Btn_Weather;
    public Button Btn_Sensor;
    public Button Btn_Control;
    public Button Btn_Parking;
    public Button Btn_Water;

	// �˾�â ����
	public GameObject Popup_Weather;
    public GameObject Popup_Sensor;
    public GameObject Popup_Control;
    public GameObject Popup_Warning;
    public GameObject Popup_Parking;
    public GameObject Popup_Water;

	// �˾�â �ݱ� ��ư
	public Button Btn_Close_Weather;
    public Button Btn_Close_Sensor;
    public Button Btn_Close_Control;
    public Button Btn_Close_Warning;
    public Button Btn_Close_Parking;
    public Button Btn_Close_Water;

	// Start is called before the first frame update
	void Awake()
    {
        // �˾�â ��Ȱ��ȭ -> ���� ȭ�鿡�� �˾�â ���̸� �ȵǴϱ�
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

    // ���� ��ư Ŭ���� �˾�â Ȱ��ȭ 
    public void Open_Popup_Weather()
    {
        // �˾�â Ȱ��ȭ �̺�Ʈ 
        Popup_Weather.SetActive(true);
        Popup_Weather.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
        Popup_Weather.transform.SetAsLastSibling();
    }
    public void Close_Popup_Weather()
    {
        Popup_Weather.SetActive(false);
    }

    // ���� ��ư Ŭ���� �˾�â Ȱ��ȭ 
    public void Open_Popup_Sensor()
    {
        // �˾�â Ȱ��ȭ �̺�Ʈ
        Popup_Sensor.SetActive(true);
        Popup_Sensor.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
        Popup_Sensor.transform.SetAsLastSibling();
    }
	public void Open_Popup_Water()
	{
		// �˾�â Ȱ��ȭ �̺�Ʈ
		Popup_Water.SetActive(true);
		Popup_Water.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
		Popup_Water.transform.SetAsLastSibling();
	}
	public void Close_Popup_Sensor()
    {
        Popup_Sensor.SetActive(false);
    }
    // ���� ��ư Ŭ���� �˾�â Ȱ��ȭ
    public void Open_Popup_Control()
    {
        // �˾�â Ȱ��ȭ �̺�Ʈ
        Popup_Control.SetActive(true);
        Popup_Control.GetComponent<GraphicRaycaster>().enabled = false;
        Popup_Warning.SetActive(true);
        Popup_Warning.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
        Popup_Warning.transform.SetAsLastSibling();
        
    }
    public void Close_Popup_Warning() 
    {
        Debug.Log("���â �ݱ� Ŭ��");;
        Popup_Warning.SetActive(false);
        Popup_Control.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
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
	// �������� ��ư Ŭ���� �˾�â Ȱ��ȭ
	public void Open_Popup_Parking()
    {
        // �˾�â Ȱ��ȭ �̺�Ʈ
        Popup_Parking.SetActive(true);
        Popup_Parking.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
        Popup_Parking.transform.SetAsLastSibling();
    }
    public void Close_Popup_Parking()
    {
        Popup_Parking.SetActive(false);
    }

}
