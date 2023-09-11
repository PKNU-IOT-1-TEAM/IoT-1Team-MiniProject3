using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class CGuard_Controller : MonoBehaviour
{
	private string CGuard_Bar_Pivot = "CGuard_Bar_Pivot"; 
	private GameObject CGuard;
	private int CGuard_Sensor_Level;

	void Start()
	{
		CGuard = GameObject.Find(CGuard_Bar_Pivot);
		// Debug.Log(CGuard.transform.rotation);

		if (CGuard != null)
		{
			Debug.Log("���ܱ� ������Ʈ ã�� �Ϸ�");
		}
		CGuard.transform.rotation = Quaternion.Euler(0, 0, -90);
	}

	void Update()
	{
		CGuard_Sensor_Level = EventManager.Instance.Sensor_Data.AD2_RCV_CGuard;
		// ���� ���� ȸ���� �����մϴ�.
		if (CGuard_Sensor_Level == 80)
		{
			CGuard.transform.rotation = Quaternion.Euler(0, 0, -90); // (0, 0, 0) ȸ������ ����
			// transform.Rotate(new Vector3(0, 90, 180));
		}
		else if (CGuard_Sensor_Level == 200)
		{
			CGuard.transform.rotation = Quaternion.Euler(0, 0, 0); // (-90, 0, 0) ȸ������ ����
			// transform.Rotate(new Vector3(-90, 90, 180));
		}
		else
		{

		}
	}
}
