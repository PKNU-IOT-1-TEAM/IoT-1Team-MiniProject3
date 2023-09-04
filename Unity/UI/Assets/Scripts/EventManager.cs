using UnityEngine;
using System;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // ���� ������ Ŭ������ �ν��ͽ��� ��� ������ ����
    public SensorData Sensor_Data;
    private float pre_WGuard_Wave; // ������ ���� cm ������ ����
    private int Wave_Count = 0; // �� �ߺ� Ƚ�� ����
                                // �г� ���� ��û �̺�Ʈ�� ����
    public event Action<MapPinInfo> OnPanelCreationRequested;
    public event Action<MapPinInfo> OnPanelDestructionResponsed;

	public InputField inputField;
	public Button Btn_Submit;
	public Button Btn_Cancel;

    private string nfc_temp = "";
	public void Start()
	{
		//// Submit ��ư�� �Լ� ����
		//Btn_Submit.onClick.AddListener(Click_Submit_Btn);
		//// Cancel ��ư�� �Լ� ����
		//Btn_Cancel.onClick.AddListener(Click_Cancel_Btn);
	}

	private void Awake()
    {
        if (Instance == null)
        {
            Debug.Log("EventManager.Instance ����");
            Instance = this;
            Sensor_Data = new SensorData();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // �� ��ü�� �� ��ȯ �� �ı����� �ʵ��� ����
    }

    // �г� ���� ��û �Լ�
    public void RequestPanelCreation(MapPinInfo mapPinInfo)
    {
        // �г� ���� ��û �̺�Ʈ�� �߻���Ŵ
        Debug.Log("�г� ���� ��û �̺�Ʈ�� �߻���Ŵ");
        OnPanelCreationRequested?.Invoke(mapPinInfo);

    }
    // �г� �ı� ���� �Լ�
    public void ResponsePanelDestruction(MapPinInfo mapPinInfo)
    {
        // �г� �ı� ���� �̺�Ʈ�� �߻���Ŵ
        Debug.Log("�г� �ı� ���� �̺�Ʈ�� �߻���Ŵ");
        OnPanelDestructionResponsed?.Invoke(mapPinInfo);
    }

    // ���� ������ �ʱ�ȭ
    public void SetSensorData(string message)
    {
        // ���� NFC�� �ӽ� ����
        nfc_temp = Sensor_Data.AD4_RCV_NFC;

		// mqtt�� ���� message(json����)�� Sensor_Data ��ü��.
		JsonUtility.FromJsonOverwrite(message, Sensor_Data);

		// �Ҽ��� �ڸ� ���� (�µ���, ������)
        float floatValue = float.Parse(Sensor_Data.AD1_RCV_Temperature);
		Sensor_Data.AD1_RCV_Temperature = floatValue.ToString("F1"); // "F1"�� �Ҽ��� ���ڸ����� ����ϴ� ���� ������
        floatValue = float.Parse(Sensor_Data.AD1_RCV_Humidity);
		Sensor_Data.AD1_RCV_Humidity = floatValue.ToString("F1"); // "F1"�� �Ҽ��� ���ڸ����� ����ϴ� ���� ������

        // ���ܸ� ���� ���� set
		if (Sensor_Data.AD3_RCV_WGuard_Wave == 0.0f)    // �ʱ� ���´� �ϰ�����
        {
            Sensor_Data.AD3_State_WGuard = "�ϰ� ����";
		}
        else if (Sensor_Data.AD3_RCV_WGuard_Wave != pre_WGuard_Wave)    // ���簪�̶� �������� �ٸ��� ������
        {
            Sensor_Data.AD3_State_WGuard = "���� ��";
            Wave_Count = 0;
        }
        else    // ���簪�� �������� ������ ���� ��! //���� ����
        {
            Wave_Count += 1;
            if (Sensor_Data.AD3_RCV_WGuard_Wave < 6.0f && Wave_Count >= 4)
            {
                Sensor_Data.AD3_State_WGuard = "��� ����";
            }
            else if (Wave_Count >= 5)
            {
                Sensor_Data.AD3_State_WGuard = "�ϰ� ����";
            }
            else
            {
                Sensor_Data.AD3_State_WGuard = "���� ��";
            }

        }
        //      else if(Sensor_Data.AD3_RCV_WGuard_Wave <= 5.5f )
        //      {
        //          Sensor_Data.AD3_State_WGuard = "��� ����";
        //}
        //      else if(Sensor_Data.AD3_RCV_WGuard_Wave >= 8.5f && Sensor_Data.AD3_RCV_WGuard_Wave <= 13.0f)
        //      {
        //	Sensor_Data.AD3_State_WGuard = "�ϰ� ����";
        //}
        //      else
        //      {
        //	Sensor_Data.AD3_State_WGuard = "���� ��";
        //}

        pre_WGuard_Wave = Sensor_Data.AD3_RCV_WGuard_Wave;  // ���� �� ����



        // NFC ������ ����
        if (Sensor_Data.AD4_RCV_NFC == "None" || Sensor_Data.AD4_RCV_NFC == "")
		{
			Sensor_Data.AD4_RCV_NFC = nfc_temp;
		}

	}

	//void Click_Submit_Btn()
	//{
	//	Instance.Sensor_Data.AD4_RCV_WL_CNNT = inputField.text;
	//}
	//void Click_Cancel_Btn()
	//{
 //       // ���� ������ �ǵ�����
	//}
}
