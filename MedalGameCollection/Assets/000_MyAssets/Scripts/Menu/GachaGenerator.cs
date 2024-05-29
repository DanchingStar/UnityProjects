using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menu
{
    public class GachaGenerator : MonoBehaviour
    {
        [SerializeField] private GachaResultPanel gachaResultPanel;

        public List<PartsStatus> listCommon;
        public List<PartsStatus> listUncommon;
        public List<PartsStatus> listRare;
        public List<PartsStatus> listSuperRare;
        public List<PartsStatus> listHyperRare;

        private const string RARITY_C = "Common";
        private const string RARITY_UC = "Uncommon";
        private const string RARITY_R = "Rare";
        private const string RARITY_SR = "SuperRare";
        private const string RARITY_HR = "HyperRare";

        private const string RESULT_C = "RarityC";
        private const string RESULT_UC = "RarityUC";
        private const string RESULT_R = "RarityR";
        private const string RESULT_SR = "RaritySR";
        private const string RESULT_HR = "RarityHR";

        //private string[] partsKindsNames; 

        public struct PartsStatus
        {
            public int partskinds;
            public int partsNumber;
            public bool isHave;

            public string name;
            public string rarity;
            public Sprite sprite;
        }

        void Start()
        {
            //partsKindsNames = new string[]{
            //    "00_Background", "01_Outline" , "02_Accessory","03_Wrinkle" , "04_Ear",
            //    "05_Mouth", "06_Nose", "07_Eyebrow" , "08_Eye" , "09_Glasses", "10_Hair"};

            InitArray();
            MakeGachaPoolAll();

        }

        /// <summary>
        /// 配列の初期化
        /// </summary>
        private void InitArray()
        {
            listCommon = new List<PartsStatus>();
            listUncommon = new List<PartsStatus>();
            listRare = new List<PartsStatus>();
            listSuperRare = new List<PartsStatus>();
            listHyperRare = new List<PartsStatus>();
        }

        /// <summary>
        /// 全てのレアリティのプールを作る
        /// </summary>
        private void MakeGachaPoolAll()
        {
            MakeGachaPool(RARITY_C, listCommon);
            MakeGachaPool(RARITY_UC, listUncommon);
            MakeGachaPool(RARITY_R, listRare);
            MakeGachaPool(RARITY_SR, listSuperRare);
            MakeGachaPool(RARITY_HR, listHyperRare);
        }

        /// <summary>
        /// 指定したレアリティのプールを作る
        /// </summary>
        /// <param name="rarity"></param>
        /// <param name="list"></param>
        private void MakeGachaPool(string rarity, List<PartsStatus> list)
        {
            for (int i = 0; i < PlayerInformationManager.NUMBER_OF_PLOFILELIST; i++) 
            {
                for (int j = 0; j < PlayerInformationManager.Instance.profileListEntries[i].itemList.Count; j++) 
                {
                    if (rarity == PlayerInformationManager.Instance.profileListEntries[i].itemList[j].rarity.ToString())
                    {
                        PartsStatus item;
                        item.partskinds = i;
                        item.partsNumber = j;
                        item.isHave = PlayerInformationManager.Instance.GetHaveProfileParts(i, j);
                        item.name = PlayerInformationManager.Instance.profileListEntries[i].itemList[j].name;
                        item.rarity = rarity;
                        item.sprite = PlayerInformationManager.Instance.profileListEntries[i].itemList[j].sprite;

                        list.Add(item);

                        //Debug.Log($"GachaGenerator.MakeGachaPool : Add {item.name}");
                    }
                }
            }

            Debug.Log($"GachaGenerator.MakeGachaPool : {rarity} {list.Count}");
        }

        /// <summary>
        /// ガチャ結果(レアリティ)を受信して、ガチャ結果（内容）を抽選する
        /// </summary>
        /// <param name="resultRarityList">ガチャ結果(レアリティ)のリスト</param>
        public void ReceptionGachaResultAndLotteryGachaAll(List<string> resultRarityList)
        {
            List<PartsStatus> resultContentList = new List<PartsStatus>();
            bool errorFlg = false;

            foreach (string item in resultRarityList)
            {
                switch (item)
                {
                    case RESULT_C:
                        resultContentList.Add(LotteryGacha(RESULT_C)) ;
                        break;
                    case RESULT_UC:
                        resultContentList.Add(LotteryGacha(RESULT_UC));
                        break;
                    case RESULT_R:
                        resultContentList.Add(LotteryGacha(RESULT_R));
                        break;
                    case RESULT_SR:
                        resultContentList.Add(LotteryGacha(RESULT_SR));
                        break;
                    case RESULT_HR:
                        resultContentList.Add(LotteryGacha(RESULT_HR));
                        break;
                    default:
                        errorFlg = true;
                        break;
                }
            }

            if (errorFlg)
            {
                Debug.LogError($"GachaGenerator.ReceptionGachaResultAndLotteryGachaAll : Error");
            }
            else
            {
                foreach (var item in resultContentList)
                {
                    gachaResultPanel.SpawnGachaResultPrefab(item);
                }
            }
        }

        /// <summary>
        /// ガチャ失敗を受け取ったとき
        /// </summary>
        public void ReceptionGachaFailure(string str)
        {
            gachaResultPanel.SpawnGachaFailurePrefab(str);
        }

        /// <summary>
        /// 引数のレアリティによってガチャの排出結果を抽選する
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        private PartsStatus LotteryGacha(string rarity)
        {
            int length;
            bool errorFlg = false;

            switch (rarity)
            {
                case RESULT_C:
                    length = listCommon.Count;
                    break;
                case RESULT_UC:
                    length = listUncommon.Count;
                    break;
                case RESULT_R:
                    length = listRare.Count;
                    break;
                case RESULT_SR:
                    length = listSuperRare.Count;
                    break;
                case RESULT_HR:
                    length = listHyperRare.Count;
                    break;
                default:
                    length = 0;
                    errorFlg = true;
                    break;
            }

            int randNum = Random.Range(0, length);

            PartsStatus partsStatus = new PartsStatus();

            switch (rarity)
            {
                case RESULT_C:
                    partsStatus = listCommon[randNum];
                    break;
                case RESULT_UC:
                    partsStatus = listUncommon[randNum];
                    break;
                case RESULT_R:
                    partsStatus = listRare[randNum];
                    break;
                case RESULT_SR:
                    partsStatus = listSuperRare[randNum];
                    break;
                case RESULT_HR:
                    partsStatus = listHyperRare[randNum];
                    break;
                default:
                    errorFlg = true;
                    break;
            }

            if (errorFlg)
            {
                Debug.LogError($"GachaGenerator.LotteryGacha : Error");
                return new PartsStatus();
            }
            else
            {
                if (!partsStatus.isHave)
                {
                    UpdatePartsStatusIsHave(rarity, randNum); //以降、所持済みにする
                    PlayerInformationManager.Instance.AcquisitionParts(partsStatus.partskinds, partsStatus.partsNumber);
                }
                return partsStatus;
            }
        }

        /// <summary>
        /// ガチャプールの要素の一つのisHaveをTrueにする
        /// </summary>
        /// <param name="rarity"></param>
        /// <param name="number"></param>
        private void UpdatePartsStatusIsHave(string rarity,int number)
        {
            PartsStatus partsStatus = new PartsStatus();
            bool errorFlg = false;

            switch (rarity)
            {
                case RESULT_C:
                    partsStatus = listCommon[number];
                    partsStatus.isHave = true;
                    listCommon[number] = partsStatus;
                    break;
                case RESULT_UC:
                    partsStatus = listUncommon[number];
                    partsStatus.isHave = true;
                    listUncommon[number] = partsStatus;
                    break;
                case RESULT_R:
                    partsStatus = listRare[number];
                    partsStatus.isHave = true;
                    listRare[number] = partsStatus;
                    break;
                case RESULT_SR:
                    partsStatus = listSuperRare[number];
                    partsStatus.isHave = true;
                    listSuperRare[number] = partsStatus;
                    break;
                case RESULT_HR:
                    partsStatus = listHyperRare[number];
                    partsStatus.isHave = true;
                    listHyperRare[number] = partsStatus;
                    break;
                default:
                    errorFlg = true;
                    break;
            }

            if(errorFlg)
            {
                Debug.LogError($"GachaGenerator.UpdatePartsStatusIsHave : Error");
            }
        }

        /// <summary>
        /// 結果画面の表示を切り替える
        /// </summary>
        /// <param name="flg"></param>
        public void DisplayGachaPanel(bool flg)
        {
            gachaResultPanel.DisplayGachaPanel(flg);
        }


    }
}
