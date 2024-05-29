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
        /// �����_����bool�̒l��Ԃ�
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
        /// �{�[�����B�̏�ɏ�����Ƃ��ɗ^�����
        /// </summary>
        /// <param name="b">���Ȃ�E�A���Ȃ獶</param>
        /// <param name="power">�^����͂̋���(1000������)</param>
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
        /// �{�[�����~�܂�̂�h��
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
        /// �{�[�����ړ����������ɍ��킹�ĉ�]������
        /// </summary>
        private void BallRotate()
        {
            var translation = rb.velocity * Time.deltaTime; // �ʒu�̕ω���
            var distance = translation.magnitude; // �ړ���������
            var scaleXYZ = transform.lossyScale; // ���[���h��Ԃł̃X�P�[������l
            var scale = Mathf.Max(scaleXYZ.x, scaleXYZ.y, scaleXYZ.z); // �e���̂����ő�̃X�P�[��
            var angle = distance / (sc.radius * scale); // ������]����ׂ���
            var axis = Vector3.Cross(Vector3.up, translation).normalized; // ������]����ׂ���
            var deltaRotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis); // ���݂̉�]�ɉ�����ׂ���]

            // ���݂̉�]���炳���deltaRotation������]������
            rb.MoveRotation(deltaRotation * rb.rotation);
        }

    }
}