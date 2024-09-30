using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnkanPrefab : NakiPrefab
{
    public void SetStatus(MahjongManager.PaiKinds _thisKind,
        int _totalNumber1, int _arrayNumber1, int _totalNumber2, int _arrayNumber2,
        int _totalNumber3, int _arrayNumber3, int _totalNumber4, int _arrayNumber4)
    {
        base.SetStatusFirst(NakiPlace.None, MahjongManager.NakiKinds.Ankan);

        paiPrefabs[0].SetStatus(_thisKind, _totalNumber1, _arrayNumber1, MahjongManager.PlayerKind.Player);
        paiPrefabs[1].SetStatus(_thisKind, _totalNumber2, _arrayNumber2, MahjongManager.PlayerKind.Player);
        paiPrefabs[2].SetStatus(_thisKind, _totalNumber3, _arrayNumber3, MahjongManager.PlayerKind.Player);
        paiPrefabs[3].SetStatus(_thisKind, _totalNumber4, _arrayNumber4, MahjongManager.PlayerKind.Player);

        MahjongManager.MentsuKinds mentsuKind = MahjongManager.MentsuKinds.Kantsu;
        myMentsuStatus = MahjongManager.Instance.GetMentsuStatus(mentsuKind, _thisKind, false);
    }

}

[CustomEditor(typeof(AnkanPrefab))] // NakiPrefabを拡張する
public class NakiPrefabDisplayLogForAnkan : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Display Log of Status"))
        {
            AnkanPrefab yourScript = (AnkanPrefab)target;
            yourScript.InspectorButtonFunction();
        }

    }
}
