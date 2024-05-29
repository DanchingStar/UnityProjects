using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ShopContentsPanelPrefab : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI minNumText;
        [SerializeField] TextMeshProUGUI maxNumText;
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI selectNumText;
        [SerializeField] Image lostItemImage;
        [SerializeField] Image getItemImage;
        [SerializeField] TextMeshProUGUI lostItemText;
        [SerializeField] TextMeshProUGUI getItemText;
        [SerializeField] TextMeshProUGUI confirmText;
        [SerializeField] Button yesButton;

        [SerializeField] private Sprite medalSprite;
        [SerializeField] private Sprite spCoinSprite;
        [SerializeField] private Sprite gachaTicketSprite;

        private ShopContents myShopContents;

        private int medalNum;
        private int spCoinNum;
        private int gachaTicketNum;

        //private int sliderMaxValue;

        private int exchangeNum;

        private const int RATE_MEDAL_TO_GACHA_TICKET = 100;
        private const int RATE_SP_COIN_TO_MEDAL = 25;

        private bool waitingPlayFabFlg;

        public enum ShopContents
        {
            MedalToGachaTicket,
            SPCoinToMedal,
            SPCoinToGachaTicket,
        }

        /// <summary>
        /// �����̏����Z�b�g����
        /// </summary>
        /// <param name="shopContents"></param>
        public void SetMyStatus(ShopContents shopContents)
        {
            myShopContents = shopContents;

            waitingPlayFabFlg = false;
            medalNum = PlayerInformationManager.Instance.GetHaveMedal();
            spCoinNum = PlayerInformationManager.Instance.GetSPCoin();
            gachaTicketNum = PlayFabManager.Instance.GetHaveGachaTicket();

            InitSliderAndButton();

            if (shopContents == ShopContents.MedalToGachaTicket)
            {
                lostItemImage.sprite = medalSprite;
                getItemImage.sprite = gachaTicketSprite;
            }
            else if(shopContents == ShopContents.SPCoinToMedal)
            {
                lostItemImage.sprite = spCoinSprite;
                getItemImage.sprite = medalSprite;
            }
            else if(shopContents == ShopContents.SPCoinToGachaTicket)
            {
                lostItemImage.sprite = spCoinSprite;
                getItemImage.sprite = gachaTicketSprite;
            }
            else
            {

            }
        }

        /// <summary>
        /// �X���C�_�[����̏����ݒ�
        /// </summary>
        /// <param name="shopContents"></param>
        private void InitSliderAndButton()
        {
            slider.minValue = 1;

            if (myShopContents == ShopContents.MedalToGachaTicket)
            {
                slider.maxValue = (int)(medalNum / RATE_MEDAL_TO_GACHA_TICKET);
            }
            else if (myShopContents == ShopContents.SPCoinToMedal || myShopContents == ShopContents.SPCoinToGachaTicket)
            {
                slider.maxValue = spCoinNum;
            }
            else
            {
                MenuSceneManager.Instance.SendInformationText("�H�H�H");
                slider.maxValue = 0;
            }

            slider.value = slider.minValue;

            minNumText.text = slider.minValue.ToString();
            maxNumText.text = slider.maxValue.ToString();

            UpdateSliderSelectValue();

            if (slider.maxValue < slider.minValue)
            {
                yesButton.interactable = false;
                slider.interactable = false;
                confirmText.text = "�����Ă��閇����\n����Ȃ��̂�\n�����ł��܂���B";
            }
            else
            {
                confirmText.text = "������̎�������ԕi��\n�ł��܂���B\n�������m�肵�܂����H";
            }
        }

        /// <summary>
        /// �X���C�_�[�̒l���ύX���ꂽ�Ƃ�
        /// </summary>
        public void UpdateSliderSelectValue()
        {
            exchangeNum = (int)slider.value;
            selectNumText.text = slider.value.ToString();

            int beforeLostItem = 0;
            int beforeGetItem = 0;
            int afterLostItem = 0;
            int afterGetItem = 0;

            switch (myShopContents)
            {
                case ShopContents.MedalToGachaTicket:
                    beforeLostItem = medalNum;
                    beforeGetItem = gachaTicketNum;
                    afterLostItem = medalNum - (exchangeNum * RATE_MEDAL_TO_GACHA_TICKET);
                    afterGetItem = gachaTicketNum + exchangeNum;
                    break;
                case ShopContents.SPCoinToMedal:
                    beforeLostItem = spCoinNum;
                    beforeGetItem = medalNum;
                    afterLostItem = spCoinNum - exchangeNum;
                    afterGetItem = medalNum + (exchangeNum * RATE_SP_COIN_TO_MEDAL);
                    break;
                case ShopContents.SPCoinToGachaTicket:
                    beforeLostItem = spCoinNum;
                    beforeGetItem = gachaTicketNum;
                    afterLostItem = spCoinNum - exchangeNum;
                    afterGetItem = gachaTicketNum + exchangeNum;
                    break;
                default:
                    break;
            }

            lostItemText.text = $"{beforeLostItem} �� {afterLostItem}";
            getItemText.text = $"{beforeGetItem} �� {afterGetItem}";








        }

        /// <summary>
        /// ��������{�^�����������Ƃ�
        /// </summary>
        public void PushYesButton()
        {
            if (waitingPlayFabFlg) return;

            slider.interactable = false;
            waitingPlayFabFlg = true;

            switch (myShopContents)
            {
                case ShopContents.MedalToGachaTicket:
                    PlayFabManager.Instance.AddGachaTicket(exchangeNum, this.gameObject);
                    break;
                case ShopContents.SPCoinToMedal:
                    ReceptionGachaTicketExchange(true);
                    break;
                case ShopContents.SPCoinToGachaTicket:
                    PlayFabManager.Instance.AddGachaTicket(exchangeNum, this.gameObject);
                    break;
                default:
                    ReceptionGachaTicketExchange(false);
                    break;
            }
        }

        /// <summary>
        /// �L�����Z���{�^�����������Ƃ�
        /// </summary>
        public void PushNoButton()
        {
            if (waitingPlayFabFlg) return;

            Destroy(this.gameObject);        
        }

        /// <summary>
        /// �K�`���`�P�Ƃ̌���������������PlayFabManager�����M����
        /// </summary>
        /// <param name="flg"></param>
        public void ReceptionGachaTicketExchange(bool flg)
        {
            waitingPlayFabFlg = false;

            if (flg)
            {
                switch (myShopContents)
                {
                    case ShopContents.MedalToGachaTicket:
                        PlayerInformationManager.Instance.ConsumptionMedal(exchangeNum * RATE_MEDAL_TO_GACHA_TICKET);
                        break;
                    case ShopContents.SPCoinToMedal:
                        PlayerInformationManager.Instance.ConsumptionSPCoin(exchangeNum);
                        PlayerInformationManager.Instance.AcquisitionMedal(exchangeNum * RATE_SP_COIN_TO_MEDAL);
                        break;
                    case ShopContents.SPCoinToGachaTicket:
                        PlayerInformationManager.Instance.ConsumptionSPCoin(exchangeNum);
                        break;
                    default:
                        ReceptionGachaTicketExchange(false);
                        return;
                }

                MenuSceneManager.Instance.SendInformationText("�����A�����ł��܂����B");
                PushNoButton();
            }
            else
            {
                MenuSceneManager.Instance.SendInformationText("�����Ɏ��s���܂����B");
                PushNoButton();
            }
        }
    }
}
