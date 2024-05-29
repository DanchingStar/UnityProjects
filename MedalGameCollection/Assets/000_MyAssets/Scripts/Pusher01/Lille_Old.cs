using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class Lille_Old : MonoBehaviour
    {
        /// <summary> �����̃X�v���C�g��\�����邽�߂̔z�� </summary>
        [SerializeField] private Sprite[] numberSprites;
        /// <summary> �}�X�N�����Ɏg�p����Quad�I�u�W�F�N�g </summary>
        [SerializeField] private SpriteMask mask;
        /// <summary> �ŏ��ɕ\�����Ă���}���̔ԍ� </summary>
        [SerializeField] private int firstNumber;

        /// <summary> �}�������̊Ԋu </summary>
        private const float DISTANCE_POSITION = 4.8f;
        /// <summary> ���[���̃X�s�[�h(��) </summary>
        private const float SPEED_FAST = 7f;
        /// <summary> ���[���̃X�s�[�h(��) </summary>
        private const float SPEED_MIDDLE = 1.6f;
        /// <summary> ���[���̃X�s�[�h(�x) </summary>
        private const float SPEED_SLOW = 0.6f;

        /// <summary> �}���̃I�u�W�F�N�g </summary>
        private SpriteRenderer[] numberObjects;
        /// <summary> ��~����}�����󂯎��A�~�܂�O�̃t���O </summary>
        private bool stopFlg;
        /// <summary> �~�܂��Ă����Ԃ̃t���O </summary>
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
        /// �ϐ��̏�����
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
        /// �}���̏�����
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
        /// ��]���J�n����
        /// </summary>
        public void StartSpin()
        {
            if (!finishFlg) return;

            StartCoroutine(NumberFlowRoutine());
        }

        /// <summary>
        /// ��]���~�߂�
        /// </summary>
        /// <param name="number">�~�߂����}���̔ԍ�</param>
        public void StopNumber(int number)
        {
            stopNumber = number;
            stopFlg = true;
        }

        /// <summary>
        /// �X�s�����̃R���[�`��
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
        /// NumberFlowRoutine����while���[�v
        /// </summary>
        /// <param name="nowPosition">nowPosition</param>
        /// <param name="speed">���[���̃X�s�[�h</param>
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
        /// �����𑫂��Z���A�l��}���̐��Ɏ��߂�
        /// </summary>
        /// <param name="number">���̒l</param>
        /// <param name="add">���Z����l(���̒l���\)</param>
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
        /// ���[������~���Ă��邩�̃t���O��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns>finishFlg</returns>
        public bool GetFinishFlg()
        {
            return finishFlg;
        }

    }
}
