using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesKeyboard : MonoBehaviour
{
    [SerializeField] private XylophoneManager.Notes notes;

    [SerializeField] private GameObject stickerPrefab;

    private NotesKeyboard myNotesKeyboard;
    private Transform mainBoardTransform;
    private Renderer myRenderer;
    private Material myMaterial;
    private XylophoneSticker mySticker;

    /// <summary> 自身のデフォルトカラー </summary>
    private readonly Color COLOR_DEFAULT = new Color32(183, 135, 103, 255);
    /// <summary> 正解時に変わるカラー </summary>
    private readonly Color COLOR_SUCCESS = Color.cyan;
    /// <summary> ミス時に変わるカラー </summary>
    private readonly Color COLOR_FAILURE = Color.red;
    /// <summary> ミス時に変わるカラー </summary>
    private readonly Color COLOR_Another = Color.blue;

    /// <summary> 色が元の色へと変化するスピード </summary>
    private const float CHANGE_COLOR_SPEED = 0.3f;
    /// <summary> 変化させたい色 </summary>
    private Color changeColor;
    /// <summary> 色変更を停止させたいときにtrue </summary>
    private bool changeColorWaitFlg;
    /// <summary> 0から1をとり、大きくなるにつれデフォルトカラーに近づく </summary>
    private float changeColorLeap;

    /// <summary> 次のノーツのときに点滅するカラー </summary>
    private readonly Color COLOR_NEXT_SUPPORT = Color.yellow;
    /// <summary> 次のノーツのときに点滅するスピード </summary>
    private const float COLOR_NEXT_SPEED = 3f;
    /// <summary> 自身が次のノーツであるフラグ </summary>
    private bool nextNotesFlg;

    /// <summary> 連続して同じ音を鳴らす際に目立つように、Lerpの値を調整する値 </summary>
    private const float UNDER_LERP_VALUE = 0.2f;

    private void Start()
    {
        InitVariables();
        InitMyNotes();
    }

    private void Update()
    {
        UpdateColor();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Player hitPlayer = other.gameObject.GetComponent<Player>();

    //    StepMe(hitPlayer);
    //}

    /// <summary>
    /// 色を徐々に元の色へと戻していく
    /// </summary>
    private void UpdateColor()
    {
        if (changeColorWaitFlg) return;
        //if (changeColorLeap > 1f) return;

        Color displayColor = Color.Lerp(changeColor, COLOR_DEFAULT, changeColorLeap);

        changeColorLeap += Time.deltaTime * CHANGE_COLOR_SPEED;

        if (changeColorLeap < UNDER_LERP_VALUE) changeColorLeap = UNDER_LERP_VALUE;

        if (nextNotesFlg)
        {
            float value = (-Mathf.Cos(changeColorLeap * 2f * Mathf.PI * COLOR_NEXT_SPEED) + 1) / 2;
            myMaterial.color = Color.Lerp(displayColor, COLOR_NEXT_SUPPORT, value);
        }
        else
        {
            if (myMaterial.color == displayColor) return;
            myMaterial.color = displayColor;
        }
    }

    /// <summary>
    /// マテリアルの色を指定した色に変える
    /// </summary>
    /// <param name="color"></param>
    public void ChangeMaterialColor(Color color)
    {
        changeColorLeap = 0f;
        changeColor = color;
        myMaterial.color = changeColor;
    }

    /// <summary>
    /// 正解を踏んだ時に変えるべき色に変える
    /// </summary>
    public void ChangeMaterialColorSuccess()
    {
        ChangeMaterialColor(COLOR_SUCCESS);
    }

    /// <summary>
    /// ミスを踏んだ時に変えるべき色に変える
    /// </summary>
    public void ChangeMaterialColorFailure()
    {
        ChangeMaterialColor(COLOR_FAILURE);
    }

    /// <summary>
    /// フリースタイルモードなどで踏んだ時に変えるべき色に変える
    /// </summary>
    public void ChangeMaterialColorAnother()
    {
        ChangeMaterialColor(COLOR_Another);
    }

    /// <summary>
    /// 変数の初期化
    /// </summary>
    private void InitVariables()
    {
        myNotesKeyboard = GetComponent<NotesKeyboard>();
        mainBoardTransform = gameObject.transform.Find("Main");
        myRenderer = mainBoardTransform.gameObject.GetComponent<Renderer>();
        myMaterial = new Material(myRenderer.material);
        myRenderer.material = myMaterial;
        changeColorLeap = 0;
        changeColorWaitFlg = false;
        nextNotesFlg = false;
    }

    /// <summary>
    /// 自身のノーツを名前に合わせて初期化する
    /// </summary>
    private void InitMyNotes()
    {
        string myName = gameObject.name;

        if (Enum.TryParse( myName, out XylophoneManager.Notes auth))
        {
            notes = auth;
        }
        else
        {
            Debug.LogError($"NotesKeyboard.InitMyNotes : Not Find Item Name -> {myName}");
            notes = XylophoneManager.Notes.None;
        }

        ChangeMaterialColor(COLOR_DEFAULT);

        if (PlayerInformationManager.Instance.GetDisplayXylophoneStickerFlg()) 
        {
            var obj = Instantiate(stickerPrefab);
            mySticker = obj.GetComponent<XylophoneSticker>();
            mySticker.InitMyStatus(notes, mainBoardTransform);
        }
        else
        {
            mySticker = null;
        }

    }

    /// <summary>
    /// プレイヤーに踏まれたとき
    /// </summary>
    /// <param name="player"></param>
    public void StepMe(Player player)
    {
        if (player == null) return;
        if (!player.GetJumpFlg()) return;

        player.Landing();
        GameSceneManager.Instance.ReceptionStepKeyboard(myNotesKeyboard);
    }

    /// <summary>
    /// 自身のノーツを返すゲッター
    /// </summary>
    /// <returns></returns>
    public XylophoneManager.Notes GetMyNotes()
    {
        return notes;
    }

    /// <summary>
    /// 次回のノーツ化のフラグを変えるセッター
    /// </summary>
    /// <param name="flg"></param>
    public void SetNextNotesFlg(bool flg)
    {
        nextNotesFlg = flg;
    }

}
