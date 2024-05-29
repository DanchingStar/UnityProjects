using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private int flg = 0;
    void Start()
    {
        
    }

    void Update()
    {
        if (flg == 0)
        {
            ToMenu();
        }
    }

    private void ToMenu()
    {
        flg = 1;
        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }
}
