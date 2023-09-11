using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IS_WL_Controller : MonoBehaviour
{
    private string Inside_Water_Level_1 = "Inside_Water_Level_1"; // ã�� ���� ������Ʈ �̸�
    private string Inside_Water_Level_2 = "Inside_Water_Level_2"; // ã�� ���� ������Ʈ �̸�
    private string Inside_Water_Level_3 = "Inside_Water_Level_3"; // ã�� ���� ������Ʈ �̸�

    private GameObject Water_Level_1; // ���� ������Ʈ ���� ����
    private GameObject Water_Level_2; // ���� ������Ʈ ���� ����
    private GameObject Water_Level_3; // ���� ������Ʈ ���� ����

    private float updateInterval = 1.0f; // ������Ʈ �ֱ� (5��)

    private float SensorData_Water_Level; // �����κ��� ���� ���� ������

	// ���α׷��� ���� �� ���� ���� ���� �� ��ǥ �� ������ �ٸ��� ������ ���� �ؾߵȴ�.
	// ���� ������Ʈ ���� ��ġ �� Y = 279.62f : ���� �ν� �Ǿ��� �� ������Ʈ ���� ��ġ = 282.0f : �����뼱 Y = 285.0f
	// �⺻ ��ġ
	// Inside_Water_Level_1 : -109.3485 -42223 -2494964 ���� ��ǥ��
	// 2108.73, 278.18, 939.01
	// 2108.73, 278.18, 939.01
	// Inside_Water_Level_2 : -2417370 -42223 -3641050 ���� ��ǥ��
	// 1997.91, 278.18, 1172.76
	// Inside_Water_Level_3 : 0.8125 -42223 -4972127 ���� ��ǥ��
	// 1869.19, 278.18, 939.00

	void Start()
    {
        // "Inside_Water_Level_1" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
        Water_Level_1 = GameObject.Find(Inside_Water_Level_1);
        // "Inside_Water_Level_2" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
        Water_Level_2 = GameObject.Find(Inside_Water_Level_2);
        // "Inside_Water_Level_3" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
        Water_Level_3 = GameObject.Find(Inside_Water_Level_3);

        //Debug.Log(Water_Level_1.transform.position);
        //Debug.Log(Water_Level_2.transform.position);
        //Debug.Log(Water_Level_3.transform.position);

        // ��� ���� ������Ʈ�� ã�������� Ȯ��
        if (Water_Level_1 != null && Water_Level_2 != null && Water_Level_3 != null)
        {
            Debug.Log("������Ʈ ã�� �Ϸ�");

            // �ֱ������� ���� ���� �о���� �Լ��� ȣ��
            // 5�� Read_IS_WL �Լ� ����
            InvokeRepeating("Read_IS_WL", 0f, updateInterval);
        }
    }

    private void Read_IS_WL()
    {

        // Debug.Log(Water_Level_1.transform.position);
        // Debug.Log(Water_Level_2.transform.position);
        // Debug.Log(Water_Level_3.transform.position);

        // API �����Ϳ��� �Ƴ��α� ���˽� ���� ���� ���� �����ͼ� �Ľ��Ͽ� ����
        SensorData_Water_Level = EventManager.Instance.Sensor_Data.AD4_RCV_WL_CNNT;

        if (SensorData_Water_Level == 0)
        {
            // ���� ������Ʈ ��ġ �� 0�� ���� �Ⱥ����� ��
            Water_Level_1.transform.localPosition = new Vector3(-109.3485f, - 42223f, - 2494964f);
            Water_Level_2.transform.localPosition = new Vector3(-2417370f, - 42223f, - 3641050f);
            Water_Level_3.transform.localPosition = new Vector3(0.8125f, - 42223f, - 4972127f);
        }
        // ���� 40 �̻��̸� ���̻� ǥ�� �� �ʿ䰡 ���� �ִ밪�� 285.0f�� ����
        else if (SensorData_Water_Level > 40) 
        {
            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
            Water_Level_1.transform.localPosition = new Vector3(-109.3485f, 70000, -2494964f);
            Water_Level_2.transform.localPosition = new Vector3(-2417370f, 70000, -3641050f);
            Water_Level_3.transform.localPosition = new Vector3(0.8125f, 70000, -4972127f);
        }
        // 0 ~ 40 ������ ���� ���� �����ָ� �ȴ�.
        else
        {
            // ���� ���� ����ȭ�Ͽ� ���� ��ȭ�� ���
            float normalizedSensorValue = SensorData_Water_Level / 40.0f;

            // ���� ������Ʈ�� Y ��ġ�� 0���� 285.0f���� ��ȭ��Ŵ
            float newYPosition_1 = Mathf.Lerp(0, 70000, normalizedSensorValue);
            float newYPosition_2 = Mathf.Lerp(0, 70000, normalizedSensorValue);
            float newYPosition_3 = Mathf.Lerp(0, 70000, normalizedSensorValue);

            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� Y ��ġ�� ����
            Vector3 newPosition_1 = new Vector3(-109.3485f, newYPosition_1, -2494964f);
            Vector3 newPosition_2 = new Vector3(-2417370f, newYPosition_2, -3641050f);
            Vector3 newPosition_3 = new Vector3(0.8125f, newYPosition_3, -4972127f);

            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
            Water_Level_1.transform.localPosition = newPosition_1;
            Water_Level_2.transform.localPosition = newPosition_2;
            Water_Level_3.transform.localPosition = newPosition_3;
        }
    }
}
