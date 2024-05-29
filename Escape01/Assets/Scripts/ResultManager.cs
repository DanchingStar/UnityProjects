using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    [SerializeField] private AudioSource seYes = null;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;
    private float valueBGM;
    private float valueSE;

    private void Start()
    {
        InitSound();
    }

    private void InitSound()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        valueBGM = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        valueSE = PlayerPrefs.GetFloat("SEVolume", 0.5f);

        myAudioBGM.volume = valueBGM;
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = valueSE;
        }
    }

    public void OnShareButton()
    {
        string url = "";
        //string image_path = "";
        string str1 = "やった！！\n";

        string str2 = "閉じ込められた部屋から抜け出したぞ！！\n";

        string str3 = "さあ、みんなも挑戦してみよう！！\n";

        string str5 = "#脱出ゲーム ";

        string str6 = "#閉鎖されたマンションの一室 ";

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
            //text = text + "その他の機種\n";
            Debug.Log("Other OS");
        }

        SocialConnector.SocialConnector.Share(text, url); //第1引数:テキスト,第2引数:URL,第3引数:画像
    }

    public void OnTitleButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }
        FadeManager.Instance.LoadScene("Menu", 1.5f);
    }

}
