using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JanKen
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource[] clips;

        [SerializeField] private AudioSource myAudioSource;

        private float BGM_Volume;
        private float SE_Volume;

        [SerializeField] private GameObject BGMObject;
        [SerializeField] private GameObject SEObjectParent;

        private AudioSource myAudioBGM;
        private Transform myAudioTF;

        [Serializable]
        public enum SoundType
        {
            BattleStart,
            BattleReStart,
            Decide,
            Win,
            Lose,
            MedalPayout,
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

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="seType"></param>
        public void PlaySE(SoundType seType)
        {
            clips[(int)seType].Play();
        }

        /// <summary>
        /// SEを１つだけ再生する。再生中の場合、それは止める。
        /// </summary>
        /// <param name="soundType"></param>
        public void PlaySEOnlyOne(SoundType soundType)
        {
            if (myAudioSource.isPlaying)
            {
                myAudioSource.Stop();
            }
            myAudioSource = clips[(int)soundType];
            myAudioSource.Play();
        }

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
