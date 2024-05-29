using System;
using UnityEngine;

[Serializable]
public class IconItem
{
    public string name;
    public Sprite sprite;

    public IconItem(string name, Sprite sprite)
    {
        this.name = name;
        this.sprite = sprite;
    }
}
