using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class BallSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] ballPrefab;

        /// <summary>
        /// ボールをスポーンさせる
        /// </summary>
        public void SpawnBall()
        {
            GameObject myGameobject = Instantiate(ballPrefab[Random.Range(0, ballPrefab.Length)],
                this.transform.localPosition, Quaternion.identity, this.transform);

            if (myGameobject != null)
            {
                GameManager.Instance.SetDisableBallStartFlg(true);
            }
        }

        /// <summary>
        /// ボールをスポーンさせる(デバッグ用)
        /// </summary>
        /// <param name="tr">ボールを出現させる場所のTransform</param>
        public void SpawnBall(Transform tf)
        {
            if (GameManager.Instance.GetDisableBallStartFlg()) return;

            GameObject myGameobject = Instantiate(ballPrefab[Random.Range(0, ballPrefab.Length)],
                tf.position, Quaternion.identity, this.transform);

            if (myGameobject != null)
            {
                GameManager.Instance.SetDisableBallStartFlg(true);
            }
            else
            {
                Debug.Log($"SpawnBall(tf) : myGameobject is null");
            }
        }
    }
}
