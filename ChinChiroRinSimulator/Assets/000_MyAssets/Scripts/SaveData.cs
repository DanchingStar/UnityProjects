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
    //public string playerName; //�v���C���[��
    //public int logInDays; //���O�C������
    //public string lastLoginDay; //�ŏI���O�C����
    //public bool todaySNSShareFlg; //�{��SNS�V�F�A�ς݂��̃t���O
    //public int rewardTimes; //Reward�L��������������
    //public int shareDays; //SNS�V�F�A��������

    //public int selectCharacterNumber; //�ݒ肵�Ă���L�����N�^�[�ԍ�
    //public int selectIconBackgroundNumber; //�ݒ肵�Ă���A�C�R���w�i�ԍ�
    //public int selectIconFrameNumber; //�ݒ肵�Ă���A�C�R���g�ԍ�

    //public int clearStarNumForObtainSpCoin; //�uSP�R�C�������炤�v�őO���������Ƃ��̃X�^�[��
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

