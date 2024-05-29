using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;

public class AdMobReward : MonoBehaviour
{
    private RewardedAd rewardedAd;

#if UNITY_ANDROID
    private string adUnitId = "ca-app-pub-7223826824285484/4435429253";  //�{��
    //private string adUnitId = "ca-app-pub-3940256099942544/5224354917";  //�e�X�g
#elif UNITY_IOS
    //private string adUnitId = "�L�����j�b�gID���R�s�y�iiOS�j";  //�{��
    private string adUnitId = "ca-app-pub-3940256099942544/1712485313";  //�e�X�g
#else
    private string adUnitId = "unexpected_platform";
#endif

    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Text rewardText;

    [SerializeField] private AudioSource seYes = null;
    [SerializeField] private AudioSource showReward = null;

    private void Start()
    {
        //MobileAds.Initialize(initStatus => { }); //�A�v���N�����Ɉ�x�K�����s�i���̃X�N���v�g�Ŏ��s���Ă�����s�v�j

        this.rewardedAd = new RewardedAd(adUnitId);

        // Load�������Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Load���s���Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // �\�����Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // �\�����s���Ɏ��s����֐��̓o�^
        //this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // ��V�󂯎�莞�Ɏ��s����֐��̓o�^
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // �L������鎞�Ɏ��s����֐��̓o�^
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        RequestReward(); //�L�������[�h

        rewardText.text = "";
    }

    /// <summary>
    /// Reward�L�������[�h
    /// </summary>
    private void RequestReward()
    {
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }

