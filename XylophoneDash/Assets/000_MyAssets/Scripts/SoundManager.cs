using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] seSource;

    [SerializeField] private GameObject BGMObjectParent;
    [SerializeField] private GameObject SEObjectParent;
    [SerializeField] private GameObject KeyboardObjectParent;

    private AudioSource[] xySource;

    private float BGM_Volume;
    private float SE_Volume;
    private float Keyboard_Volume;

    private const string KEY_VOLUME_BGM = "KEY_VOLUME_BGM";
    private const string KEY_VOLUME_SE = "KEY_VOLUME_SE";
    private const string KEY_VOLUME_KEYBOARD = "KEY_VOLUME_KEYBOARD";
    private const float DEFAULT_VOLUME = 1.0f;

    private Transform myBgmAudioTF;
    private Transform mySeAudioTF;
    private Transform myKeyboardAudioTF;

    private AudioSource stageMusicAudioSource;
    private AudioSource successMusicAudioSource;

    private const int XYLOPHONE_SOURCE_SIZE = 5;

    [Serializable]
    public enum SoundSeType
    {
        None,
        Yes,
        No,
        Ok,
        Select,
        InformationCutIn,
        KiraKira,

        GameClear,
        GameOver,
        GameCountDounReady,
        GameCountDounStart,

        GameMenuOpen,
        GameMenuClose,
        GameMenuSelect,
        GameMenuSceneMove,

        WadaikoA,
        WadaikoB,
        WadaikoC,
        ToyTrain,
    }

    //[Serializable]
    //public enum SoundBgmType
    //{
    //    Tuujou,
    //    Kakuhen,
    //    Super,
    //}

    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
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
        myKeyboardAudioTF = KeyboardObjectParent.GetComponent<Transform>();

        stageMusicAudioSource = myBgmAudioTF.Find("StageMusic").GetComponent<AudioSource>();
        successMusicAudioSource = myBgmAudioTF.Find("SuccessMusic").GetComponent<AudioSource>();

        InitXylophoneAudioSource();

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
    /// ��Ƀ��j���[��ʂŎg�����Ղ�SE�̏�����
    /// </summary>
    private void InitXylophoneAudioSource()
    {
        xySource = new AudioSource[XYLOPHONE_SOURCE_SIZE];

        for (int i = 0; i < XYLOPHONE_SOURCE_SIZE; i++)
        {
            switch (i)
            {
                case 0:
                    xySource[i] = myKeyboardAudioTF.Find("Xy52").GetComponent<AudioSource>();
                    break;
                case 1:
                    xySource[i] = myKeyboardAudioTF.Find("Xy54").GetComponent<AudioSource>();
                    break;
                case 2:
                    xySource[i] = myKeyboardAudioTF.Find("Xy56").GetComponent<AudioSource>();
                    break;
                case 3:
                    xySource[i] = myKeyboardAudioTF.Find("Xy57").GetComponent<AudioSource>();
                    break;
                case 4:
                    xySource[i] = myKeyboardAudioTF.Find("Xy59").GetComponent<AudioSource>();
                    break;
                default:
                    Debug.LogError($"SoundManager.InitXylophoneAudioSource : Error -> [i] = {i}");
                    break;
            }
        }
    }

    /// <summary>
    /// SE���Đ�����
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SoundSeType seType)
    {
        seSource[(int)seType].Play();

        //Debug.Log($"SoundManager.PlaySE : SE Type -> {seType}");
    }

    /// <summary>
    /// ��Ƀ��j���[�V�[���Ō��Ղ�@�������ɂȂ炷SE���Đ�����
    /// </summary>
    /// <param name="mode"></param>
    public void PlayXylophoneSE(MenuSceneManager.SelectMode mode)
    {
        int number = -1;

        switch(mode)
        {
            case MenuSceneManager.SelectMode.StageClearMode:
                number = 0;
                break;
            case MenuSceneManager.SelectMode.RankingMode:
                number = 1;
                break;
            case MenuSceneManager.SelectMode.FreeMode:
                number = 2;
                break;
            case MenuSceneManager.SelectMode.Shop:
                number = 3;
                break;
            case MenuSceneManager.SelectMode.Setting:
                number = 4;
                break;
            default:
                Debug.LogError($"SoundManager.PlayXylophoneSE : Error -> Mode = {mode}");
                return;
        }

        xySource[number].Play();

        //Debug.Log($"SoundManager.PlayXylophoneSE : Xylophone Type -> {mode}");
    }

    /// <summary>
    /// �������̉��y���Z�b�g����
    /// </summary>
    /// <param name="_SuccessMusicClip"></param>
    public void SetSuccessMusic(AudioClip _SuccessMusicClip)
    {
        successMusicAudioSource.clip = _SuccessMusicClip;
    }

    /// <summary>
    /// �������̉��y���Đ�����
    /// </summary>
    public void PlaySuccessMusic()
    {
        if (successMusicAudioSource.clip != null)
        {
            successMusicAudioSource.Play();
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
        ChangeVolumeValue(myKeyboardAudioTF, Keyboard_Volume);
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
    /// ���Ղ̃{�����[����ύX����
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForKeyboard(float value)
    {
        Keyboard_Volume = value;
        ChangeVolumeValue(myKeyboardAudioTF, Keyboard_Volume);
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
        PlayerPrefs.SetFloat(KEY_VOLUME_KEYBOARD, Keyboard_Volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// ���ꂼ��̃{�����[���̒l��PlayerPrefs����ǂݍ���
    /// </summary>
    private void LoadPlayerPrefs()
    {
        BGM_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_BGM, DEFAULT_VOLUME);
        SE_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_SE, DEFAULT_VOLUME);
        Keyboard_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_KEYBOARD, DEFAULT_VOLUME);
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

    /// <summary>
    /// ���Ղ̃{�����[���̒l��Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForKeyboard()
    {
        return Keyboard_Volume;
    }


}
