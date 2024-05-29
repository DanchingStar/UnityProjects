using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
    public CommonData commonData;

    public StageClear stageClear;

    public float[] rankingTime;

    public HaveItems haveItems;

    StageClearStarFlgs INIT_SAMPLE_FOR_STAR_FLAGS;

    public SaveData()
    {
        commonData = new CommonData();
        stageClear = new StageClear();
        haveItems = new HaveItems();

        INIT_SAMPLE_FOR_STAR_FLAGS = new StageClearStarFlgs(false, false, false, false, 0f);
    }

    public void InitializeStageClearStarFlgsForNormal(int ElementCount)
    {
        stageClear.normal = Enumerable.Repeat(INIT_SAMPLE_FOR_STAR_FLAGS, ElementCount).ToArray();
    }

    public void InitializeStageClearStarFlgsForHard(int ElementCount)
    {
        stageClear.hard = Enumerable.Repeat(INIT_SAMPLE_FOR_STAR_FLAGS, ElementCount).ToArray();
    }

    public void InitializeStageClearStarFlgsForVeryHard(int ElementCount)
    {
        stageClear.veryhard = Enumerable.Repeat(INIT_SAMPLE_FOR_STAR_FLAGS, ElementCount).ToArray();
    }

    public void InitializeRankingTime(int ElementCount)
    {
        rankingTime = Enumerable.Repeat(0f, ElementCount).ToArray();
    }

    public void InitializeHaveCharacters(int ElementCount)
    {
        haveItems.characters = Enumerable.Repeat(0, ElementCount).ToArray();
    }

    public void InitializeHaveBackgrounds(int ElementCount)
    {
        haveItems.backgrounds = Enumerable.Repeat(0, ElementCount).ToArray();
    }

    public void InitializeHaveFlames(int ElementCount)
    {
        haveItems.flames = Enumerable.Repeat(0, ElementCount).ToArray();
    }

}

[Serializable]
public class CommonData
{
    public string playerName; //プレイヤー名
    public int logInDays; //ログイン日数
    public string lastLoginDay; //最終ログイン日
    public bool todaySNSShareFlg; //本日SNSシェア済みかのフラグ
    public int rewardTimes; //Reward広告を視聴した回数
    public int shareDays; //SNSシェアした日数

    public int selectCharacterNumber; //設定しているキャラクター番号
    public int selectIconBackgroundNumber; //設定しているアイコン背景番号
    public int selectIconFrameNumber; //設定しているアイコン枠番号

    public int clearStarNumForObtainSpCoin; //「SPコインをもらう」で前回もらったときのスター数
}

[Serializable]
public class StageClearStarFlgs
{
    public bool star1;
    public bool star2;
    public bool star3;
    public bool starAll;
    public float bestTime;

    public StageClearStarFlgs(bool star1, bool star2, bool star3, bool starAll, float bestTime)
    {
        this.star1 = star1;
        this.star2 = star2;
        this.star3 = star3;
        this.starAll = starAll;
        this.bestTime = bestTime;
    }

    public StageClearStarFlgs(StageClearStarFlgs stageClearStarFlgs)
    {
        this.star1 = stageClearStarFlgs.star1;
        this.star2 = stageClearStarFlgs.star2;
        this.star3 = stageClearStarFlgs.star3;
        this.starAll = stageClearStarFlgs.starAll;
        this.bestTime = stageClearStarFlgs.bestTime;
    }
}

[Serializable]
public class StageClear
{
    public StageClearStarFlgs[] normal;
    public StageClearStarFlgs[] hard;
    public StageClearStarFlgs[] veryhard;
}

[Serializable]
public class HaveItems
{
    public int[] characters;
    public int[] backgrounds;
    public int[] flames;
}


