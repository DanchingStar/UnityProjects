using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : ManagerParent 
{
    public Player1 myPlayer1;
    public Player2 myPlayer2;

    public GameMenu myMenuManager;

    public GameObject gameOverCanvas;
    public GameObject gameClearCanvas;

    public Camera mainCamera;
    public GameObject anotherCamera;

    public Button nextStageButton;
    public TextMeshProUGUI troubleTextObject;

    Vector3 startCameraMaker, endCameraMaker; //カメラのスタート地点とゴール地点
    Quaternion startCameraQuaternion, endCameraQuaternion;//カメラのスタート角度とゴール角度

    float presentCameraLocation = 0f; //現在地を示す[ 0 <= x <= 1 ]

    bool firstFlag = true; //カメラ用フラグ

    const float gameOverDelayTime = 0.7f; //ゲームオーバー画面を出すまでの時間
    const float gameClearDelayTime = 2.0f; //ゲームクリア画面を出すまでの時間
    float countTime = 0f; //設定した時間までの時間を管理

    bool gameOverFlag = false; //ゲームオーバーが確定したらtrue
    bool gameClearFlag = false; //ゲームクリアが確定したらtrue

    bool clearMissSEFlag = true; //クリアまたはミスのときに一度だけSEが再生されるためのフラグ

    int nowStage = 0; //現在のステージ数を取得
    int highStage = 0; //最高到達ステージ数を取得

    int countTrouble = 0; //手数をカウント
    bool countTroubleFlag = false; //手数をカウントするためのフラグ

    // Start is called before the first frame update
    void Start()
    {
        if (myBanner)
        {
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }

        OnSoundPlay(SoundManager.BGM_Type.GameBGM);

        startCameraMaker = mainCamera.transform.position;
        endCameraMaker = startCameraMaker;

        startCameraQuaternion = mainCamera.transform.rotation;
        endCameraQuaternion = startCameraQuaternion;

        presentCameraLocation = 0f;

        nowStage = int.Parse(Regex.Replace(SceneManager.GetActiveScene().name, @"[^0-9]", ""));
        highStage = PlayerPrefs.GetInt("ClearStage", 0);

        //Debug.Log($"nowStage : {nowStage}");
        //Debug.Log($"highStage : {highStage}");

        if (nowStage >= maxStage) 
        {
            nextStageButton.interactable = false; //ボタンを無効化
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer1.WalkToHole() == 1) //穴に向かって歩いているとき
        {
            MoveCamera(myPlayer1.transform.position, mainCamera);
        }
        else if (myPlayer1.WalkToHole() == 2) //穴に到達したとき
        {
            GameOverDisplay();
        }
        else if (myPlayer2.WalkToHole() == 1) //穴に向かって歩いているとき
        {
            MoveCamera(myPlayer2.transform.position, mainCamera);
        }
        else if (myPlayer2.WalkToHole() == 2) //穴に到達したとき
        {
            GameOverDisplay();
        }
        else if(gameClearFlag == true) //ゲームクリアしたとき
        {
            GameClearDisplay();
            MoveCamera(myPlayer1.transform.position, mainCamera);
            //if(presentCameraLocation >= 1.0f)
            //{
            //    anotherCamera.SetActive(true);
            //}
        }

        if (gameOverFlag == false && myPlayer1.WalkToHole() >= 1) //Player1が落ちるの確定したとき
        {
            gameOverFlag = true;
        }
        else if (gameOverFlag == false && myPlayer2.WalkToHole() >= 1) //Player2が落ちるの確定したとき
        {
            gameOverFlag = true;
        }

        if (gameClearFlag == false && myPlayer1.GoalCheck() == true && myPlayer2.GoalCheck() == true)  //ゲームクリアが確定したとき
        {
            gameClearFlag = true;
            if (nowStage >= highStage) //初めてこのステージをクリアしたとき
            {
                PlayerPrefs.SetInt("ClearStage", nowStage);
                PlayerPrefs.Save();
            }
        }

        CountTrouble(); //手数をカウント
        troubleTextObject.text = "手数 : " + countTrouble.ToString(); //手数を表示するテキストを変更
    }


    void MoveCamera(Vector3 maker , Camera camera)
    {
        if (firstFlag == true) //穴に向かった時に初回に呼ばれ、カメラの目的地を設定する
        {
            endCameraMaker = new Vector3(maker.x, 6f, maker.z - 5f); //カメラの目的地の設定
            endCameraQuaternion = Quaternion.Euler(45f,0f,0f); //カメラの角度変更の設定
            firstFlag = false;
        }
        else //設定したカメラの目的地に向かって寄る
        {
            presentCameraLocation += Time.deltaTime * 1.1f;   // 現在のカメラの位置
            camera.transform.position = Vector3.Lerp(startCameraMaker, endCameraMaker, presentCameraLocation);// カメラオブジェクトの移動
            camera.transform.rotation = Quaternion.Lerp(startCameraQuaternion, endCameraQuaternion, presentCameraLocation);// カメラの回転
        }
    }

    void GameOverDisplay() //ゲームオーバーの画面を出す関数
    {
        if (countTime < gameOverDelayTime)
        {
            countTime += Time.deltaTime;
        }
        else
        {
            if (clearMissSEFlag) //一度だけSEを再生する
            {
                OnSoundPlay(SoundManager.SE_Type.Miss);
                clearMissSEFlag = false;
            }
            gameOverCanvas.SetActive(true);
        }

    }
    void GameClearDisplay() //ゲームクリアの画面を出す関数
    {
        if (countTime < gameClearDelayTime)
        {
            countTime += Time.deltaTime;
        }
        else
        {
            if (clearMissSEFlag) //一度だけSEを再生する
            {
                OnSoundPlay(SoundManager.SE_Type.Clear);
                clearMissSEFlag = false;
            }
            gameClearCanvas.SetActive(true);
            anotherCamera.SetActive(true);
        }

    }


    void CountTrouble() //手数をカウントするための関数
    {
        if (myPlayer1.inputManagerFlag == true || myPlayer2.inputManagerFlag == true)
        {
            countTroubleFlag = true;
        }
        else
        {
            if (countTroubleFlag == true && (myPlayer1.IsGetKeyOK() == false || myPlayer2.IsGetKeyOK() == false)) 
            {
                countTrouble++;
                countTroubleFlag = false;
                OnSoundPlay(SoundManager.SE_Type.Walk);
            }
        }
    }


    public void PushNextStage() //クリア後にNexrStageをクリックしたとき
    {
        OnSoundPlay(SoundManager.SE_Type.Yes);

        int n = nowStage + 1;
        string text = "Game" + n;
        ChangeScene(text,30); //30%でインタースティシャル広告を呼び出して、シーン変更
    }

    public void ClickSNS() //SNSボタンをクリックしたときに呼ばれる関数
    {
        string url = "";
        string image_path = "";
        string text = "";

        string aaa = "ステージ";
        string stage = nowStage.ToString();
        string bbb = "を";
        string score = countTrouble.ToString();
        string ccc = "手でクリア!!\nさあ、あなたもやってみよう(^^)/\n#運命共同兄弟 #UKK ";

        text = aaa + stage + bbb + score + ccc;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar";
            image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#Android\n";
            Debug.Log("Android");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            image_path = Application.persistentDataPath + "/SS.png";
            text = text + "#iPhone\n";
            Debug.Log("iPhone");
        }
        else
        {
            url = "https://www.google.com";
            image_path = Application.persistentDataPath + "/SS.png";
            //text = text + "その他の機種\n";
            Debug.Log("Other OS");
        }

        //SocialConnector.SocialConnector.Share(text); //第1引数:テキスト,第2引数:URL,第3引数:画像
        SocialConnector.SocialConnector.Share(text, url); //第1引数:テキスト,第2引数:URL,第3引数:画像
        //Debug.Log("SNS");
    }


    public bool PauseCheck() //現在ポーズ中かチェックする関数
    {
        if (myMenuManager.MenuPauseCheck() == true)
        {
            return true;
        }

        return false;
    }
    public bool GameOverCheck() //GameOverが確定したかチェックする関数
    {
        return gameOverFlag;
    }
    public bool GameClearCheck() //Gameクリアが確定したかチェックする関数
    {
        return gameClearFlag;
    }

    public int NowStageCheck() //現在のステージ名を返す関数
    {
        return nowStage;
    }

    public int GetMaxStage() //maxStageを返すだけの関数
    {
        return maxStage;
    }
    public bool GetDebugMuteMode() //debugMuteModeを返すだけの関数
    {
        return debugMuteMode;
    }



}
