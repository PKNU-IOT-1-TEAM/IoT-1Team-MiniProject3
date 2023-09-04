using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyWaterBuoyancy.Editor;

public class Predict_Warning : MonoBehaviour
{
    public GameObject panelWarning; // panel_Warning 오브젝트를 연결해주는 변수
    public GameObject txtWarning; // Txt_Warning 오브젝트를 연결해주는 변수
    //public GameObject btnClose; // Btn_Close 버튼 오브젝트를 연결해주는 변수

    private float updateInterval = 1f; // 업데이트 주기 (1초)

	private float predict_save = 0;

	private void Start()
    {
		ClosePopup();
		// 최초 실행 후 5분 마다 UpdateRiverFlow 함수를 호출
		InvokeRepeating("UpdatePredict", 0f, updateInterval);
    }
	private void Update()
	{
		Debug.Log("실행됨");
		UpdatePredict();
	}
	private void UpdatePredict()
	{
		try
		{
			if (predict_save != float.Parse(Commons.predictData.predict.ToString()))
			{
				predict_save = float.Parse(Commons.predictData.predict.ToString());

				if (predict_save > 50f) 
				{
					txtWarning.SetActive(true);
					txtWarning.GetComponent<Text>().text = "침수 확률이 50%를 초과하였습니다. 대비해 주세요.";
					OpenPopup();
				}
				else 
				{
					txtWarning.SetActive(false);
					ClosePopup();
				}
			}
		}

		catch (Exception e)
		{
			Debug.Log(e.Message);
		}

	}


	public void OpenPopup()
	{
		// 팝업창 활성화 이벤트 
		panelWarning.SetActive(true);
		panelWarning.transform.parent.transform.SetAsLastSibling();   // 패널의 부모도 젤 위로 올리기
		panelWarning.transform.SetAsLastSibling();
	}

	public void ClosePopup()
	{
		panelWarning.SetActive(false);
	}
}