using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Rain_Controller : MonoBehaviour
{
    private string Rain = "Rain"; // ã�� ��ƼŬ �ý��� �̸�
    private string Rain1 = "Rain1"; // ã�� ��ƼŬ �ý��� �̸�

    private ParticleSystem PSys_Rain; // ã�� ��ƼŬ �ý���
    private ParticleSystem PSys_Rain1; // ã�� ��ƼŬ �ý���

    private float minSimulationSpeed = 0.5f; // �ּ� �ùķ��̼� �ӷ�
    private float maxSimulationSpeed = 2.0f; // �ִ� �ùķ��̼� �ӷ�
    private float updateInterval = 10f; // ������Ʈ �ֱ� (10��)
    private float DB_PredictData_Rain;

    private void Start()
    {
        // "Rain"�̶�� �̸��� ������Ʈ�� ã�Ƽ� ����
        GameObject Rain_Object = GameObject.Find(Rain);
        // "Rain1"�̶�� �̸��� ������Ʈ�� ã�Ƽ� ����
        GameObject Rain1_Object = GameObject.Find(Rain1);
        // ������Ʈ�� ã�������� �ش� ��ƼŬ �ý��� ������Ʈ�� ������
        if (Rain_Object != null && Rain1_Object != null)
        {
            PSys_Rain = Rain_Object.GetComponent<ParticleSystem>(); // PSys_Rain �ʱ�ȭ
            PSys_Rain1 = Rain1_Object.GetComponent<ParticleSystem>(); // PSys_Rain1 �ʱ�ȭ
        }

		// �ֱ������� ���� �о���� �Լ��� ȣ��
		InvokeRepeating("ReadRainfall", 0f, updateInterval);
    }

    private void ReadRainfall()
    {
        try
        {
            // API �����Ϳ��� ������ ���� �����ͼ� �Ľ��Ͽ� ����
            DB_PredictData_Rain = float.Parse(Commons.predictData.rain); // string �� float ������ Parse
            UpdateRainEffect(DB_PredictData_Rain);
        }
        catch (System.Exception)
        {
            // DB���� �������� �������� ������ �� [null���� �´�] - ������ 0 ó��
            UpdateRainEffect(0);
        }
    }

    public void UpdateRainEffect(float rainfall)
    {
        float newSimulationSpeed; // �� �ùķ��̼� �ӵ� ��

        if (rainfall == 0) // �������� 0�� �� ��ƼŬ ��Ȱ��ȭ - �� ���� ����
        {
            PSys_Rain.gameObject.SetActive(false);
            PSys_Rain1.gameObject.SetActive(false);
        }
        else // �������� 0�� �ƴϸ� ��ƼŬ Ȱ��ȭ
        {
            PSys_Rain.gameObject.SetActive(true);
            PSys_Rain1.gameObject.SetActive(true);
            // �������� ����ȭ�Ͽ� �ùķ��̼� �ӷ��� ����
            float normalizedRainfall = Mathf.Clamp(rainfall / 100.0f, 1.0f, 10.0f); // 1.0~10.0 ���� ������ ����ȭ
            newSimulationSpeed = Mathf.Lerp(minSimulationSpeed, maxSimulationSpeed, normalizedRainfall);

            // ��ƼŬ �ý����� main�� ����Ͽ� �ùķ��̼� �ӷ��� ����
            var Rain_Speed = PSys_Rain.main;
            var Rain1_Speed = PSys_Rain1.main;

            Rain_Speed.simulationSpeed = newSimulationSpeed;
            Rain1_Speed.simulationSpeed = newSimulationSpeed;
        }
    }
}
