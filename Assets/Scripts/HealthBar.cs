using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class determines the behavior of the LIGHT health bar only
//Normal health bar behavior is in Player.cs
public class HealthBar : MonoBehaviour
{
    public static bool changed, sameValue;
    private int counter = 0;
    private float normal, light;
    public GameObject healthBarNormal;
    public GameObject healthBarLight;

    // Use this for initialization
    void Start()
    {
        changed = false;
        sameValue = false;
        normal = light = 1;
    }

    // Update is called once per frame
    void Update()
    {
        sameValue = normal == light;
        Debug.Log("Norm " + normal + " , light " + light);
        if (changed)
        {
            if (healthBarNormal.GetComponent<Slider>().value == normal)
            {
                //do nothing, wait for health var to change
            }
            else
            {
                changed = false;
                normal = healthBarNormal.GetComponent<Slider>().value;
            }
        }

        else if (!sameValue)
        {
            if (normal > light)
            {
                light = normal;
                healthBarLight.GetComponent<Slider>().value = light;
            }

            else if (normal < light)
            {
                light -= 0.0025f;
                healthBarLight.GetComponent<Slider>().value -= 0.0025f;
            }
        }
    }
}
