using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource seYes = null;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject HintPanel;
    [SerializeField] GameObject TitlePanel;
    [SerializeField] GameObject SoundPanel;

    [SerializeField] private Slider myBGMSlider;
    [SerializeField] private Slider mySESlider;
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    private void Start()
    {
        InitSlider();

        menuPanel.SetActive(false);
    }

    public void OnMenuButton()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        if (seYes != null)
        {
            seYes.Play();
        }

        menuPanel.SetActive(true);
    }

    public void OnMenuBackButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        menuPanel.SetActive(false);
    }

    public void OnHintButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        HintPanel.SetActive(true);
    }

    public void OnTitleButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        TitlePanel.SetActive(true);
    }

    public void OnSoundButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        SoundPanel.SetActive(true);
    }

    public void OnSomethingBackButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        if (SoundPanel.activeSelf)
        {
            SoundPanel.SetActive(false);
            SetSliderPrefs();
        }
        else
        {
            HintPanel.SetActive(false);
            TitlePanel.SetActive(false);
        }
    }

    public void OnDecideTitleButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    public void OnShareButton()
    {
        string url = "";
        //string image_path = "";

        string str1 = "�C���t����������߂��Ă�I�H\n";

        string str2 = "��������ĕ������甲���o�����I�I\n";

        string str3 = "�����A�݂�Ȃ����킵�Ă݂悤�I�I\n";

        string str5 = "#�E�o�Q�[�� ";

        string str6 = "#�����ꂽ�}���V�����̈ꎺ ";

        string text = str1 + str2 + str3 + str5 + str6;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar.Escape01";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#Android\n";
            Debug.Log("Android");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#iPhone\n";
            Debug.Log("iPhone");
        }
        else
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            //text = text + "���̑��̋@��\n";
            Debug.Log("Other OS");
        }

        SocialConnector.SocialConnector.Share(text, url); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
    }

    private void InitSlider()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myBGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        mySESlider.value = PlayerPrefs.GetFloat("SEVolume", 0.5f);

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
        PlayerPrefs.Save();
    }

}
