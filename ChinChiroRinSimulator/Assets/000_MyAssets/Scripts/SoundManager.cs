using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] seSource;

    [SerializeField] private GameObject BGMObjectParent;
    [SerializeField] private GameObject SEObjectParent;

    private float BGM_Volume;
    private float SE_Volume;

    private const string KEY_VOLUME_BGM = "KEY_VOLUME_BGM";
    private const string KEY_VOLUME_SE = "KEY_VOLUME_SE";
    private const float DEFAULT_VOLUME = 1.0f;

    private Transform myBgmAudioTF;
    private Transform mySeAudioTF;

    private AudioSource stageMusicAudioSource;

    [Serializable]
    public enum SoundSeType
    {
        None,
        Yes,
        No,
        Ok,
        Select,

        StandBy,

        Dice1,
        Dice2,
        Dice3,
        Dice4,

        ResultS,
        ResultA,
        ResultB,
        ResultC,
        ResultD,
        ResultE,

    }

    [Serializable]
    public enum SoundBgmType
    {
        AllScene,
    }

    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        myBgmAudioTF = BGMObjectParent.GetComponent<Transform>();
        mySeAudioTF = SEObjectParent.GetComponent<Transform>();

        stageMusicAudioSource = myBgmAudioTF.Find("StageMusic").GetComponent<AudioSource>();

        LoadPlayerPrefs();
        VolumeChangeAll();
    }

    private void Update()
    {
        //if (UICommonPanelManager.Instance.GetIsActiveSoundSettingPanel())
        //{
        //    VolumeChange();
        //}
    }

    /// <summary>
    /// SE���Đ�����
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SoundSeType seType)
    {
        seSource[(int)seType].Play();

        //Debug.Log($"SoundManager.PlaySE : Play = {seType}");
    }

    /// <summary>
    /// �T�C�R���̉��������_���ōĐ�����
    /// </summary>
    public void PlayDiceRandomSE()
    {
        int randNum = UnityEngine.Random.Range(0, 4);

        switch (randNum)
        {
            case 0:
                PlaySE(SoundSeType.Dice1);
                break;
            case 1:
                PlaySE(SoundSeType.Dice2);
                break;
            case 2:
                PlaySE(SoundSeType.Dice3);
                break;
            case 3:
                PlaySE(SoundSeType.Dice4);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �X�e�[�W�̉��y���Z�b�g����
    /// </summary>
    /// <param name="_SuccessMusicClip"></param>
    public void SetStageMusic(AudioClip _StageMusicClip)
    {
        stageMusicAudioSource.clip = _StageMusicClip;
    }

    /// <summary>
    /// �X�e�[�W�̉��y���Đ�����
    /// </summary>
    public void PlayStageMusic()
    {
        if (stageMusicAudioSource.clip != null)
        {
            stageMusicAudioSource.Play();
        }
    }

    /// <summary>
    /// �X�e�[�W�̉��y���~����
    /// </summary>
    public void StopStageMusic()
    {
        if (stageMusicAudioSource.clip != null)
        {
            stageMusicAudioSource.Stop();
        }
    }

    /// <summary>
    /// �S�Ẵ{�����[����ύX����
    /// </summary>
    private void VolumeChangeAll()
    {
        ChangeVolumeValue(myBgmAudioTF, BGM_Volume);
        ChangeVolumeValue(mySeAudioTF, SE_Volume);
    }

    /// <summary>
    /// BGM�̃{�����[����ύX����
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForBGM(float value)
    {
        BGM_Volume = value;
        ChangeVolumeValue(myBgmAudioTF, BGM_Volume);
    }

    /// <summary>
    /// SE�̃{�����[����ύX����
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForSE(float value)
    {
        SE_Volume = value;
        ChangeVolumeValue(mySeAudioTF, SE_Volume);
    }

    /// <summary>
    /// �w�肵�����̃{�����[���̒l�����ۂɕύX����
    /// </summary>
    /// <param name="tf"></param>
    /// <param name="value"></param>
    private void ChangeVolumeValue(Transform tf, float value)
    {
        foreach (Transform childTF in tf)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = value;
        }
    }

    /// <summary>
    /// ���ꂼ��̃{�����[���̒l��PlayerPrefs�ɕۑ�����
    /// </summary>
    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat(KEY_VOLUME_BGM, BGM_Volume);
        PlayerPrefs.SetFloat(KEY_VOLUME_SE, SE_Volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ���ꂼ��̃{�����[���̒l��PlayerPrefs����ǂݍ���
    /// </summary>
    private void LoadPlayerPrefs()
    {
        BGM_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_BGM, DEFAULT_VOLUME);
        SE_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_SE, DEFAULT_VOLUME);
    }

    /// <summary>
    /// BGM�̃{�����[���̒l��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForBGM()
    {
        return BGM_Volume;
    }

    /// <summary>
    /// SE�̃{�����[���̒l��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForSE()
    {
        return SE_Volume;
    }



}
