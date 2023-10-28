using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LightRotate : MonoBehaviour
{

    private bool isNight = false;

    [SerializeField] private float nightFogDensity; // 밤 상태의 Fog 밀도
    [SerializeField] private float dayFogDensity; // 낮 상태의 Fog 밀도
    [SerializeField] private float fogDensityCalc; // 증감량 비율
    private float currentFogDensity;

    [SerializeField] GameObject light1;
    [SerializeField] GameObject light2;

    [SerializeField] GameObject[] lamps;

    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;


    void Update()
    {
        // 계속 태양을 X 축 중심으로 회전. 현실시간 1초에  0.1f * secondPerRealTimeSecond 각도만큼 회전
        transform.Rotate(Vector3.right, 360 / Managers.Time.DayDuration * Time.deltaTime);

        if (Managers.Time.GetHour() == 21) // x 축 회전값 170 이상이면 밤이라고 하겠음
            isNight = true;
        else if (Managers.Time.GetHour() == 6)  // x 축 회전값 10 이상이면 낮이라고 하겠음
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
            //가로등 점등

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
