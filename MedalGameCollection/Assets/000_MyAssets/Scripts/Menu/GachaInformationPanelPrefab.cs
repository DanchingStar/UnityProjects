using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class GachaInformationPanelPrefab : MonoBehaviour
    {
        [SerializeField] private Image myImage;
        [SerializeField] private TextMeshProUGUI myName;
        [SerializeField] private TextMeshProUGUI myRarity;
        [SerializeField] private TextMeshProUGUI myKinds;
        [SerializeField] private TextMeshProUGUI myIsHave;

        private const string RARITY_C = "Common";
        private const string RARITY_UC = "Uncommon";
        private const string RARITY_R = "Rare";
        private const string RARITY_SR = "SuperRare";
        private const string RARITY_HR = "HyperRare";

        public void SetMyPanel(GachaGenerator.PartsStatus partsStatus)
        {
            myImage.sprite = partsStatus.sprite;
            myName.text = partsStatus.name;
            myRarity.text = MakeKindsString(partsStatus.rarity);
            myKinds.text = MakeKindsString(partsStatus.partskinds);
            myIsHave.text = MakeIsHaveText(partsStatus.isHave);
        }

        private string MakeKindsString(string rarityString)
        {
            string str;

            switch (rarityString)
            {
                case RARITY_C:
                    str = "コモン";
                    break;
                case RARITY_UC:
                    str = "アンコモン";
                    break;
                case RARITY_R:
                    str = "レア!";
                    break;
                case RARITY_SR:
                    str = "スーパーレア!!";
                    break;
                case RARITY_HR:
                    str = "ハイパーレア!!!";
                    break;
                default:
                    str = "";
                    break;
            }

            return str;
        }

        private string MakeKindsString(int num)
        {
            string str;

            switch (num)
            {
                case 0:
                    str = "背景";
                    break;
                case 1:
                    str = "輪郭";
                    break;
                case 2:
                    str = "アクセサリー";
                    break;
                case 3:
                    str = "しわ";
                    break;
                case 4:
                    str = "耳";
                    break;
                case 5:
                    str = "口";
                    break;
                case 6:
                    str = "鼻";
                    break;
                case 7:
                    str = "眉毛";
                    break;
                case 8:
                    str = "目";
                    break;
                case 9:
                    str = "メガネ";
                    break;
                case 10:
                    str = "頭";
                    break;
                default:
                    str = "";
                    break;
            }

            return str;
        }

        private string MakeIsHaveText(bool flg)
        {
            string str;

            if (flg)
            {
                str = "所持済み";
            }
            else
            {
                str = "初ゲット!!";
            }

            return str;
        }

        public void PushCloseButton()
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No1);
            Destroy(this.gameObject);
        }
    }
}
