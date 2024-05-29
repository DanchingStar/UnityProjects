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
    /// 主にメニュー画面で使う鍵盤のSEの初期化
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
    /// SEを再生する
    /// </summary>
    /// <param name="seType"></param>
    public void PlaySE(SoundSeType seType)
    {
        seSource[(int)seType].Play();

        //Debug.Log($"SoundManager.PlaySE : SE Type -> {seType}");
    }

    /// <summary>
    /// 主にメニューシーンで鍵盤を叩いた時にならすSEを再生する
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
    /// 正解時の音楽をセットする
    /// </summary>
    /// <param name="_SuccessMusicClip"></param>
    public void SetSuccessMusic(AudioClip _SuccessMusicClip)
    {
        successMusicAudioSource.clip = _SuccessMusicClip;
    }

    /// <summary>
    /// 正解時の音楽を再生する
    /// </summary>
    public void PlaySuccessMusic()
    {
        if (successMusicAudioSource.clip != null)
        {
            successMusicAudioSource.Play();
        }
    }

    /// <summary>
    /// ステージの音楽をセットする
    /// </summary>
    /// <param name="_SuccessMusicClip"></param>
    public void SetStageMusic(AudioClip _StageMusicClip)
    {
        stageMusicAudioSource.clip = _StageMusicClip;
    }

    /// <summary>
    /// ステージの音楽を再生する
    /// </summary>
    public void PlayStageMusic()
    {
        if (stageMusicAudioSource.clip != null)
        {
            stageMusicAudioSource.Play();
        }
    }

    /// <summary>
    /// ステージの音楽を停止する
    /// </summary>
    public void StopStageMusic()
    {
        if (stageMusicAudioSource.clip != null)
        {
            stageMusicAudioSource.Stop();
        }
    }

    /// <summary>
    /// 全てのボリュームを変更する
    /// </summary>
    private void VolumeChangeAll()
    {
        ChangeVolumeValue(myBgmAudioTF, BGM_Volume);
        ChangeVolumeValue(mySeAudioTF, SE_Volume);
        ChangeVolumeValue(myKeyboardAudioTF, Keyboard_Volume);
    }

    /// <summary>
    /// BGMのボリュームを変更する
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForBGM(float value)
    {
        BGM_Volume = value;
        ChangeVolumeValue(myBgmAudioTF, BGM_Volume);
    }

    /// <summary>
    /// SEのボリュームを変更する
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForSE(float value)
    {
        SE_Volume = value;
        ChangeVolumeValue(mySeAudioTF, SE_Volume);
    }

    /// <summary>
    /// 鍵盤のボリュームを変更する
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVolumeForKeyboard(float value)
    {
        Keyboard_Volume = value;
        ChangeVolumeValue(myKeyboardAudioTF, Keyboard_Volume);
    }

    /// <summary>
    /// 指定した音のボリュームの値を実際に変更する
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
    /// それぞれのボリュームの値をPlayerPrefsに保存する
    /// </summary>
    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat(KEY_VOLUME_BGM, BGM_Volume);
        PlayerPrefs.SetFloat(KEY_VOLUME_SE, SE_Volume);
        PlayerPrefs.SetFloat(KEY_VOLUME_KEYBOARD, Keyboard_Volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// それぞれのボリュームの値をPlayerPrefsから読み込む
    /// </summary>
    private void LoadPlayerPrefs()
    {
        BGM_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_BGM, DEFAULT_VOLUME);
        SE_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_SE, DEFAULT_VOLUME);
        Keyboard_Volume = PlayerPrefs.GetFloat(KEY_VOLUME_KEYBOARD, DEFAULT_VOLUME);
    }

    /// <summary>
    /// BGMのボリュームの値を返すゲッター
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForBGM()
    {
        return BGM_Volume;
    }

    /// <summary>
    /// SEのボリュームの値を返すゲッター
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForSE()
    {
        return SE_Volume;
    }

    /// <summary>
    /// 鍵盤のボリュームの値を返すゲッター
    /// </summary>
    /// <returns></returns>
    public float GetVolumeForKeyboard()
    {
        return Keyboard_Volume;
    }


}
