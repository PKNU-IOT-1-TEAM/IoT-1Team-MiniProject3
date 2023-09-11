using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsParkingGarage : MonoBehaviour
{
    public GameObject barPrefab;
    public GameObject carPrefab;

    private Animator carAnimator;
    private Animator barAnimator;

    public static bool animationsPlaying; // 애니메이션 실행 여부 체크
    public static bool parkingInAnimationPlayed; // AD1_RCV_IR_Sensor가 "0"일 때 한 번만 재생되도록 관리
    public static bool CarParking; // 차가 들어왔는지 여부
    public static float updateInterval = 1.0f; // 업데이트 주기 (1초)

    private void Start()
    {
        Hide();
        carAnimator = carPrefab.GetComponent<Animator>();
        barAnimator = barPrefab.GetComponent<Animator>();
        InvokeRepeating("Parking_Check", 0f, updateInterval);
    }

    private void Hide()
    {
        carPrefab.SetActive(false);
    }

    private void Parking_Check()
    {
        if (EventManager.Instance.Sensor_Data.AD2_State_CGuard == "ON" &&  !animationsPlaying)
        {
            carPrefab.SetActive(true);
            StartCoroutine(PlayAnimationsInsideParkingGarage());
        }
    }

    private IEnumerator PlayAnimationsInsideParkingGarage()
    {
        animationsPlaying = true; // 애니메이션 실행 여부
        // Car 애니메이션 실행
        carAnimator.Play("car_moving_in_1");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Bar 애니메이션 실행
        barAnimator.Play("bar_open");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 다른 Car 애니메이션 실행
        carAnimator.Play("car_moving_in_2");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 다른 Bar 애니메이션 실행
        barAnimator.Play("bar_close");
        yield return new WaitForSeconds(barAnimator.GetCurrentAnimatorStateInfo(0).length);
        CarParking = true;

        Hide();
        animationsPlaying = false;
    }
}
