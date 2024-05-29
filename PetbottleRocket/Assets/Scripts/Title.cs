using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TitleSceneのManager的Script
/// </summary>
public class Title : MonoBehaviour
{
    [SerializeField] private GameObject highScoreObject = null;
    [SerializeField] private GameObject versionObject = null;

    [SerializeField] private GameObject creditCanvas = null;
    [SerializeField] private GameObject howToPlayCanvas = null;

    [SerializeField] private AdMobBanner myBanner = null; //バナー広告を扱う
    [SerializeField] private AdMobInterstitial myInterstitial = null; //インタースティシャル広告を扱う

    [SerializeField] private AudioClip SE_Yes;
    private AudioSource audioSource;

    private float highScore;

    private string prefStringHighScore = "HighScore";


    private void Start()
    {
       highScore = PlayerPrefs.GetFloat(prefStringHighScore, 0);
        Text highScoreText = highScoreObject.GetComponent<Text>();
        highScoreText.text = "ハイスコア : " + highScore.ToString("F2");

        Text versionText = versionObject.GetComponent<Text>();
        versionText.text = "Ver " + Application.version;

        audioSource = GetComponent<AudioSource>();

        if (myBanner)
        {
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }
    }

    public void PushCredit()
    {
        creditCanvas.SetActive(true);
        audioSource.PlayOneShot(SE_Yes);
    }
    public void PushHowToPlay()
    {
        howToPlayCanvas.SetActive(true);
        audioSource.PlayOneShot(SE_Yes);
    }
    public void PushCreditReturn()
    {
        creditCanvas.SetActive(false);
        audioSource.PlayOneShot(SE_Yes);
    }
    public void PushHowToPlayReturn()
    {
        howToPlayCanvas.SetActive(false);
        audioSource.PlayOneShot(SE_Yes);
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
    #endregion
}
