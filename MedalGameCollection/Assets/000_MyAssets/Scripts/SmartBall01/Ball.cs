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

            // �������ɗ͂�������
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
        /// <param name="power">�^����͂̋���</param>
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
            if (stopTime > 1f)
            {
                AddPowerOnNail(MakeRandomBool(), 50f * mass);
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

        /// <summary>
        /// second�b��ɏ�����
        /// </summary>
        /// <returns>������܂ł̎���</returns>
        private IEnumerator DestroyStaging(float second)
        {
            yield return new WaitForSeconds(second);
            Destroy(this.gameObject);
        }
    }
}