    /// <summary>
    /// �L���Ƃ̃C���^���N�V�����Ń��[�U�[�ɕ�V��^����ׂ��Ƃ��ɌĂяo�����n���h��
    /// </summary>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        RewardContent();
    }

    /// <summary>
    /// �L��������ꂽ�Ƃ��ɌĂяo�����n���h��
    /// </summary>
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        RequestReward();

        rewardPanel.SetActive(true);
    }

    /// <summary>
    /// ������ĂׂΓ��悪�����i�Ⴆ�΃{�^���������Ȃǁj
    /// </summary>
    public void ShowReawrd()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        if (this.rewardedAd.IsLoaded())
        {
            hintPanel.SetActive(false);

            this.rewardedAd.Show();
        }
    }

    /// <summary>
    /// Reward��V�̓��e
    /// </summary>
    private void RewardContent()
    {
        if (showReward != null)
        {
            showReward.Play();
        }

        rewardText.text = MakeRewardText();
    }

    /// <summary>
    /// RewardPanel�����Ƃ�
    /// </summary>
    public void CloseRewardPanel()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        rewardText.text = "";
        rewardPanel.SetActive(false);
    }

    /// <summary>
    /// Reward�œ�����q���g�̕��������
    /// </summary>
    /// <returns>�q���g�̕���</returns>
    private string MakeRewardText()
    {
        string str = "�����[�h�G���[001\n��V�̃e�L�X�g���܂�\n�ݒ肳��Ă��܂���B";
        GimmickName.Type nowGimmick = GimmickName.Type.None;

        foreach (GimmickName.Type value in Enum.GetValues(typeof(GimmickName.Type)))
        {
            if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(GimmickName.Type), value), 0) && value != GimmickName.Type.None)
            {
                nowGimmick = value;
                break;
            }
        }

        Debug.Log(nowGimmick);

        switch (nowGimmick)
        {
            case GimmickName.Type.None:
                str = "�S�Ă̓����������I\n" +
                      "���S���e����(^^)\n" +
                      "�J�����o������E�o���悤�I�I";
                break;
            case GimmickName.Type.RockPaperScissors:
                str = "�΂̔��̉��ɂ���p�l����\n" +
                      "�G���Ă݂悤�I\n" +
                      "�L���̕ǂɏ����Ă���G��\n" +
                      "�q���g����I\n" +
                      "[��][�͂���][��]��\n" +
                      "�Ή�����悤�ɁA\n" +
                      "[�O�[][�`���L][�p�[]��\n" +
                      "�p�l�������킹�悤�I�I";
                break;
            case GimmickName.Type.Books:
                str = "�{�I�ɂ���{�ɒ��ځI\n" +
                      "�ǂɏ����Ă��郏�C���̉��ɂ���\n" +
                      "�����o���̂���Ƌ�̎d�|����\n" +
                      "�����z�u���ˁI\n" +
                      "�{�I�̖{�̈ʒu�ƐF�ɍ��킹��\n" +
                      "�d�|���̐F��ς��Ă݂悤�I�I";
                break;
            case GimmickName.Type.Clock:
                str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                      "�S�̎d�|�������Ă���\n" +
                      "�Ƌ����ˁI\n" +
                      "�^�񒆂̉��̎d�|����\n" +
                      "���Ԃ���͂���ƊJ���݂����I\n" +
                      "�����̒��ɂ��鎞�v��������\n" +
                      "���Ԃ���͂��Ă݂悤�I�I";
                break;
            case GimmickName.Type.Mugcups:
                str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                      "�S�̎d�|�������Ă���\n" +
                      "�Ƌ����ˁI\n" +
                      "�E�̎d�|����\n" +
                      "���ɑΉ����Ă���̂��ȁH\n" +
                      "TV�ƃ\�t�@�̊Ԃɂ�����̏��\n" +
                      "�T�̃}�O�J�b�v���u���Ă�ˁI\n" +
                      "�d�|���ɑΉ����Ă���ꏊ��\n" +
                      "�F�����킹�Ă݂悤�I�I";
                break;
            case GimmickName.Type.Chess:
                str = "�Ԃ����ƃR���s���[�^�̊Ԃ�\n" +
                      "�d�|�������Ă�Ƌ����ˁI\n" +
                      "���̊i�q��̔Ղ͂Ȃ񂾂낤�H\n" +
                      "TV�ƃ\�t�@�̊Ԃɂ�����̏��\n" +
                      "�`�F�X�Ղ��u���Ă���ˁI\n" +
                      "�Ղɂ͔��ƍ��̋\n" +
                      "��������u���Ă���ˁI\n" +
                      "�d�|���̔ՂɃ`�F�X�Ղ̋��\n" +
                      "�F���Ή�����悤��\n" +
                      "���u���Ă݂悤�I�I\n" +
                      "�������ǂ�������OK����^^";
                break;
            case GimmickName.Type.ComputerPowerSupply:
                str = "�f�X�N�̏��\n" +
                      "�R���s���[�^������ˁI\n" +
                      "��ʂ��^���Â����ǁA\n" +
                      "�ǂ����܂��d����\n" +
                      "�����Ă��Ȃ��݂����c\n" +
                      "���j�^�[�̉E���̋@�B��\n" +
                      "�F�̓d���{�^����\n" +
                      "�����Ă݂悤�I�I";
                break;
            case GimmickName.Type.UseTvRemoteController:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.TvRemoteController), 0))
                {
                    str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                          "�S�̎d�|�������Ă���\n" +
                          "�Ƌ����ˁI\n" +
                          "�E�̎d�|�����������̂�\n" +
                          "���̔����J������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�傫��TV��\n" +
                          "�ǂɐݒu����Ă���ˁI\n" +
                          "��ʂ��^���Â�����\n" +
                          "�d���������Ȃ����ȁH\n" +
                          "�����Ă��郊���R����I���\n" +
                          "TV�Ɏg���Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.PCMonitor:
                str = "�①�ɂɎd�|�������Ă���ˁI\n" +
                      "�d�|���̃{�^����������\n" +
                      "�_�̒������ς��݂����I\n" +
                      "�ǂ����Ƀq���g�͂Ȃ����ȁH\n" +
                      "�d������ꂽ�R���s���[�^��\n" +
                      "�����̃O���t���`���Ă����I\n" +
                      "�O���t�̒����ɑΉ�����悤��\n" +
                      "�①�ɂ̎d�|���̒�����\n" +
                      "���킹�Ă݂悤�I�I";
                break;
            case GimmickName.Type.UseMatchSet:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchBou), 0))
                {
                    str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                          "�S�̎d�|�������Ă���\n" +
                          "�Ƌ����ˁI\n" +
                          "�^�񒆂̉��̎d�|�����������̂�\n" +
                          "���̈����o�����J������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchBako), 0))
                {
                    str = "�Ԃ����ƃR���s���[�^�̊Ԃ�\n" +
                          "�d�|�������Ă�Ƌ����ˁI\n" +
                          "���̎d�|�����������̂�\n" +
                          "���̈����o�����J������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.MatchSet), 0))
                {
                    str = "�}�b�`�_�ƃ}�b�`�����E�����ˁI\n" +
                          "�ǂ��炩�̃A�C�e����\n" +
                          "�I�����Ă݂悤�I";
                }
                else
                {
                    str = "�Ԃ����̉��ɐC�䂪����ˁI\n" +
                          "�������E�\�N�����邯��\n" +
                          "�΂͂��Ă��Ȃ��݂����c\n" +
                          "�����Ă���}�b�`���g����\n" +
                          "���E�\�N�ɉ΂����Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.Kotatsu:
                str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                      "�S�̎d�|�������Ă���\n" +
                      "�Ƌ����ˁI\n" +
                      "���̎d�|����\n" +
                      "���ɑΉ����Ă���̂��ȁH\n" +
                      "�Q���̂������̎����\n" +
                      "�S�̍��֎q���u���Ă����I\n" +
                      "�d�|���ɑΉ����Ă���ꏊ��\n" +
                      "�F�����킹�Ă݂悤�I�I";
                break;
            case GimmickName.Type.TVMonitor:
                str = "�Q���ɂU�̈����o��������\n" +
                      "�����L���r�l�b�g������ˁI\n" +
                      "��ɂ͏����Ȕ������邯��\n" +
                      "�܂��J���Ȃ��݂����c\n" +
                      "�Ƃ���ł��̃L���r�l�b�g����\n" +
                      "�ǂ����ł݂��悤�ȁc�H\n" +
                      "TV�ɉf���Ă���L���r�l�b�g��\n" +
                      "�����悤�Ɉ����o����\n" +
                      "�J���Ă݂悤�I�I";
                break;
            case GimmickName.Type.Wine:
                str = "�Q���̑傫�ȋ��̉E����\n" +
                      "���b�J�[������ˁI\n" +
                      "�����I�Ȍ`�̎d�|�������ǁA\n" +
                      "�ǂ����Ō��Ȃ��������ȁH\n" +
                      "�ǂɏ����Ă��郏�C����\n" +
                      "�ƂĂ����Ă���ˁI\n" +
                      "���C���̗L���������\n" +
                      "�d�|���̏ꏊ�ƑΉ�������\n" +
                      "���킹�Ă݂悤�I�I";
                break;
            case GimmickName.Type.UseRing:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Ring), 0))
                {
                    str = "�Q���ɂU�̈����o��������\n" +
                          "�����L���r�l�b�g������ˁI\n" +
                          "��ɂ͏����Ȕ������邯��\n" +
                          "�d�|�����������̂ŊJ������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�{�I�Ɏ�̃I�u�W�F������ˁI\n" +
                          "���������w�ւ��E�����̂ŁA\n" +
                          "���̎�ɛƂ߂Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.DuckAndPigeon:
                str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                      "�S�̎d�|�������Ă���\n" +
                      "�Ƌ����ˁI\n" +
                      "�^�񒆂̏�̎d�|����\n" +
                      "�A�q���ƃn�g�̐���\n" +
                      "�����Ƃ����͂���݂����c\n" +
                      "�Q���̃��b�J�[��\n" +
                      "�A�q���̂��������\n" +
                      "�n�g�̒u�����������񂠂�ˁI\n" +
                      "���ꂼ��̐��𐔂���\n" +
                      "�d�|���ɓ��͂��Ă݂悤�I�I";
                break;
            case GimmickName.Type.UseKey:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.BrokenKeyA), 0))
                {
                    str = "�x�b�h�̑����̕�����\n" +
                          "�؂̐F�ŏ����߂�\n" +
                          "�L���r�l�b�g������ˁI\n" +
                          "��̈����o����\n" +
                          "�����������Ă���݂����c\n" +
                          "�ł��A�ǂ���牺�̈����o����\n" +
                          "�J���Ă���݂������ˁI\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.BrokenKeyB), 0))
                {
                    str = "�ǂɏ����Ă��郏�C���̉��ɂ���\n" +
                          "�����o���̂���Ƌ����ˁI\n" +
                          "���̎d�|���͉������̂ŁA\n" +
                          "���̈����o���͊J���Ă����I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Glue), 0))
                {
                    str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                          "�S�̎d�|�������Ă���\n" +
                          "�Ƌ����ˁI\n" +
                          "���̎d�|�����������̂�\n" +
                          "���̔����J������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Key), 0))
                {
                    str = "��ꂽ�����Q�E�����ˁI\n" +
                          "���Ƃ��ƂP�������݂����I\n" +
                          "�ڒ��܂������Ă���̂�\n" +
                          "�������Ă݂悤�I�I\n" +
                          "��ꂽ�����ڒ��܂�\n" +
                          "�����ꂩ�̃A�C�e����\n" +
                          "�I�����Ă݂悤�I";
                }
                else
                {
                    str = "�x�b�h�̑����̕�����\n" +
                          "�؂̐F�ŏ����߂�\n" +
                          "�L���r�l�b�g������ˁI\n" +
                          "��̈����o����\n" +
                          "�����������Ă���݂����c\n" +
                          "�ڒ��܂ł������������g������\n" +
                          "�J���񂶂�Ȃ����ȁH\n" +
                          "�A�C�e�����̌���I������\n" +
                          "�����Ɏg���Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.UseHisha:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Hisha), 0))
                {
                    str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                          "�S�̎d�|�������Ă���\n" +
                          "�Ƌ����ˁI\n" +
                          "�^�񒆂̏�̎d�|�����������̂�\n" +
                          "���̈����o�����J������I\n" +
                          "���ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�Q���ɏ����Ղ��u���Ă���ˁI\n" +
                          "�ł��A�������̋\n" +
                          "����Ă��Ȃ��݂����c\n" +
                          "�����Ă���[��]�̋���g����\n" +
                          "�����Ղ̑���Ă��Ȃ��ꏊ��\n" +
                          "���u���Ă݂悤�I�I\n" +
                          "�ł������̃��[����m��Ȃ���\n" +
                          "�u���ꏊ��������Ȃ�(-_-;)\n" +
                          "���������ɒu���Ă�����\n" +
                          "�����ꏊ�ɓ������u����\n" +
                          "���v����^^";
                }
                break;
            case GimmickName.Type.UseKin:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Kin), 0))
                {
                    str = "�x�b�h�̑����̕�����\n" +
                          "�؂̐F�ŏ����߂�\n" +
                          "�L���r�l�b�g������ˁI\n" +
                          "��̈����o����\n" +
                          "�����������Ă������ǁA\n" +
                          "�����J������ˁI\n" +
                          "�����o���̒��ɂ���A�C�e����\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�Q���ɏ����Ղ��u���Ă���ˁI\n" +
                          "�ł��A�������̋\n" +
                          "����Ă��Ȃ��݂����c\n" +
                          "�����Ă���[����]�̋���g����\n" +
                          "�����Ղ̑���Ă��Ȃ��ꏊ��\n" +
                          "���u���Ă݂悤�I�I\n" +
                          "�ł������̃��[����m��Ȃ���\n" +
                          "�u���ꏊ��������Ȃ�(-_-;)\n" +
                          "���������ɒu���Ă�����\n" +
                          "�����ꏊ�ɓ������u����\n" +
                          "���v����^^";
                }
                break;
            case GimmickName.Type.UseKei:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Koma_Kei), 0))
                {
                    str = "�①�ɂƊJ�����΂̔��Ƃ̊Ԃ�\n" +
                          "�S�̎d�|�������Ă���\n" +
                          "�Ƌ����ˁI\n" +
                          "�^�񒆂̈����o�������\n" +
                          "�����̋�u���Ă���ˁI\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�Q���ɏ����Ղ��u���Ă���ˁI\n" +
                          "�ł��A�������̋\n" +
                          "����Ă��Ȃ��݂����c\n" +
                          "�����Ă���[�j]�̋���g����\n" +
                          "�����Ղ̑���Ă��Ȃ��ꏊ��\n" +
                          "���u���Ă݂悤�I�I\n" +
                          "�ł������̃��[����m��Ȃ���\n" +
                          "�u���ꏊ��������Ȃ�(-_-;)\n" +
                          "���������ɒu���Ă�����\n" +
                          "�����ꏊ�ɓ������u����\n" +
                          "���v����^^";
                }
                break;
            case GimmickName.Type.ShogiBan:
                str = "�Q���ɏ����Ղ��u���Ă���ˁI\n" +
                      "�ł��A�������̋\n" +
                      "����Ă��Ȃ��݂����c\n" +
                      "�����Ă������g����\n" +
                      "�����Ղ̑���Ă��Ȃ��ꏊ��\n" +
                      "���u���Ă݂悤�I�I\n" +
                      "�ł������̃��[����m��Ȃ���\n" +
                      "�u���ꏊ��������Ȃ�(-_-;)\n" +
                      "���������ɒu���Ă�����\n" +
                      "�����ꏊ�ɓ������u����\n" +
                      "���v����^^";
                break;
            case GimmickName.Type.PictureName:
                str = "�L���ɂ܂��J���Ă��Ȃ�\n" +
                      "����������ˁI\n" +
                      "�A���t�@�x�b�g���T����\n" +
                      "���͂���݂������ˁI\n" +
                      "�ǂ����ɏ����Ă��Ȃ����ȁH\n" +
                      "�Q���̕ǂɊG������Ă��āA\n" +
                      "�^�C�g�����T�������ˁI\n" +
                      "�����̎d�|����\n" +
                      "���͂��Ă݂悤�I�I";
                break;
            case GimmickName.Type.UseIronPipe:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.IronPipe), 0))
                {
                    str = "TV�̑O�ɂ���\�t�@�̉���\n" +
                          "���������Ă���݂����c�I\n" +
                          "���ׂďE���Ă݂悤�I�I";
                }
                else
                {
                    str = "�̔��̐�ɂ���\n" +
                          "�g�C���ɍs���Ă݂�ƁA\n" +
                          "�g�C���b�g�y�[�p�[��\n" +
                          "�������ɒu���Ă���ˁI\n" +
                          "�ł��A�肪�͂��Ȃ��݂����c\n" +
                          "�����Ă���S�p�C�v���g����\n" +
                          "�͂��񂶂�Ȃ����ȁH\n" +
                          "�A�C�e�����̓S�p�C�v��I���\n" +
                          "�������̃g�C���b�g�y�[�p�[��\n" +
                          "�g���Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.UseToiletPaper:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.ToiletPaper), 0))
                {
                    str = "�����Ƃ���ɂ�����\n" +
                          "�g�C���b�g�y�[�p�[��\n" +
                          "�����Ă����ˁI\n" +
                          "�E���Ă݂悤�I�I";
                }
                else
                {
                    str = "�g�C���ɂ͗m���̕֊킪����ˁI\n" +
                          "�ł��A�����Ȃ��݂����c\n" +
                          "�ƂĂ������Ă��܂���(-_-;)\n" +
                          "�g�C���b�g�y�[�p�[��\n" +
                          "�����Ă���̂ŁA\n" +
                          "�����[���Ă����悤�I�I";
                }
                break;
            case GimmickName.Type.RefrigeratorCans:
                str = "�g�C���̐�ɂ�\n" +
                      "�o�X���[��������ˁI\n" +
                      "�V���[�P�[�X�����邯�ǁA\n" +
                      "���͈Â��Č����Ȃ��݂����c\n" +
                      "�������ɂ���d�|�����������I\n" +
                      "�①�ɂ̒��ɂ���\n" +
                      "�ʂ̏ꏊ�Ɛ����q���g����I\n" +
                      "�Ή�����ʒu�Ɛ���\n" +
                      "�d�|���ɍ��킹�Ă݂悤�I�I";
                break;
            case GimmickName.Type.LockerAndRestroomTips:
                str = "�Q���̃��b�J�[��\n" +
                      "�g�C���̃J�o�[�ɓ\���Ă���\n" +
                      "�Q���̐}�ɒ��ڂ��悤�I\n" +
                      "���̒ʂ�ʒu��\n" +
                      "�ʂ����ʒu�̐��������킹��ƁA\n" +
                      "�Ȃ�ƂT���̐��l���\�ꂽ�ˁI\n" +
                      "�T���̐��l�Ɍ��o���͂��邩�ȁH\n" +
                      "�R���s���[�^�̉E���ɂ���\n" +
                      "�����o���̎d�|����\n" +
                      "�T���̐��l����͂��悤�I�I";
                break;
            case GimmickName.Type.UseAxe:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Axe), 0))
                {
                    str = "��̃I�u�W�F�Ɏw�ւ�Ƃ߂��\n" +
                          "�������̑傫�Ȕ����J�����ˁI\n" +
                          "���ɂ͉��������Ă���̂��ȁH\n" +
                          "���ׂďE���Ă݂悤�I�I";
                }
                else
                {
                    str = "�L���ɂ͂Ȃ�Ƃ��Ӗ��[��\n" +
                          "�ؔ����u���Ă���ˁI\n" +
                          "���ɂ͉��������Ă���̂��ȁH\n" +
                          "���ׂĂ݂������ǁA\n" +
                          "�J�����͖����݂����c\n" +
                          "�ǂ����悤�H\n" +
                          "�����͗͋Z���ˁI\n" +
                          "�����Ă��镀���g���āA\n" +
                          "�ؔ����Ɖ󂵂Ă��܂����I�I";
                }
                break;
            case GimmickName.Type.UnderThePillow:
                str = "�x�b�h�͒��ׂĂ݂����ȁH\n" +
                      "���̉��ɂ͉�������݂����I\n" +
                      "���ׂĂ݂悤�I�I";
                break;
            case GimmickName.Type.ThreeCardsTips:
                str = "[�x�b�h�̏�ɂ��閍�̉�]\n" +
                      "[�L���ɂ���󂵂��ؔ��̒�]\n" +
                      "[�o�X���[���̃V���[�P�[�X�̒�]\n" +
                      "�������R���̃J�[�h���q���g��\n" +
                      "�o�X�^�u�̃K���X�����J���悤�I\n" +
                      "�J�[�h�̂U�F�͎d�|���̂U�F��\n" +
                      "�Ή����Ă����I\n" +
                      "�d�|���̏�ɂ���}�̓q���g�ŁA\n" +
                      "�}���J�[�h�̋L���ʂ�ɂȂ����\n" +
                      "���ꂼ��P���̐�����������I\n" +
                      "�ƂĂ�������Ǌ撣���āI�I";
                break;
            case GimmickName.Type.UsePai_1:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_1), 0))
                {
                    str = "�R���s���[�^�̉E���ɂ���\n" +
                          "�T���̓�͂��łɉ������ˁI\n" +
                          "�����o���̒��ɂ�\n" +
                          "���������Ă���̂��ȁH\n" +
                          "���ׂďE���Ă݂悤�I�I";
                }
                else
                {
                    str = "���悢��Ō�̎d�|�����ˁI\n" +
                          "�L���̐�ɂ���o���̔��ɂ�\n" +
                          "�h�A�m�u�̉��ɑ���������I\n" +
                          "���ڂ݂��R���邯��\n" +
                          "������Ƃ߂��Ȃ����ȁH\n" +
                          "�����Ă��閃���v��\n" +
                          "�s�b�^�������H\n" +
                          "[����]�Ƃ�����Ă���v��\n" +
                          "�P�Ԃ̂��ڂ݂ɛƂ߂Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.UsePai_2:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_2), 0))
                {
                    str = "�����Ղɑ���Ȃ����\n" +
                          "�S�Č����Ēu�����ˁI\n" +
                          "�������ɂ���Ƌ�̈����o����\n" +
                          "�J�����݂�������I\n" +
                          "���������Ă���̂��ȁH\n" +
                          "���ׂďE���Ă݂悤�I�I";
                }
                else
                {
                    str = "���悢��Ō�̎d�|�����ˁI\n" +
                          "�L���̐�ɂ���o���̔��ɂ�\n" +
                          "�h�A�m�u�̉��ɑ���������I\n" +
                          "���ڂ݂��R���邯��\n" +
                          "������Ƃ߂��Ȃ����ȁH\n" +
                          "�����Ă��閃���v��\n" +
                          "�s�b�^�������H\n" +
                          "���ۂ��Q�`����Ă���v��\n" +
                          "�Q�Ԃ̂��ڂ݂ɛƂ߂Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.UsePai_3:
                if (0 == PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), Item.Type.Pai_3), 0))
                {
                    str = "�o�X�^�u�̎d�|����\n" +
                          "�����ɉ������Ƃ��ł����ˁI\n" +
                          "����������ȁH\n" +
                          "�o�X�^�u�̒��𒲂ׂ��\n" +
                          "�ǂ����r���a��\n" +
                          "���������������Ă���݂����I\n" +
                          "���ׂďE���Ă݂悤�I�I";
                }
                else
                {
                    str = "���悢��Ō�̎d�|�����ˁI\n" +
                          "�L���̐�ɂ���o���̔��ɂ�\n" +
                          "�h�A�m�u�̉��ɑ���������I\n" +
                          "���ڂ݂��R���邯��\n" +
                          "������Ƃ߂��Ȃ����ȁH\n" +
                          "�����Ă��閃���v��\n" +
                          "�s�b�^�������H\n" +
                          "�c�_���R�`����Ă���v��\n" +
                          "�R�Ԃ̂��ڂ݂ɛƂ߂Ă݂悤�I�I";
                }
                break;
            case GimmickName.Type.FinalDoor:
                str = "���悢��Ō�̎d�|�����ˁI\n" +
                      "�L���̐�ɂ���o���̔��ɂ�\n" +
                      "�h�A�m�u�̉��ɑ���������I\n" +
                      "���ڂ݂��R���邯��\n" +
                      "������Ƃ߂��Ȃ����ȁH\n" +
                      "�����Ă��閃���v��\n" +
                      "�s�b�^�������H\n" +
                      "�����Ă��閃���v��\n" +
                      "�R�̂��ڂ݂ɛƂ߂Ă݂悤�I�I";
                break;
            default:
                str = "�����[�h�G���[002\n" +
                      "�z��O�̐i�s�󋵂ł��B";
                break;
        }

        return str;
    }
}