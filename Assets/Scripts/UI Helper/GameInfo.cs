using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    public GameObject Banner;
    public List<Sprite> InfoSprites;
    public static List<int> InfoStack=new List<int>();
    public AudioSource Notification;
    public static bool start = true;
    public void Switch()
    {
        if (InfoStack.Count>0)
        {
            AddLavaxium();
            Banner.GetComponent<Image>().sprite = InfoSprites[InfoStack[0]];
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            Notification.Play();
            InfoStack.RemoveAt(0);
        }
        else
        {
            gameObject.SetActive(false);
            start = true;
        }
            

    }
    public void AddLavaxium()
    {
        
        if (InfoStack[0] == 0)
        {
            Menu.Lavaxium += 100;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
        else if (InfoStack[0] == 1)
        {
            Menu.Lavaxium += 200;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
        else if (InfoStack[0] == 2)
        {
            Menu.Lavaxium += 300;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
        else if (InfoStack[0] == 3)
        {
            Menu.Lavaxium += 400;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
        else if (InfoStack[0] == 4)
        {
            Menu.Lavaxium += 500;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
        else if (InfoStack[0] == 5)
        {
            Menu.Lavaxium += 600;
            Menu.Score.Lavaxium(Menu.Lavaxium, true);
        }
    }
    public void First()
    {
        if (InfoStack.Count>0 && start)
        {
            AddLavaxium();
            Banner.GetComponent<Image>().sprite = InfoSprites[InfoStack[0]];
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            Notification.Play();
            InfoStack.RemoveAt(0);
            start = false;
        }
    }
}
