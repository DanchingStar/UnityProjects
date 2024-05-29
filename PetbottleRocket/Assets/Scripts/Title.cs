using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TitleScene��Manager�IScript
/// </summary>
public class Title : MonoBehaviour
{
    [SerializeField] private GameObject highScoreObject = null;
    [SerializeField] private GameObject versionObject = null;

    [SerializeField] private GameObject creditCanvas = null;
    [SerializeField] private GameObject howToPlayCanvas = null;

    [SerializeField] private AdMobBanner myBanner = null; //�o�i�[�L��������
    [SerializeField] private AdMobInterstitial myInterstitial = null; //�C���^�[�X�e�B�V�����L��������

    [SerializeField] private AudioClip SE_Yes;
    private AudioSource audioSource;

    private float highScore;

    private string prefStringHighScore = "HighScore";


    private void Start()
    {
       highScore = PlayerPrefs.GetFloat(prefStringHighScore, 0);
        Text highScoreText = highScoreObject.GetComponent<Text>();
        highScoreText.text = "�n�C�X�R�A : " + highScore.ToString("F2");

        Text versionText = versionObject.GetComponent<Text>();
        versionText.text = "Ver " + Application.version;

        audioSource = GetComponent<AudioSource>();

        if (myBanner)
        {
            myBanner.RequestBanner(); //�o�i�[�L�����Ăяo��
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
    #endregion
}
