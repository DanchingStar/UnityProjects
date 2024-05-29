using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XylophoneSticker : MonoBehaviour
{
    [SerializeField] private TextMeshPro notesText;
    private XylophoneManager.Notes myNotes;

    private readonly Vector3 DEFAULT_POSITION = new Vector3(0f, 0.55f, 0f);
    private readonly Vector3 DEFAULT_ROTATION = new Vector3(90, 0, 0);
    private readonly Vector3 DEFAULT_SCALE = new Vector3(0.75f, 0.75f, 1);

    /// <summary>
    /// 生成時の初期化
    /// </summary>
    /// <param name="notes">自身のノーツ</param>
    /// <param name="mainBoard">NotesKeyboardオブジェクトの中のMainオブジェクトのtransform</param>
    public void InitMyStatus(XylophoneManager.Notes notes ,Transform mainBoard)
    {
        myNotes = notes;

        SetNotesText();

        transform.localScale = DEFAULT_SCALE;

        transform.SetParent(mainBoard);
        transform.localPosition = DEFAULT_POSITION;
        transform.localRotation = Quaternion.Euler(DEFAULT_ROTATION);
    }

    /// <summary>
    /// 自身の音階名テキストをセットする
    /// </summary>
    public void SetNotesText()
    {
        notesText.text = XylophoneManager.Instance.GetNotesName(myNotes);
    }

}
