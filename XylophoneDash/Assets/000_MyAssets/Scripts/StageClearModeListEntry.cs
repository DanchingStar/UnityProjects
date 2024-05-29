using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageClearModeListEntry", menuName = "Stage Clear Mode List Entry")]
public class StageClearModeListEntry : ScriptableObject
{
    public List<StageClearModeList> itemList = new List<StageClearModeList>();

}
