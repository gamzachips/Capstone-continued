using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager
{
    public Slider slider;

    [SerializeField]
    public float maxEnergy;
    [SerializeField]
    private float curEnergy;

    public void Start()
    {
        slider = GameObject.Find("Energy").GetComponent<Slider>();
        maxEnergy = 100;
        curEnergy = 100;
        slider.maxValue = maxEnergy;
        slider.minValue = 0;
        slider.value = curEnergy;
    }

    public void Update()
    {
        slider.value = curEnergy;
    }

    public void DecreaseEnergy(float energy)
    {
        curEnergy -= energy;
        if(curEnergy < 0 )
        {
            curEnergy = 0;
        }
    }

    public void IncreaseEnergy(float energy)
    {
        curEnergy += energy;
        if (curEnergy > maxEnergy)
        {
            curEnergy = maxEnergy;
        }
        if (curEnergy < 0)
        {
            curEnergy = 0;
        }
    }
}
