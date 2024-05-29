using System;
using UnityEngine;

[Serializable]
public class Profile
{
    public string name;
    public Sprite sprite;
    public Rarity rarity;

    public enum Rarity
    {
        None,
        Default,
        Common,
        Uncommon,
        Rare,
        SuperRare,
        HyperRare,
        EventR,
        EventSR,
        EventHR,
    }

    public Profile(string name, Sprite sprite, Rarity rarity)
    {
        this.name = name;
        this.sprite = sprite;
        this.rarity = rarity;
    }
}
