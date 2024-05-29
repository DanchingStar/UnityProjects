using System;
using UnityEngine;

[Serializable]
public class UnionItemStatus
{
    public enum Group
    {
        NoGroup,
        MatchSet,
        Key,
    }

    public Group group;
    public int num;

}
