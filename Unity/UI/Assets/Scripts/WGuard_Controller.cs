using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class WGuard_Controller : MonoBehaviour
//{
//    private string WGuard_Cube = "WGuard_Cube"; // ã�� ������ ������Ʈ �̸�

//    private GameObject WGuard; // ������ ������Ʈ ���� ����

//    private float updateInterval = 0.5f; // ������Ʈ �ֱ� (1��)

//    private float WGuard_SensorData_Level; // ������ �����κ��� ���� ������ �Ÿ� ������

//    // ���α׷��� ���� �� ���� ���� ���� �� ��ǥ �� ������ �ٸ��� ������ ���� �ؾߵȴ�.
//    // ������ ������Ʈ ���� ��ġ �� Y = 532652f : ���� �ν� �Ǿ��� �� ������Ʈ ���� ��ġ = 1035739f
//    // �⺻ ��ġ
//    // WGuard_Cube : -410129 532650.9 -5011084 ���� ��ǥ��
//    // -410129 532650.9f -5011084
//    // -410129 1035739f -5011084


//    void Start()
//    {
//        // "WGuard_Cube" �̸��� ������ ������Ʈ�� ã�Ƽ� ������ ����
//        WGuard = GameObject.Find(WGuard_Cube);
//        //Debug.Log(WGuard.transform.position);

//        // ��� ���� ������Ʈ�� ã�������� Ȯ��
//        if (WGuard != null)
//        {
//            Debug.Log("������ ������Ʈ ã�� �Ϸ�");

//            // �ֱ������� ���� ���� �о���� �Լ��� ȣ��
//            // 1�� ���� Read_IS_WL �Լ� ����
//            InvokeRepeating("Read_WGuard", 0f, updateInterval);
//        }
//    }

//    private void Read_WGuard()
//    {
//        // MQTT �����Ϳ��� ������ ���� ���� �����ͼ� �Ľ��Ͽ� ����
//        WGuard_SensorData_Level = EventManager.Instance.Sensor_Data.AD3_RCV_WGuard_Wave;
//        Debug.Log(WGuard_SensorData_Level);

//        if (WGuard_SensorData_Level == 0)
//        {
//            // ���� ������Ʈ ��ġ �� 0�� ���� �Ⱥ����� ��
//            WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
//        }
//        // ������ �ּҰ� 5 �ִ� �� 8

//        //// ���� 40 �̻��̸� ���̻� ǥ�� �� �ʿ䰡 ���� �ִ밪�� 532650.9f�� ����
//        else if (WGuard_SensorData_Level > 8.5)
//        {
//            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
//            WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
//        }
//        else if (WGuard_SensorData_Level < 5.5)
//        {
//            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
//            WGuard.transform.localPosition = new Vector3(-410129f, 1035739f, -5011084f);
//        }
//        // 0 ~ 40 ������ ���� ���� �����ָ� �ȴ�.
//        else
//        {
//			// �⺻ ��ġ
//			// WGuard_Cube : -410129 532650.9 -5011084 ���� ��ǥ��
//			// -410129f 532650.9f -5011084f
//			// -410129f 1035739f -5011084f
//			// ���� ���� ����ȭ�Ͽ� ���� ��ȭ�� ���
//			float normalizedSensorValue = (WGuard_SensorData_Level - 5.5f) / (8.5f - 5.5f);

//			// ���� ������Ʈ�� Y ��ġ�� 0���� 285.0f���� ��ȭ��Ŵ
//			float newYPosition_1 = Mathf.Lerp(532650.9f, 1035739f, normalizedSensorValue);

//            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� Y ��ġ�� ����
//            Vector3 newPosition_1 = new Vector3(-410129f, newYPosition_1, -5011084f);

//            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
//            WGuard.transform.localPosition = newPosition_1;
//		}
//    }
//}


public class WGuard_Controller : MonoBehaviour
{
	private string WGuard_Cube = "WGuard_Cube";
	private GameObject WGuard;
	private float updateInterval = 0.1f;
	private float WGuard_SensorData_Level;

	private float smoothedSensorValue = 0f;

	private List<float> sensorDataList = new List<float>(); // ����Ʈ�� ����
	private int filterWindowSize = 5;

	private float MIN_VALUE = 8.5f;
	private float MAX_VALUE = 5.5f;

	void Start()
	{
		WGuard = GameObject.Find(WGuard_Cube);

		if (WGuard != null)
		{
			Debug.Log("������ ������Ʈ ã�� �Ϸ�");
			InvokeRepeating("Read_WGuard", 0f, updateInterval);
		}
	}

	private void Read_WGuard()
	{
		WGuard_SensorData_Level = EventManager.Instance.Sensor_Data.AD3_RCV_WGuard_Wave;

		// ���ο� ���� �����͸� ����Ʈ�� �߰�
		sensorDataList.Add(WGuard_SensorData_Level);

		// ����Ʈ�� ũ�Ⱑ ������ ũ�⺸�� ũ�� ó�� ������ ����
		if (sensorDataList.Count > filterWindowSize)
		{
			sensorDataList.RemoveAt(0);
		}

		// ����Ʈ �� �������� ��� ���
		smoothedSensorValue = CalculateMovingAverage();

		if (smoothedSensorValue == 0)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
		}
		else if (smoothedSensorValue > MIN_VALUE)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 532650.9f, -5011084f);
		}
		else if (smoothedSensorValue < MAX_VALUE)
		{
			WGuard.transform.localPosition = new Vector3(-410129f, 1035739f, -5011084f);
		}
		else
		{
			float normalizedSensorValue = (smoothedSensorValue - MAX_VALUE) / (MIN_VALUE - MAX_VALUE);
			float newYPosition_1 = Mathf.Lerp(1035739f, 532650.9f, normalizedSensorValue);
			Vector3 newPosition_1 = new Vector3(-410129f, newYPosition_1, -5011084f);
			WGuard.transform.localPosition = newPosition_1;
		}
	}

	private float CalculateMovingAverage()
	{
		// ����Ʈ �� �������� ��� ���
		float sum = 0f;
		foreach (float value in sensorDataList)
		{
			sum += value;
		}
		return sum / sensorDataList.Count;
	}
}

