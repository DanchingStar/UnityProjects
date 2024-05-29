using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuManager : ManagerParent
{
    [SerializeField] private Text versionText;
    [SerializeField] private GameObject myRecordPanel;
    [SerializeField] private GameObject myOptionPanel;
    [SerializeField] private GameObject myCreditPanel;

    [SerializeField] private Button myStartButton;
    [SerializeField] private Button myContinueButton;
    [SerializeField] private Button myCollectionButton;
    [SerializeField] private Button myRecordButton;
    [SerializeField] private Button myConfigurationButton;
    [SerializeField] private Button myCreditButton;

    [SerializeField] private Slider myBGMSlider;
    [SerializeField] private Slider mySESlider;
    [SerializeField] private Slider mySpeedSlider;
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    [SerializeField] private ScrollRect creditTextScrollRect;
    [SerializeField] private ScrollRect recordTextScrollRect;

    [SerializeField] private Text recordText;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    void Start()
    {
        myBanner.RequestBanner();

        versionText.text = $"Ver {Application.version}";

        if (!PlayerPrefs.HasKey("ClearNumber"))
        {
            myCollectionButton.interactable = false;
            myRecordButton.interactable = false;
        }
        if (!PlayerPrefs.HasKey("NextSceneName"))
        {
            myContinueButton.interactable = false;
        }

        InitSlider();

        SetRecordText();
    }


    private void InitSlider()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myBGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        mySESlider.value = PlayerPrefs.GetFloat("SEVolume", 0.5f);
        mySpeedSlider.value = PlayerPrefs.GetFloat("TextSpeed", 0.1f);

        myAudioBGM.volume = myBGMSlider.value;
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = mySESlider.value;
        }
        //captionSpeed = mySpeedSlider.value;

        myBGMSlider.onValueChanged.AddListener(value => myAudioBGM.volume = value);
        foreach (Transform childTF in myAudioTF)
        {
            mySESlider.onValueChanged.AddListener(value => childTF.gameObject.GetComponent<AudioSource>().volume = value);
        }
        //mySpeedSlider.onValueChanged.AddListener(value => captionSpeed = value);
    }

    private void SetSliderPrefs()
    {
        PlayerPrefs.SetFloat("BGMVolume", myBGMSlider.value);
        PlayerPrefs.SetFloat("SEVolume", mySESlider.value);
        PlayerPrefs.SetFloat("TextSpeed", mySpeedSlider.value);
        PlayerPrefs.Save();
    }

    private void SetRecordText()
    {
        string recordStr = "\n";

        for (int i = 0; i < 6; i++) 
        {
            int num;
            string str = "";
            switch (i) 
            {
                case 0:
                    num = PlayerPrefs.GetInt("ClearNumber");
                    str += "クリア回数 : ";
                    break;
                case 1:
                    num = PlayerPrefs.GetInt("RecordMyMoney");
                    str += "最高所持金 : ";
                    break;
                case 2:
                    num = PlayerPrefs.GetInt("RecordMotherMoney");
                    str += "最高預けたお金 : ";
                    break;
                case 3:
                    num = PlayerPrefs.GetInt("RecordTotalMoney");
                    str += "最高トータル金額 : ";
                    break;
                case 4:
                    num = PlayerPrefs.GetInt("RecordItemNumber");
                    str += "最高アイテムゲット数 : ";
                    break;
                case 5:
                    num = PlayerPrefs.GetInt("RecordHeartfulNumber");
                    str += "最高ハートフルポイント : ";
                    break;
                default:
                    num = 0;
                    break;
            }
            str += num.ToString();
            str += "\n";
            recordStr += str;
        }
        recordText.text = recordStr;
    }

    public void OnStartButton()
    {
        FadeManager.Instance.LoadScene("Game_Prologue", 0.3f);
    }

    public void OnContinueButton()
    {
        string nextStr = PlayerPrefs.GetString("NextSceneName");


        FadeManager.Instance.LoadScene(nextStr, 0.3f);
    }

    public void OnCollectionButton()
    {
        FadeManager.Instance.LoadScene("Collection", 0.3f);
    }

    public void OnRecordButton()
    {
        myRecordPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(recordTextScrollRect.verticalScrollbar.gameObject);
        recordTextScrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnOptionButton()
    {
        myOptionPanel.SetActive(true);
    }

    public void OnCreditButton()
    {
        myCreditPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(creditTextScrollRect.verticalScrollbar.gameObject);
        creditTextScrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnRecordBackButton()
    {
        myRecordPanel.SetActive(false);
    }

    public void OnOptionBackButton()
    {
        SetSliderPrefs();
        myOptionPanel.SetActive(false);
    }

    public void OnCreditBackButton()
    {
        myCreditPanel.SetActive(false);
    }

}
