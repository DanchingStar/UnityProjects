using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class Ball : MonoBehaviour
    {
        private Rigidbody rb;
        private SphereCollider sc;

        private float mass = 0;
        private float velocity = 0;
        private float stopTime = 0;

        // private bool respawnFlf = false;
        private bool isClear = false;

        private void Start()
        {
            this.GetComponent<Transform>().Rotate(Vector3.one * Random.Range(0, 360));
            rb = this.GetComponent<Rigidbody>();
            sc = this.GetComponent<SphereCollider>();

            mass = rb.mass;

            // 下方向に力を加える
            rb.AddForce(Random.Range(300, 500) * mass * new Vector3(0, -1, -1.73f));
        }

        private void Update()
        {
            if (!isClear)
            {
                BallStopStop();

                //if (rb.velocity.magnitude < 5f && rb.velocity.magnitude > 2f)
                //{
                //    rb.AddForce(rb.velocity.normalized * 500, ForceMode.Force);
                //}
                //Debug.Log($"rb.velocity : {rb.velocity.magnitude}");

            }
            BallRotate();
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.gameObject.tag == "Point" && isClear == false)
            {
                int getNum = trigger.GetComponent<PointTrigger>().GetPointNumber();
                GameManager.Instance.SetPointNumber(getNum);

                isClear = true;
                GameManager.Instance.SetDisableBallStartFlg(false);
                // Destroy(this.GetComponent<Ball>());
            }
            else if (trigger.gameObject.tag == "Despawn")
            {
                GameManager.Instance.SetDisableBallStartFlg(false);

                if (GameManager.Instance.GetFinishFlg() == false)
                {
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.BallOut);
                }

                StartCoroutine(DestroyStaging(1));
            }
            else if (trigger.gameObject.tag == "TypeA")
            {
                GameManager.Instance.ChangeRespawnIgnoreCollision(sc, true);
            }
            else if (trigger.gameObject.tag == "TypeB")
            {
                GameManager.Instance.ChangeRespawnIgnoreCollision(sc, false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isClear == false && collision.gameObject.tag == "Other")
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.BallCollide);
            }
        }


        /// <summary>
        /// ランダムでboolの値を返す
        /// </summary>
        /// <returns></returns>
        private bool MakeRandomBool()
        {
            if (0.5 < (Random.Range(0f, 1f)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ボールが釘の上に乗ったときに与える力
        /// </summary>
        /// <param name="b">正なら右、負なら左</param>
        /// <param name="power">与える力の強さ</param>
        private void AddPowerOnNail(bool b, float power)
        {
            if (b)
            {
                // Debug.Log("Right");
                rb.AddForce(Vector3.right * power);
            }
            else
            {
                // Debug.Log("Left");
                rb.AddForce(Vector3.left * power);
            }
        }

        /// <summary>
        /// ボールが止まるのを防ぐ
        /// </summary>
        private void BallStopStop()
        {
            velocity = rb.velocity.magnitude;
            // Debug.Log(velocity);
            if (velocity < 0.001f)
            {
                stopTime += Time.deltaTime;
            }
            else
            {
                stopTime = 0;
            }
            if (stopTime > 1f)
            {
                AddPowerOnNail(MakeRandomBool(), 50f * mass);
                stopTime = 0;
            }
        }

        /// <summary>
        /// ボールを移動した方向に合わせて回転させる
        /// </summary>
        private void BallRotate()
        {
            var translation = rb.velocity * Time.deltaTime; // 位置の変化量
            var distance = translation.magnitude; // 移動した距離
            var scaleXYZ = transform.lossyScale; // ワールド空間でのスケール推定値
            var scale = Mathf.Max(scaleXYZ.x, scaleXYZ.y, scaleXYZ.z); // 各軸のうち最大のスケール
            var angle = distance / (sc.radius * scale); // 球が回転するべき量
            var axis = Vector3.Cross(Vector3.up, translation).normalized; // 球が回転するべき軸
            var deltaRotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis); // 現在の回転に加えるべき回転

            // 現在の回転からさらにdeltaRotationだけ回転させる
            rb.MoveRotation(deltaRotation * rb.rotation);
        }

        /// <summary>
        /// second秒後に消える
        /// </summary>
        /// <returns>消えるまでの時間</returns>
        private IEnumerator DestroyStaging(float second)
        {
            yield return new WaitForSeconds(second);
            Destroy(this.gameObject);
        }
    }
}