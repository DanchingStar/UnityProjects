using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : ManagerParent
{
    public struct ItemInformationString
    {
        public string itemName;
        public string itemGetInformation;
        public string itemNoneInformation;
    }

    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    [SerializeField] private Text textAchievementRate;

    [SerializeField] private GameObject informationPanel;
    [SerializeField] private Image informationItemImage;
    [SerializeField] private Text itemImageNameText;
    [SerializeField] private Text itremImageInformationText;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Image[] itemImage;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    private int[] recordItemFlags = new int[Enum.GetNames(typeof(RecordItemEnum)).Length];

    private bool informationFlg = false;

    private int itemCount;
    private int totalItemCount;
    private float achievementRate;

    private ItemInformationString[] itemInformationString = new ItemInformationString[Enum.GetNames(typeof(RecordItemEnum)).Length];


    // Start is called before the first frame update
    void Start()
    {
        {
            myBanner.RequestBanner();

            LoadPrefabs();
            InformationSetting();
            DisplayItemImage();

            AchievementRateTextSetting();

            InitSound();
        }
    }
    // Update is called once per frame
    void Update()
    {

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

    private void LoadPrefabs()
    {
        itemCount = 0;
        totalItemCount = 0;
        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            recordItemFlags[(int)i] = PlayerPrefs.GetInt(i.ToString(), 0);
            totalItemCount++;
            if (recordItemFlags[(int)i]==1)
            {
                itemCount++;
            }

            Debug.Log($"{i.ToString()} : {recordItemFlags[(int)i]}");
        }
        achievementRate = (float)itemCount / (float)totalItemCount * 100f;
    }

    private void AchievementRateTextSetting()
    {
        textAchievementRate.text = "達成率 : " + achievementRate.ToString("F0") + "%";

        if (achievementRate >= 99)
        {
            StartCoroutine(_rainbow(textAchievementRate, 3f));
        }
        else if (achievementRate >= 80)
        {
            textAchievementRate.color = Color.red;
        }
        else if (achievementRate >= 50)
        {
            textAchievementRate.color = new Color(0.5f,0f,1f,1f);
        }
        else
        {
            textAchievementRate.color = Color.blue;
        }
    }

    private void DisplayItemImage()
    {
        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            if (recordItemFlags[(int)i] == 1)
            {
                itemImage[(int)i].sprite = itemSprites[(int)i];
            }
            else
            {
                itemImage[(int)i].sprite = noneSprite;
            }
        }
    }
    public void OnImageObject(int n)
    {
        if (informationFlg) return;
     
        if (recordItemFlags[n] ==1)
        {
            informationItemImage.sprite = itemSprites[n];
            itemImageNameText.text = itemInformationString[n].itemName;
            itremImageInformationText.text = itemInformationString[n].itemGetInformation;
        }
        else
        {
            informationItemImage.sprite = noneSprite;
            itemImageNameText.text = "？？？";
            itremImageInformationText.text = itemInformationString[n].itemNoneInformation;
        }

        informationFlg = true;
        informationPanel.SetActive(true);

    }

    public void OnToBackButton()
    {
        informationFlg = false;
        informationPanel.SetActive(false);
    }

    public void OnSNSButton()
    {
        string url = "";
        //string image_path = "";
        string text = "";

        string str1 = "コレクション達成率 : " + achievementRate.ToString("F0") + "%!\n";

        string str2 = "";
        if (achievementRate >= 99f)
        {
            str2 = "コンプリートおめでとう!!!\n";
        }
        else if(achievementRate >= 80f)
        {
            str2 = "コンプリートまであと少し!!\n";
        }
 
        string str9 = "#お年玉アドベンチャー";

        text = str1 + str2 + str9;

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

    IEnumerator _rainbow(Text text, float speed)
    {
        //無限ループ
        while (true)
        {
            //カラーを変化させる処理
            text.color = Color.HSVToRGB(Time.time / speed % 1, 1, 1);
            //1フレーム待つ
            yield return new WaitForFixedUpdate();
        }
    }

    private void InformationSetting()
    {
        itemInformationString[0].itemName = "栄養ドリンク";
        itemInformationString[1].itemName = "アメちゃん";
        itemInformationString[2].itemName = "古い万年筆";
        itemInformationString[3].itemName = "遊園地のチケット";
        itemInformationString[4].itemName = "キラキラした石";
        itemInformationString[5].itemName = "ショルダーバッグ";
        itemInformationString[6].itemName = "羽子板";
        itemInformationString[7].itemName = "福笑い";
        itemInformationString[8].itemName = "野球のグローブ";
        itemInformationString[9].itemName = "くまのぬいぐるみ";
        itemInformationString[10].itemName = "大人っぽい財布";
        itemInformationString[11].itemName = "レスラーの応援うちわ";
        itemInformationString[12].itemName = "欲しかったゲーム機";
        itemInformationString[13].itemName = "パパからもらったゲーム機";
        itemInformationString[14].itemName = "ママが編んでくれたマフラー";
        itemInformationString[15].itemName = "ハズレ馬券";
        itemInformationString[16].itemName = "桜柄のしおり";
        itemInformationString[17].itemName = "かわいい和菓子";

        itemInformationString[0].itemNoneInformation = "12月31日にゲット可能";
        itemInformationString[1].itemNoneInformation = "12月31日にゲット可能";
        itemInformationString[2].itemNoneInformation = "1月1日にゲット可能";
        itemInformationString[3].itemNoneInformation = "1月1日にゲット可能";
        itemInformationString[4].itemNoneInformation = "1月1日にゲット可能";
        itemInformationString[5].itemNoneInformation = "1月2日にゲット可能";
        itemInformationString[6].itemNoneInformation = "1月2日にゲット可能";
        itemInformationString[7].itemNoneInformation = "1月2日にゲット可能";
        itemInformationString[8].itemNoneInformation = "1月2日にゲット可能";
        itemInformationString[9].itemNoneInformation = "1月2日にゲット可能";
        itemInformationString[10].itemNoneInformation = "1月3日にゲット可能";
        itemInformationString[11].itemNoneInformation = "1月4日にゲット可能";
        itemInformationString[12].itemNoneInformation = "ノーマルエンディング到達でゲット可能";
        itemInformationString[13].itemNoneInformation = "ノーマルエンディングアナザー到達でゲット可能";
        itemInformationString[14].itemNoneInformation = "グッドエンディング到達でゲット可能";
        itemInformationString[15].itemNoneInformation = "バッドエンディング到達でゲット可能";
        itemInformationString[16].itemNoneInformation = "特別エンディング1到達でゲット可能";
        itemInformationString[17].itemNoneInformation = "特別エンディング2到達でゲット可能";

        itemInformationString[0].itemGetInformation = "祖父からもらった飲み物。自販機とかでたまに見かけるけど、他の飲み物と比べて内容量が少ないのに値段が変わらないのなんだかもったいなく感じるよね。";
        itemInformationString[1].itemGetInformation = "祖母からもらったまんまるキャンディ。アメちゃんと呼ぶのは関西圏の人に多いイメージ。このゲームの中でもっとも現実でももらえそうな物はこれかもしれないね。";
        itemInformationString[2].itemGetInformation = "祖父からもらった筆記用具。最近では万年筆を実際に見たことない人も多いのでは？普段から使いこなしている人ってエレガントでカッコいい印象を持っちゃいます。";
        itemInformationString[3].itemGetInformation = "祖母からもらったチケット。ここでなぞなぞです！世の中にはいろんなチケットがあるけど、他人に配慮や心遣いをするチケットってな～んだ？正解は[エチケット]でした！";
        itemInformationString[4].itemGetInformation = "祖父母の家に遊びに来た三毛猫からもらった輝く石。川辺などにある石や宝石店で取り扱っている石の違いは何なのだろうか。見た目？レア度？教えて先生(^^)/";
        itemInformationString[5].itemGetInformation = "叔母からもらったバッグ。バッグにもいろいろな種類があるけど、ほぼ何も入らない大きさのマイクロバッグという物の存在には驚いたね。物の概念を覆す発想力に脱帽。";
        itemInformationString[6].itemGetInformation = "兄と姉からもらった羽子板。羽子板の使い道といえば羽根つきが一般的だと思っていたけど、正月の飾りとして用いるという使い方もあるらしいね。ウチには無かったな。";
        itemInformationString[7].itemGetInformation = "伯父からもらった福笑い。定番は[おかめ]や[ひょっとこ]かな？友達や兄弟など、知人の顔で福笑いを作って遊ぶのも面白いよ！ただ、本人には許可を取って楽しくね！";
        itemInformationString[8].itemGetInformation = "兄からもらった野球のグローブ。買ったばかりで新品のグローブだと手に馴染まなくて使いづらいよね。使いやすいグローブはたくさん練習してきた証なのかも？";
        itemInformationString[9].itemGetInformation = "姉からもらったくまのぬいぐるみ。ゲームセンターなどのクレーンゲームでは今でもぬいぐるみが定番かな？あれ、難しすぎないか？一度くらい獲ってみたいなぁ…。";
        itemInformationString[10].itemGetInformation = "父からもらった長財布。大人になってくるとカード類が増えてきて財布が大きくなりがち。キャッシュレス化からカードレス化が進むと財布も小さくなっていくのかな？";
        itemInformationString[11].itemGetInformation = "父からもらったうちわ。「アイドルの応援ではよく見かけるけど、プロレスの応援でもうちわなんて使うの？」と思っているそこのあなた！実際に使う人もいるんですよ！";
        itemInformationString[12].itemGetInformation = "お年玉で買ったゲーム機。最近はスマホが普及して、携帯ゲーム機が少なりつつある傾向。現代の子どもさんからすると[通信ケーブル]とか信じられないんだろうなぁ…。";
        itemInformationString[13].itemGetInformation = "父が買ってくれたゲーム機。ゲーム機に限らず車やスマホなど、中身は全く同じ機能でも、カラーのバリエーションが複数あるだけで、ユーザーは選ぶのワクワクするよね。";
        itemInformationString[14].itemGetInformation = "母が夜なべして編んでくれたマフラー。母から何をもらったら「まぁ許してやるか」ってなるかなと考えて出てきたのがマフラーだった。ちなみに作者なら絶対に許さない。";
        itemInformationString[15].itemGetInformation = "父から押し付けられたゴミ。レースの結果次第ではお宝になりえる。たった数分で価値がここまで大きく変わるものは他にあるだろうか？いや、ない。（古文の反語的な文）";
        itemInformationString[16].itemGetInformation = "知らない男性からもらったしおり。なんだかんだ最近では本もデータ化されつつあり、しおりを持っている人も減ってきたのでは？時代についていくのも大変ですね。";
        itemInformationString[17].itemGetInformation = "知らない女性からもらった和菓子。味だけではなく見た目にも凝っている和菓子を見かけると、職人の情熱を感じるよね。まぁそれは和菓子に限った話じゃないか。";
    }
}
