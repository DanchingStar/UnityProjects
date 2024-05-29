using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class MedalPayFall : MonoBehaviour
    {
        [SerializeField] private GameObject[] medalPrefab;

        private const int FALL_RANGE_WIDTH_X = 8;
        private const int FALL_RANGE_WIDTH_Z = 2;

        private int fallMedals;
        private bool coroutineFlg;
        private void Start()
        {
            fallMedals = 0;
            coroutineFlg = false;
        }

        private void Update()
        {
            FallMedal();
        }


        /// <summary>
        /// 降ってくるメダルを引数の数だけ増やす
        /// </summary>
        /// <param name="num">降らすメダルの枚数</param>
        public void AddFallMedals(int num)
        {
            fallMedals += num;
        }

        /// <summary>
        /// 条件を満たすとき、メダルを降らす
        /// </summary>
        private void FallMedal()
        {
            if (fallMedals <= 0) return;
            if (coroutineFlg) return;

            StartCoroutine(FallMedalsCoroutine());
        }

        /// <summary>
        /// メダルを降らすコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator FallMedalsCoroutine()
        {
            coroutineFlg = true;
            while (fallMedals > 0)
            {
                int randNumX = Random.Range(-FALL_RANGE_WIDTH_X, FALL_RANGE_WIDTH_X + 1);
                int randNumZ = Random.Range(-FALL_RANGE_WIDTH_Z, FALL_RANGE_WIDTH_Z + 1);

                GameObject obj = Instantiate(medalPrefab[Random.Range(0, medalPrefab.Length)],
                    this.transform.position + (Vector3.right * randNumX) + (Vector3.forward * randNumZ), Random.rotation, this.transform);

                obj.GetComponent<Rigidbody>().AddForce(Vector3.down * 100000);

                fallMedals--;

                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalFall);

                yield return new WaitForSeconds(0.1f);
            }
            coroutineFlg = false;
        }
    }

}
