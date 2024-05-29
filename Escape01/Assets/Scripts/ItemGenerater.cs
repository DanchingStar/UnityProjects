using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerater : MonoBehaviour
{
    [SerializeField] ItemListEntity itemListEntity;

    public static ItemGenerater instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Item Spawn(Item.Type type)
    {
        foreach (var item in itemListEntity.itemList)
        {
            if(item.type == type)
            {
                return new Item(item.type, item.sprite, item.unionGroup);
            }
        }
        return null;
    }

    public Item StringToItem(string itemName)
    {
        foreach (var item in itemListEntity.itemList)
        {
            if (item.type.ToString() == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
