using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class GachaResultPrefab : MonoBehaviour
    {
        [SerializeField] Image myImage;
        [SerializeField] TextMeshProUGUI myNewText;
        [SerializeField] TextMeshProUGUI myRarityText;

        [SerializeField] private GameObject gachaInformationPanelPrefab;

        private GachaGenerator.PartsStatus myStatus;

        /// <summary>
        /// 自分の情報をセットする
        /// </summary>
        /// <param name="partsStatus"></param>
        public void SetMyStatus(GachaGenerator.PartsStatus partsStatus)
        {
            myStatus = partsStatus;
            myImage.sprite = partsStatus.sprite;
            myRarityText.text = partsStatus.rarity;
            myNewText.gameObject.SetActive(!partsStatus.isHave);
        }

        /// <summary>
        /// 自分のボタンを押したとき
        /// </summary>
        public void PushMyImageButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes1);

            GameObject canvas = GameObject.FindGameObjectWithTag("Point");

            GameObject obj = Instantiate(gachaInformationPanelPrefab, canvas.transform);

            //obj.transform.localPosition = Vector3.zero;

            obj.GetComponent<GachaInformationPanelPrefab>().SetMyPanel(myStatus);
        }
    }
}
