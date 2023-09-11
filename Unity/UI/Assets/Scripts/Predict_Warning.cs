using MySql.Data.MySqlClient;
using System.Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyWaterBuoyancy.Editor;

public class Predict_Warning : MonoBehaviour
{
    public GameObject panelWarning; // panel_Warning ������Ʈ�� �������ִ� ����
    public GameObject txtWarning; // Txt_Warning ������Ʈ�� �������ִ� ����
    //public GameObject btnClose; // Btn_Close ��ư ������Ʈ�� �������ִ� ����

    private float updateInterval = 1f; // ������Ʈ �ֱ� (1��)

	private float predict_save = 0;

	private void Start()
    {
		ClosePopup();
		// ���� ���� �� 5�� ���� UpdateRiverFlow �Լ��� ȣ��
		InvokeRepeating("UpdatePredict", 0f, updateInterval);
    }
	private void Update()
	{
		Debug.Log("�����");
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
					txtWarning.GetComponent<Text>().text = "ħ�� Ȯ���� 50%�� �ʰ��Ͽ����ϴ�. ����� �ּ���.";
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
		// �˾�â Ȱ��ȭ �̺�Ʈ 
		panelWarning.SetActive(true);
		panelWarning.transform.parent.transform.SetAsLastSibling();   // �г��� �θ� �� ���� �ø���
		panelWarning.transform.SetAsLastSibling();
	}

	public void ClosePopup()
	{
		panelWarning.SetActive(false);
	}
}