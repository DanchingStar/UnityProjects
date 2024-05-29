using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public CommonData commonData;

    public SaikoroMySet saikoroMySet;

    public SaveData()
    {
        commonData = new CommonData();
        saikoroMySet = new SaikoroMySet();
    }
}

[Serializable]
public class CommonData
{
    //public string playerName; //プレイヤー名
    //public int logInDays; //ログイン日数
    //public string lastLoginDay; //最終ログイン日
    //public bool todaySNSShareFlg; //本日SNSシェア済みかのフラグ
    //public int rewardTimes; //Reward広告を視聴した回数
    //public int shareDays; //SNSシェアした日数

    //public int selectCharacterNumber; //設定しているキャラクター番号
    //public int selectIconBackgroundNumber; //設定しているアイコン背景番号
    //public int selectIconFrameNumber; //設定しているアイコン枠番号

    //public int clearStarNumForObtainSpCoin; //「SPコインをもらう」で前回もらったときのスター数
}

[Serializable]
public class MyDice
{
    public string name = "";
    public DiceController.DiceEyeKinds[] changeEyes = new DiceController.DiceEyeKinds[Enum.GetValues(typeof(DiceController.DiceEyeKinds)).Length];
}

[Serializable]
public class SaikoroMySet
{
    public List<MyDice> myDice;
}

