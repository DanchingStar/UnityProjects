using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float jumpHeight = 0.7f;
    private float gameoverHeight = -3f;
    private float disappearHeight = -500f;

    private bool jumpFlg;
    private bool gameoverFlg;

    private Vector3 myPosition;

    //private CharacterController controller;
    //private ThirdPersonController characterController;
    //private StarterAssetsInputs starterAssetsInputs;
    //private PlayerInput playerInput;
    //private Vector3 externalForce;

    private void Start()
    {
        ChangeJumpFlg(false);
        ChangeGameoverFlg(false);
        //controller = GetComponent<CharacterController>();
        //characterController = GetComponent<ThirdPersonController>();
        //starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        //playerInput = GetComponent<PlayerInput>();
        //externalForce = Vector3.zero;
    }

    private void Update()
    {
        UpdateMyPosition();
        CheckJump();
        CheckGameover();
        CheckDisappearHeight();

        //// 外部からの力を加える
        //controller.Move(externalForce);

        //// 次のフレームのために外部からの力をリセット
        //externalForce = Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.transform.parent.gameObject.tag == "Keyboard")
        {
            if (!jumpFlg) return;
            hit.gameObject.transform.parent.gameObject.GetComponent<NotesKeyboard>().StepMe(GetComponent<Player>());
        }
        //else if(hit.gameObject.tag == "Enemy")
        //{
        //    Vector3 damageVector = hit.gameObject.GetComponent<EnemyBase>().HitPlayer(GetComponent<Player>());

        //    StartCoroutine(DamageMe(damageVector));

        //    Debug.Log($"Damage {damageVector}");
        //}
        //else
        //{
        //    Debug.Log($"collision ! {hit.gameObject.name}");
        //}
    }

    //private IEnumerator DamageMe(Vector3 powor)
    //{
    //    GameSceneManager.Instance.SwitchOperatePlayerEnable(false);
    //    //GetComponent<CharacterController>().enabled = false;

    //    // どうにかして力を加えたい !!!

    //    //AddForce(powor);
    //    //GetComponent<Rigidbody>().AddForce(powor,ForceMode.VelocityChange);

    //    float roopCount = 60;
    //    Vector3 firstPos = transform.position;
    //    for (int i = 0; i < roopCount; i++)
    //    {
    //        transform.position = Vector3.Lerp(firstPos, firstPos + powor, (float)(i + 1) / (float)roopCount);
    //        yield return new WaitForSeconds((float)1 / (float)roopCount);
    //    }
    //    Debug.Log($"{firstPos} ->{firstPos + powor}");

    //    //yield return new WaitForSeconds(1f);

    //    GameSceneManager.Instance.SwitchOperatePlayerEnable(true);
    //    //GetComponent<CharacterController>().enabled = true;
    //}


    /// <summary>
    /// ジャンプしたかチェックする
    /// </summary>
    private void CheckJump()
    {
        if (jumpFlg) return; // すでにジャンプ中なら除外
        if (jumpHeight > gameObject.transform.position.y) return; // 高さで判断
        ChangeJumpFlg(true);
    }

    /// <summary>
    /// GameOverしたかチェックする
    /// </summary>
    private void CheckGameover()
    {
        if (gameoverFlg) return; // すでにGameOverなら除外
        if (gameoverHeight < gameObject.transform.position.y) return; // 高さで判断
        ChangeGameoverFlg(true);
    }

    /// <summary>
    /// GameOverしたかチェックする
    /// </summary>
    private void CheckDisappearHeight()
    {
        if (!gameoverFlg) return; // GameOverでないなら除外
        if (disappearHeight < gameObject.transform.position.y) return; // 高さで判断
        Destroy(gameObject);
    }

    /// <summary>
    /// myPositionを更新する
    /// </summary>
    private void UpdateMyPosition()
    {
        myPosition = gameObject.transform.position;
    }

    // ジャンプしたときに呼ぶ(削除)
    //public void Jumping()
    //{
    //    ChangeJumpFlg(true);
    //}

    /// <summary>
    /// 着地したとき、他メソッドから受信する
    /// </summary>
    /// <param name="notes">鳴らしたい音のノーツ名</param>
    public void Landing()
    {
        if (!jumpFlg) return;

        ChangeJumpFlg(false);
    }

    /// <summary>
    /// ジャンプしているかどうかのフラグを切り替える
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeJumpFlg(bool flg)
    {
        if (jumpFlg == flg) return;

        jumpFlg = flg;

        //if (flg)
        //{
        //    Debug.Log($"Player.ChangeJumpFlg : Player Jump");
        //}
        //else
        //{
        //    Debug.Log($"Player.ChangeJumpFlg : Player Land");
        //}
    }

    /// <summary>
    /// GameOverかどうかのフラグを切り替える
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeGameoverFlg(bool flg)
    {
        if (gameoverFlg == flg) return;

        gameoverFlg = flg;

        if (flg)
        {
            Debug.Log($"Player.ChangeGameoverFlg : Player GameOver");

            GameSceneManager.Instance.ReceptionGameOver();
        }
    }

    /// <summary>
    /// ジャンプしているかどうかのフラグを返すゲッター
    /// </summary>
    /// <returns>jumpFlg</returns>
    public bool GetJumpFlg()
    {
        return jumpFlg;
    }

    /// <summary>
    /// Playerの位置を返すゲッター
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition()
    {
        return myPosition;
    }

    ///// <summary>
    ///// 外部からの力を設定するメソッド
    ///// </summary>
    ///// <param name="force"></param>
    //public void AddForce(Vector3 force)
    //{
    //    externalForce = force;
    //}

}
