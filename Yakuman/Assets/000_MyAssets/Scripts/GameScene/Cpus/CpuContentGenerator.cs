using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuContentGenerator : MonoBehaviour
{
    [SerializeField] private CpuContents[] cpuContentList;

    [Serializable]
    public class CpuContents
    {
        public string name;
        public CpuParent cpuChild;
        public Sprite sprite;
    }

    public string GetCpuName(int number)
    {
        return cpuContentList[number].name;
    }

    public CpuParent GetCpuChild(int number)
    {
        return cpuContentList[number].cpuChild;
    }

    public CpuParent GetCpuChild(string _name)
    {
        foreach (var item in cpuContentList)
        {
            return item.cpuChild;
        }
        return null;
    }

    public Sprite GetSprite(int number)
    {
        return cpuContentList[number].sprite;
    }

    public Sprite GetSprite(string _name)
    {
        foreach (var item in cpuContentList)
        {
            return item.sprite;
        }
        return null;
    }

    public int GetLength()
    {
        return cpuContentList.Length;
    }

}
