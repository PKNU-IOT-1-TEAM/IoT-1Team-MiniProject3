using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsParkingGarage : MonoBehaviour
{
    public GameObject barPrefab;
    public GameObject carPrefab;

    private Animator carAnimator;
    private Animator barAnimator;

    public static bool animationsPlaying; // �ִϸ��̼� ���� ���� üũ
    public static bool parkingInAnimationPlayed; // AD1_RCV_IR_Sensor�� "0"�� �� �� ���� ����ǵ��� ����
    public static bool CarParking; // ���� ���Դ��� ����
    public static float updateInterval = 1.0f; // ������Ʈ �ֱ� (1��)

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
        animationsPlaying = true; // �ִϸ��̼� ���� ����
        // Car �ִϸ��̼� ����
        carAnimator.Play("car_moving_in_1");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Bar �ִϸ��̼� ����
        barAnimator.Play("bar_open");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // �ٸ� Car �ִϸ��̼� ����
        carAnimator.Play("car_moving_in_2");
        yield return new WaitForSeconds(carAnimator.GetCurrentAnimatorStateInfo(0).length);

        // �ٸ� Bar �ִϸ��̼� ����
        barAnimator.Play("bar_close");
        yield return new WaitForSeconds(barAnimator.GetCurrentAnimatorStateInfo(0).length);
        CarParking = true;

        Hide();
        animationsPlaying = false;
    }
}
