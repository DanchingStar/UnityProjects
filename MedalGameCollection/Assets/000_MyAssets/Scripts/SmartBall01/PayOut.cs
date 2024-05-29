using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class PayOut : MonoBehaviour
    {
        [SerializeField] private GameObject[] medalPrefab;

        private bool isGameWait = false; 

        public static PayOut Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// ���_�����X�|�[��������
        /// </summary>
        public void SpawnMedals(int num)
        {
            StartCoroutine(SpawnMedalsCoroutine(num));
        }

        private IEnumerator SpawnMedalsCoroutine(int num)
        {
            isGameWait = true;

            for (int i = 0; i < num; i++)
            {
                Instantiate(medalPrefab[Random.Range(0, medalPrefab.Length)],
                    this.transform.position, Quaternion.identity, this.transform);

                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalPayout);
                PlayerInformationManager.Instance.AcquisitionMedal(1);

                yield return new WaitForSeconds(0.1f);
            }

            isGameWait = false;
        }

        /// <summary>
        /// isGameWait�̃Q�b�^�[
        /// </summary>
        /// <returns></returns>
        public bool GetIsGameWait()
        {
            return isGameWait;
        }
    }
}
