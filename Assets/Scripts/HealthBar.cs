using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class determines the behavior of the LIGHT health bar only
//Normal health bar behavior is in Player.cs (or ShadeMageBoss.cs for boss health)
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
        if (changed)
        {
            if (healthBarNormal.GetComponent<Slider>().value == normal)
            {
                //do nothing, wait for health bar to change
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
            }

            else if (normal < light)
            {
                light -= 0.0035f;
                if (normal > light)
                    light = normal;
            }

            healthBarLight.GetComponent<Slider>().value = light;
        }
    }
}
