using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
    public CommonData commonData;
    public ProfileData profileData;
    public HaveProfileParts haveProfileParts;
    public DropBallsData dropBallsData;
    public JanKenData janKenData;
    public Pusher01Data pusher01Data;
    public SmartBall01Data smartBall01Data;

    public SaveData()
    {
        commonData = new CommonData();
        profileData = new ProfileData();
        haveProfileParts = new HaveProfileParts();
        dropBallsData = new DropBallsData();
        janKenData = new JanKenData();
        pusher01Data = new Pusher01Data();
        smartBall01Data = new SmartBall01Data();
    }

    public void InitializeHaveProfileParts(int[] ElementCount)
    {
        InitializeHaveDefaultPart(ref haveProfileParts.haveBackground, ElementCount, 0);
        InitializeHaveDefaultPart(ref haveProfileParts.haveOutline, ElementCount, 1);
        InitializeHaveDefaultPart(ref haveProfileParts.haveAccessory, ElementCount, 2);
        InitializeHaveDefaultPart(ref haveProfileParts.haveWrinkle, ElementCount, 3);
        InitializeHaveDefaultPart(ref haveProfileParts.haveEar, ElementCount, 4);
        InitializeHaveDefaultPart(ref haveProfileParts.haveMouth, ElementCount, 5);
        InitializeHaveDefaultPart(ref haveProfileParts.haveNose, ElementCount, 6);
        InitializeHaveDefaultPart(ref haveProfileParts.haveEyebrow, ElementCount, 7);
        InitializeHaveDefaultPart(ref haveProfileParts.haveEye, ElementCount, 8);
        InitializeHaveDefaultPart(ref haveProfileParts.haveGlasses, ElementCount, 9);
        InitializeHaveDefaultPart(ref haveProfileParts.haveHair, ElementCount, 10);

    }

    private void InitializeHaveDefaultPart(ref bool[] box,int[] ElementCount,int number)
    {
        if (box == null)
        {
            // Debug.Log($"InitializeHaveDefaultPart : box == null : number.{number}");

            box = Enumerable.Repeat<bool>(false, ElementCount[number]).ToArray();

            //for (int i = 0; i < ElementCount[number]; i++)
            //{
            //    if (PlayerInformationManager.Instance.profileListEntries[number].itemList[i].rarity == Profile.Rarity.Default)
            //    {
            //        box[i] = true;
            //    }
            //    else
            //    {
            //        box[i] = false;
            //    }
            //}
        }
        else
        {
            Debug.Log($"InitializeHaveDefaultPart : 初期化済み？？");
        }
    }
}

[Serializable]
public class CommonData
{
    public string playerName; //プレイヤー名
    public int logInDays; //ログイン日数
    public int haveMedal; //持っているメダル
    public int haveSPCoin; //持っているSPコイン
    public int playerLevel; //プレイヤーレベル
    public int playerExperience; //プレイヤーの経験値
    public string lastLoginDay; //最終ログイン日
    public bool todaySNSShareFlg; //本日SNSシェア済みかのフラグ
    public int rewardTimes; //Reward広告を視聴した回数
    public int shareDays; //SNSシェアした日数
}

[Serializable]
public class ProfileData
{
    public int myBackgroundNumber; //背景
    //public int myBackgroundVerticalPosition;
    //public int myBackgroundHorizontalPosition;
    public int myOutlineNumber; //輪郭(肌色)
    public int myOutlineVerticalPosition;
    public int myOutlineHorizontalPosition;
    public int myAccessoryNumber; //アクセサリー
    public int myAccessoryVerticalPosition;
    public int myAccessoryHorizontalPosition;
    public int myWrinkleNumber; //しわ
    public int myWrinkleVerticalPosition;
    public int myWrinkleHorizontalPosition;
    public int myEarNumber; //耳
    public int myEarVerticalPosition;
    public int myEarHorizontalPosition;
    public int myMouthNumber; //口
    public int myMouthVerticalPosition;
    public int myMouthHorizontalPosition;
    public int myNoseNumber; //鼻
    public int myNoseVerticalPosition;
    public int myNoseHorizontalPosition;
    public int myEyebrowNumber; //眉
    public int myEyebrowVerticalPosition;
    public int myEyebrowHorizontalPosition;
    public int myEyeNumber; //目
    public int myEyeVerticalPosition;
    public int myEyeHorizontalPosition;
    public int myGlassesNumber; //メガネ
    public int myGlassesVerticalPosition;
    public int myGlassesHorizontalPosition;
    public int myHairNumber; //髪
    public int myHairVerticalPosition;
    public int myHairHorizontalPosition;
}

[Serializable]
public class HaveProfileParts
{
    public bool[] haveBackground;
    public bool[] haveOutline;
    public bool[] haveAccessory;
    public bool[] haveWrinkle;
    public bool[] haveEar;
    public bool[] haveMouth;
    public bool[] haveNose;
    public bool[] haveEyebrow;
    public bool[] haveEye;
    public bool[] haveGlasses;
    public bool[] haveHair;
}

[Serializable]
public class DropBallsData
{
    public int perfectClearTimes;
    public int shotTimes;
    public int getMedalsTotal;
    public int lostBallsTotal;
}

[Serializable]
public class JanKenData
{
    public int battleTimes;
    public int winTimes;
    public int getMedalsTotal;
}

[Serializable]
public class Pusher01Data
{
    public int betMedalTotal;
    public int getMedalTotal;
    public int bonusTimes;
}

[Serializable]
public class SmartBall01Data
{
    public int betMedalTotal;
    public int getMedalTotal;
    public int maxLines;
}


