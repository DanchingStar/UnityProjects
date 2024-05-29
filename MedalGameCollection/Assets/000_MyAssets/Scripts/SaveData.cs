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
            Debug.Log($"InitializeHaveDefaultPart : �������ς݁H�H");
        }
    }
}

[Serializable]
public class CommonData
{
    public string playerName; //�v���C���[��
    public int logInDays; //���O�C������
    public int haveMedal; //�����Ă��郁�_��
    public int haveSPCoin; //�����Ă���SP�R�C��
    public int playerLevel; //�v���C���[���x��
    public int playerExperience; //�v���C���[�̌o���l
    public string lastLoginDay; //�ŏI���O�C����
    public bool todaySNSShareFlg; //�{��SNS�V�F�A�ς݂��̃t���O
    public int rewardTimes; //Reward�L��������������
    public int shareDays; //SNS�V�F�A��������
}

[Serializable]
public class ProfileData
{
    public int myBackgroundNumber; //�w�i
    //public int myBackgroundVerticalPosition;
    //public int myBackgroundHorizontalPosition;
    public int myOutlineNumber; //�֊s(���F)
    public int myOutlineVerticalPosition;
    public int myOutlineHorizontalPosition;
    public int myAccessoryNumber; //�A�N�Z�T���[
    public int myAccessoryVerticalPosition;
    public int myAccessoryHorizontalPosition;
    public int myWrinkleNumber; //����
    public int myWrinkleVerticalPosition;
    public int myWrinkleHorizontalPosition;
    public int myEarNumber; //��
    public int myEarVerticalPosition;
    public int myEarHorizontalPosition;
    public int myMouthNumber; //��
    public int myMouthVerticalPosition;
    public int myMouthHorizontalPosition;
    public int myNoseNumber; //�@
    public int myNoseVerticalPosition;
    public int myNoseHorizontalPosition;
    public int myEyebrowNumber; //��
    public int myEyebrowVerticalPosition;
    public int myEyebrowHorizontalPosition;
    public int myEyeNumber; //��
    public int myEyeVerticalPosition;
    public int myEyeHorizontalPosition;
    public int myGlassesNumber; //���K�l
    public int myGlassesVerticalPosition;
    public int myGlassesHorizontalPosition;
    public int myHairNumber; //��
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


