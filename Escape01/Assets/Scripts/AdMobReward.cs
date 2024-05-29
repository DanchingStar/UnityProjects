using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class AdMobReward : MonoBehaviour
{
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-7223826824285484/4435429253";  //本番
    //private string adUnitId = "ca-app-pub-3940256099942544/5224354917";  //テスト
#elif UNITY_IOS
    //private string adUnitId = "広告ユニットIDをコピペ（iOS）";  //本番
    private string adUnitId = "ca-app-pub-3940256099942544/1712485313";  //テスト
#else
    private string adUnitId = "unexpected_platform";
#endif

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Text rewardText;

    [SerializeField] private AudioSource seYes = null;
    [SerializeField] private AudioSource showReward = null;

    private void Start()
    {
        //MobileAds.Initialize(initStatus => { }); //アプリ起動時に一度必ず実行（他のスクリプトで実行していたら不要）

        this.rewardedAd = new RewardedAd(adUnitId);

        // Load成功時に実行する関数の登録
        //this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Load失敗時に実行する関数の登録
        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // 表示時に実行する関数の登録
        //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // 表示失敗時に実行する関数の登録
        //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // 報酬受け取り時に実行する関数の登録
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // 広告を閉じる時に実行する関数の登録
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        RequestReward(); //広告をロード

        rewardText.text = "";
    }

    /// <summary>
    /// Reward広告をロード
    /// </summary>
    private void RequestReward()
    {
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    /// <summary>
    /// 広告とのインタラクションでユーザーに報酬を与えるべきときに呼び出されるハンドル
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardContent();
    }

    /// <summary>
    /// 広告が閉じられたときに呼び出されるハンドル
    /// </summary>
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestReward();

        rewardPanel.SetActive(true);
    }

    /// <summary>
    /// これを呼べば動画が流れる（例えばボタン押下時など）
    /// </summary>
    public void ShowReawrd()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        if (this.rewardedAd.IsLoaded())
        {
            hintPanel.SetActive(false);

            this.rewardedAd.Show();
        }
    }

    /// <summary>
    /// Reward報酬の内容
    /// </summary>
    private void RewardContent()
    {
        if (showReward != null)
        {
            showReward.Play();
        }

        rewardText.text = MakeRewardText();
    }

    /// <summary>
    /// RewardPanelを閉じるとき
    /// </summary>
    public void CloseRewardPanel()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        rewardText.text = "";
        rewardPanel.SetActive(false);
    }

    /// <summary>
    /// Rewardで得られるヒントの文言を作る
    /// </summary>
    /// <returns>ヒントの文章</returns>
    private string MakeRewardText()
    {
        string str = "リワードエラー001\n報酬のテキストがまだ\n設定されていません。";
        GimmickName.Type nowGimmick = GimmickName.Type.None;

        foreach (GimmickName.Type value in Enum.GetValues(typeof(GimmickName.Type)))
        {
            if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(GimmickName.Type), value), 0) && value != GimmickName.Type.None)
            {
                nowGimmick = value;
                break;
            }
        }

        Debug.Log(nowGimmick);

        switch (nowGimmick)
        {
            case GimmickName.Type.None:
                str = "全ての謎を解いたよ！\n" +
                      "完全制覇だね(^^)\n" +
                      "開いた出口から脱出しよう！！";
                break;
            case GimmickName.Type.RockPaperScissors:
                str = "緑の扉の横にあるパネルを\n" +
                      "触ってみよう！\n" +
                      "廊下の壁に飾っている絵が\n" +
                      "ヒントだよ！\n" +
                      "[石][はさみ][紙]に\n" +
                      "対応するように、\n" +
                      "[グー][チョキ][パー]の\n" +
                      "パネルを合わせよう！！";
                break;
            case GimmickName.Type.Books:
                str = "本棚にある本に注目！\n" +
                      "壁に飾っているワインの下にある\n" +
                      "引き出しのある家具の仕掛けと\n" +
                      "同じ配置だね！\n" +
                      "本棚の本の位置と色に合わせて\n" +
                      "仕掛けの色を変えてみよう！！";
                break;
            case GimmickName.Type.Clock:
                str = "冷蔵庫と開いた緑の扉との間に\n" +
                      "４つの仕掛けがついている\n" +
                      "家具があるね！\n" +
                      "真ん中の下の仕掛けは\n" +
                      "時間を入力すると開くみたい！\n" +
                      "部屋の中にある時計を見つけて\n" +
                      "時間を入力してみよう！！";
                break;
            case GimmickName.Type.Mugcups:
                str = "冷蔵庫と開いた緑の扉との間に\n" +
                      "４つの仕掛けがついている\n" +
                      "家具があるね！\n" +
                      "右の仕掛けは\n" +
                      "何に対応しているのかな？\n" +
                      "TVとソファの間にある机の上に\n" +
                      "５つのマグカップが置いてるね！\n" +
                      "仕掛けに対応している場所に\n" +
                      "色を合わせてみよう！！";
                break;
            case GimmickName.Type.Chess:
                str = "赤い扉とコンピュータの間に\n" +
                      "仕掛けがついてる家具があるね！\n" +
                      "この格子状の盤はなんだろう？\n" +
                      "TVとソファの間にある机の上に\n" +
                      "チェス盤が置いてあるね！\n" +
                      "盤には白と黒の駒が\n" +
                      "たくさん置いてあるね！\n" +
                      "仕掛けの盤にチェス盤の駒と\n" +
                      "色が対応するように\n" +
                      "駒を置いてみよう！！\n" +
                      "多いけどゆっくりでOKだよ^^";
                break;
            case GimmickName.Type.ComputerPowerSupply:
                str = "デスクの上に\n" +
                      "コンピュータがあるね！\n" +
                      "画面が真っ暗だけど、\n" +
                      "どうやらまだ電源が\n" +
                      "入っていないみたい…\n" +
                      "モニターの右横の機械の\n" +
                      "青色の電源ボタンを\n" +
                      "押してみよう！！";
                break;
            case GimmickName.Type.UseTvRemoteController:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.TvRemoteController), 0))
                {
                    str = "冷蔵庫と開いた緑の扉との間に\n" +
                          "４つの仕掛けがついている\n" +
                          "家具があるね！\n" +
                          "右の仕掛けを解いたので\n" +
                          "その扉が開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "大きなTVが\n" +
                          "壁に設置されているね！\n" +
                          "画面が真っ暗だけど\n" +
                          "電源を入れられないかな？\n" +
                          "持っているリモコンを選んで\n" +
                          "TVに使ってみよう！！";
                }
                break;
            case GimmickName.Type.PCMonitor:
                str = "冷蔵庫に仕掛けがついているね！\n" +
                      "仕掛けのボタンを押すと\n" +
                      "棒の長さが変わるみたい！\n" +
                      "どこかにヒントはないかな？\n" +
                      "電源を入れたコンピュータに\n" +
                      "何かのグラフが描いているよ！\n" +
                      "グラフの長さに対応するように\n" +
                      "冷蔵庫の仕掛けの長さを\n" +
                      "合わせてみよう！！";
                break;
            case GimmickName.Type.UseMatchSet:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchBou), 0))
                {
                    str = "冷蔵庫と開いた緑の扉との間に\n" +
                          "４つの仕掛けがついている\n" +
                          "家具があるね！\n" +
                          "真ん中の下の仕掛けを解いたので\n" +
                          "その引き出しが開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchBako), 0))
                {
                    str = "赤い扉とコンピュータの間に\n" +
                          "仕掛けがついてる家具があるね！\n" +
                          "この仕掛けを解いたので\n" +
                          "下の引き出しが開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchSet), 0))
                {
                    str = "マッチ棒とマッチ箱を拾ったね！\n" +
                          "どちらかのアイテムを\n" +
                          "選択してみよう！";
                }
                else
                {
                    str = "赤い扉の横に燭台があるね！\n" +
                          "白いロウソクがあるけど\n" +
                          "火はついていないみたい…\n" +
                          "持っているマッチを使って\n" +
                          "ロウソクに火をつけてみよう！！";
                }
                break;
            case GimmickName.Type.Kotatsu:
                str = "冷蔵庫と開いた緑の扉との間に\n" +
                      "４つの仕掛けがついている\n" +
                      "家具があるね！\n" +
                      "左の仕掛けは\n" +
                      "何に対応しているのかな？\n" +
                      "寝室のこたつ机の周りに\n" +
                      "４つの座椅子が置いてあるよ！\n" +
                      "仕掛けに対応している場所に\n" +
                      "色を合わせてみよう！！";
                break;
            case GimmickName.Type.TVMonitor:
                str = "寝室に６つの引き出しがある\n" +
                      "白いキャビネットがあるね！\n" +
                      "上には小さな箱があるけど\n" +
                      "まだ開かないみたい…\n" +
                      "ところでこのキャビネットって\n" +
                      "どこかでみたような…？\n" +
                      "TVに映っているキャビネットと\n" +
                      "同じように引き出しを\n" +
                      "開いてみよう！！";
                break;
            case GimmickName.Type.Wine:
                str = "寝室の大きな鏡の右横に\n" +
                      "ロッカーがあるね！\n" +
                      "特徴的な形の仕掛けだけど、\n" +
                      "どこかで見なかったかな？\n" +
                      "壁に飾ってあるワインと\n" +
                      "とても似ているね！\n" +
                      "ワインの有無や向きを\n" +
                      "仕掛けの場所と対応させて\n" +
                      "合わせてみよう！！";
                break;
            case GimmickName.Type.UseRing:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Ring), 0))
                {
                    str = "寝室に６つの引き出しがある\n" +
                          "白いキャビネットがあるね！\n" +
                          "上には小さな箱があるけど\n" +
                          "仕掛けを解いたので開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "本棚に手のオブジェがあるね！\n" +
                          "せっかく指輪を拾ったので、\n" +
                          "この手に嵌めてみよう！！";
                }
                break;
            case GimmickName.Type.DuckAndPigeon:
                str = "冷蔵庫と開いた緑の扉との間に\n" +
                      "４つの仕掛けがついている\n" +
                      "家具があるね！\n" +
                      "真ん中の上の仕掛けは\n" +
                      "アヒルとハトの数を\n" +
                      "両方とも入力するみたい…\n" +
                      "寝室のロッカーに\n" +
                      "アヒルのおもちゃと\n" +
                      "ハトの置物がたくさんあるね！\n" +
                      "それぞれの数を数えて\n" +
                      "仕掛けに入力してみよう！！";
                break;
            case GimmickName.Type.UseKey:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.BrokenKeyA), 0))
                {
                    str = "ベッドの足元の方向に\n" +
                          "木の色で小さめの\n" +
                          "キャビネットがあるね！\n" +
                          "上の引き出しは\n" +
                          "鍵がかかっているみたい…\n" +
                          "でも、どうやら下の引き出しは\n" +
                          "開いているみたいだね！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.BrokenKeyB), 0))
                {
                    str = "壁に飾っているワインの下にある\n" +
                          "引き出しのある家具があるね！\n" +
                          "この仕掛けは解いたので、\n" +
                          "下の引き出しは開いているよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Glue), 0))
                {
                    str = "冷蔵庫と開いた緑の扉との間に\n" +
                          "４つの仕掛けがついている\n" +
                          "家具があるね！\n" +
                          "左の仕掛けを解いたので\n" +
                          "その扉が開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Key), 0))
                {
                    str = "壊れた鍵を２つ拾ったね！\n" +
                          "もともと１つだったみたい！\n" +
                          "接着剤を持っているので\n" +
                          "くっつけてみよう！！\n" +
                          "壊れた鍵か接着剤の\n" +
                          "いずれかのアイテムを\n" +
                          "選択してみよう！";
                }
                else
                {
                    str = "ベッドの足元の方向に\n" +
                          "木の色で小さめの\n" +
                          "キャビネットがあるね！\n" +
                          "上の引き出しは\n" +
                          "鍵がかかっているみたい…\n" +
                          "接着剤でくっつけた鍵を使ったら\n" +
                          "開くんじゃないかな？\n" +
                          "アイテム欄の鍵を選択して\n" +
                          "鍵穴に使ってみよう！！";
                }
                break;
            case GimmickName.Type.UseHisha:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Hisha), 0))
                {
                    str = "冷蔵庫と開いた緑の扉との間に\n" +
                          "４つの仕掛けがついている\n" +
                          "家具があるね！\n" +
                          "真ん中の上の仕掛けを解いたので\n" +
                          "その引き出しが開いたよ！\n" +
                          "中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "寝室に将棋盤が置いてあるね！\n" +
                          "でも、いくつかの駒が\n" +
                          "足りていないみたい…\n" +
                          "持っている[飛]の駒を使って\n" +
                          "将棋盤の足りていない場所に\n" +
                          "駒を置いてみよう！！\n" +
                          "でも将棋のルールを知らなくて\n" +
                          "置く場所が分からない(-_-;)\n" +
                          "向こう側に置いてある駒と\n" +
                          "同じ場所に同じ駒を置くと\n" +
                          "大丈夫だよ^^";
                }
                break;
            case GimmickName.Type.UseKin:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Kin), 0))
                {
                    str = "ベッドの足元の方向に\n" +
                          "木の色で小さめの\n" +
                          "キャビネットがあるね！\n" +
                          "上の引き出しは\n" +
                          "鍵がかかっていたけど、\n" +
                          "もう開けたよね！\n" +
                          "引き出しの中にあるアイテムを\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "寝室に将棋盤が置いてあるね！\n" +
                          "でも、いくつかの駒が\n" +
                          "足りていないみたい…\n" +
                          "持っている[金将]の駒を使って\n" +
                          "将棋盤の足りていない場所に\n" +
                          "駒を置いてみよう！！\n" +
                          "でも将棋のルールを知らなくて\n" +
                          "置く場所が分からない(-_-;)\n" +
                          "向こう側に置いてある駒と\n" +
                          "同じ場所に同じ駒を置くと\n" +
                          "大丈夫だよ^^";
                }
                break;
            case GimmickName.Type.UseKei:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Kei), 0))
                {
                    str = "冷蔵庫と開いた緑の扉との間に\n" +
                          "４つの仕掛けがついている\n" +
                          "家具があるね！\n" +
                          "真ん中の引き出しより上に\n" +
                          "将棋の駒が置いてあるね！\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "寝室に将棋盤が置いてあるね！\n" +
                          "でも、いくつかの駒が\n" +
                          "足りていないみたい…\n" +
                          "持っている[桂]の駒を使って\n" +
                          "将棋盤の足りていない場所に\n" +
                          "駒を置いてみよう！！\n" +
                          "でも将棋のルールを知らなくて\n" +
                          "置く場所が分からない(-_-;)\n" +
                          "向こう側に置いてある駒と\n" +
                          "同じ場所に同じ駒を置くと\n" +
                          "大丈夫だよ^^";
                }
                break;
            case GimmickName.Type.ShogiBan:
                str = "寝室に将棋盤が置いてあるね！\n" +
                      "でも、いくつかの駒が\n" +
                      "足りていないみたい…\n" +
                      "持っている駒を使って\n" +
                      "将棋盤の足りていない場所に\n" +
                      "駒を置いてみよう！！\n" +
                      "でも将棋のルールを知らなくて\n" +
                      "置く場所が分からない(-_-;)\n" +
                      "向こう側に置いてある駒と\n" +
                      "同じ場所に同じ駒を置くと\n" +
                      "大丈夫だよ^^";
                break;
            case GimmickName.Type.PictureName:
                str = "廊下にまだ開いていない\n" +
                      "青い扉があるね！\n" +
                      "アルファベットを５文字\n" +
                      "入力するみたいだね！\n" +
                      "どこかに書いていないかな？\n" +
                      "寝室の壁に絵が飾れていて、\n" +
                      "タイトルが５文字だね！\n" +
                      "青い扉の仕掛けに\n" +
                      "入力してみよう！！";
                break;
            case GimmickName.Type.UseIronPipe:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.IronPipe), 0))
                {
                    str = "TVの前にあるソファの下に\n" +
                          "何か落ちているみたい…！\n" +
                          "調べて拾ってみよう！！";
                }
                else
                {
                    str = "青の扉の先にある\n" +
                          "トイレに行ってみると、\n" +
                          "トイレットペーパーが\n" +
                          "高い所に置いてあるね！\n" +
                          "でも、手が届かないみたい…\n" +
                          "持っている鉄パイプを使えば\n" +
                          "届くんじゃないかな？\n" +
                          "アイテム欄の鉄パイプを選んで\n" +
                          "高い所のトイレットペーパーに\n" +
                          "使ってみよう！！";
                }
                break;
            case GimmickName.Type.UseToiletPaper:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.ToiletPaper), 0))
                {
                    str = "高いところにあった\n" +
                          "トイレットペーパーが\n" +
                          "落ちてきたね！\n" +
                          "拾ってみよう！！";
                }
                else
                {
                    str = "トイレには洋式の便器があるね！\n" +
                          "でも、紙がないみたい…\n" +
                          "とても困ってしまうね(-_-;)\n" +
                          "トイレットペーパーを\n" +
                          "持っているので、\n" +
                          "紙を補充してあげよう！！";
                }
                break;
            case GimmickName.Type.RefrigeratorCans:
                str = "トイレの先には\n" +
                      "バスルームがあるね！\n" +
                      "ショーケースがあるけど、\n" +
                      "中は暗くて見えないみたい…\n" +
                      "すぐ下にある仕掛けを解こう！\n" +
                      "冷蔵庫の中にある\n" +
                      "缶の場所と数がヒントだよ！\n" +
                      "対応する位置と数を\n" +
                      "仕掛けに合わせてみよう！！";
                break;
            case GimmickName.Type.LockerAndRestroomTips:
                str = "寝室のロッカーと\n" +
                      "トイレのカバーに貼っている\n" +
                      "２枚の図に注目しよう！\n" +
                      "矢印の通る位置と\n" +
                      "通った位置の数字を合わせると、\n" +
                      "なんと５桁の数値が表れたね！\n" +
                      "５桁の数値に見覚えはあるかな？\n" +
                      "コンピュータの右下にある\n" +
                      "引き出しの仕掛けに\n" +
                      "５桁の数値を入力しよう！！";
                break;
            case GimmickName.Type.UseAxe:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Axe), 0))
                {
                    str = "手のオブジェに指輪を嵌めると\n" +
                          "すぐ下の大きな箱が開いたね！\n" +
                          "中には何が入っているのかな？\n" +
                          "調べて拾ってみよう！！";
                }
                else
                {
                    str = "廊下にはなんとも意味深な\n" +
                          "木箱が置いてあるね！\n" +
                          "中には何が入っているのかな？\n" +
                          "調べてみたいけど、\n" +
                          "開け口は無いみたい…\n" +
                          "どうしよう？\n" +
                          "ここは力技だね！\n" +
                          "持っている斧を使って、\n" +
                          "木箱ごと壊してしまおう！！";
                }
                break;
            case GimmickName.Type.UnderThePillow:
                str = "ベッドは調べてみたかな？\n" +
                      "枕の下には何かあるみたい！\n" +
                      "調べてみよう！！";
                break;
            case GimmickName.Type.ThreeCardsTips:
                str = "[ベッドの上にある枕の下]\n" +
                      "[廊下にある壊した木箱の中]\n" +
                      "[バスルームのショーケースの中]\n" +
                      "見つけた３枚のカードをヒントに\n" +
                      "バスタブのガラス扉を開けよう！\n" +
                      "カードの６色は仕掛けの６色と\n" +
                      "対応しているよ！\n" +
                      "仕掛けの上にある図はヒントで、\n" +
                      "図をカードの記号通りになぞると\n" +
                      "それぞれ１桁の数字が現れるよ！\n" +
                      "とても難しいけど頑張って！！";
                break;
            case GimmickName.Type.UsePai_1:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_1), 0))
                {
                    str = "コンピュータの右下にある\n" +
                          "５桁の謎はすでに解いたね！\n" +
                          "引き出しの中には\n" +
                          "何が入っているのかな？\n" +
                          "調べて拾ってみよう！！";
                }
                else
                {
                    str = "いよいよ最後の仕掛けだね！\n" +
                          "廊下の先にある出口の扉には\n" +
                          "ドアノブの下に台座があるよ！\n" +
                          "くぼみが３つあるけど\n" +
                          "何かを嵌められないかな？\n" +
                          "持っている麻雀牌が\n" +
                          "ピッタリかも？\n" +
                          "[一萬]とかかれている牌を\n" +
                          "１番のくぼみに嵌めてみよう！！";
                }
                break;
            case GimmickName.Type.UsePai_2:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_2), 0))
                {
                    str = "将棋盤に足りない駒は\n" +
                          "全て見つけて置けたね！\n" +
                          "すぐ左にある家具の引き出しが\n" +
                          "開いたみたいだよ！\n" +
                          "何が入っているのかな？\n" +
                          "調べて拾ってみよう！！";
                }
                else
                {
                    str = "いよいよ最後の仕掛けだね！\n" +
                          "廊下の先にある出口の扉には\n" +
                          "ドアノブの下に台座があるよ！\n" +
                          "くぼみが３つあるけど\n" +
                          "何かを嵌められないかな？\n" +
                          "持っている麻雀牌が\n" +
                          "ピッタリかも？\n" +
                          "青い丸が２つ描かれている牌を\n" +
                          "２番のくぼみに嵌めてみよう！！";
                }
                break;
            case GimmickName.Type.UsePai_3:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_3), 0))
                {
                    str = "バスタブの仕掛けを\n" +
                          "無事に解くことができたね！\n" +
                          "難しかったかな？\n" +
                          "バスタブの中を調べると\n" +
                          "どうやら排水溝に\n" +
                          "何か引っかかっているみたい！\n" +
                          "調べて拾ってみよう！！";
                }
                else
                {
                    str = "いよいよ最後の仕掛けだね！\n" +
                          "廊下の先にある出口の扉には\n" +
                          "ドアノブの下に台座があるよ！\n" +
                          "くぼみが３つあるけど\n" +
                          "何かを嵌められないかな？\n" +
                          "持っている麻雀牌が\n" +
                          "ピッタリかも？\n" +
                          "縦棒が３つ描かれている牌を\n" +
                          "３番のくぼみに嵌めてみよう！！";
                }
                break;
            case GimmickName.Type.FinalDoor:
                str = "いよいよ最後の仕掛けだね！\n" +
                      "廊下の先にある出口の扉には\n" +
                      "ドアノブの下に台座があるよ！\n" +
                      "くぼみが３つあるけど\n" +
                      "何かを嵌められないかな？\n" +
                      "持っている麻雀牌が\n" +
                      "ピッタリかも？\n" +
                      "持っている麻雀牌を\n" +
                      "３つのくぼみに嵌めてみよう！！";
                break;
            default:
                str = "リワードエラー002\n" +
                      "想定外の進行状況です。";
                break;
        }

        return str;
    }
}