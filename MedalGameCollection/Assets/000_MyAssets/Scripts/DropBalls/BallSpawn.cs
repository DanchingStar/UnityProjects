using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class BallSpawn : MonoBehaviour
    {
        [SerializeField] private GameObject[] ballPrefab;

        /// <summary>
        /// �{�[�����X�|�[��������
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
        /// �{�[�����X�|�[��������(�f�o�b�O�p)
        /// </summary>
        /// <param name="tr">�{�[�����o��������ꏊ��Transform</param>
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
