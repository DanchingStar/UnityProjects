using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RankingModeListEntry", menuName = "Ranking Mode List Entry")]
public class RankingModeListEntry : ScriptableObject
{
    public List<RankingModeList> itemList = new List<RankingModeList>();
}
