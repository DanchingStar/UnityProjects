using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class MedalSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] medalPrefab;
        [SerializeField] private GameObject[] medalOfStartPositionPrefab;


        private void Start()
        {
            SpawnMedalOfStart();
        }
        /// <summary>
        /// メダルをスポーンさせる
        /// </summary>
        public void SpawnMedal()
        {
           // if (GameManager.Instance.GetDisableMedalStartFlg()) return;

            GameObject myGameobject = Instantiate(medalPrefab[Random.Range(0, medalPrefab.Length)],
                this.transform.localPosition, Quaternion.Euler(90,0,0), this.transform);

            //if (myGameobject != null)
            //{
            //   GameManager.Instance.SetDisableMedalStartFlg(true);
            //}
        }

        /// <summary>
        /// ゲームスタート時にメダルをスポーンさせる
        /// </summary>
        /// <param name="tr">ボールを出現させる場所のTransform</param>
        public void SpawnMedalOfStart()
        {
            for (int x = 0; x < 10; x++) 
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        float ax = x * 2.25f;
                        float ay = y * 0.35f;
                        float az = z * 2.25f;
                        Instantiate(medalOfStartPositionPrefab[Random.Range(0, medalOfStartPositionPrefab.Length)],
                         new Vector3(-10f - 0.125f + ax, -7.825f + ay, -18.5f + az + (y * 1.125f)), Quaternion.identity, this.transform);
                    }
                }
            }

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        float ax = x * 2.25f;
                        float ay = y * 0.35f;
                        float az = z * 2.25f;
                        Instantiate(medalOfStartPositionPrefab[Random.Range(0, medalOfStartPositionPrefab.Length)],
                         new Vector3(-10f - 0.125f + ax, -4.8f + ay, -5.75f + az + (y * 1.125f)), Quaternion.identity, this.transform);
                    }
                }
            }
        }

    }
}
