using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoMenuScenePanelPrefab : MonoBehaviour
{
    public void PushButton(bool flg)
    {
        if (flg)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);
            FadeManager.Instance.LoadScene("Menu", 0.5f);
        }
        else
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);
            Destroy(gameObject);
        }
    }
}
