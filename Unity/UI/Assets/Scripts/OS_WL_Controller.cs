using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OS_WL_Controller : MonoBehaviour
{
    private string Outside_Water_Level = "Outside_Water_Level"; // ã�� ���� ������Ʈ �̸�

    private GameObject Water_Level; // ���� ������Ʈ ���� ����

    private float updateInterval = 300f; // ������Ʈ �ֱ� (5��)

    private float DB_RiverFlowData_Water_Level; // API DB�κ��� ���� ���� ������

    // ���α׷��� ���� �� ���� ���� ���� �� ��ǥ �� ������ �ٸ��� ������ ���� �ؾߵȴ�.
    // ���� ������Ʈ ���� ��ġ �� Y = 282.2562f : ���� �ν� �Ǿ��� �� ������Ʈ ���� ��ġ Y = f : �����뼱 Y = f
    //-45457.70, -939.00, -27823.31
    void Start()
    {
        // "Inside_Water_Level_1" �̸��� ������Ʈ�� ã�Ƽ� ������ ����
        Water_Level = GameObject.Find(Outside_Water_Level);
        // Debug.Log(Water_Level.transform.position);
        // ��� ���� ������Ʈ�� ã�������� Ȯ��
        if (Water_Level != null)
        {
            Debug.Log("������Ʈ ã�� �Ϸ�");

            // �ֱ������� ���� ���� �о���� �Լ��� ȣ��
            // 30�� Read_OS_WL �Լ� ����
            InvokeRepeating("Read_OS_WL", 0f, updateInterval);
        }
    }

    private void Read_OS_WL()
    {
        // Debug.Log(Water_Level.transform.position);

        // API �����Ϳ��� API �� ���� ���� ���� �����ͼ� �Ľ��Ͽ� ����
        DB_RiverFlowData_Water_Level = float.Parse(Commons.riverFlowData.waterLevel[1]);

        if (DB_RiverFlowData_Water_Level == 0)
        {
            // ���� ������Ʈ ��ġ �� 0�� ���� �Ⱥ����ߵȴ�.
            Water_Level.transform.position = new Vector3(-45457.7f, -939.00f, -27823.31f);
        }
        // ���� 40 �̻��̸� ���̻� ǥ�� �� �ʿ䰡 ���� �ִ밪�� 4.493164f�� ����
        else if (DB_RiverFlowData_Water_Level > 5.2)
        {
            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
            Water_Level.transform.position = new Vector3(-45457.7f, 4.493164f, -27823.31f);
        }
        // 0 ~ 40 ������ ���� ���� �����ָ� �ȴ�.
        else
        {
            // ���� ���� ����ȭ�Ͽ� ���� ��ȭ�� ���
            float normalizedSensorValue = DB_RiverFlowData_Water_Level / 5.2f;

            // ���� ������Ʈ�� Y ��ġ�� -939.00f���� 4.493164f���� ��ȭ��Ŵ
            float newYPosition_1 = Mathf.Lerp(-939.00f, 4.493164f, normalizedSensorValue);

            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� Y ��ġ�� ����
            Vector3 newPosition_1 = new Vector3(-45457.7f, newYPosition_1, -27823.31f);

            // ���ο� ��ġ ���� �����Ͽ� ���� ������Ʈ�� ��ġ ����
            Water_Level.transform.position = newPosition_1;
        }
    }
}