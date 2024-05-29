using System;
using UnityEngine;

[Serializable]
public class GimmickName
{
    public enum Type
    {
        None,
        RockPaperScissors,
        Books,
        Clock,
        Mugcups,
        Chess,
        ComputerPowerSupply,
        UseTvRemoteController,
        PCMonitor,
        UseMatchSet,
        Kotatsu,
        TVMonitor,
        Wine,
        UseRing,
        DuckAndPigeon,
        UseKey,
        UseHisha,
        UseKin,
        UseKei,
        ShogiBan,
        PictureName,
        UseIronPipe,
        UseToiletPaper,
        RefrigeratorCans,
        LockerAndRestroomTips,
        UseAxe,
        UnderThePillow,
        ThreeCardsTips,
        UsePai_1,
        UsePai_2,
        UsePai_3,
        FinalDoor,
    }

    public Type type;
    public string HintMwssage;
}
