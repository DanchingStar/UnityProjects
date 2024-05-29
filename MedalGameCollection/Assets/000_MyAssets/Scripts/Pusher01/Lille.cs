using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class Lille : MonoBehaviour
    {
        /// <summary> 図柄が書いてあるQuadのPrefab </summary>
        [SerializeField] private GameObject[] numberQuadPrefabs;
        /// <summary> マスク処理に使用するQuadオブジェクト </summary>
        [SerializeField] private GameObject maskObject;
        /// <summary> 最初に表示している図柄の番号 </summary>
        [SerializeField] private int firstNumber;

        /// <summary> 図柄たちの間隔 </summary>
        private const float DISTANCE_POSITION = 4.8f;
        /// <summary> リールのスピード(速) </summary>
        private const float SPEED_FAST = 10f;
        /// <summary> リールのスピード(中) </summary>
        private const float SPEED_MIDDLE = 1.6f;
        /// <summary> リールのスピード(遅) </summary>
        private const float SPEED_SLOW = 0.6f;

        /// <summary> 図柄のオブジェクト </summary>
        private GameObject[] numberObjects;
        /// <summary> 停止する図柄を受け取り、止まる前のフラグ </summary>
        private bool stopFlg;
        /// <summary> 止まっている状態のフラグ </summary>
        private bool finishFlg;

        private bool reachFlg;
        private int reachNum;

        private float maskPositionY;
        private float maxYPosition;
        private float minYPosition;
        private int nowNumber;
        private int stopNumber;

        private void Start()
        {
            Initvariable();
            InitializeNumberObjects();
        }

        private void Update()
        {

        }

        /// <summary>
        /// 変数の初期化
        /// </summary>
        private void Initvariable()
        {
            numberObjects = new GameObject[numberQuadPrefabs.Length];
            maskPositionY = maskObject.gameObject.transform.localPosition.y;
            nowNumber = firstNumber;
            stopNumber = 0;
            maxYPosition = maskPositionY + DISTANCE_POSITION;
            minYPosition = maskPositionY - DISTANCE_POSITION;
            stopFlg = false;
            finishFlg = true;
        }

        /// <summary>
        /// 図柄の初期化
        /// </summary>
        private void InitializeNumberObjects()
        {
            for (int i = 0; i < numberQuadPrefabs.Length; i++)
            {
                GameObject numberObject = Instantiate(numberQuadPrefabs[i], Vector3.zero, Quaternion.identity, this.transform);

                numberObject.transform.localPosition = Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, 0);

                numberObjects[i] = numberObject;
            }

            numberObjects[nowNumber].transform.localPosition = Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, 0.5f);
        }

        /// <summary>
        /// 回転を開始する
        /// </summary>
        public void StartSpin()
        {
            if (!finishFlg) return;

            StartCoroutine(NumberFlowRoutine());
        }


        /// <summary>
        /// 回転を止める
        /// </summary>
        /// <param name="_number">止めたい図柄の番号</param>
        /// <param name="_reachFlg">リーチ時に中リールのときTrueにする</param>
        /// <param name="_reachNum">左リールの図柄でもいれておこう</param>
        public void StopNumber(int _number, bool _reachFlg, int _reachNum)
        {
            stopNumber = _number;
            reachFlg = _reachFlg;
            reachNum = _reachNum;
            stopFlg = true;
        }

        /// <summary>
        /// スピン中のコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator NumberFlowRoutine()
        {
            stopFlg = false;
            finishFlg = false;
            float nowPosition = 0;
            int keyNum;

            while (!stopFlg)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_FAST);
                yield return null;
            }

            if (reachFlg)
            {
                keyNum = CalcAddNumber(reachNum, -4);
            }
            else
            {
                keyNum = CalcAddNumber(stopNumber, -2);
            }

            while (nowNumber != keyNum)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_FAST);
                yield return null;
            }

            while (nowNumber != stopNumber)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_MIDDLE);
                yield return null;
            }

            while (nowPosition <= 0.5f)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_SLOW);
                yield return null;
            }

            numberObjects[nowNumber].transform.localPosition =
                Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, 0.5f);

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.LilleStop);

            finishFlg = true;

            yield return null;
        }

        /// <summary>
        /// NumberFlowRoutine内のwhileループ
        /// </summary>
        /// <param name="nowPosition">nowPosition</param>
        /// <param name="speed">リールのスピード</param>
        /// <returns>nowPosition</returns>
        private float LoopSpriteRotate(float nowPosition, float speed)
        {
            nowPosition += Time.deltaTime * speed;

            numberObjects[nowNumber].transform.localPosition =
                Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, nowPosition);

            if (nowPosition >= 1)
            {
                nowPosition = 0;

                numberObjects[nowNumber].transform.localPosition =
                    Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, nowPosition);

                nowNumber = CalcAddNumber(nowNumber, 1);
            }

            return nowPosition;
        }

        /// <summary>
        /// 数字を足し算し、値を図柄の数に収める
        /// </summary>
        /// <param name="number">元の値</param>
        /// <param name="add">加算する値(負の値も可能)</param>
        /// <returns></returns>
        private int CalcAddNumber(int number,int add)
        {
            number += add;

            if(number >= numberQuadPrefabs.Length)
            {
                number -= numberQuadPrefabs.Length;
            }
            else if (number < 0)
            {
                number += numberQuadPrefabs.Length;
            }

            return number;
        }

        /// <summary>
        /// リールが停止しているかのフラグを返すゲッター
        /// </summary>
        /// <returns>finishFlg</returns>
        public bool GetFinishFlg()
        {
            return finishFlg;
        }

    }
}
