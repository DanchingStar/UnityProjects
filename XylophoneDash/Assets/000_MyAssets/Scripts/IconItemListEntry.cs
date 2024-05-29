using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconItemListEntry", menuName = "Icon Item List Entry")]
public class IconItemListEntry : ScriptableObject
{
    public List<IconItem> itemList = new List<IconItem>();
}
