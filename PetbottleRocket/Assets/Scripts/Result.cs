using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ResultScene��Manager�IScript
/// </summary>
public class Result : MonoBehaviour
{
    [SerializeField] private GameObject nowScoreObject = null;
    [SerializeField] private GameObject highScoreObject = null;
    [SerializeField] private GameObject highScoreCanvas = null;

    [SerializeField] private AdMobBanner myBanner = null; //�o�i�[�L��������
    [SerializeField] private AdMobInterstitial myInterstitial = null; //�C���^�[�X�e�B�V�����L��������

    [SerializeField] private AudioClip SE_Yes;
    private AudioSource audioSource;

    private float nowScore;
    private float highScore;

    private string prefStringNowScore = "NowScore";
    private string prefStringHighScore = "HighScore";

    private bool isOverHighScore = false;

    private float startTime = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nowScore = PlayerPrefs.GetFloat(prefStringNowScore, 0);
        highScore = PlayerPrefs.GetFloat(prefStringHighScore, 0);
        isOverHighScore = nowScore > highScore ? true : false;

        UpdateText();
        DisplayHiScoreCanvas();

        if (myBanner)
        {
            myBanner.RequestBanner(); //�o�i�[�L�����Ăяo��
        }
    }

    private void Update()
    {
        // PlayerPrefs.GetFloat�����Ԃ�������݂����B
        // PlayerPrefs.SetFloat�̕��������炵���A
        // �ǂݍ��݂��m���ɏI����Ă��珑�����݂���K�v������B
        if (startTime >1f && startTime < 10f)
        {
            UpdateHighScore();
            startTime += 20f;
        }
        else if(startTime <= 1f)
        {
            startTime += Time.deltaTime;
        }
    }

    #region Scene�W�����v�p���\�b�h
    public void SwitchToGameScene()
    {
        audioSource.PlayOneShot(SE_Yes);

        myInterstitial.RequestInterstitial(); //�C���^�[�X�e�B�V�����L�����Ăяo��
        if (myBanner)
        {
            myBanner.DestroyBanner();
        }

        FadeManager.Instance.LoadScene("Game", 0.3f);
    }

    public void SwitchToTitleScene()
    {
        audioSource.PlayOneShot(SE_Yes);
  
        myInterstitial.RequestInterstitial(); //�C���^�[�X�e�B�V�����L�����Ăяo��
        if (myBanner)
        {
            myBanner.DestroyBanner();
        }

       FadeManager.Instance.LoadScene("Title", 0.3f);
    }
    #endregion

    #region SNS�p���\�b�h
    /// <summary>
    /// SNS�p�B�{�^���������̂݌Ăяo�����B
    /// </summary>
    public void PushSNS()
    {
        string url = "";
        //string image_path = "";
        string text = "";

        string aaa = "����";
        string strscore = nowScore.ToString("F2");
        string bbb = "m��т܂����!!\n�����A���Ȃ�������Ă݂悤(^^)/\n#�y�b�g�{�g�����P�b�g ";

        text = aaa + strscore + bbb;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar.PetbottleRocket";
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

        //SocialConnector.SocialConnector.Share(text); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
        SocialConnector.SocialConnector.Share(text, url); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
        //Debug.Log("SNS");
    }
    #endregion

    private void UpdateText()
    {
        Text nowScoreText = nowScoreObject.GetComponent<Text>();
        Text highScoreText = highScoreObject.GetComponent<Text>();

        highScoreText.text = "�@�n�C�X�R�A : " + highScore.ToString("F2");
        nowScoreText.text = "����̃X�R�A : " + nowScore.ToString("F2");
    }

    private void UpdateHighScore()
    {
        if (isOverHighScore)
        {
            PlayerPrefs.SetFloat(prefStringHighScore, nowScore);
            PlayerPrefs.Save();
        }
    }

    private void DisplayHiScoreCanvas()
    {
        if(isOverHighScore)
        {
            highScoreCanvas.SetActive(true);
        }
        else
        {
            highScoreCanvas.SetActive(false);
        }
    }
}

