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
                    str = "�R����";
                    break;
                case RARITY_UC:
                    str = "�A���R����";
                    break;
                case RARITY_R:
                    str = "���A!";
                    break;
                case RARITY_SR:
                    str = "�X�[�p�[���A!!";
                    break;
                case RARITY_HR:
                    str = "�n�C�p�[���A!!!";
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
                    str = "�w�i";
                    break;
                case 1:
                    str = "�֊s";
                    break;
                case 2:
                    str = "�A�N�Z�T���[";
                    break;
                case 3:
                    str = "����";
                    break;
                case 4:
                    str = "��";
                    break;
                case 5:
                    str = "��";
                    break;
                case 6:
                    str = "�@";
                    break;
                case 7:
                    str = "����";
                    break;
                case 8:
                    str = "��";
                    break;
                case 9:
                    str = "���K�l";
                    break;
                case 10:
                    str = "��";
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
                str = "�����ς�";
            }
            else
            {
                str = "���Q�b�g!!";
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
