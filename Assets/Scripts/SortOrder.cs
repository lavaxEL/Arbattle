using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortOrder : MonoBehaviour
{
    SpriteRenderer SpriteRenderer;
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void RegenStart()
    {
        Main.Regen = true;
        GetComponent<Animator>().SetInteger("cntrl", 0);
    }
    public void Skeleton()
    {
        GetComponent<Animator>().SetInteger("cntrl", 0);
    }
    // Update is called once per frame
    void Update()
    {
        float pos = transform.position.y;
        if (pos >= 0)
            SpriteRenderer.sortingOrder = 30000 - (int)(pos*100);
        else
            SpriteRenderer.sortingOrder = 30000 + (int)(Mathf.Abs(pos)*100);
    }
}
