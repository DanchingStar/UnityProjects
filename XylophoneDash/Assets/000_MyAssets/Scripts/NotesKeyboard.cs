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

    /// <summary> ���g�̃f�t�H���g�J���[ </summary>
    private readonly Color COLOR_DEFAULT = new Color32(183, 135, 103, 255);
    /// <summary> �������ɕς��J���[ </summary>
    private readonly Color COLOR_SUCCESS = Color.cyan;
    /// <summary> �~�X���ɕς��J���[ </summary>
    private readonly Color COLOR_FAILURE = Color.red;
    /// <summary> �~�X���ɕς��J���[ </summary>
    private readonly Color COLOR_Another = Color.blue;

    /// <summary> �F�����̐F�ւƕω�����X�s�[�h </summary>
    private const float CHANGE_COLOR_SPEED = 0.3f;
    /// <summary> �ω����������F </summary>
    private Color changeColor;
    /// <summary> �F�ύX���~���������Ƃ���true </summary>
    private bool changeColorWaitFlg;
    /// <summary> 0����1���Ƃ�A�傫���Ȃ�ɂ�f�t�H���g�J���[�ɋ߂Â� </summary>
    private float changeColorLeap;

    /// <summary> ���̃m�[�c�̂Ƃ��ɓ_�ł���J���[ </summary>
    private readonly Color COLOR_NEXT_SUPPORT = Color.yellow;
    /// <summary> ���̃m�[�c�̂Ƃ��ɓ_�ł���X�s�[�h </summary>
    private const float COLOR_NEXT_SPEED = 3f;
    /// <summary> ���g�����̃m�[�c�ł���t���O </summary>
    private bool nextNotesFlg;

    /// <summary> �A�����ē�������炷�ۂɖڗ��悤�ɁALerp�̒l�𒲐�����l </summary>
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
    /// �F�����X�Ɍ��̐F�ւƖ߂��Ă���
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
    /// �}�e���A���̐F���w�肵���F�ɕς���
    /// </summary>
    /// <param name="color"></param>
    public void ChangeMaterialColor(Color color)
    {
        changeColorLeap = 0f;
        changeColor = color;
        myMaterial.color = changeColor;
    }

    /// <summary>
    /// �����𓥂񂾎��ɕς���ׂ��F�ɕς���
    /// </summary>
    public void ChangeMaterialColorSuccess()
    {
        ChangeMaterialColor(COLOR_SUCCESS);
    }

    /// <summary>
    /// �~�X�𓥂񂾎��ɕς���ׂ��F�ɕς���
    /// </summary>
    public void ChangeMaterialColorFailure()
    {
        ChangeMaterialColor(COLOR_FAILURE);
    }

    /// <summary>
    /// �t���[�X�^�C�����[�h�Ȃǂœ��񂾎��ɕς���ׂ��F�ɕς���
    /// </summary>
    public void ChangeMaterialColorAnother()
    {
        ChangeMaterialColor(COLOR_Another);
    }

    /// <summary>
    /// �ϐ��̏�����
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
    /// ���g�̃m�[�c�𖼑O�ɍ��킹�ď���������
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
    /// �v���C���[�ɓ��܂ꂽ�Ƃ�
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
    /// ���g�̃m�[�c��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public XylophoneManager.Notes GetMyNotes()
    {
        return notes;
    }

    /// <summary>
    /// ����̃m�[�c���̃t���O��ς���Z�b�^�[
    /// </summary>
    /// <param name="flg"></param>
    public void SetNextNotesFlg(bool flg)
    {
        nextNotesFlg = flg;
    }

}
