using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UnionItemListEntity : ScriptableObject
{
    public List<UnionItemStatus> UnionItemList = new List<UnionItemStatus>();
}
