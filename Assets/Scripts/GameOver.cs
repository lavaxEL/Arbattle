using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image Loading;
    float time;
    public static bool can = false;
    private void OnEnable() 
    {
        Loading.fillAmount = 1;
        time = Time.realtimeSinceStartup;
        can = true;
    }
    void Update()
    { 
        if (Time.realtimeSinceStartup-time>=0.05f && can)
        {
            time = Time.realtimeSinceStartup;
            Loading.fillAmount -=0.01f;
        }
        if (Loading.fillAmount <= 0 && can)
        {
            can = false;
            Main.ExitGO = true;
        }
    }
}
