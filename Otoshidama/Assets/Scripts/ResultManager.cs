using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : ManagerParent
{
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    [SerializeField] private GameObject objTextResultTitle;
    [SerializeField] private GameObject objTextEvaluation1;
    [SerializeField] private GameObject objTextEvaluation2;
    [SerializeField] private GameObject objTextEvaluation3;
    [SerializeField] private GameObject objTextEvaluation4;
    [SerializeField] private GameObject objTextEvaluation5;
    [SerializeField] private GameObject objTextResul1;
    [SerializeField] private GameObject objTextResul2;
    [SerializeField] private GameObject objTextResul3;
    [SerializeField] private GameObject objTextResul4;
    [SerializeField] private GameObject objTextResul5;

    [SerializeField] private Button mySNSButton;
    [SerializeField] private Button myToMenuButton;

    private Text textEvaluation1;
    private Text textEvaluation2;
    private Text textEvaluation3;   
    private Text textEvaluation4;
    private Text textEvaluation5;
    private Text textResult1;
    private Text textResult2;
    private Text textResult3;
    private Text textResult4;
    private Text textResult5;

    private string strEvaluation1;
    private string strEvaluation2;
    private string strEvaluation3;
    private string strEvaluation4;
    private string strEvaluation5;
    private string strResul1;
    private string strResul2;
    private string strResul3;
    private string strResul4;
    private string strResul5;

    private int clearNumber;
    private int recordMyMoney;
    private int recordMotherMoney;
    private int recordTotalMoney;
    private int recordItemNumber;
    private int recordHeartfulNumber;

    private int thisTimeMyMoney;
    private int thisTimeMotherMoney;
    private int thisTimeTotalMoney;

    private int[] myItemFlags = new int[Enum.GetNames(typeof(MyItemFlagsEnum)).Length];
    private int[] recordItemFlags = new int[Enum.GetNames(typeof(RecordItemEnum)).Length];
    private int thisTimeItemCount = 0;
    private int thisTimeHeartfulPoint;

    private bool isGetGame;

    void Start()
    {
        myBanner.RequestBanner();

        LoadPrefabs();
        UpdatePrefabs();
        SavePrefabs();

        ALLGetComponentText();
        UpdateTextString();
        AllStringRead();

        InitSound();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnSNSButton()
    {
        string url = "";
        //string image_path = "";
        string text = "";

        string str1 = strEvaluation1 + " : " + strResul1 + "\n";
        string str2 = strEvaluation2 + " : " + strResul2 + "\n";
        string str3 = strEvaluation3 + " : " + strResul3 + "\n";
        string str4 = strEvaluation4 + " : " + strResul4 + "\n";
        string str5 = strEvaluation5 + " : " + strResul5 + "\n";
        string str6 = "#お年玉アドベンチャー";

        text = str1 + str2 + str3 + str4 + str5 + str6;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar.Otoshidama";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + " #Android\n";
            Debug.Log("Android");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + " #iPhone\n";
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
    public void OnToMenuButton()
    {
        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    private void LoadPrefabs()
    {
        clearNumber = PlayerPrefs.GetInt("ClearNumber", 0);
        recordMyMoney = PlayerPrefs.GetInt("RecordMyMoney", 0);
        recordMotherMoney = PlayerPrefs.GetInt("RecordMotherMoney", 0);
        recordTotalMoney = PlayerPrefs.GetInt("RecordTotalMoney", 0);
        recordItemNumber = PlayerPrefs.GetInt("RecordItemNumber", 0);
        recordHeartfulNumber = PlayerPrefs.GetInt("RecordHeartfulNumber", 0);

        thisTimeMyMoney = PlayerPrefs.GetInt("HaveMoney", 0);
        thisTimeMotherMoney = PlayerPrefs.GetInt("MotherMoney", 0);
        thisTimeTotalMoney = thisTimeMyMoney + thisTimeMotherMoney;

        foreach (var i in Enum.GetValues(typeof(MyItemFlagsEnum)))
        {
            myItemFlags[(int)i] = PlayerPrefs.GetInt(i.ToString(), -1);

            if (myItemFlags[(int)i] == 1)
            {
                thisTimeItemCount++; //アイテム数を数える
            }
        }
        thisTimeHeartfulPoint = PlayerPrefs.GetInt("HeartfulPoint", 0);

        if (PlayerPrefs.GetInt("ItemEndingNormal",0) == 1 || PlayerPrefs.GetInt("ItemEndingAnother",0) == 1)
        {
            isGetGame = true;
        }
        else
        {
            isGetGame = false;
        }

        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            recordItemFlags[(int)i] = PlayerPrefs.GetInt(i.ToString(), 0);

        }
    }

    private void UpdatePrefabs()
    {
        PlayerPrefs.DeleteKey("NextSceneName"); //中断データを消す

        clearNumber++;

        if (thisTimeMyMoney > recordMyMoney )
        {
            recordMyMoney = thisTimeMyMoney;
        }

        if(thisTimeMotherMoney > recordMotherMoney )
        {
            recordMotherMoney = thisTimeMotherMoney;
        }

        if(thisTimeTotalMoney > recordTotalMoney )
        {
            recordTotalMoney = thisTimeTotalMoney;
        }

        if(thisTimeItemCount > recordItemNumber)
        {
            recordItemNumber = thisTimeItemCount;
        }

        if(thisTimeHeartfulPoint > recordHeartfulNumber)
        {
            recordHeartfulNumber = thisTimeHeartfulPoint;
        }

        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            if (recordItemFlags[(int)i] == 0)
            {
                if (myItemFlags[(int)i] == 1)
                {
                    recordItemFlags[(int)i] = 1;
                }
            }
        }
    }

    private void SavePrefabs()
    {
        PlayerPrefs.SetInt("ClearNumber", clearNumber);
        PlayerPrefs.SetInt("RecordMyMoney", recordMyMoney);
        PlayerPrefs.SetInt("RecordMotherMoney", recordMotherMoney);
        PlayerPrefs.SetInt("RecordTotalMoney", recordTotalMoney);
        PlayerPrefs.SetInt("RecordItemNumber", recordItemNumber);
        PlayerPrefs.SetInt("RecordHeartfulNumber", recordHeartfulNumber);

        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            PlayerPrefs.SetInt(i.ToString(), recordItemFlags[(int)i]);
        }

        PlayerPrefs.Save();
    }

    private void ALLGetComponentText()
    {
        textEvaluation1 = objTextEvaluation1.GetComponent<Text>();
        textEvaluation2 = objTextEvaluation2.GetComponent<Text>();
        textEvaluation3 = objTextEvaluation3.GetComponent<Text>();
        textEvaluation4 = objTextEvaluation4.GetComponent<Text>();
        textEvaluation5 = objTextEvaluation5.GetComponent<Text>();
        textResult1 = objTextResul1.GetComponent<Text>();
        textResult2 = objTextResul2.GetComponent<Text>();
        textResult3 = objTextResul3.GetComponent<Text>();
        textResult4 = objTextResul4.GetComponent<Text>();
        textResult5 = objTextResul5.GetComponent<Text>();
    }

 
    private void UpdateTextString()
    {
        if (isGetGame)
        {
            textResult1.text = "達成";
        }
        else
        {
            textResult1.text = "未達成";
        }
        textResult2.text = "￥" + thisTimeMyMoney.ToString("N0");
        textResult3.text = "￥" + thisTimeMotherMoney.ToString("N0");
        textResult4.text = thisTimeItemCount.ToString() + "個";
        textResult5.text = thisTimeHeartfulPoint.ToString() + "ポイント";
    }

    private void AllStringRead()
    {
        strEvaluation1 = textEvaluation1.text.ToString();
        strEvaluation2 = textEvaluation2.text.ToString();
        strEvaluation3 = textEvaluation3.text.ToString();
        strEvaluation4 = textEvaluation4.text.ToString();
        strEvaluation5 = textEvaluation5.text.ToString();
        strResul1 = textResult1.text.ToString();
        strResul2 = textResult2.text.ToString();
        strResul3 = textResult3.text.ToString();
        strResul4 = textResult4.text.ToString();
        strResul5 = textResult5.text.ToString();
    }

    private void InitSound()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myAudioBGM.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
        }

    }
}
