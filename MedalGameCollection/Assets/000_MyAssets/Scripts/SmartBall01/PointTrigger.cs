using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class PointTrigger : MonoBehaviour
    {
        [SerializeField] private int myNumber;

        /// <summary>
        /// �|�C���g���̔ԍ���Ԃ��Q�b�^�[
        /// </summary>
        /// <returns>�|�C���g���̔ԍ�</returns>
        public int GetPointNumber()
        {
            return myNumber;
        }
    }
}
