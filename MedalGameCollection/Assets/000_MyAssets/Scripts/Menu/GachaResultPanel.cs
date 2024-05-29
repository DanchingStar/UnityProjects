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
        /// ���̃p�l���̕\����؂�ւ���
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
                    Destroy(child.gameObject); //�����̎q����Destroy����
                }
            }
        }

        /// <summary>
        /// ���ʉ�ʂ����{�^�����������Ƃ�
        /// </summary>
        public void PushCloseButton()
        {
            DisplayGachaPanel(false);
            MenuSceneManager.Instance.UpdateGachaTicketText();
            MenuSceneManager.Instance.SetFalseWaitingGachaResultFlg();
        }

        /// <summary>
        /// �K�`���̓��e�𐶐�����
        /// </summary>
        /// <param name="status"></param>
        public void SpawnGachaResultPrefab(PartsStatus status)
        {
            GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

            obj.GetComponent<GachaResultPrefab>().SetMyStatus(status);
        }

        /// <summary>
        /// �K�`�����s��`����Prefab�𐶐�����
        /// </summary>
        /// <param name="str">�\������G���[���b�Z�[�W</param>
        public void SpawnGachaFailurePrefab(string str)
        {
            GameObject obj = Instantiate(gachaFailurePrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

            obj.GetComponent<GachaFailurePrefab>().SetMyMessage(str);
        }



    }

}
