using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MakeSprite
{
    public class CombineSprites : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image accessoryImage;
        [SerializeField] private Image wrinkleImage;
        [SerializeField] private Image earImage;
        [SerializeField] private Image mouthImage;
        [SerializeField] private Image noseImage;
        [SerializeField] private Image eyebrowImage;
        [SerializeField] private Image eyeImage;
        [SerializeField] private Image glassesImage;
        [SerializeField] private Image hairImage;

        /// <summary> ���ݕ\������Ă���Sprite�̃��X�g�ԍ� </summary>
        private int[] nowDisplaySpriteNumber;
        /// <summary> ���݂̃p�[�c��X(H)�ʒu </summary>
        private int[] nowPartsPositionX;
        /// <summary> ���݂̃p�[�c��Y(V)�ʒu </summary>
        private int[] nowPartsPositionY;

        /// <summary> ���X�g�ɕ\�������Image(Button)��Prefab </summary>
        [SerializeField] private GameObject selectImagePrefab;
        /// <summary> ���X�g�ɕ\������邯�ǌ����Ȃ��_�~�[��Prefab </summary>
        [SerializeField] private GameObject dummyImagePrefab;
        /// <summary> ��ʉE�̂������Ƃ�panel </summary>
        [SerializeField] private GameObject[] panels;

        /// <summary> �u�ۑ����܂����v�̃e�L�X�g </summary>
        [SerializeField] private TextMeshProUGUI saveInformationText;

        /// <summary> �����Ă���p�[�c���X�g���\�������GameObject�̐e </summary>
        private GameObject[] contentObjects;
        /// <summary> �������Ƃ�panel�̉��Ɉʒu����A�p�[�c�̈ʒu��ݒ肷��p�l�� </summary>
        private GameObject[] positionSettingPanels;
        /// <summary> �}�X�N�����摜�����̃��X�g </summary>
        private List<Image> images = new List<Image>();

        /// <summary> ���ݕ\������Ă���p�l���̃C���f�b�N�X </summary>
        public int Index { get; private set; }

        /// <summary> �{�^���������Ă��悢���̃t���O </summary>
        private bool buttonDisableFlg = false;

        public static CombineSprites Instance;
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
            InitArray();

            MakeMaskImage();

            InitContents();
            DisplaySelectImagePrefabs();

            DisplaySaveText(false);
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// �K�v�Ȕz��̏�����
        /// </summary>
        private void InitArray()
        {
            int len = panels.Length;

            nowDisplaySpriteNumber = new int[len];
            nowPartsPositionX = new int[len];
            nowPartsPositionY = new int[len];
            contentObjects = new GameObject[len];
            positionSettingPanels = new GameObject[len];
        }

        /// <summary>
        /// �}�X�N���ꂽ�摜�Ƀf�[�^�x�[�X����摜�����蓖�Ă�
        /// </summary>
        private void MakeMaskImage()
        {
            int len = PlayerInformationManager.Instance.settingPartsNumber.Length;

            for (int i = 0; i < len; i++)
            {
                switch (i)
                {
                    case 0: SetSprite(i, backgroundImage); break;
                    case 1: SetSprite(i, outlineImage); break;
                    case 2: SetSprite(i, accessoryImage); break;
                    case 3: SetSprite(i, wrinkleImage); break;
                    case 4: SetSprite(i, earImage); break;
                    case 5: SetSprite(i, mouthImage); break;
                    case 6: SetSprite(i, noseImage); break;
                    case 7: SetSprite(i, eyebrowImage); break;
                    case 8: SetSprite(i, eyeImage); break;
                    case 9: SetSprite(i, glassesImage); break;
                    case 10: SetSprite(i, hairImage); break;
                    default:
                        Debug.LogWarning($"CombineSprites.LoadDataBase : switch is default : i = {i}");
                        return;
                }
            }
        }

        /// <summary>
        /// �w�肵���摜�Ƀf�[�^�x�[�X�̉摜�����蓖�Ă�
        /// </summary>
        /// <param name="databaseNumber">�f�[�^�x�[�X���̑O�ɂ��Ă�ԍ�</param>
        /// <param name="image">�\����̉摜</param>
        private void SetSprite(int databaseNumber, Image image)
        {
            if (PlayerInformationManager.Instance.profileListEntries[databaseNumber]
                .itemList[PlayerInformationManager.Instance.settingPartsNumber[databaseNumber]].sprite != null)
            {
                // �摜�̈ʒu���擾
                nowPartsPositionX[databaseNumber] = PlayerInformationManager.Instance.settingPartsPositionH[databaseNumber];
                nowPartsPositionY[databaseNumber] = PlayerInformationManager.Instance.settingPartsPositionV[databaseNumber];

                // �\�����Ă���摜���Y������摜�ɕύX
                image.sprite = PlayerInformationManager.Instance.profileListEntries[databaseNumber]
                    .itemList[PlayerInformationManager.Instance.settingPartsNumber[databaseNumber]].sprite;

                // �\�����Ă���摜�̈ʒu��ύX
                RectTransform pos = image.GetComponent<RectTransform>();
                pos.localPosition = new Vector3(
                    pos.localPosition.x + nowPartsPositionX[databaseNumber],
                    pos.localPosition.y + nowPartsPositionY[databaseNumber],
                    pos.localPosition.z);

                // �\�����Ă���摜�̃��X�g�ԍ����擾
                nowDisplaySpriteNumber[databaseNumber] = PlayerInformationManager.Instance.settingPartsNumber[databaseNumber];

                // image�̃A�h���X���i�[����(���X�g�\��)
                images.Add(image);
            }
        }

        /// <summary>
        /// Image�{�^�������������Ƃ���M�����Ƃ�
        /// </summary>
        /// <param name="category">������Image�̃J�e�S��</param>
        /// <param name="listNumber">�������{�^���̃��X�g�ԍ�</param>
        public void ReceptionPushImageButton(SelectImageButton.Category category, int listNumber)
        {
            int categoryNumber = (int)category;
            switch (categoryNumber)
            {
                case 0: SetSelectSprite(categoryNumber, listNumber, backgroundImage); break;
                case 1: SetSelectSprite(categoryNumber, listNumber, outlineImage); break;
                case 2: SetSelectSprite(categoryNumber, listNumber, accessoryImage); break;
                case 3: SetSelectSprite(categoryNumber, listNumber, wrinkleImage); break;
                case 4: SetSelectSprite(categoryNumber, listNumber, earImage); break;
                case 5: SetSelectSprite(categoryNumber, listNumber, mouthImage); break;
                case 6: SetSelectSprite(categoryNumber, listNumber, noseImage); break;
                case 7: SetSelectSprite(categoryNumber, listNumber, eyebrowImage); break;
                case 8: SetSelectSprite(categoryNumber, listNumber, eyeImage); break;
                case 9: SetSelectSprite(categoryNumber, listNumber, glassesImage); break;
                case 10: SetSelectSprite(categoryNumber, listNumber, hairImage); break;
                default:
                    Debug.LogWarning($"CombineSprites.LoadDataBase : switch is default : i = {categoryNumber}");
                    return;
            }

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            DisplaySaveText(false);

            // �ݒ肷��p�[�c�����擾
            string str = PlayerInformationManager.Instance.profileListEntries[categoryNumber].itemList[listNumber].name;

            // �p�[�c���̃e�L�X�g���X�V
            UpdatePartsNameText(positionSettingPanels[categoryNumber].GetComponent<PositionSettingPanel>(), str);
        }

        /// <summary>
        /// ��M�����{�^���ɍ��킹�ĉ摜��ς���
        /// </summary>
        /// <param name="databaseNumber">�f�[�^�x�[�X���̑O�ɂ��Ă�ԍ�</param>
        /// <param name="image">�\����̉摜</param>
        /// <param name="listNumber"></param>
        private void SetSelectSprite(int databaseNumber, int listNumber, Image image)
        {
            if (PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[listNumber].sprite != null)
            {
                image.sprite = PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[listNumber].sprite;

                // �V�����\������Ă���摜�̃��X�g�ԍ����X�V
                nowDisplaySpriteNumber[databaseNumber] = listNumber;
            }
        }

        /// <summary>
        /// �{�^���������ƁA���̃p�l����\������(�e�X�g�p)
        /// </summary>
        public void TestPushChangePanelButton()
        {
            panels[Index].SetActive(false);

            Index++;
            if (Index >= panels.Length)
            {
                Index = 0;
            }

            panels[Index].SetActive(true);
        }

        /// <summary>
        /// �{�^���������ƁA�Y������p�l����\������
        /// </summary>
        /// <param name="index">�p�l���̔ԍ�</param>
        public void PushChangePanelButton(int index)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            if (Index == index) return;

            panels[Index].SetActive(false);

            Index = index;
            panels[Index].SetActive(true);
        }

        /// <summary>
        /// Content�̏ꏊ��T����contentObjects�ɐݒ肷��
        /// </summary>
        private void InitContents()
        {
            for (int i = 0; i < contentObjects.Length; i++)
            {
                // panelObject���̎q�v�f��T��
                Transform contentTransform = panels[i].transform.Find("ImageListPanel/Scroll View/Viewport/Content");
                if (contentTransform != null)
                {
                    contentObjects[i] = contentTransform.gameObject;
                }
                else
                {
                    Debug.LogError($"ChangePanel.InitContents : Not Find Content Number.{i}");
                }
            }
        }

        /// <summary>
        /// Content�Ɏ����Ă���p�[�c��Image��\������
        /// </summary>
        private void DisplaySelectImagePrefabs()
        {
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveBackground, 0);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveOutline, 1);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveAccessory, 2);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveWrinkle, 3);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveEar, 4);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveMouth, 5);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveNose, 6);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveEyebrow, 7);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveEye, 8);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveGlasses, 9);
            DisplaySelectImagePrefabs(ref PlayerInformationManager.Instance.haveProfileParts.haveHair, 10);
        }

        /// <summary>
        /// DisplaySelectImagePrefabs()�̒��g
        /// </summary>
        /// <param name="boolArray">ref PlayerInformationManager.Instance.haveProfileParts.�Z�Z</param>
        /// <param name="number">�p�[�c���̔ԍ�</param>
        private void DisplaySelectImagePrefabs(ref bool[] boolArray, int number)
        {
            int count = 0;

            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i] == true)
                {
                    if (selectImagePrefab != null && contentObjects[number] != null)
                    {
                        GameObject instantiatedPrefab = Instantiate(selectImagePrefab, contentObjects[number].transform);

                        // ��������Prefab�̃J�e�S�����w�肷��
                        instantiatedPrefab.GetComponent<SelectImageButton>().SetCategory((SelectImageButton.Category)number);

                        // ��������Prefab�̃��X�g�ԍ����w�肷��
                        instantiatedPrefab.GetComponent<SelectImageButton>().SetListNumber(i);

                        // ��������Prefab�ɑ΂��ĉ摜��ݒ肷��
                        instantiatedPrefab.GetComponent<Image>().sprite = PlayerInformationManager.Instance.profileListEntries[number].itemList[i].sprite;

                        // ���A���e�BText�̕������ύX
                        instantiatedPrefab.GetComponent<SelectImageButton>().rarityText.text
                            = PlayerInformationManager.Instance.profileListEntries[number].itemList[i].rarity.ToString();

                        count++;
                    }
                    else
                    {
                        Debug.LogWarning("CombineSprites.DisplaySelectImagePrefabs : Prefab is Nothing");
                    }
                }
                else
                {

                }
            }

            // �v�f���ЂƂ����Ȃ��Ƃ��AGridLayoutGroup���������\�������悤�A�_�~�[�̈�𐶐�����
            while (count <= 1)
            {
                Instantiate(dummyImagePrefab, contentObjects[number].transform);
                count++;
            }
        }

        /// <summary>
        /// �󂯎�����l����A�p�[�c�̃|�W�V������ς���
        /// </summary>
        /// <param name="databaseNumber">�f�[�^�x�[�X���̑O�ɏ����Ă���A�f�[�^�x�[�X�̎�ޔԍ�</param>
        /// <param name="posX">X(H)�̒l</param>
        /// <param name="posY">Y(V)�̒l</param>
        public void MovePartsPosition(int databaseNumber, int posX, int posY)
        {
            // �ʒu���X�V
            images[databaseNumber].GetComponent<Transform>().localPosition = new Vector3(posX, posY, 0);

            // �V�����ʒu�̏����X�V
            nowPartsPositionX[databaseNumber] = posX;
            nowPartsPositionY[databaseNumber] = posY;

            DisplaySaveText(false);
        }

        /// <summary>
        /// PositionSettingPanel�����т���
        /// </summary>
        /// <param name="panel">�Y������PositionSettingPanel</param>
        public void InitPositionSettingPanels(PositionSettingPanel panel)
        {
            // �f�[�^�x�[�X�̎�ޔԍ����擾
            int databaseNumber = panel.myPanelNumber;

            // �p�l���̃A�h���X�����ѕt��
            positionSettingPanels[databaseNumber] = panel.gameObject;

            // �����Őݒ肵�Ă���p�[�c�����擾
            string str = PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[nowDisplaySpriteNumber[databaseNumber]].name;
            
            // �p�[�c���̃e�L�X�g���X�V
            UpdatePartsNameText(panel,str);
        }

        /// <summary>
        /// �p�[�c���̕\�����X�V����
        /// </summary>
        /// <param name="panel">�Y������PositionSettingPanel</param>
        /// <param name="nameString">�p�[�c���̕�����</param>
        private void UpdatePartsNameText(PositionSettingPanel panel , string nameString)
        {
            panel.partsNameText.text = nameString;
        }

        /// <summary>
        /// Save�̃{�^�����������Ƃ�
        /// </summary>
        public void PushSaveButton()
        {
            if (buttonDisableFlg) return;

            buttonDisableFlg = true;

            PlayerInformationManager.Instance.SaveSetProfile(nowDisplaySpriteNumber, nowPartsPositionX, nowPartsPositionY);

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.OK);

            DisplaySaveText(true);

            // Debug.Log($"CombineSprites.PushSaveButton : Save is Finish");

            buttonDisableFlg = false;
        }

        /// <summary>
        /// Cancel�{�^�����������Ƃ�
        /// </summary>
        public void PushCancelButton()
        {
            if (buttonDisableFlg) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

            buttonDisableFlg = true;

            FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 0.3f);
        }

        /// <summary>
        /// �u�ۑ����܂����v�̃e�L�X�g�̕\����\�������߂�
        /// </summary>
        /// <param name="flg"></param>
        private void DisplaySaveText(bool flg)
        {
            saveInformationText.gameObject.SetActive(flg);
        }

    }
}