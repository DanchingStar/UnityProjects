using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class FireTrigger : MonoBehaviour
    {
        private GameObject triggerObject;
        private bool isAbleFlg = false;

        void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // �����Ɂ��������Ƃ��Ɂ~�~����Ƃ����������L�q���܂��B

                triggerObject = other.gameObject;
                isAbleFlg = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                // �����Ɂ��������Ƃ��Ɂ~�~����Ƃ����������L�q���܂��B

                triggerObject = null;
                isAbleFlg = false;
            }
        }

        /// <summary>
        /// ����̂���{�[���ɏ�����̗͂�������
        /// </summary>
        /// <param name="power"></param>
        public void FireBall(float power)
        {
            if (isAbleFlg)
            {
                Rigidbody rigidbody = triggerObject.GetComponent<Rigidbody>();
                float mass = rigidbody.mass;
                rigidbody.AddForce(Vector3.forward * mass * power);
            }
        }
    }
}

