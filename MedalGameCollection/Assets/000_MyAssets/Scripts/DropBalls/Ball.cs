using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropBalls
{
    public class Ball : MonoBehaviour
    {
        private int startMaxPower = 1000;
        private int startMinPower = 10000;

        private Rigidbody rb;
        private SphereCollider sc;

        private bool isValid = false;

        private float velocity = 0;
        private float stopTime = 0;

        private void Start()
        {
            this.GetComponent<Transform>().Rotate(Vector3.one * Random.Range(0, 360));
            rb = this.GetComponent<Rigidbody>();
            sc = this.GetComponent<SphereCollider>();

            int power = Random.Range(startMaxPower, startMinPower);
            rb.AddForce(power * Vector3.down);
            // Debug.Log(power);
        }

        private void Update()
        {
            if (!isValid)
            {
                BallStopStop();

                BallRotate();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                GameManager.Instance.SetGameOverFlgTrue(false);
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.GameOver);
                isValid = true;
            }
            else if (collision.gameObject.tag == "Other")
            {
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.CollideNail);
                if (this.transform.position.y > collision.gameObject.transform.position.y)
                {
                    AddPowerOnNail(MakeRandomBool(), 1000);
                }
            }
            else if (collision.gameObject.tag == "Despawn")
            {
                if (!isValid)
                {
                    GameManager.Instance.SetDisableBallStartFlg(false);
                }
                if (!GameManager.Instance.GetGameOverFlg())
                {
                    SoundManager.Instance.PlaySE(SoundManager.SoundSeType.BallOut);
                    PlayerInformationManager.Instance.IncrementField("DropBallsData", "lostBallsTotal");
                }
                Destroy(this.gameObject);
            }
            else if (collision.gameObject.tag == "Finish" && isValid == false)
            {
                isValid = true;
                GameManager.Instance.SetDisableBallStartFlg(false);
                GameManager.Instance.SetOnBall();
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
        /// <param name="power">与える力の強さ(1000が普通)</param>
        private void AddPowerOnNail(bool b, int power)
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
            if (stopTime > 0.1f)
            {
                AddPowerOnNail(MakeRandomBool(), 500);
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

    }
}