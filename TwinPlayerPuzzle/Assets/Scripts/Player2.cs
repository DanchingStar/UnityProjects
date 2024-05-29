using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    void Start()
    {
        DoStart(); //初期設定など
    }

    void Update()
    {
        if (myGameManager.PauseCheck()) //ポーズ中の場合
        {
            return; //何もしない
        }

        if (walkToHole == 0) //穴とは無縁のとき
        {
            if (myGameManager.GameClearCheck() == false) //まだゲームクリアしていない場合
            {
                if (isGetKeyOK == true) //キーを受け付けているとき
                {
                    if (waitFlag == true) //waitFlagがtrueのとき
                    {
                        waitFlag = false;
                        return; //何もしない
                    }
                    DoIsGetKeyTrue(); //入力されるキーで目的地を決める
                }
            }
        }

        if (walkToHole == 0 || walkToHole == 1) //穴と無縁のとき,または穴へ向かっているとき
        {
            if (isGetKeyOK == false)  //キーを受け付けないとき
            {
                DoIsGetKeyFalse(); //指定されたマスまで歩く
                if (waitFlag == false) //waitFlagがfalseのとき
                {
                    waitFlag = true;
                }
            }
        }
        else //穴に到達しているとき
        {
            FallToAbyss(); //奈落へと落下する
        }

        if (myGameManager.GameClearCheck() == true) //ゲームクリアしたとき
        {
            GameClearAction(); //バンザイのポーズをとる
        }
    }
}
