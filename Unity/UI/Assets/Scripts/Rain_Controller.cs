using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Rain_Controller : MonoBehaviour
{
    private string Rain = "Rain"; // 찾을 파티클 시스템 이름
    private string Rain1 = "Rain1"; // 찾을 파티클 시스템 이름

    private ParticleSystem PSys_Rain; // 찾은 파티클 시스템
    private ParticleSystem PSys_Rain1; // 찾은 파티클 시스템

    private float minSimulationSpeed = 0.5f; // 최소 시뮬레이션 속력
    private float maxSimulationSpeed = 2.0f; // 최대 시뮬레이션 속력
    private float updateInterval = 10f; // 업데이트 주기 (10초)
    private float DB_PredictData_Rain;

    private void Start()
    {
        // "Rain"이라는 이름의 오브젝트를 찾아서 대입
        GameObject Rain_Object = GameObject.Find(Rain);
        // "Rain1"이라는 이름의 오브젝트를 찾아서 대입
        GameObject Rain1_Object = GameObject.Find(Rain1);
        // 오브젝트가 찾아졌으면 해당 파티클 시스템 컴포넌트를 가져옴
        if (Rain_Object != null && Rain1_Object != null)
        {
            PSys_Rain = Rain_Object.GetComponent<ParticleSystem>(); // PSys_Rain 초기화
            PSys_Rain1 = Rain1_Object.GetComponent<ParticleSystem>(); // PSys_Rain1 초기화
        }

		// 주기적으로 값을 읽어오는 함수를 호출
		InvokeRepeating("ReadRainfall", 0f, updateInterval);
    }

    private void ReadRainfall()
    {
        try
        {
            // API 데이터에서 강수량 값을 가져와서 파싱하여 저장
            DB_PredictData_Rain = float.Parse(Commons.predictData.rain); // string 값 float 형으로 Parse
            UpdateRainEffect(DB_PredictData_Rain);
        }
        catch (System.Exception)
        {
            // DB에서 강수량을 꺼내오지 못했을 시 [null값이 온다] - 강수량 0 처리
            UpdateRainEffect(0);
        }
    }

    public void UpdateRainEffect(float rainfall)
    {
        float newSimulationSpeed; // 새 시뮬레이션 속도 값

        if (rainfall == 0) // 강수량이 0일 때 파티클 비활성화 - 비가 오지 않음
        {
            PSys_Rain.gameObject.SetActive(false);
            PSys_Rain1.gameObject.SetActive(false);
        }
        else // 강수량이 0이 아니면 파티클 활성화
        {
            PSys_Rain.gameObject.SetActive(true);
            PSys_Rain1.gameObject.SetActive(true);
            // 강수량을 정규화하여 시뮬레이션 속력을 조절
            float normalizedRainfall = Mathf.Clamp(rainfall / 100.0f, 1.0f, 10.0f); // 1.0~10.0 사이 값으로 정규화
            newSimulationSpeed = Mathf.Lerp(minSimulationSpeed, maxSimulationSpeed, normalizedRainfall);

            // 파티클 시스템의 main을 사용하여 시뮬레이션 속력을 변경
            var Rain_Speed = PSys_Rain.main;
            var Rain1_Speed = PSys_Rain1.main;

            Rain_Speed.simulationSpeed = newSimulationSpeed;
            Rain1_Speed.simulationSpeed = newSimulationSpeed;
        }
    }
}
