using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class PayOut : MonoBehaviour
    {
        [SerializeField] private GameObject[] medalPrefab;


        /// <summary>
        /// メダルをスポーンさせる
        /// </summary>
        /// <param name="getMedalNum">獲得するメダルの枚数</param>
        public void SpawnMedals(int getMedalNum)
        {
            StartCoroutine(SpawnMedalsCoroutine(getMedalNum));
        }

        /// <summary>
        /// メダルを生成するコルーチン
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private IEnumerator SpawnMedalsCoroutine(int num)
        {
            GameManager.Instance.SetDisableBallStartFlg(true);

            for (int i = 0; i < num; i++)
            {
                Instantiate(medalPrefab[Random.Range(0, medalPrefab.Length)],
                    this.transform.position, Quaternion.identity, this.transform);

                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalPayout);
                PlayerInformationManager.Instance.AcquisitionMedal(1);

                yield return new WaitForSeconds(0.1f);
            }

            GameManager.Instance.SetGameOverFlgTrue(true);
        }
    }
}
