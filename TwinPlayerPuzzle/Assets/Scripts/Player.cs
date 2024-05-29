using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] walls; //壁の位置を取得用 
    public GameObject[] holes; //穴の位置を取得用 
    public GameObject[] reverses; //逆マスの位置を取得用 
    public GameObject[] goals; //ゴールの位置を取得用 

    public GameManager myGameManager;
    public InputManager myInputManager;

    protected Animator animator; //アニメーター
    protected const int walk = 1, idle = 5; //アニメーション"legs"のパラメータ
    protected Vector3 startMaker, endMaker; //歩くスタート地点とゴール地点
    protected float presentLocation = 0f; //現在地を示す[ 0 <= x <= 1 ]
    protected bool isGetKeyOK = true; //キーを受け付けるか判定する

    protected const float speed = 1.0F; //歩くスピード
    protected const int floorSizeX = 9; //床のサイズ
    protected const int floorSizeZ = 9;
    protected const int zeroPositionX = 4; //座標0が配列の何番目か
    protected const int zeroPositionZ = 4;
    protected const int neutral = 0, wall = 1, hole = 2, reverse = 3, goal = 9; //wallHoleCheck用の定数
    protected int[,] wallHoleCheck = new int[floorSizeX, floorSizeZ]; //壁か穴があるか調べるための配列

    protected int walkToHole = 0; // 0:穴じゃないところ,1:穴に向かっている,2;穴に到達
    protected bool reverseFlag = false; //逆マスのフラグ
    protected bool goalFlag = false; //ゴールのフラグ
    protected float goalActionTime = 0f; //ゴールした後のポーズを管理する

    [System.NonSerialized] public bool inputManagerFlag = false; //InputManagerとやり取りするためのフラグ
    protected bool waitFlag = false; //1フレーム入力を受け付けないためのフラグ

    protected bool keyInputFlag = false; //キーが入力されたときの確認フラグ
    protected int xDistance = 0, zDistance = 0; //入力キーごとに進むべき道を加算

    protected void PositionStatusReset() //スタート位置とエンド位置をリセットする
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("legs", idle);//アニメーション[立ち止まる]
        animator.SetInteger("arms", idle);//アニメーション[立ち止まる]

        startMaker = new Vector3(this.transform.localPosition.x, 0.5f, this.transform.localPosition.z);
        endMaker = startMaker;
        presentLocation = 0f;
    }


    protected void WalkToHoleAction() //穴へ向かうときの処理とアクション
    {
        walkToHole = 1; //穴に向かっているフラグ
        isGetKeyOK = false;//キー入力を受け付けないようにする
        animator.SetInteger("arms", 12);//アニメーション[見回す]
    }

    protected void DoStart()
    {
        PositionStatusReset();

        for (int x = 0; x < floorSizeX; x++)  //wallHoleCheckの初期化
        {
            for (int z = 0; z < floorSizeZ; z++)
            {
                wallHoleCheck[x, z] = neutral;
            }
        }

        if (walls != null)
        {
            foreach (var i in walls) //壁がある場所をwallHoleCheckが覚える
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = wall;
            }
        }
        if (holes != null)
        {
            foreach (var i in holes) //穴がある場所をwallHoleCheckが覚える
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = hole;
            }
        }
        if (reverses != null)
        {
            foreach (var i in reverses) //逆マスがある場所をwallHoleCheckが覚える
            {
                int x = (int)i.transform.localPosition.x;
                int z = (int)i.transform.localPosition.z;

                wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = reverse;
            }
        }
        foreach (var i in goals) //ゴールがある場所をwallHoleCheckが覚える
        {
            int x = (int)i.transform.localPosition.x;
            int z = (int)i.transform.localPosition.z;

            wallHoleCheck[x + zeroPositionX, z + zeroPositionZ] = goal;
        }
    }

    protected void DoIsGetKeyTrue()
    {

        if (myInputManager.CheckInputUp())  //キーを受け付けてるときに上を押された場合
        {
            keyInputFlag = true;
            xDistance = 0;
            zDistance = 1;
            this.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//上を向く
        }
        else if (myInputManager.CheckInputDown())  //キーを受け付けてるときに下を押された場合
        {
            keyInputFlag = true;
            xDistance = 0;
            zDistance = -1;
            this.transform.rotation = Quaternion.Euler(0f, -90f, 0f);//下を向く
        }
        else if (myInputManager.CheckInputRight())  //キーを受け付けてるときに右を押された場合
        {
            keyInputFlag = true;
            xDistance = 1;
            zDistance = 0;
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//右を向く
        }
        else if (myInputManager.CheckInputLeft())  //キーを受け付けてるときに左を押された場合

        {
            keyInputFlag = true;
            xDistance = -1;
            zDistance = 0;
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);//左を向く
        }
        else
        {
            keyInputFlag = false;
            xDistance = 0;
            zDistance = 0;
        }

        if (keyInputFlag)
        {
            if(reverseFlag) // 逆マスにいる場合
            {
                xDistance *= -1;
                zDistance *= -1;
            }

            if ((int)startMaker.z + zeroPositionZ + zDistance >= floorSizeZ || (int)startMaker.z + zeroPositionZ + zDistance < 0
                || (int)startMaker.x + zeroPositionX + xDistance >= floorSizeX || (int)startMaker.x + zeroPositionX + xDistance < 0) //枠外へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //目的地の設定
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == hole) //穴へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //目的地の設定
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == neutral) //普通の道へ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //目的地の設定
                goalFlag = false;
                reverseFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == reverse) //逆マスへ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //目的地の設定
                goalFlag = false;
                reverseFlag = true;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == wall) //壁へ歩こうとした場合
            {
                //Debug.Log("wall");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + xDistance, (int)startMaker.z + zeroPositionZ + zDistance] == goal) //ゴールへ歩こうとした場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x + xDistance, startMaker.y, startMaker.z + zDistance); //目的地の設定
                goalFlag = true;
                reverseFlag = false;
            }
            inputManagerFlag = false;
        }


        //以下古い処理
        /********************************************************************************************************************************************************
        //キーを受け付けてるときに上を押された場合
        if (myInputManager.CheckInputUp())
        {
            this.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//上を向く
            if ((int)startMaker.z + zeroPositionZ + 1 >= floorSizeZ) //枠外へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == hole) //穴へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == neutral) //普通の道へ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //目的地の設定
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == wall) //壁へ歩こうとした場合
            {
                //Debug.Log("wall ↑");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ + 1] == goal) //ゴールへ歩こうとした場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z + 1.0f); //目的地の設定
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //キーを受け付けてるときに下を押された場合
        else if (myInputManager.CheckInputDown())
        {
            this.transform.rotation = Quaternion.Euler(0f, -90f, 0f);//下を向く
            if ((int)startMaker.z + zeroPositionZ - 1 < 0) //枠外へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == hole) //穴へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == neutral) //普通の道へ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //目的地の設定
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == wall) //壁へ歩こうとした場合
            {
                //Debug.Log("wall ↓");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX, (int)startMaker.z + zeroPositionZ - 1] == goal) //ゴールへ歩こうとした場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x, startMaker.y, startMaker.z - 1.0f); //目的地の設定
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //キーを受け付けてるときに右を押された場合
        else if (myInputManager.CheckInputRight())
        {
            this.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//右を向く
            if ((int)startMaker.x + zeroPositionX + 1 >= floorSizeX) //枠外へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == hole) //穴へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == neutral) //普通の道へ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //目的地の設定
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == wall) //壁へ歩こうとした場合
            {
                //Debug.Log("wall →");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX + 1, (int)startMaker.z + zeroPositionZ] == goal) //ゴールへ歩こうとした場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x + 1.0f, startMaker.y, startMaker.z); //目的地の設定
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        //キーを受け付けてるときに左を押された場合
        else if (myInputManager.CheckInputLeft())
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);//左を向く
            if ((int)startMaker.x + zeroPositionX - 1 < 0) //枠外へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == hole) //穴へ歩いた場合
            {
                WalkToHoleAction(); //穴へ歩いた時の処理
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //目的地の設定
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == neutral) //普通の道へ歩いた場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //目的地の設定
                goalFlag = false;
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == wall) //壁へ歩こうとした場合
            {
                //Debug.Log("wall ←");
            }
            else if (wallHoleCheck[(int)startMaker.x + zeroPositionX - 1, (int)startMaker.z + zeroPositionZ] == goal) //ゴールへ歩こうとした場合
            {
                isGetKeyOK = false;//キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x - 1.0f, startMaker.y, startMaker.z); //目的地の設定
                goalFlag = true;
            }
            inputManagerFlag = false;
        }
        ********************************************************************************************************************************************************/
    }

    protected void DoIsGetKeyFalse()
    {
        presentLocation += Time.deltaTime * speed;   // 現在の位置
        animator.SetInteger("legs", walk); //アニメーション[歩く]
        animator.SetInteger("arms", walk); //アニメーション[歩く]
        transform.localPosition = Vector3.Lerp(startMaker, endMaker, presentLocation);// オブジェクトの移動

        if (presentLocation >= 1f) //目標地点に到達したとき
        {
            isGetKeyOK = true;//キー入力を受け付けるようにする
            PositionStatusReset();//現在地を設定しなおす

            if (walkToHole == 1) //穴に到達した場合
            {
                walkToHole = 2; //穴に到達フラグ
                isGetKeyOK = false; //キー入力を受け付けないようにする
                endMaker = new Vector3(startMaker.x, startMaker.y - 100f, startMaker.z); //目的地(落下)の設定
            }
        }
    }

    protected void FallToAbyss() //奈落へと落ちていくとき
    {
        if(presentLocation <= 0f)
        {
            myGameManager.OnSoundPlay(SoundManager.SE_Type.Fall);
        }
        presentLocation += Time.deltaTime * speed * 0.1f;   // 現在の位置
        animator.SetInteger("arms", 17); //アニメーション[両手を上げる]
        transform.localPosition = Vector3.Lerp(startMaker, endMaker, presentLocation);// オブジェクトの移動
    }
    protected void GameClearAction() //ゲームクリアしたときのアクション
    {
        goalActionTime += Time.deltaTime;

        if (goalActionTime < 1f)
        {
            animator.SetInteger("arms", 17); //アニメーション[両手を上げる]
        }
        else
        {
            animator.SetInteger("arms", idle); //アニメーション[両手を下げる]
        }

        if(goalActionTime >= 2f)
        {
            goalActionTime -= 2f;
        }
    }

    public int WalkToHole() //穴へ向かっている状態を返すだけの関数
    {
        return walkToHole;
    }
    public bool GoalCheck() //ゴール地点を踏んでいるか返すだけの関数
    {
        return goalFlag ;
    }

    public bool IsGetKeyOK() //入力を受け付けているか返すだけの関数
    {
        return isGetKeyOK;
    }
}
