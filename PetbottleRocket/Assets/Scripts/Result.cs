using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ResultSceneのManager的Script
/// </summary>
public class Result : MonoBehaviour
{
    [SerializeField] private GameObject nowScoreObject = null;
    [SerializeField] private GameObject highScoreObject = null;
    [SerializeField] private GameObject highScoreCanvas = null;

    [SerializeField] private AdMobBanner myBanner = null; //バナー広告を扱う
    [SerializeField] private AdMobInterstitial myInterstitial = null; //インタースティシャル広告を扱う

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
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }
    }

    private void Update()
    {
        // PlayerPrefs.GetFloatが時間がかかるみたい。
        // PlayerPrefs.SetFloatの方が速いらしく、
        // 読み込みが確実に終わってから書き込みする必要がある。
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

    #region Sceneジャンプ用メソッド
    public void SwitchToGameScene()
    {
        audioSource.PlayOneShot(SE_Yes);

        myInterstitial.RequestInterstitial(); //インタースティシャル広告を呼び出す
        if (myBanner)
        {
            myBanner.DestroyBanner();
        }

        FadeManager.Instance.LoadScene("Game", 0.3f);
    }

    public void SwitchToTitleScene()
    {
        audioSource.PlayOneShot(SE_Yes);
  
        myInterstitial.RequestInterstitial(); //インタースティシャル広告を呼び出す
        if (myBanner)
        {
            myBanner.DestroyBanner();
        }

       FadeManager.Instance.LoadScene("Title", 0.3f);
    }
    #endregion

    #region SNS用メソッド
    /// <summary>
    /// SNS用。ボタン押下時のみ呼び出される。
    /// </summary>
    public void PushSNS()
    {
        string url = "";
        //string image_path = "";
        string text = "";

        string aaa = "大空を";
        string strscore = nowScore.ToString("F2");
        string bbb = "m飛びまわった!!\nさあ、あなたもやってみよう(^^)/\n#ペットボトルロケット ";

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
            //text = text + "その他の機種\n";
            Debug.Log("Other OS");
        }

        //SocialConnector.SocialConnector.Share(text); //第1引数:テキスト,第2引数:URL,第3引数:画像
        SocialConnector.SocialConnector.Share(text, url); //第1引数:テキスト,第2引数:URL,第3引数:画像
        //Debug.Log("SNS");
    }
    #endregion

    private void UpdateText()
    {
        Text nowScoreText = nowScoreObject.GetComponent<Text>();
        Text highScoreText = highScoreObject.GetComponent<Text>();

        highScoreText.text = "　ハイスコア : " + highScore.ToString("F2");
        nowScoreText.text = "今回のスコア : " + nowScore.ToString("F2");
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

