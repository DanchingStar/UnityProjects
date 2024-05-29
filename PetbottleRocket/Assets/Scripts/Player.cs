using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    /// <summary>�o�i�[�L��������</summary>
    [SerializeField] private AdMobBanner myBanner = null;

    /// <summary>���@�̊p�x�ύX�̃X�s�[�h</summary>
    [SerializeField] private float rotateSpeed = 20.0f;
    /// <summary>���@�̐i�ރX�s�[�h</summary>
    [SerializeField] private float moveOnSpeed = 3.0f;
    /// <summary>���@�̗��߂�X�s�[�h</summary>
    [SerializeField] private float accumulateSpeed = 50.0f;

    /// <summary>�X�R�A�̃e�L�X�g��ݒ肷��</summary>
    [SerializeField] private GameObject scoreText = null;

    /// <summary>�p�x�\�����̓����Ȃ�����ݒ肷��</summary>
    [SerializeField] private GameObject displayAngleNeutral = null;
    /// <summary>�p�x�\�����̓�������ݒ肷��</summary>
    [SerializeField] private GameObject displayAngleChange = null;

    /// <summary>CameraShake�X�N���v�g��\���Ă���I�u�W�F�N�g��ݒ肷��</summary>
    [SerializeField] private CameraShake shake;

    [SerializeField] private AudioClip SE_SmallHit;
    [SerializeField] private AudioClip SE_Collision;
    [SerializeField] private AudioClip SE_Explosion;
    [SerializeField] private AudioClip SE_FlyAway;
    private AudioSource audioSource;

    /// <summary>���@�̊p�x�A+���������A-���E����</summary>
    private float playerRotate = 0.0f;

    /// <summary>���߂Ă��鎞��</summary>
    private float accumulateTime = 0.0f;

    /// <summary>�Q�[���I�[�o�[�ɂȂ�����true</summary>
    private bool isGameOverFlg = false;

    /// <summary>�Q�[���I�[�o�[�ɂȂ��Ă���̎���</summary>
    private float timeGameOver = 0.0f;

    /// <summary>�Q�[���I�[�o�[�ɂȂ��Ă���̎���</summary>
    private float nowScore = 0.0f;

    /// <summary>NowScore��pref�ɋL������Ƃ��̃L�[(������)</summary>
    private string prefStringNowScore = "NowScore";


    void Start()
    {
        isGameOverFlg = false;
        timeGameOver = 0f;
        playerRotate = 0.0f;
        displayAngleNeutral.SetActive(false);
        displayAngleChange.SetActive(false);
        nowScore = 0.0f;
        PlayerPrefs.DeleteKey(prefStringNowScore);
        audioSource = GetComponent<AudioSource>();

        if (myBanner)
        {
            myBanner.RequestBanner(); //�o�i�[�L�����Ăяo��
        }
    }

    void Update()
    {
        if (!isGameOverFlg)
        {
            PlayerInputTap();

            ScoreManager();

            CheckGameOver();
        }
        else
        {
            GameOver();
        }

        PlayerFreeRotate();

        PlayerRotate();

        PlayerMoveOn();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit!!");

            var targetEnemy = collision.gameObject.GetComponent<Enemy>();

            if (targetEnemy.IsDependsPlayerAngle)
            {
                if (playerRotate >= 0f && playerRotate < 160f)
                {
                    playerRotate += targetEnemy.DamageAngle;
                }
                else if (playerRotate < 0f && playerRotate > -160f)
                {
                    playerRotate -= targetEnemy.DamageAngle;
                }
            }
            else
            {
                playerRotate += targetEnemy.DamageAngle;
            }

            float ang = Mathf.Abs(targetEnemy.DamageAngle);

            if(ang < 5f)
            {
                audioSource.PlayOneShot(SE_SmallHit);
            }
            else if(ang < 20f)
            { 
                audioSource.PlayOneShot(SE_Collision);
            }
            else
            {
                audioSource.PlayOneShot(SE_Explosion);
            }

            //��ʂ�h�炷
            shake.Shake(0.1f, ang * 0.02f );


        }
    }


    /// <summary>
    /// �v���C���[�̓��͂ɑ΂��ď�������֐�
    /// </summary>
    private void PlayerInputTap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            accumulateTime = 0.0f;
            displayAngleNeutral.SetActive(true);
            displayAngleChange.SetActive(true);
        }
        if (Input.GetMouseButton(0))
        {
            accumulateTime += Time.deltaTime;
            if (this.transform.rotation.z <= 0f)
            {
                displayAngleChange.transform.localEulerAngles = new Vector3(0, 0, accumulateTime * accumulateSpeed);
            }
            else
            {
                displayAngleChange.transform.localEulerAngles = new Vector3(0, 0, -accumulateTime * accumulateSpeed);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(this.transform.rotation.z <= 0f)
            {
                playerRotate += accumulateTime * accumulateSpeed;
            }
            else
            {
                playerRotate -= accumulateTime * accumulateSpeed;

            }
            displayAngleNeutral.SetActive(false);
            displayAngleChange.SetActive(false);
            displayAngleChange.transform.localEulerAngles = displayAngleNeutral.transform.localEulerAngles;
        }
    }

    /// <summary>
    /// ���@��������]����֐�
    /// </summary>
    private void PlayerFreeRotate()
    {
        if (playerRotate >= 0f && playerRotate < 160f )
        {
            playerRotate += Time.deltaTime * rotateSpeed;
        }
        else if(playerRotate < 0f && playerRotate > -160f)
        {
            playerRotate -= Time.deltaTime * rotateSpeed;
        }
        else
        {

        }
    }

    /// <summary>
    /// ��]�����s����֐�
    /// </summary>
    private void PlayerRotate()
    {
        this.transform.rotation = Quaternion.Euler(0f, 0f, playerRotate);
    }

    /// <summary>
    /// �O�ɐi�ފ֐�
    /// </summary>
    private void PlayerMoveOn()
    {
        transform.position += transform.up * moveOnSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �X�R�A���X�V���ăe�L�X�g�ɕ\������֐�
    /// </summary>
    private void ScoreManager()
    {
        nowScore = this.transform.position.y;
        // �I�u�W�F�N�g����Text�R���|�[�l���g���擾
        Text t = scoreText.GetComponent<Text>();
        // �e�L�X�g�̕\�������ւ���
        t.text = "�X�R�A : " + nowScore.ToString("F2");
    }

    /// <summary>
    /// �Q�[���I�[�o�[�𔻒肷��֐�
    /// </summary>
    private void CheckGameOver()
    {
        if (playerRotate > 90f)
        {
            isGameOverFlg = true;
        }
        if (playerRotate < -90f)
        {
            isGameOverFlg = true;
        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[���ɓ����֐�
    /// </summary>
    private void GameOver()
    {
        if(timeGameOver == 0f)
        {
            audioSource.PlayOneShot(SE_FlyAway);

            displayAngleNeutral.SetActive(false);
            displayAngleChange.SetActive(false);
            //Debug.Log("�Q�[���I�[�o�[");

            PlayerPrefs.SetFloat(prefStringNowScore, nowScore);
            PlayerPrefs.Save();
        }
        else if (timeGameOver > 2f)
        {
            if (myBanner)
            {
                myBanner.DestroyBanner();
            }

            FadeManager.Instance.LoadScene("Result", 0.3f);
        }

        timeGameOver += Time.deltaTime;
    }

    /// <summary>
    /// �Q�[���I�[�o�[�t���O�̃Q�b�^�[���{
    /// </summary>
    public bool GetGameOverFlg()
    {
        return isGameOverFlg;
    }


}

