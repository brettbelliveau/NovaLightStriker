using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This class determines the behavior of the LIGHT health bar only
//Normal health bar behavior is in Player.cs (or ShadeMageBoss.cs for boss health)
public class BossHealthBar : MonoBehaviour
{
    public static bool changed, sameValue;
    private float counter = 0;
    private float normal, light;
    public static bool init;
    public GameObject healthBarNormal;
    public GameObject healthBarLight;
    public GameObject text;

    // Use this for initialization
    void Start()
    {
        changed = false;
        sameValue = false;
        normal = light = 1;
        healthBarNormal.GetComponent<Slider>().value = healthBarLight.GetComponent<Slider>().value = 0;
        init = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (init == false)
        {
            counter += 0.006f;
            healthBarNormal.GetComponent<Slider>().value = counter;
            healthBarLight.GetComponent<Slider>().value = counter;

            if (counter > 1)
            {
                init = true;
                ShadeMageBoss.barInit = true;
                SkeletonBoss.barInit = true;
                Player.stopMovement = false;
            }
        }

        sameValue = normal == light;
        if (changed)
        {
            if (healthBarNormal.GetComponent<Slider>().value == normal && sameValue)
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
                light -= 0.0025f;
                if (normal > light)
                    light = normal;
            }

            healthBarLight.GetComponent<Slider>().value = light;
        }
    }
}
