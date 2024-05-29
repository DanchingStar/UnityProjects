using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomDiceButtonPrefab : MonoBehaviour
{
    [SerializeField] private Image[] backgroundImages;

    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private GameObject blackPanel;

    private Image[] eyeImages;

    private GameObject parentPanel;

    private int myMysetNumber;

    private bool finishStartFlg = false;

    private readonly Color backgroundColor = new Color(0.5f, 0.5f, 0.5f);

    private const string MENU_SCENE_NAME = "Menu";
    private const string GAME_SCENE_NAME = "Game";

    private void Start()
    {
        if (finishStartFlg) return;
        finishStartFlg = true;

        eyeImages = new Image[backgroundImages.Length];
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            eyeImages[i] = backgroundImages[i].transform.GetChild(0).GetComponent<Image>();
        }

        blackPanel.SetActive(false);
    }

    public void SetMyStatus(GameObject _panel, int _MyNumber)
    {
        if (!finishStartFlg) Start();

        parentPanel = _panel;
        myMysetNumber = _MyNumber;

        MyDice myDice = new MyDice();
        myDice = SaveDataManager.Instance.GetMyDice(myMysetNumber);

        nameText.text = myDice.name;

        for (int i = 0; i < backgroundImages.Length; i++)
        {
            backgroundImages[i].color = backgroundColor;
            if (myDice.changeEyes[i + 1] == DiceController.DiceEyeKinds.None)
            {
                eyeImages[i].sprite = DiceController.Instance.GetEyeSprite((DiceController.DiceEyeKinds)(i + 1));
                eyeImages[i].color = Color.white;
            }
            else
            {
                eyeImages[i].sprite = DiceController.Instance.GetEyeSprite(myDice.changeEyes[i + 1]);
                eyeImages[i].color = Color.red;
            }
        }

        //Debug.Log($"CustomDiceButtonPrefab.SetMyStatus : Number = {myMysetNumber} , name = {nameText.text}");
    }

    public void SetMyStatusDefault(GameObject _panel)
    {
        if (!finishStartFlg) Start();

        parentPanel = _panel;
        myMysetNumber = -1;
        nameText.text = DiceController.Instance.GetDefaultDiceName();

        for (int i = 0; i < backgroundImages.Length; i++)
        {
            backgroundImages[i].color = backgroundColor;
            eyeImages[i].sprite = DiceController.Instance.GetEyeSprite((DiceController.DiceEyeKinds)(i+1));
            eyeImages[i].color = Color.white;
        }

        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            gameObject.GetComponent<Button>().interactable = false;
            blackPanel.SetActive(true);
        }

        //Debug.Log($"CustomDiceButtonPrefab.SetMyStatusDefault");
    }

    public void PushMyButton()
    {
        if (SceneManager.GetActiveScene().name == MENU_SCENE_NAME)
        {
            parentPanel.GetComponent<EditDiceMenuPanelPrefab>().ReceptionCustomDiceButtonPrefab(myMysetNumber);
        }
        else if (SceneManager.GetActiveScene().name == GAME_SCENE_NAME)
        {
            parentPanel.GetComponent<CustomDicePanelPrefab>().ReceptionCustomDiceButtonPrefab(myMysetNumber);
        }
        else
        {
            Debug.LogWarning($"CustomDiceButtonPrefab.PushMyButton : Scene Name Error = {SceneManager.GetActiveScene().name}");
        }
    }

}
