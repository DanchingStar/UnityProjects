using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SmartBall01
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] seSource;
        //[SerializeField] private AudioClip[] bgmClips;

        private AudioSource myAudioSource;

        private float BGM_Volume;
        private float SE_Volume;

        [SerializeField] private GameObject BGMObject;
        [SerializeField] private GameObject SEObjectParent;

        private AudioSource myAudioBGM;
        private Transform myAudioTF;

        [Serializable]
        public enum SoundSeType
        {
            GameStart,
            SpringNow,
            SpringL,
            SpringM,
            SpringS,
            BallIn,
            BallCollide,
            BallOut,
            Bingo,
            GameFinish,
            GameReset,
            MedalPayout,
        }

        //[Serializable]
        //public enum SoundBgmType
        //{
        //}

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
            myAudioSource = GetComponent<AudioSource>();

            myAudioBGM = BGMObject.GetComponent<AudioSource>();
            myAudioTF = SEObjectParent.GetComponent<Transform>();

            BGM_Volume = -1;
            SE_Volume = -1;

            VolumeChange();
        }

        private void Update()
        {
            if (UICommonPanelManager.Instance.GetIsActiveSoundSettingPanel())
            {
                VolumeChange();
            }
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="seType"></param>
        public void PlaySE(SoundSeType seType)
        {
            seSource[(int)seType].Play();
        }

        /// <summary>
        /// SEを１つだけ再生する。再生中の場合、それは止める。
        /// </summary>
        /// <param name="seType"></param>
        public void PlaySEOnlyOne(SoundSeType seType)
        {
            if (myAudioSource.isPlaying)
            {
                myAudioSource.Stop();
            }
            myAudioSource = seSource[(int)seType];
            myAudioSource.Play();
        }

        ///// <summary>
        ///// BGMを変えて再生する
        ///// </summary>
        ///// <param name="bgmType"></param>
        //public void ChangeAndPlayBgm(SoundBgmType bgmType)
        //{
        //    if (myAudioBGM.isPlaying)
        //    {
        //        myAudioBGM.Stop();
        //    }
        //    myAudioBGM.clip = bgmClips[(int)bgmType];
        //    myAudioBGM.Play();
        //}

        /// <summary>
        /// BGMとSEのボリュームを変更する
        /// </summary>
        private void VolumeChange()
        {
            if (UICommonPanelManager.Instance.GetBGMVolume() != BGM_Volume)
            {
                BGM_Volume = UICommonPanelManager.Instance.GetBGMVolume();
                myAudioBGM.volume = BGM_Volume;
            }
            if (UICommonPanelManager.Instance.GetSEVolume() != SE_Volume)
            {
                SE_Volume = UICommonPanelManager.Instance.GetSEVolume();
                foreach (Transform childTF in myAudioTF)
                {
                    childTF.gameObject.GetComponent<AudioSource>().volume = SE_Volume;
                }
            }
        }



    }
}
