using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    /// <summary>バナー広告を扱う</summary>
    [SerializeField] private AdMobBanner myBanner = null;

    /// <summary>自機の角度変更のスピード</summary>
    [SerializeField] private float rotateSpeed = 20.0f;
    /// <summary>自機の進むスピード</summary>
    [SerializeField] private float moveOnSpeed = 3.0f;
    /// <summary>自機の溜めるスピード</summary>
    [SerializeField] private float accumulateSpeed = 50.0f;

    /// <summary>スコアのテキストを設定する</summary>
    [SerializeField] private GameObject scoreText = null;

    /// <summary>角度予測線の動かない方を設定する</summary>
    [SerializeField] private GameObject displayAngleNeutral = null;
    /// <summary>角度予測線の動く方を設定する</summary>
    [SerializeField] private GameObject displayAngleChange = null;

    /// <summary>CameraShakeスクリプトを貼っているオブジェクトを設定する</summary>
    [SerializeField] private CameraShake shake;

    [SerializeField] private AudioClip SE_SmallHit;
    [SerializeField] private AudioClip SE_Collision;
    [SerializeField] private AudioClip SE_Explosion;
    [SerializeField] private AudioClip SE_FlyAway;
    private AudioSource audioSource;

    /// <summary>自機の角度、+が左向き、-が右向き</summary>
    private float playerRotate = 0.0f;

    /// <summary>溜めている時間</summary>
    private float accumulateTime = 0.0f;

    /// <summary>ゲームオーバーになったらtrue</summary>
    private bool isGameOverFlg = false;

    /// <summary>ゲームオーバーになってからの時間</summary>
    private float timeGameOver = 0.0f;

    /// <summary>ゲームオーバーになってからの時間</summary>
    private float nowScore = 0.0f;

    /// <summary>NowScoreをprefに記憶するときのキー(文字列)</summary>
    private string prefStringNowScore = "NowScore";


    void Start()
    {
        isGameOverFlg = false;
        timeGameOver = 0f;
        playerRotate = 0.0f;
        displayAngleNeutral.SetActive(false);
        displayAngleChange.SetActive(false);
        nowScore = 0.0f;
        PlayerPrefs.DeleteKey(prefStringNowScore);
        audioSource = GetComponent<AudioSource>();

        if (myBanner)
        {
            myBanner.RequestBanner(); //バナー広告を呼び出す
        }
    }

    void Update()
    {
        if (!isGameOverFlg)
        {
            PlayerInputTap();

            ScoreManager();

            CheckGameOver();
        }
        else
        {
            GameOver();
        }

        PlayerFreeRotate();

        PlayerRotate();

        PlayerMoveOn();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit!!");

            var targetEnemy = collision.gameObject.GetComponent<Enemy>();

            if (targetEnemy.IsDependsPlayerAngle)
            {
                if (playerRotate >= 0f && playerRotate < 160f)
                {
                    playerRotate += targetEnemy.DamageAngle;
                }
                else if (playerRotate < 0f && playerRotate > -160f)
                {
                    playerRotate -= targetEnemy.DamageAngle;
                }
            }
            else
            {
                playerRotate += targetEnemy.DamageAngle;
            }

            float ang = Mathf.Abs(targetEnemy.DamageAngle);

            if(ang < 5f)
            {
                audioSource.PlayOneShot(SE_SmallHit);
            }
            else if(ang < 20f)
            { 
                audioSource.PlayOneShot(SE_Collision);
            }
            else
            {
                audioSource.PlayOneShot(SE_Explosion);
            }

            //画面を揺らす
            shake.Shake(0.1f, ang * 0.02f );


        }
    }


    /// <summary>
    /// プレイヤーの入力に対して処理する関数
    /// </summary>
    private void PlayerInputTap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            accumulateTime = 0.0f;
            displayAngleNeutral.SetActive(true);
            displayAngleChange.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            accumulateTime += Time.deltaTime;
            if (this.transform.rotation.z <= 0f)
            {
                displayAngleChange.transform.localEulerAngles = new Vector3(0, 0, accumulateTime * accumulateSpeed);
            }
            else
            {
                displayAngleChange.transform.localEulerAngles = new Vector3(0, 0, -accumulateTime * accumulateSpeed);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(this.transform.rotation.z <= 0f)
            {
                playerRotate += accumulateTime * accumulateSpeed;
            }
            else
            {
                playerRotate -= accumulateTime * accumulateSpeed;

            }
            displayAngleNeutral.SetActive(false);
            displayAngleChange.SetActive(false);
            displayAngleChange.transform.localEulerAngles = displayAngleNeutral.transform.localEulerAngles;
        }
    }

    /// <summary>
    /// 自機が自動回転する関数
    /// </summary>
    private void PlayerFreeRotate()
    {
        if (playerRotate >= 0f && playerRotate < 160f )
        {
            playerRotate += Time.deltaTime * rotateSpeed;
        }
        else if(playerRotate < 0f && playerRotate > -160f)
        {
            playerRotate -= Time.deltaTime * rotateSpeed;
        }
        else
        {

        }
    }

    /// <summary>
    /// 回転を実行する関数
    /// </summary>
    private void PlayerRotate()
    {
        this.transform.rotation = Quaternion.Euler(0f, 0f, playerRotate);
    }

    /// <summary>
    /// 前に進む関数
    /// </summary>
    private void PlayerMoveOn()
    {
        transform.position += transform.up * moveOnSpeed * Time.deltaTime;
    }

    /// <summary>
    /// スコアを更新してテキストに表示する関数
    /// </summary>
    private void ScoreManager()
    {
        nowScore = this.transform.position.y;
        // オブジェクトからTextコンポーネントを取得
        Text t = scoreText.GetComponent<Text>();
        // テキストの表示を入れ替える
        t.text = "スコア : " + nowScore.ToString("F2");
    }

    /// <summary>
    /// ゲームオーバーを判定する関数
    /// </summary>
    private void CheckGameOver()
    {
        if (playerRotate > 90f)
        {
            isGameOverFlg = true;
        }
        if (playerRotate < -90f)
        {
            isGameOverFlg = true;
        }
    }

    /// <summary>
    /// ゲームオーバー時に動く関数
    /// </summary>
    private void GameOver()
    {
        if(timeGameOver == 0f)
        {
            audioSource.PlayOneShot(SE_FlyAway);

            displayAngleNeutral.SetActive(false);
            displayAngleChange.SetActive(false);
            //Debug.Log("ゲームオーバー");

            PlayerPrefs.SetFloat(prefStringNowScore, nowScore);
            PlayerPrefs.Save();
        }
        else if (timeGameOver > 2f)
        {
            if (myBanner)
            {
                myBanner.DestroyBanner();
            }

            FadeManager.Instance.LoadScene("Result", 0.3f);
        }

        timeGameOver += Time.deltaTime;
    }

    /// <summary>
    /// ゲームオーバーフラグのゲッターロボ
    /// </summary>
    public bool GetGameOverFlg()
    {
        return isGameOverFlg;
    }


}

