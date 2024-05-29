using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class Lille : MonoBehaviour
    {
        /// <summary> �}���������Ă���Quad��Prefab </summary>
        [SerializeField] private GameObject[] numberQuadPrefabs;
        /// <summary> �}�X�N�����Ɏg�p����Quad�I�u�W�F�N�g </summary>
        [SerializeField] private GameObject maskObject;
        /// <summary> �ŏ��ɕ\�����Ă���}���̔ԍ� </summary>
        [SerializeField] private int firstNumber;

        /// <summary> �}�������̊Ԋu </summary>
        private const float DISTANCE_POSITION = 4.8f;
        /// <summary> ���[���̃X�s�[�h(��) </summary>
        private const float SPEED_FAST = 10f;
        /// <summary> ���[���̃X�s�[�h(��) </summary>
        private const float SPEED_MIDDLE = 1.6f;
        /// <summary> ���[���̃X�s�[�h(�x) </summary>
        private const float SPEED_SLOW = 0.6f;

        /// <summary> �}���̃I�u�W�F�N�g </summary>
        private GameObject[] numberObjects;
        /// <summary> ��~����}�����󂯎��A�~�܂�O�̃t���O </summary>
        private bool stopFlg;
        /// <summary> �~�܂��Ă����Ԃ̃t���O </summary>
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
        /// �ϐ��̏�����
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
        /// �}���̏�����
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
        /// <param name="_number">�~�߂����}���̔ԍ�</param>
        /// <param name="_reachFlg">���[�`���ɒ����[���̂Ƃ�True�ɂ���</param>
        /// <param name="_reachNum">�����[���̐}���ł�����Ă�����</param>
        public void StopNumber(int _number, bool _reachFlg, int _reachNum)
        {
            stopNumber = _number;
            reachFlg = _reachFlg;
            reachNum = _reachNum;
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
        /// ���[������~���Ă��邩�̃t���O��Ԃ��Q�b�^�[
        /// </summary>
        /// <returns>finishFlg</returns>
        public bool GetFinishFlg()
        {
            return finishFlg;
        }

    }
}
