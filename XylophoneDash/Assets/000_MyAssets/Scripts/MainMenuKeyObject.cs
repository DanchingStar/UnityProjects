using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuKeyObject : MonoBehaviour
{
    [SerializeField] private MenuSceneManager.SelectMode myMode;

    private Material keyObjectMaterial;
    private Material stringMaterial;

    private const int TIMES_BLINKING = 4;
    private const float TIME_CHANGE = 0.1f;

    private const float SPEED_FLASH_STRING = 3f;

    private Color COLOR_DEFAULT_KEY_OBJECT = Color.white;
    private Color COLOR_DEFAULT_STRING = Color.white;

    private readonly Color COLOR_CHANGE_KEY_OBJECT = Color.yellow;
    private readonly Color COLOR_CHANGE_STRING = Color.cyan;

    private bool decideFlg;

    private void Start()
    {
        keyObjectMaterial = GetComponent<MeshRenderer>().material;
        stringMaterial = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;

        COLOR_DEFAULT_KEY_OBJECT = keyObjectMaterial.color;
        COLOR_DEFAULT_STRING = stringMaterial.color;

        decideFlg = false;
    }

    private void Update()
    {
        if (!decideFlg)
        {
            float value = ((Mathf.Sin(MenuSceneManager.Instance.GetTimerForMainMenuKeyObject() * SPEED_FLASH_STRING) + 1f) / 2f);
            Color col = new Color(value, COLOR_DEFAULT_STRING.g, COLOR_DEFAULT_STRING.b, COLOR_DEFAULT_STRING.a);
            stringMaterial.color = col;
        }
    }

    public void TapMyObject()
    {
        if (MenuSceneManager.Instance.GetNowSelectMode() != MenuSceneManager.SelectMode.FirstMenu) return;

        decideFlg = true;

        MenuSceneManager.Instance.TapXylophone(myMode);
        StartCoroutine(Staging());
    }

    private IEnumerator Staging()
    {
        int timer = 0;

        while (timer < TIMES_BLINKING)
        {
            timer++;
            ChangeColor();
            yield return new WaitForSeconds(TIME_CHANGE);
        }
        decideFlg = false;
    }

    private void ChangeColor()
    {
        if (keyObjectMaterial.color == COLOR_DEFAULT_KEY_OBJECT)
        {
            keyObjectMaterial.color = COLOR_CHANGE_KEY_OBJECT;
            stringMaterial.color = COLOR_CHANGE_STRING;
        }
        else
        {
            keyObjectMaterial.color = COLOR_DEFAULT_KEY_OBJECT;
            stringMaterial.color = COLOR_DEFAULT_STRING;
        }
    }

}
