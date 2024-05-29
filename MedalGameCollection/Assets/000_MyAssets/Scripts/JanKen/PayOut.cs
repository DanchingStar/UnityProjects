using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JanKen
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
        /// メダルをスポーンさせる
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

                SoundManager.Instance.PlaySE(SoundManager.SoundType.MedalPayout);

                yield return new WaitForSeconds(0.1f);
            }

            isGameWait = false;
        }

        /// <summary>
        /// isGameWaitのゲッター
        /// </summary>
        /// <returns></returns>
        public bool GetIsGameWait()
        {
            return isGameWait;
        }
    }
}
