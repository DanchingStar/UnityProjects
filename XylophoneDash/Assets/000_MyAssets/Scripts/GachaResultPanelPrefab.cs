using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaResultPanelPrefab : MonoBehaviour
{

    [SerializeField] private GameObject insidePanel;
    [SerializeField] private GameObject spawnArea;
    [SerializeField] private GameObject gachaResultPrefab;

    private void Start()
    {

    }

    private void Update()
    {


    }

    /// <summary>
    /// ���ʉ�ʂ����{�^�����������Ƃ�
    /// </summary>
    public void PushCloseButton()
    {
        MenuSceneManager.Instance.UpdateGachaTicketText();
        MenuSceneManager.Instance.ReceptionCloseFromGachaResultPanelPrefab();

        Destroy(gameObject);
    }

    /// <summary>
    /// �K�`���̓��e�𐶐�����
    /// </summary>
    /// <param name="status"></param>
    public void SpawnGachaResultPrefab(PlayerInformationManager.GachaItemKind itemKinds, int itemNumber, bool newFlg)
    {
        GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

        obj.GetComponent<GachaResultPrefab>().SetMyStatus(itemKinds, itemNumber, newFlg);
    }

    /// <summary>
    /// �K�`�����s��`����Prefab�𐶐�����
    /// </summary>
    /// <param name="str">�\������G���[���b�Z�[�W</param>
    public void SpawnGachaFailurePrefab(string str)
    {
        GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

        obj.GetComponent<GachaResultPrefab>().SetMyStatusFailure(str);
    }
}
