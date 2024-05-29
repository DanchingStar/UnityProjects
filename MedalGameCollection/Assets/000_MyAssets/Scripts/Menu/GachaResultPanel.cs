using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Menu.GachaGenerator;

namespace Menu
{
    public class GachaResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject insidePanel;
        [SerializeField] private GameObject spawnArea;
        [SerializeField] private GameObject gachaResultPrefab;
        [SerializeField] private GameObject gachaFailurePrefab;

        private Transform myTransform;

        private void Start()
        {
            myTransform = this.gameObject.GetComponent<RectTransform>();

            DisplayGachaPanel(false);
        }

        private void Update()
        {            


        }

        /// <summary>
        /// このパネルの表示を切り替える
        /// </summary>
        /// <param name="flg"></param>
        public void DisplayGachaPanel(bool flg)
        {
            if (flg)
            {
                myTransform.localScale = new Vector3(1,1,1);
                insidePanel.SetActive(true);
            }
            else
            {
                insidePanel.SetActive(false);
                myTransform.localScale = new Vector3(0, 0, 0);

                foreach (Transform child in spawnArea.transform)
                {
                    Destroy(child.gameObject); //自分の子供をDestroyする
                }
            }
        }

        /// <summary>
        /// 結果画面を閉じるボタンを押したとき
        /// </summary>
        public void PushCloseButton()
        {
            DisplayGachaPanel(false);
            MenuSceneManager.Instance.UpdateGachaTicketText();
            MenuSceneManager.Instance.SetFalseWaitingGachaResultFlg();
        }

        /// <summary>
        /// ガチャの内容を生成する
        /// </summary>
        /// <param name="status"></param>
        public void SpawnGachaResultPrefab(PartsStatus status)
        {
            GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

            obj.GetComponent<GachaResultPrefab>().SetMyStatus(status);
        }

        /// <summary>
        /// ガチャ失敗を伝えるPrefabを生成する
        /// </summary>
        /// <param name="str">表示するエラーメッセージ</param>
        public void SpawnGachaFailurePrefab(string str)
        {
            GameObject obj = Instantiate(gachaFailurePrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

            obj.GetComponent<GachaFailurePrefab>().SetMyMessage(str);
        }



    }

}
