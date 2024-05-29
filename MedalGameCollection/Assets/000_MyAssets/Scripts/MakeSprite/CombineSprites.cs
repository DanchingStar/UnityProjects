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

        /// <summary> 現在表示されているSpriteのリスト番号 </summary>
        private int[] nowDisplaySpriteNumber;
        /// <summary> 現在のパーツのX(H)位置 </summary>
        private int[] nowPartsPositionX;
        /// <summary> 現在のパーツのY(V)位置 </summary>
        private int[] nowPartsPositionY;

        /// <summary> リストに表示されるImage(Button)のPrefab </summary>
        [SerializeField] private GameObject selectImagePrefab;
        /// <summary> リストに表示されるけど見えないダミーのPrefab </summary>
        [SerializeField] private GameObject dummyImagePrefab;
        /// <summary> 画面右のおおもとのpanel </summary>
        [SerializeField] private GameObject[] panels;

        /// <summary> 「保存しました」のテキスト </summary>
        [SerializeField] private TextMeshProUGUI saveInformationText;

        /// <summary> 持っているパーツリストが表示されるGameObjectの親 </summary>
        private GameObject[] contentObjects;
        /// <summary> おおもとのpanelの下に位置する、パーツの位置を設定するパネル </summary>
        private GameObject[] positionSettingPanels;
        /// <summary> マスクされる画像たちのリスト </summary>
        private List<Image> images = new List<Image>();

        /// <summary> 現在表示されているパネルのインデックス </summary>
        public int Index { get; private set; }

        /// <summary> ボタンを押してもよいかのフラグ </summary>
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
        /// 必要な配列の初期化
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
        /// マスクされた画像にデータベースから画像を割り当てる
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
        /// 指定した画像にデータベースの画像を割り当てる
        /// </summary>
        /// <param name="databaseNumber">データベース名の前についてる番号</param>
        /// <param name="image">表示先の画像</param>
        private void SetSprite(int databaseNumber, Image image)
        {
            if (PlayerInformationManager.Instance.profileListEntries[databaseNumber]
                .itemList[PlayerInformationManager.Instance.settingPartsNumber[databaseNumber]].sprite != null)
            {
                // 画像の位置を取得
                nowPartsPositionX[databaseNumber] = PlayerInformationManager.Instance.settingPartsPositionH[databaseNumber];
                nowPartsPositionY[databaseNumber] = PlayerInformationManager.Instance.settingPartsPositionV[databaseNumber];

                // 表示している画像を該当する画像に変更
                image.sprite = PlayerInformationManager.Instance.profileListEntries[databaseNumber]
                    .itemList[PlayerInformationManager.Instance.settingPartsNumber[databaseNumber]].sprite;

                // 表示している画像の位置を変更
                RectTransform pos = image.GetComponent<RectTransform>();
                pos.localPosition = new Vector3(
                    pos.localPosition.x + nowPartsPositionX[databaseNumber],
                    pos.localPosition.y + nowPartsPositionY[databaseNumber],
                    pos.localPosition.z);

                // 表示している画像のリスト番号を取得
                nowDisplaySpriteNumber[databaseNumber] = PlayerInformationManager.Instance.settingPartsNumber[databaseNumber];

                // imageのアドレスを格納する(リスト構造)
                images.Add(image);
            }
        }

        /// <summary>
        /// Imageボタンを押したことを受信したとき
        /// </summary>
        /// <param name="category">押したImageのカテゴリ</param>
        /// <param name="listNumber">押したボタンのリスト番号</param>
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

            // 設定するパーツ名を取得
            string str = PlayerInformationManager.Instance.profileListEntries[categoryNumber].itemList[listNumber].name;

            // パーツ名のテキストを更新
            UpdatePartsNameText(positionSettingPanels[categoryNumber].GetComponent<PositionSettingPanel>(), str);
        }

        /// <summary>
        /// 受信したボタンに合わせて画像を変える
        /// </summary>
        /// <param name="databaseNumber">データベース名の前についてる番号</param>
        /// <param name="image">表示先の画像</param>
        /// <param name="listNumber"></param>
        private void SetSelectSprite(int databaseNumber, int listNumber, Image image)
        {
            if (PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[listNumber].sprite != null)
            {
                image.sprite = PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[listNumber].sprite;

                // 新しく表示されている画像のリスト番号を更新
                nowDisplaySpriteNumber[databaseNumber] = listNumber;
            }
        }

        /// <summary>
        /// ボタンを押すと、次のパネルを表示する(テスト用)
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
        /// ボタンを押すと、該当するパネルを表示する
        /// </summary>
        /// <param name="index">パネルの番号</param>
        public void PushChangePanelButton(int index)
        {
            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.Yes);

            if (Index == index) return;

            panels[Index].SetActive(false);

            Index = index;
            panels[Index].SetActive(true);
        }

        /// <summary>
        /// Contentの場所を探してcontentObjectsに設定する
        /// </summary>
        private void InitContents()
        {
            for (int i = 0; i < contentObjects.Length; i++)
            {
                // panelObject内の子要素を探す
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
        /// Contentに持っているパーツのImageを表示する
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
        /// DisplaySelectImagePrefabs()の中身
        /// </summary>
        /// <param name="boolArray">ref PlayerInformationManager.Instance.haveProfileParts.〇〇</param>
        /// <param name="number">パーツ名の番号</param>
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

                        // 生成したPrefabのカテゴリを指定する
                        instantiatedPrefab.GetComponent<SelectImageButton>().SetCategory((SelectImageButton.Category)number);

                        // 生成したPrefabのリスト番号を指定する
                        instantiatedPrefab.GetComponent<SelectImageButton>().SetListNumber(i);

                        // 生成したPrefabに対して画像を設定する
                        instantiatedPrefab.GetComponent<Image>().sprite = PlayerInformationManager.Instance.profileListEntries[number].itemList[i].sprite;

                        // レアリティTextの文字列を変更
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

            // 要素がひとつしかないとき、GridLayoutGroupが正しく表示されるよう、ダミーの一つを生成する
            while (count <= 1)
            {
                Instantiate(dummyImagePrefab, contentObjects[number].transform);
                count++;
            }
        }

        /// <summary>
        /// 受け取った値から、パーツのポジションを変える
        /// </summary>
        /// <param name="databaseNumber">データベース名の前に書いてある、データベースの種類番号</param>
        /// <param name="posX">X(H)の値</param>
        /// <param name="posY">Y(V)の値</param>
        public void MovePartsPosition(int databaseNumber, int posX, int posY)
        {
            // 位置を更新
            images[databaseNumber].GetComponent<Transform>().localPosition = new Vector3(posX, posY, 0);

            // 新しい位置の情報を更新
            nowPartsPositionX[databaseNumber] = posX;
            nowPartsPositionY[databaseNumber] = posY;

            DisplaySaveText(false);
        }

        /// <summary>
        /// PositionSettingPanelを結びつける
        /// </summary>
        /// <param name="panel">該当するPositionSettingPanel</param>
        public void InitPositionSettingPanels(PositionSettingPanel panel)
        {
            // データベースの種類番号を取得
            int databaseNumber = panel.myPanelNumber;

            // パネルのアドレスを結び付け
            positionSettingPanels[databaseNumber] = panel.gameObject;

            // 初期で設定しているパーツ名を取得
            string str = PlayerInformationManager.Instance.profileListEntries[databaseNumber].itemList[nowDisplaySpriteNumber[databaseNumber]].name;
            
            // パーツ名のテキストを更新
            UpdatePartsNameText(panel,str);
        }

        /// <summary>
        /// パーツ名の表示を更新する
        /// </summary>
        /// <param name="panel">該当するPositionSettingPanel</param>
        /// <param name="nameString">パーツ名の文字列</param>
        private void UpdatePartsNameText(PositionSettingPanel panel , string nameString)
        {
            panel.partsNameText.text = nameString;
        }

        /// <summary>
        /// Saveのボタンを押したとき
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
        /// Cancelボタンを押したとき
        /// </summary>
        public void PushCancelButton()
        {
            if (buttonDisableFlg) return;

            SoundManager.Instance.PlaySE(SoundManager.SoundSeType.No);

            buttonDisableFlg = true;

            FadeManager.Instance.LoadScene(SceneManager.GetActiveScene().name, 0.3f);
        }

        /// <summary>
        /// 「保存しました」のテキストの表示非表示を決める
        /// </summary>
        /// <param name="flg"></param>
        private void DisplaySaveText(bool flg)
        {
            saveInformationText.gameObject.SetActive(flg);
        }

    }
}