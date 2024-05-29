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
    /// 結果画面を閉じるボタンを押したとき
    /// </summary>
    public void PushCloseButton()
    {
        MenuSceneManager.Instance.UpdateGachaTicketText();
        MenuSceneManager.Instance.ReceptionCloseFromGachaResultPanelPrefab();

        Destroy(gameObject);
    }

    /// <summary>
    /// ガチャの内容を生成する
    /// </summary>
    /// <param name="status"></param>
    public void SpawnGachaResultPrefab(PlayerInformationManager.GachaItemKind itemKinds, int itemNumber, bool newFlg)
    {
        GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

        obj.GetComponent<GachaResultPrefab>().SetMyStatus(itemKinds, itemNumber, newFlg);
    }

    /// <summary>
    /// ガチャ失敗を伝えるPrefabを生成する
    /// </summary>
    /// <param name="str">表示するエラーメッセージ</param>
    public void SpawnGachaFailurePrefab(string str)
    {
        GameObject obj = Instantiate(gachaResultPrefab, Vector3.zero, Quaternion.identity, spawnArea.transform);

        obj.GetComponent<GachaResultPrefab>().SetMyStatusFailure(str);
    }
}
