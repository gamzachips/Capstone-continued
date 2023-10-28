using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LightRotate : MonoBehaviour
{

    private bool isNight = false;

    [SerializeField] private float nightFogDensity; // �� ������ Fog �е�
    [SerializeField] private float dayFogDensity; // �� ������ Fog �е�
    [SerializeField] private float fogDensityCalc; // ������ ����
    private float currentFogDensity;

    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;

    [SerializeField] GameObject[] lamps;

    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;


    void Update()
    {
        // ��� �¾��� X �� �߽����� ȸ��. ���ǽð� 1�ʿ�  0.1f * secondPerRealTimeSecond ������ŭ ȸ��
        transform.Rotate(Vector3.right, 360 / Managers.Time.DayDuration * Time.deltaTime);

        if (Managers.Time.GetHour() == 21) // x �� ȸ���� 170 �̻��̸� ���̶�� �ϰ���
            isNight = true;
        else if (Managers.Time.GetHour() == 6)  // x �� ȸ���� 10 �̻��̸� ���̶�� �ϰ���
            isNight = false;

        if (isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
                
            }
            light1.SetActive(false);
            light2.SetActive(false);

            RenderSettings.skybox = nightSkybox;
            //���ε� ����

            foreach(var lamp in lamps)
            {
                Light[] lights = lamp.GetComponentsInChildren<Light>();
                foreach (var l in lights)
                    l.enabled = true;
            }
        }
        else
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
            light1.SetActive(true);
            light2.SetActive(true);

            foreach (var lamp in lamps)
            {
                Light[] lights = lamp.GetComponentsInChildren<Light>();
                foreach (var l in lights)
                    l.enabled = false;
            }
            RenderSettings.skybox = daySkybox;
        }
    }
}
