using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rainbow : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    private Text image; //使う対象によって型を変える必要アリ

    void Start()
    {
        //イメージコンポーネント取得
        image = GetComponent<Text>();
        //imageのカラーを変更するコルーチンをスタート
        StartCoroutine(_rainbow());
    }

    //虹色に変化するコルーチン
    IEnumerator _rainbow()
    {
        //無限ループ
        while (true)
        {
            //カラーを変化させる処理
            image.color = Color.HSVToRGB(Time.time / speed % 1, 1, 1);
            //1フレーム待つ
            yield return new WaitForFixedUpdate();
        }
    }
}