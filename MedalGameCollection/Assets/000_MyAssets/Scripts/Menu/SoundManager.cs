using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] seSource;

        [SerializeField] private GameObject BGMObject;
        [SerializeField] private GameObject SEObjectParent;

        [SerializeField] private Slider myBGMSlider;
        [SerializeField] private Slider mySESlider;

        private const string BGM_VOLUME_KEY = "BGMVolume";
        private const string SE_VOLUME_KEY = "SEVolume";

        private float BGM_Volume;
        private float SE_Volume;

        private AudioSource myAudioBGM;
        private Transform myAudioTF;

        [Serializable]
        public enum SoundSeType
        {
            Yes1,
            Yes2,
            No1,
            No2,
            Move,
            InformationCutIn,
        }

        public static SoundManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            myAudioBGM = BGMObject.GetComponent<AudioSource>();
            myAudioTF = SEObjectParent.GetComponent<Transform>();

            InitSlider();
            InitVolume();
        }

        /// <summary>
        /// ���ʂ̃X���C�_�[�̏����ݒ�
        /// </summary>
        private void InitSlider()
        {
            myBGMSlider.value = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
            mySESlider.value = PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.5f);

            BGM_Volume = myBGMSlider.value;
            myBGMSlider.onValueChanged.AddListener(value => VolumeChangeBGM(value));

            SE_Volume = mySESlider.value;
            mySESlider.onValueChanged.AddListener(value => VolumeChangeSE(value));
        }

        /// <summary>
        /// ���ʐݒ��ۑ�����
        /// </summary>
        public void SaveSliderPrefs()
        {
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, myBGMSlider.value);
            PlayerPrefs.SetFloat(SE_VOLUME_KEY, mySESlider.value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// SE���Đ�����
        /// </summary>
        /// <param name="seType"></param>
        public void PlaySE(SoundSeType seType)
        {
            seSource[(int)seType].Play();
        }

        /// <summary>
        /// ������BGM��SE�̃{�����[����ݒ肷��
        /// </summary>
        private void InitVolume()
        {
            VolumeChangeBGM(BGM_Volume);
            VolumeChangeSE(SE_Volume);
        }

        /// <summary>
        /// BGM�̃{�����[����ς���
        /// </summary>
        /// <param name="value">�l</param>
        private void VolumeChangeBGM(float value)
        {
            BGM_Volume = value;
            myAudioBGM.volume = BGM_Volume;
        }

        /// <summary>
        /// SE�̃{�����[����ς���
        /// </summary>
        /// <param name="value">�l</param>
        private void VolumeChangeSE(float value)
        {
            SE_Volume = value;
            foreach (Transform childTF in myAudioTF)
            {
                childTF.gameObject.GetComponent<AudioSource>().volume = SE_Volume;
            }
        }

    }
}
