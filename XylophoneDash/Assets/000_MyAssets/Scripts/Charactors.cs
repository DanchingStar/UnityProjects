using System;
using UnityEngine;

[Serializable]
public class Charactors
{
    public string name;
    public Sprite sprite;
    public GameObject playerPrefab;

    public Charactors(string name, Sprite sprite, GameObject playerPrefab)
    {
        this.name = name;
        this.sprite = sprite;
        this.playerPrefab = playerPrefab;
    }
}
