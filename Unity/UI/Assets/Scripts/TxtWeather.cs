using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TxtWeather : MonoBehaviour
{
	public Text Txt_Temp;           // �µ�
	public Text Txt_Rainfall;       // ����
	public Text River1_Txt_Current;               // �̼����� ��ġ
	public Text River2_Txt_Current;             // ���Ͼ� ���� ����

	public Image Icon_Weather;  // ���� �̹���

	public Sprite sunnySprite;
	public Sprite cloudSprite;
	public Sprite cloudySprite;
	public Sprite rainSprite;
	public Sprite snow_rainSprite;
	public Sprite snowSprite;
	public Sprite rain_dropSprite;
	public Sprite snow_dropSprite;

	
	private void Start()
	{
		
		Txt_Temp.text = Commons.predictData.temp + "��C";
		Txt_Rainfall.text = Commons.predictData.rain + "mm";
		River1_Txt_Current.text = Commons.riverFlowData.waterLevel[0] + "m";
		River2_Txt_Current.text = Commons.riverFlowData.waterLevel[1] + "m";
		Icon_Weather.sprite = sunnySprite;
	}
	// Update is called once per frame
	void Update()
	{

		Txt_Temp.text = Commons.predictData.temp + "��C";
		Txt_Rainfall.text = Commons.predictData.rain + "mm";
		River1_Txt_Current.text = Commons.riverFlowData.waterLevel[0] + "m";
		River2_Txt_Current.text = Commons.riverFlowData.waterLevel[1] + "m";
		Icon_Weather.sprite = WeatherImg(Commons.weatherPridictUltraData.PTY[0], Commons.weatherPridictUltraData.SKY[0]);
	}

	#region < ���� �̹��� ���� �Լ�>
	// �ϴû���, ������ ���¿� ���� ���� �̹��� ����
	public Sprite WeatherImg(string PTYCondition, string SKYCondition)
	{
		Sprite selectedSprite = null;
		if (PTYCondition == "0")
		{
			switch (SKYCondition)
			{
				case "1":
					selectedSprite = sunnySprite;
					break;
				case "3":
					selectedSprite = cloudSprite;
					break;
				case "4":
					selectedSprite = cloudySprite;
					break;
			}
		}
		else
		{
			switch (PTYCondition)
			{
				case "1":
					selectedSprite = rainSprite;
					break;
				case "2":
				case "6":
					selectedSprite = snow_rainSprite;
					break;
				case "3":
					selectedSprite = snowSprite;
					break;
				case "5":
					selectedSprite = rain_dropSprite;
					break;
				case "7":
					selectedSprite = snow_dropSprite;
					break;
			}
		}
		return selectedSprite;
	}
	#endregion
}
