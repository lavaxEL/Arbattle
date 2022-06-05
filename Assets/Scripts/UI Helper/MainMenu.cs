using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public AudioSource MenuMusic;
    public Image obj;
    public void BestScore()
    {
        Menu.Score.BestScore(Menu.BestScore, true);
        Menu.Score.BestCombo(Menu.BestCombo, true);
        Menu.Score.Lavaxium(Menu.Lavaxium, true);
       // Menu.Score.Arrow(Menu.Arrows, true);
    }
    private void OnDisable()
    {
        Menu.Score.SetBestNotActive();
    }
}
