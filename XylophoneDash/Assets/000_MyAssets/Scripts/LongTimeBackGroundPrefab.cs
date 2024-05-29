using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LongTimeBackGroundPrefab : MonoBehaviour
{
    [SerializeField] Button okButton;

    private bool buttonTappedFlg = false;

    private const string SCENE_NAME_TITLE = "Title";
    //private const string SCENE_NAME_MENU = "Menu";
    //private const string SCENE_NAME_GAME = "Game";

    public void PushOkButton()
    {
        if (buttonTappedFlg) return;

        buttonTappedFlg = true;
        okButton.interactable = false;
        GoToTitle();
    }

    private void GoToTitle()
    {
        var objects = FindObjectsOfType<MonoBehaviour>();

        foreach (var obj in objects)
        {
            if (obj.gameObject != gameObject && obj.gameObject.name != "FadeManager")
            {
                Destroy(obj.gameObject);
            }
        }

        // 初めからスタートするシーンを読み込む
        FadeManager.Instance.LoadScene(SCENE_NAME_TITLE, 0.5f);
    }
}
