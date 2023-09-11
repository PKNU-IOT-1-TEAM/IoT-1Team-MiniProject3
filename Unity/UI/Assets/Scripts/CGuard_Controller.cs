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
			Debug.Log("차단기 오브젝트 찾기 완료");
		}
		CGuard.transform.rotation = Quaternion.Euler(0, 0, -90);
	}

	void Update()
	{
		CGuard_Sensor_Level = EventManager.Instance.Sensor_Data.AD2_RCV_CGuard;
		// 값에 따라 회전을 설정합니다.
		if (CGuard_Sensor_Level == 80)
		{
			CGuard.transform.rotation = Quaternion.Euler(0, 0, -90); // (0, 0, 0) 회전으로 설정
			// transform.Rotate(new Vector3(0, 90, 180));
		}
		else if (CGuard_Sensor_Level == 200)
		{
			CGuard.transform.rotation = Quaternion.Euler(0, 0, 0); // (-90, 0, 0) 회전으로 설정
			// transform.Rotate(new Vector3(-90, 90, 180));
		}
		else
		{

		}
	}
}
