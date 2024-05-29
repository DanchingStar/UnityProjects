using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteInformationPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;

    private bool deleteFlg;

    /// <summary>
    /// 自分の情報をセットする
    /// </summary>
    /// <param name="flg">true : 削除した</param>
    public void SetMyStatus(bool flg)
    {
        deleteFlg = flg;
        UpdateTexts();
    }

    /// <summary>
    /// テキストを更新する
    /// </summary>
    private void UpdateTexts()
    {
        if (deleteFlg)
        {
            text1.text = $"データを削除しました。";
            text2.text = $"このゲームを遊んでいただき、\nありがとうございました。";
        }
        else
        {
            text1.text = $"データの削除に失敗しました。";
            text2.text = $"ログインが正常にできているか、\nご確認ください。";
        }
    }

    /// <summary>
    /// 閉じるを押したとき
    /// </summary>
    public void PushCloseButton()
    {
        if (deleteFlg)
        {
            MenuSceneManager.Instance.ReceptionAfterDelete();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
