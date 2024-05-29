using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconItemListGenerator : MonoBehaviour
{
    [SerializeField] private IconItemListEntry iconItemListEntry;

    public string GetName(int elementNumber)
    {
        return iconItemListEntry.itemList[elementNumber].name;
    }

    public Sprite GetSprite(string name)
    {
        foreach (var item in iconItemListEntry.itemList)
        {
            if (item.name == name)
            {
                return item.sprite;
            }
        }

        return null;
    }

    public Sprite GetSprite(int elementNumber)
    {
        return iconItemListEntry.itemList[elementNumber].sprite;
    }

    public int GetLength()
    {
        return iconItemListEntry.itemList.Count;
    }
}
