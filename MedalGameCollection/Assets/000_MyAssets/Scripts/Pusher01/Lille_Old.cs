using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class Lille_Old : MonoBehaviour
    {
        /// <summary> 数字のスプライトを表示するための配列 </summary>
        [SerializeField] private Sprite[] numberSprites;
        /// <summary> マスク処理に使用するQuadオブジェクト </summary>
        [SerializeField] private SpriteMask mask;
        /// <summary> 最初に表示している図柄の番号 </summary>
        [SerializeField] private int firstNumber;

        /// <summary> 図柄たちの間隔 </summary>
        private const float DISTANCE_POSITION = 4.8f;
        /// <summary> リールのスピード(速) </summary>
        private const float SPEED_FAST = 7f;
        /// <summary> リールのスピード(中) </summary>
        private const float SPEED_MIDDLE = 1.6f;
        /// <summary> リールのスピード(遅) </summary>
        private const float SPEED_SLOW = 0.6f;

        /// <summary> 図柄のオブジェクト </summary>
        private SpriteRenderer[] numberObjects;
        /// <summary> 停止する図柄を受け取り、止まる前のフラグ </summary>
        private bool stopFlg;
        /// <summary> 止まっている状態のフラグ </summary>
        private bool finishFlg;

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
            numberObjects = new SpriteRenderer[numberSprites.Length];
            maskPositionY = mask.gameObject.transform.localPosition.y;
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
            for (int i = 0; i < numberSprites.Length; i++)
            {
                GameObject numberObject = new GameObject("Number" + i);
                numberObject.transform.SetParent(transform);

                SpriteRenderer spriteRenderer = numberObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = numberSprites[i % numberSprites.Length];
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

                numberObject.transform.localPosition = Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, 0);

                numberObjects[i] = spriteRenderer;
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
        /// <param name="number">止めたい図柄の番号</param>
        public void StopNumber(int number)
        {
            stopNumber = number;
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

            while (!stopFlg)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_FAST);
                yield return null;
            }

            int keyNum = CalcAddNumber(stopNumber, -2);
            //Debug.Log($"Lille.NumberFlowRoutine : Push Stop : {keyNum}");

            while (nowNumber != keyNum)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_FAST);
                yield return null;
            }

            //Debug.Log($"Lille.NumberFlowRoutine : Before Stop : {stopNumber}");

            while (nowNumber != stopNumber)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_MIDDLE);
                yield return null;
            }

            //Debug.Log($"Lille.NumberFlowRoutine : Stop : {nowNumber}");

            while (nowPosition <= 0.5f)
            {
                nowPosition = LoopSpriteRotate(nowPosition, SPEED_SLOW);
                yield return null;
            }

            numberObjects[nowNumber].transform.localPosition =
                Vector3.Lerp(Vector3.up * maxYPosition, Vector3.up * minYPosition, 0.5f);

            //Debug.Log($"Lille.NumberFlowRoutine : Finish : {nowNumber}");

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

            if(number >= numberSprites.Length)
            {
                number -= numberSprites.Length;
            }
            else if (number < 0)
            {
                number += numberSprites.Length;
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
