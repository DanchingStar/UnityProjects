using System;
using UnityEngine;

[Serializable]
public class Item
{
    public enum Type
    {
        None,
        Pai_1,
        Pai_2,
        Pai_3,
        ToiletPaper,
        TvRemoteController,
        Axe,
        Ring,
        MatchBou,
        MatchBako,
        MatchSet,
        Glue,
        BrokenKeyA,
        BrokenKeyB,
        Key,
        IronPipe,
        Koma_Hisha,
        Koma_Kin,
        Koma_Kei,

    }


    public Type type;
    public Sprite sprite;
    public UnionItemStatus.Group unionGroup;

    public Item(Type type, Sprite sprite, UnionItemStatus.Group unionGroup)
    {
        this.type = type;
        this.sprite = sprite;
        this.unionGroup = unionGroup;
    }
}
