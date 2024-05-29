using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.SceneManagement;
using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

//using PlayFab.CloudScriptModels;

public class PlayFabManager : MonoBehaviour
{
    /// <summary> �A�J�E���g���쐬���邩�̃t���O </summary>
    private bool _shouldCreateAccount;

    /// <summary> ���O�C�����Ɏg��ID </summary>
    private string _myCustomID;

    /// <summary> �v���C���[�v���t�B�[���̌Ăяo���ɕK�v��PlayFabId </summary>
    private string myPlayFabID;

    /// <summary> �ǂݍ��񂾃v���C���[�v���t�B�[�� </summary>
    private PlayerProfileModel myPlayerProfile;

    /// <summary> �t�����h�̃��X�g </summary>
    List<FriendInfo> _friends = null;

    /// <summary> ID��ۑ����鎞��KEY </summary>
    public static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    /// <summary> ID�Ɏg�p���镶�� </summary>
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    /// <summary> �Z�[�u�f�[�^��Playfab�ɕۑ�����L�[ </summary>
    //private const string SAVE_DATA_KEY = "save";

    private const string SCENE_NAME_TITLE = "Title";
    private const string SCENE_NAME_MENU = "Menu";
    private const string SCENE_NAME_GAME = "Game";

    private const string FRIEND_TAG_FOLLOW = "Follow";
    private const string FRIEND_TAG_FOLLOWER = "Follower";

    private GameObject friendPanelObject;
    private GameObject friendDetailObject;

    public List<CatalogItem> CatalogItems { get; private set; }
    public List<StoreItem> StoreItems { get; private set; }

    private DateTime backGroundBefore;
    private DateTime backGroundAfter;

    private const string CATALOG_VERSION_TEXT = "Catalog01";
    private const string GACHA_STORE_ID_TEXT = "GachaStore";
    private const string VIRTUAL_CURRENCY_TEXT = "SP";

    private const string KEY_CHARACTER_NAME = "CharactorName";
    private const string KEY_ICON_BACK_NAME = "IconBack";
    private const string KEY_ICON_FRAME_NAME = "IconFrame";
    private const string KEY_SAVE_DATA = "SaveData";

    private const int RANKING_ITEM_NUM = 10;

    private const int GACHA_1_PRICE = 10;
    private const int GACHA_11_PRICE = 100;

    private const int BACKGROUND_TIME_SECOND = 600;

    private int haveGachaTicket;

    /// <summary> ���z�ʉ݌������ʂ�Ԃ��I�u�W�F�N�g </summary>
    private GameObject memoryShopContentsPanelPrefab;

    /// <summary>
    /// �t�����h�̃^�O��
    /// </summary>
    public enum FriendTag
    {
        Follow,
        Follower,
    }

    public static PlayFabManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        backGroundBefore = DateTime.MinValue; 
        backGroundAfter = DateTime.MinValue;
    }

    // �A�v���P�[�V�������N�����Ă���� true�A�����łȂ���� false
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && backGroundBefore == DateTime.MinValue) return;

        CheckGoToBackground(pauseStatus);
    }

#region ���O�C������

    /// <summary>
    /// ���O�C�����s
    /// </summary>
    public void Login()
    {
        _myCustomID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _myCustomID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    /// <summary>
    /// ���O�C������
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result)
    {
        bool newPlayerFlg;

        //�A�J�E���g���쐬���悤�Ƃ����̂ɁAID�����Ɏg���Ă��āA�o���Ȃ������ꍇ
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_myCustomID} �͊��Ɏg���Ă��܂��B");
            Login();//���O�C�����Ȃ���
            return;
        }

        //�A�J�E���g�쐬����ID��ۑ�
        if (result.NewlyCreated)
        {
            SaveCustomID();
            newPlayerFlg = true;
        }
        else
        {
            newPlayerFlg = false;
        }

        Debug.Log($"PlayFabManager.OnLoginSuccess : PlayFab�̃��O�C���ɐ���\n" +
            $"PlayFabId = {result.PlayFabId} , CustomId = {_myCustomID} , �V�A�J�E���g�쐬 = {result.NewlyCreated}");

        myPlayFabID = result.PlayFabId;

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            TitleSceneManager.Instance.AfterLogIn(newPlayerFlg);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.AfterCheckPlayFabLogin(true);
        }

    }

    /// <summary>
    /// ���O�C�����s
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab�̃��O�C���Ɏ��s\n{error.GenerateErrorReport()}");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            TitleSceneManager.Instance.FailureLogIn();
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.AfterCheckPlayFabLogin(false);
        }
    }

#endregion

#region �������O�C��

    /// <summary>
    /// �����R�[�h�̃f�[�^�Ń��O�C������
    /// </summary>
    /// <param name="repairId"></param>
    public void RepairLogin(string repairId)
    {
        var request = new LoginWithCustomIDRequest { CustomId = repairId, CreateAccount = false };

        PlayFabClientAPI.LoginWithCustomID(request, OnRepairLoginSuccess, OnRepairLoginFailure);
    }

    /// <summary>
    /// �������O�C������
    /// </summary>
    /// <param name="result"></param>
    private void OnRepairLoginSuccess(LoginResult result)
    {
        Debug.Log("�������O�C�� : ����");

        TitleSceneManager.Instance.AfterRepairLogin();
        //GetUserSaveData();
    }

    /// <summary>
    /// �������O�C�����s
    /// </summary>
    /// <param name="result"></param>
    private void OnRepairLoginFailure(PlayFabError error)
    {
        Debug.Log("�������O�C�� : ���s");

        // �����ő��݂��Ȃ��ꍇ�̏��������s
        TitleSceneManager.Instance.RepairFailure();
    }

#endregion

#region �J�X�^��ID�̎擾

    /// <summary>
    /// ID���擾
    /// </summary>
    /// <returns></returns>
    private string LoadCustomID()
    {
        //ID���擾
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //�ۑ�����Ă��Ȃ���ΐV�K����
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
    }

    /// <summary>
    /// ID�̕ۑ�
    /// </summary>
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _myCustomID);
    }

    /// <summary>
    /// �������ɐV����ID�̕ۑ�
    /// </summary>
    public void SaveNewCustomID(string newID)
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, newID);
    }

#endregion

#region �J�X�^��ID�̐���

    /// <summary>
    /// ID�𐶐�����
    /// </summary>
    /// <returns></returns>
    private string GenerateCustomID()
    {
        int idLength = 32;//ID�̒���
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //�����_����ID�𐶐�
        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }

#endregion

#region ���[�U�[�f�[�^�擾

    /// <summary>
    /// ���[�U�[(�v���C���[)�f�[�^�̎擾
    /// </summary>
    public void GetUserSaveData()
    {
        //GetUserDataRequest�̃C���X�^���X�𐶐�
        var request = new GetUserDataRequest();

        //���[�U�[(�v���C���[)�f�[�^�̎擾
        PlayFabClientAPI.GetUserData(request, OnSuccessGettingPlayerData, OnErrorGettingPlayerData);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̎擾�J�n");
    }

#endregion

#region ���[�U�[�f�[�^�擾����

    /// <summary>
    /// ���[�U�[(�v���C���[)�f�[�^�̎擾�ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessGettingPlayerData(GetUserDataResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessGettingPlayerData : ���[�U�[(�v���C���[)�f�[�^�̎擾�ɐ������܂���");

        SaveData save = null;
        try
        {
            save = JsonUtility.FromJson<SaveData>(result.Data[KEY_SAVE_DATA].Value);
        }
        catch (Exception error)
        {
            Debug.LogWarning($"PlayFabManager.OnSuccessGettingPlayerData : save�̓ǂݍ��ݎ��s\n{error.GetBaseException()}");

            PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(null);
        }

        PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(save);
    }

    /// <summary>
    /// ���[�U�[(�v���C���[)�f�[�^�̎擾�Ɏ��s
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorGettingPlayerData(PlayFabError error)
    {
        Debug.LogError($"PlayFabManager.OnErrorGettingPlayerData : ���[�U�[(�v���C���[)�f�[�^�̎擾�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
        PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(null);
    }

#endregion

#region ���[�U�[�f�[�^�X�V

    /// <summary>
    /// �ݒ�A�C�R���f�[�^�ƃZ�[�u�f�[�^���X�V����
    /// </summary>
    public void UpdateUserDataForAll()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            OnErrorUpdatingPlayDataForNonLogIn();
            return;
        }

        string _CharactorName = PlayerInformationManager.Instance.GetPlayerCharacterName();
        string _IconBackName = PlayerInformationManager.Instance.GetSelectIconBackgroundName();
        string _IconFrameName = PlayerInformationManager.Instance.GetSelectIconFrameName();
        SaveData save = PlayerInformationManager.Instance.GetSaveData();

        //�X�V����f�[�^
        var updateDataDict = new Dictionary<string, string>()
        {
            { KEY_CHARACTER_NAME  , _CharactorName },
            { KEY_ICON_BACK_NAME  , _IconBackName  },
            { KEY_ICON_FRAME_NAME , _IconFrameName },
            { KEY_SAVE_DATA , PlayFabSimpleJson.SerializeObject(save) }
        };
        
        var request = new UpdateUserDataRequest
        {
            Data = updateDataDict,
            Permission = UserDataPermission.Public //�A�N�Z�X���ݒ�
        };

        //���[�U�[(�v���C���[)�f�[�^�̍X�V
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerDataForIcon, OnErrorUpdatingPlayerDataForIcon);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̍X�V�J�n");

    }

    /// <summary>
    /// �ݒ�A�C�R���f�[�^�̍X�V
    /// </summary>
    public void UpdateUserDataForIcon()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            OnErrorUpdatingPlayDataForNonLogIn();
            return;
        }

        string _CharactorName = PlayerInformationManager.Instance.GetPlayerCharacterName();
        string _IconBackName = PlayerInformationManager.Instance.GetSelectIconBackgroundName();
        string _IconFrameName = PlayerInformationManager.Instance.GetSelectIconFrameName();

        //�X�V����f�[�^
        var updateDataDict = new Dictionary<string, string>()
        {
            { KEY_CHARACTER_NAME  , _CharactorName },
            { KEY_ICON_BACK_NAME  , _IconBackName  },
            { KEY_ICON_FRAME_NAME , _IconFrameName },
        };

        //UpdateUserDataRequest�̃C���X�^���X�𐶐�
        var request = new UpdateUserDataRequest
        {
            Data = updateDataDict,
            Permission = UserDataPermission.Public //�A�N�Z�X���ݒ�
        };

        //���[�U�[(�v���C���[)�f�[�^�̍X�V
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerDataForIcon, OnErrorUpdatingPlayerDataForIcon);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̍X�V�J�n");

    }

    /// <summary>
    /// �Z�[�u�f�[�^�̍X�V
    /// </summary>
    public void UpdateUserDataForSave()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            OnErrorUpdatingPlayDataForNonLogIn();
            return;
        }

        SaveData save = PlayerInformationManager.Instance.GetSaveData();

        var saveData = new Dictionary<string, string>()
        {
            { KEY_SAVE_DATA , PlayFabSimpleJson.SerializeObject(save) }
        };

        //UpdateUserDataRequest�̃C���X�^���X�𐶐�
        var request = new UpdateUserDataRequest
        {
            Data = saveData,
            Permission = UserDataPermission.Public //�A�N�Z�X���ݒ�
        };

        //���[�U�[(�v���C���[)�f�[�^�̍X�V
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnErrorUpdatingPlayerData);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̍X�V�J�n");

    }

#endregion

#region ���[�U�[�f�[�^�X�V����

    /// <summary>
    /// �ݒ�A�C�R���f�[�^�̍X�V�ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessUpdatingPlayerDataForIcon(UpdateUserDataResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessUpdatingPlayerDataForIcon : Icon Update Success");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionUpdateIconSettingPlayFab(true);
        }
    }

    /// <summary>
    /// �ݒ�A�C�R���f�[�^�̍X�V�Ɏ��s
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorUpdatingPlayerDataForIcon(PlayFabError error)
    {
        Debug.LogError($"PlayFabManager.OnErrorUpdatingPlayerDataForIcon : Icon Update Error\n{error.GenerateErrorReport()}");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionUpdateIconSettingPlayFab(false);
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�̍X�V�ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessUpdatingPlayerData(UpdateUserDataResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessUpdatingPlayerData : Save Update Success");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            TitleSceneManager.Instance.AfterUpdateUserData();
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionUpdateSaveDataPlayFab(true);
        }
    }

    /// <summary>
    /// �Z�[�u�f�[�^�̍X�V�Ɏ��s
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorUpdatingPlayerData(PlayFabError error)
    {
        Debug.LogError($"PlayFabManager.OnErrorUpdatingPlayerData : Save Update Error\n{error.GenerateErrorReport()}");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {

        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionUpdateSaveDataPlayFab(false);
        }
    }

    /// <summary>
    /// ���O�C�����Ă��Ȃ��Ƃ��ɍX�V��������ꂽ�Ƃ�
    /// </summary>
    private void OnErrorUpdatingPlayDataForNonLogIn()
    {
        Debug.LogWarning($"���O�C�����Ă��Ȃ����߁A���[�U�[(�v���C���[)�f�[�^�̍X�V�Ɏ��s���܂���");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            //Menu.MenuSceneManager.Instance.ReceptionUpdatePlayFab(false);
        }
    }

#endregion

#region ���[�U�[�f�[�^�폜

    /// <summary>
    /// ���[�U�[�f�[�^�폜
    /// </summary>
    public void DeletePlayerData()
    {
        var request = new PlayFab.AdminModels.DeleteMasterPlayerAccountRequest { PlayFabId = myPlayFabID };

        PlayFabAdminAPI.DeleteMasterPlayerAccount(request, OnDeleteSuccess, OnDeleteFailure);
    }

    /// <summary>
    /// ���[�U�[�f�[�^�폜�̐������ɌĂ�
    /// </summary>
    /// <param name="result"></param>
    private void OnDeleteSuccess(PlayFab.AdminModels.DeleteMasterPlayerAccountResult result)
    {
        Debug.Log("���[�U�[�f�[�^�̍폜�ɐ������܂����B");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionDeletePlayFab(true);
        }
    }

    /// <summary>
    /// ���[�U�[�f�[�^�폜�̎��s���ɌĂ�
    /// </summary>
    /// <param name="error"></param>
    private void OnDeleteFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionDeletePlayFab(false);
        }
    }

#endregion

#region �v���C���[�v���t�B�[���̌Ăяo��

    /// <summary>
    /// ���g�̃v���C���[�v���t�B�[���̌Ăяo��(���g�p)
    /// </summary>
    private void GetMyPlayerProfile()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = myPlayFabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                // ���̑��̏����J������ɂ́A�u���E�U����^�C�g���̐ݒ�ŃN���C�A���g�v���t�B�[���I�v�V������ύX����
                ShowDisplayName = true,
                ShowLastLogin = true
            }
        },
        result =>
        {
            myPlayerProfile = result.PlayerProfile;

            Debug.Log($"PlayFabManager.GetPlayerProfile : {myPlayerProfile.DisplayName} ,{myPlayerProfile.LastLogin}");
        },
        error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }


    /// <summary>
    /// �N���v���C���[�v���t�B�[���̌Ăяo��
    /// </summary>
    /// <param name="playFabId">�Ăяo��ID</param>
    public void GetOtherPlayerProfile(string playFabId, FriendInfo friendInfo)
    {
        if (playFabId == myPlayFabID)
        {
            GetOtherPlayerProfileFailure("����͎�����ID�ł�");
            return;
        }

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                // ���̑��̏����J������ɂ́A�u���E�U����^�C�g���̐ݒ�ŃN���C�A���g�v���t�B�[���I�v�V������ύX����
                ShowDisplayName = true,
                ShowLastLogin = true
            }
        },
        result =>
        {
            GetOtherUserData(playFabId, (DateTime)result.PlayerProfile.LastLogin, friendInfo);

            //Debug.Log($"PlayFabManager.GetOtherPlayerProfile : Success");
        },
        error =>
        {
            GetOtherPlayerProfileFailure("���[�U�[�f�[�^��\n������܂���ł���");
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �N���̃Z�[�u�f�[�^�̌Ăяo��
    /// </summary>
    /// <param name="playFabId"></param>
    /// <param name="lastLogin"></param>
    private void GetOtherUserData(string playFabId, DateTime lastLogin, FriendInfo friendInfo)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = null
        },
        result =>
        {
            //SaveData save = JsonUtility.FromJson<SaveData>(result.Data[SAVE_DATA_KEY].Value);

            if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
            {
                if (friendPanelObject == null)
                {
                    friendPanelObject = GameObject.Find("FriendPanel");
                }

                //Menu.FriendPanel friendPanel = friendPanelObject.GetComponent<Menu.FriendPanel>();

                //friendPanel.ReceptionSearchIdSuccess(save, playFabId, lastLogin, friendInfo);
            }
            //Debug.Log($"PlayFabManager.GetOtherUserData : Success");
        },
        error =>
        {
            GetOtherPlayerProfileFailure("�Z�[�u�f�[�^��\n�ǂݍ��߂܂���ł���");
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �N���̌Ăяo�������s�����Ƃ�
    /// </summary>
    /// <param name="str"></param>
    private void GetOtherPlayerProfileFailure(string str)
    {
        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            if (friendPanelObject == null)
            {
                friendPanelObject = GameObject.Find("FriendPanel");
            }

            //Menu.FriendPanel friendPanel = friendPanelObject.GetComponent<Menu.FriendPanel>();

            //friendPanel.ReceptionSearchIdFailure(str);
        }
    }

#endregion

#region �t�����h�@�\

    /// <summary>
    /// �t�����h����S�Ď擾����
    /// </summary>
    public void GetFriends(FriendTag tag)
    {
        _friends = null;

        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            //IncludeSteamFriends = false,
            //IncludeFacebookFriends = false,
            XboxToken = null
        },
        result =>
        {
            _friends = result.Friends;
            DisplayFriends(_friends, tag);
        },
        error =>
        {
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// �w�肵���^�O�̃t�����h���X�g��S�ĕ\������
    /// </summary>
    /// <param name="friendsCache"></param>
    /// <param name="tag"></param>
    private void DisplayFriends(List<FriendInfo> friendsCache, FriendTag tag)
    {
        int counter = 0;
        foreach (FriendInfo friend in friendsCache)
        {
            foreach (var item in friend.Tags)
            {
                if (item == Enum.GetName(typeof(FriendTag), tag))
                {
                    counter++;
                    GetOtherPlayerProfile(friend.FriendPlayFabId, friend);
                }
            }
        }

        if (counter == 0)
        {
            GetOtherPlayerProfileFailure("�t�H���[���Ă��郆�[�U��\n�ЂƂ�����܂���");
        }
    }

    /// <summary>
    /// �w�肵�����[�U�[���t�����h�ɂ���
    /// </summary>
    /// <param name="friendInfo"></param>
    /// <param name="friendId"></param>
    public void AddFriend(string friendId)
    {
        var request = new AddFriendRequest();

        request.FriendPlayFabId = friendId;

        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(
        request,
        result =>
        {
            SetFriendTags(friendId, FRIEND_TAG_FOLLOW);

            // Debug.Log("PlayFabManager.AddFriend : successfully!");
        },
        error =>
        {
            DisplayFriendDialogText("���̃��[�U��\n���łɃt�����h�ł�");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// �w�肵���t�����h���폜����
    /// </summary>
    /// <param name="friendInfo"></param>
    public void RemoveFriend(FriendInfo friendInfo)
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = friendInfo.FriendPlayFabId
        },
        result =>
        {
            _friends.Remove(friendInfo);

            DisplayFriendDialogText("���̃��[�U��\n�t�����h����\n�폜���܂���");
        },
        error =>
        {
            DisplayFriendDialogText("���̃��[�U��\n���łɃt�����h�ł�\n����܂���");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// �w�肵���t�����h�Ƀ^�O��ǉ�����
    /// </summary>
    /// <param name="friend"></param>
    /// <param name="newTag"></param>
    public void SetFriendTags(string friendPlayFabId, string newTag)
    {
        // update the tags with the edited list
        PlayFabClientAPI.SetFriendTags(new SetFriendTagsRequest
        {
            FriendPlayFabId = friendPlayFabId,
            Tags = new List<string> { newTag },
        },
        tagresult =>
        {
            // Make sure to save new tags locally. That way you do not have to hard-update friendlist
            // friend.Tags.Add(newTag);
            DisplayFriendDialogText("���̃��[�U��\n�t�����h��\n�ǉ����܂���");
        },
        error =>
        {
            DisplayFriendDialogText("���̃��[�U��\n�^�O�ҏW��\n���s���܂���");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// �G���[�������Ƀ��O��\������
    /// </summary>
    /// <param name="error"></param>
    private void DisplayPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    /// <summary>
    /// FriendDetail�̃_�C�A���O�Ƀe�L�X�g��\������
    /// </summary>
    /// <param name="str"></param>
    private void DisplayFriendDialogText(string str)
    {
        if (SceneManager.GetActiveScene().name != SCENE_NAME_MENU) return;

        if (friendDetailObject == null)
        {
            friendDetailObject = GameObject.Find("FriendDetail");
        }

        //Menu.FriendDetail friendDetail = friendDetailObject.GetComponent<Menu.FriendDetail>();

        //friendDetail.ActiveDialogPanel(str);
    }

#endregion

#region �\�����̍X�V

    /// <summary>
    /// �V�����\�����ɍX�V����
    /// </summary>
    /// <param name="displayName"></param>
    public void SetPlayerDisplayName(string displayName)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = displayName
            },
            result =>
            {
                Debug.Log("�\�����̍X�V����");
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// ���͂��ꂽ���O���L�����ǂ������`�F�b�N����
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public bool IsValidName(TMP_InputField inputName)
    {
        // �\�����́A�R�����ȏ�Q�T�����ȉ�
        // �����3�����ȏ�12�����ȓ��ɐݒ�
        string playerName = inputName.text;
        bool isValidName = !string.IsNullOrWhiteSpace(playerName);
        bool isNameLengthValid = playerName.Length >= 3 && playerName.Length <= 12;

        return isValidName && isNameLengthValid;
    }

#endregion

#region �X�g�A

    /// <summary>
    /// �J�^���O�f�[�^�̎擾(�K�`��)
    /// </summary>
    public void GetGachaCatalogData()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = CATALOG_VERSION_TEXT,
        }
        , result =>
        {
            Debug.Log($"PlayFabManager.GetGachaCatalogData : Success {CATALOG_VERSION_TEXT}");
            CatalogItems = result.Catalog;

            GetGachaStoreData();
        }
        , error =>
        {
            Debug.Log("PlayFabManager.GetGachaCatalogData : " + error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �X�g�A�f�[�^�̎擾(�K�`��)
    /// </summary>
    /// <param name="storeId"></param>
    public void GetGachaStoreData()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest()
        {
            CatalogVersion = CATALOG_VERSION_TEXT,
            StoreId = GACHA_STORE_ID_TEXT
        }
        , (result) =>
        {
            Debug.Log($"PlayFabManager.GetGachaStoreData : success {GACHA_STORE_ID_TEXT}");
            StoreItems = result.Store;
        }
        , (error) =>
        {
            Debug.Log("PlayFabManager.GetGachaStoreData : " + error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �K�`���̉��i��Ԃ��Q�b�^�[
    /// </summary>
    /// <param name="gachaTimes">�K�`����(1or11)</param>
    /// <returns></returns>
    public int GetGachaPrice(int gachaTimes)
    {
        int price;

        switch (gachaTimes)
        {
            case 1:
                price = GACHA_1_PRICE;
                break;
            case 11:
                price = GACHA_11_PRICE;
                break;
            default:
                price = 0;
                Debug.LogError($"PlayFabManager.GetGachaPrice : Gacha Times Error -> {gachaTimes}");
                break;
        }

        return price;
    }

    /// <summary>
    /// �K�`���̍w��
    /// </summary>
    /// <param name="gachaTimes">�K�`����(1or11)</param>
    public void PurchaseGacha(int gachaTimes)
    {
        string itemId;
        int price;

        switch (gachaTimes)
        {
            case 1:
                itemId = "Gacha1";
                price = GACHA_1_PRICE;
                break;
            case 11:
                itemId = "Gacha11";
                price = GACHA_11_PRICE;
                break;
            default:
                itemId = "Error";
                price = 0;
                break;
        }

        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
        {
            CatalogVersion = CATALOG_VERSION_TEXT,
            StoreId = GACHA_STORE_ID_TEXT,
            ItemId = itemId,
            VirtualCurrency = VIRTUAL_CURRENCY_TEXT,
            Price = price
            // �L�����N�^�[���g���ꍇ�� CharacterId �̃Z�b�g���K�v
        },
        result =>
        {
            Debug.Log($"PlayFabManager.PurchaseItem : {result.Items[0].DisplayName}�w�������I" +
                $"\n{PlayFabSimpleJson.SerializeObject(result.Items)}");

            List<string> itemNames = new List<string>();
            foreach (var item in result.Items)
            {
                if (item.ItemId != itemId)
                {
                    itemNames.Add(item.ItemId);
                }
            }

            foreach (var item in itemNames) //�K�`���̌��ʂ����O�o��
            {
                Debug.Log($"name : {item}");
            }

            MenuSceneManager.Instance.ReceptionGachaResult(itemNames);

            UpdateHaveGachaTicketNum();
        },
        error =>
        {
            // ���z�s��
            if (error.Error == PlayFabErrorCode.InsufficientFunds)
            {
                MenuSceneManager.Instance.ReceptionGachaFailure("SP�R�C����\n�s�����Ă��܂��B");
            }
            else
            {
                MenuSceneManager.Instance.ReceptionGachaFailure("�G���[���������܂����B");
            }

            Debug.LogError("PlayFabManager.PurchaseGacha : " + error.GenerateErrorReport());

        });
    }

    /// <summary>
    /// ���z�ʉݖ����̏��X�V��v��
    /// </summary>
    public void UpdateHaveGachaTicketNum()
    {
        //GetUserInventoryRequest�̃C���X�^���X�𐶐�
        var userInventoryRequest = new GetUserInventoryRequest();

        //�C���x���g���̏��̎擾
        PlayFabClientAPI.GetUserInventory(userInventoryRequest, OnSuccessUpdateHaveGachaTicketNum, OnErrorUpdateHaveGachaTicketNum);
    }

    /// <summary>
    /// ���z�ʉݖ����̏��̎擾�ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessUpdateHaveGachaTicketNum(GetUserInventoryResult result)
    {
        haveGachaTicket = result.VirtualCurrency[VIRTUAL_CURRENCY_TEXT];
    }

    /// <summary>
    /// ���z�ʉݖ����̏��̎擾�Ɏ��s
    /// </summary>
    /// <param name="result"></param>
    private void OnErrorUpdateHaveGachaTicketNum(PlayFabError error)
    {
        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.SendInformationText("SP�R�C���̏���������\n�擾�ł��܂���ł����B");
        }

        Debug.LogError("PlayFabManager.OnErrorUpdateHaveGachaTicketNum : " + error.GenerateErrorReport());
    }

    /// <summary>
    /// ���z�ʉݖ�����Ԃ�(��񂪌Â��ꍇ�A��)
    /// </summary>
    /// <returns></returns>
    public int GetHaveGachaTicket()
    {
        return haveGachaTicket;
    }

    /// <summary>
    /// ���z�ʉݒǉ��̐\��
    /// </summary>
    /// <param name="num">�ǉ����閇��</param>
    public void AddGachaTicket(int num)
    {
        var addUserVirtualCurrencyRequest = new AddUserVirtualCurrencyRequest
        {
            Amount = num,   //�ǉ�������z
            VirtualCurrency = VIRTUAL_CURRENCY_TEXT, //���z�ʉ݂̃R�[�h
        };

        PlayFabClientAPI.AddUserVirtualCurrency(addUserVirtualCurrencyRequest, OnSuccessAddGachaTicket, OnErrorAddGachaTicket);
    }

    /// <summary>
    /// ���z�ʉ݂̒ǉ��ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessAddGachaTicket(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessAddGachaTicket : ���z�ʉ݂̒ǉ��ɐ���\n" +
            $"�ύX�������z�ʉ݂̃R�[�h = {result.VirtualCurrency}\n" +
            $"�ύX��̎c�� = {result.Balance}\n" +
            $"���Z�z = {result.BalanceChange}\n");

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionAddSpCoin(false ,result.BalanceChange);
        }
    }

    /// <summary>
    /// ���z�ʉ݂̒ǉ��Ɏ��s
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorAddGachaTicket(PlayFabError error)
    {
        Debug.LogError($"OnErrorAddGachaTicket : ���z�ʉ݂̒ǉ��Ɏ��s\n{error.GenerateErrorReport()}");

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionAddSpCoin(true, 0);
        }
    }

#endregion

#region �����L���O

    /// <summary>
    /// �����L���O�ɒl���o���A�\���p�̃X�R�A��Ԃ�
    /// </summary>
    /// <param name="rankingName"></param>
    /// <param name="time"></param>
    /// <param name="missCount"></param>
    /// <returns></returns>
    public float RankingSubmitScore(string rankingName, float time, int missCount)
    {
        int rankingScore = ConversionTimeAndMissCountToRankingScore(time, missCount);
        float displayScore = ConversionRankingScoreToDisplayScore(rankingScore);

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = rankingName,
                    Value = rankingScore
                }
            }
        }, result =>
        {
            Debug.Log($"PlayFabManager.RankingSubmitScore : Ranking Send Success !\n" +
                $"Key = {rankingName} , RankingScore = {rankingScore} , DisplayScore = {displayScore}");
        }, error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });

        return displayScore;
    }

    /// <summary>
    /// ���ʏ�ʂ̃����L���O(���[�_�[�{�[�h)���擾����
    /// </summary>
    /// <param name="rankingName"></param>
    public void RankingGetLeaderboardAroundTop(string rankingName, ShowRankingPanelPrefab showRankingPanelPrefab)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = rankingName, //�����L���O��(���v���)
            MaxResultsCount = RANKING_ITEM_NUM //�����擾���邩
        };


        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            int counter = 0;
            foreach (var item in result.Leaderboard)
            {
                counter++;

                float displayScoreTime = ConversionRankingScoreToDisplayScore(item.StatValue);
                int rankPosition = item.Position + 1;
                string playerName = item.DisplayName;
                string playerId = item.PlayFabId;

                Debug.Log($"{rankPosition}�� : {playerName} ({playerId}) : �X�R�A {displayScoreTime}");
                showRankingPanelPrefab.ReceptionLeaderboardAroundTop(rankPosition, playerName, playerId, displayScoreTime);
            }
            if (counter == 0)
            {
                showRankingPanelPrefab.ReceptionFailureFromLeaderboardAroundTop("�܂��L�^������܂���");
                Debug.Log($"RankingGetLeaderboardAroundTop : Ranking Data is Nothing !");
            }
        }, error =>
        {
            showRankingPanelPrefab.ReceptionFailureFromLeaderboardAroundTop("�G���[���������܂���");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �����̏��ʎ��ӂ̃����L���O(���[�_�[�{�[�h)���擾����
    /// </summary>
    /// <param name="rankingName"></param>
    public void RankingGetLeaderboardAroundPlayer(string rankingName, ShowRankingPanelPrefab showRankingPanelPrefab)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = rankingName, //�����L���O��(���v���)
            MaxResultsCount = RANKING_ITEM_NUM //�������܂ߑO�㉽���擾���邩
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            int counter = 0;
            bool noMyDataFlg = false;
            foreach (var item in result.Leaderboard)
            {
                counter++;

                float displayScoreTime = ConversionRankingScoreToDisplayScore(item.StatValue);
                int rankPosition = item.Position + 1;
                string playerName = item.DisplayName;
                string playerId = item.PlayFabId;

                if (playerId == myPlayFabID && displayScoreTime == 0f)
                {
                    noMyDataFlg = true;
                    break;
                }

                Debug.Log($"{rankPosition}�� : {playerName} ({playerId}) : �X�R�A {displayScoreTime}");
                showRankingPanelPrefab.ReceptionLeaderboardAroundPlayer(rankPosition, playerName, playerId, displayScoreTime);
            }
            if (noMyDataFlg)
            {
                Debug.Log($"RankingGetLeaderboardAroundPlayer : My Data is Nothing !");
            }
            else if(counter == 0)
            {
                Debug.Log($"RankingGetLeaderboardAroundPlayer : Ranking Data is Nothing !");
            }
            else
            {
                showRankingPanelPrefab.ReceptionSuccessFromLeaderboardAroundPlayer();
            }
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �^�C���ƃ~�X�񐔂������L���O�ɑ���X�R�A�ɕϊ�����
    /// </summary>
    /// <param name="time"></param>
    /// <param name="missCount"></param>
    /// <returns></returns>
    private int ConversionTimeAndMissCountToRankingScore(float time,int missCount)
    {
        int score = missCount * 100;
        float hoge = time * 100f;
        score += (int)hoge;

        return -score;
    }

    /// <summary>
    /// �����L���O�ɑ���ꂽ�X�R�A�������ڏ�̃X�R�A�ɕς���
    /// </summary>
    /// <param name="rankingScore"></param>
    /// <returns></returns>
    private float ConversionRankingScoreToDisplayScore(int rankingScore)
    {
        float displayScore = -1f * (float)rankingScore / 100f;

        return displayScore;
    }

    /// <summary>
    /// �����L���O�̐l�̃A�C�R���f�[�^����M����
    /// </summary>
    /// <param name="playFabId"></param>
    /// <param name="accessRankingCardPrefab"></param>
    public void GetRankingPlayerData(string playFabId , RankingCardPrefab accessRankingCardPrefab)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playFabId,
            Keys = null
        },
        result =>
        {
            string _CharaName = result.Data[KEY_CHARACTER_NAME].Value;
            string _BackName = result.Data[KEY_ICON_BACK_NAME].Value;
            string _FrameName = result.Data[KEY_ICON_FRAME_NAME].Value;

            accessRankingCardPrefab.ReceptionSuccessFromPlayFab(_BackName, _FrameName, _CharaName);
        },
        error =>
        {
            accessRankingCardPrefab.ReceptionFailureFromPlayFab();
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    #endregion

#region �A�C�e��

    /// <summary>
    /// ������̃A�C�e�����������Ă��邩�`�F�b�N���Č��ʂ�Ԃ�
    /// </summary>
    public void CheckHaveItemNoAdMob(string _itemId)
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result =>
        {
            OnSuccessGetUserInventory(result, _itemId);
        }, error =>
        {
            OnErrorGetUserInventory(error, _itemId);
        });
    }

    /// <summary>
    /// �A�C�e���C���x���g���ɃA�N�Z�X���������Ƃ�
    /// </summary>
    /// <param name="result"></param>
    /// <param name="_itemId"></param>
    private void OnSuccessGetUserInventory(GetUserInventoryResult result , string _itemId)
    {
        bool haveFlg = false;

        foreach (ItemInstance item in result.Inventory)
        {
            if (item.ItemId == _itemId)
            {
                Debug.Log($"PlayFabManager.CheckHaveItemNoAdMob : Hit Item {_itemId} !!");
                haveFlg = true;
                break;
            }
        }

        Debug.Log($"PlayFabManager.OnSuccessGetUserInventory : Have Item �u{_itemId}�v = {haveFlg}");

        if (_itemId == "NoAdMob")
        {
            AdMobManager.Instance.ReceptionPlayFabForItem(haveFlg);
        }
    }

    /// <summary>
    /// �A�C�e���C���x���g���ɃA�N�Z�X���s�����Ƃ�
    /// </summary>
    /// <param name="error"></param>
    /// <param name="_itemId"></param>
    private void OnErrorGetUserInventory(PlayFabError error, string _itemId)
    {
        Debug.LogError($"PlayFabManager.OnErrorGetUserInventory : �A�C�e���C���x���g���̎擾�Ɏ��s���܂��� {_itemId}\n{error.GenerateErrorReport()}");

        if (_itemId == "NoAdMob")
        {
            AdMobManager.Instance.ReceptionPlayFabForItem(false);
        }
    }

    #endregion

    /// <summary>
    /// �o�b�N�O���E���h�̈ڍs�E���A���`�F�b�N����
    /// </summary>
    /// <param name="flg">�o�b�N�O���E���h�ɍs���� : true , �o�b�N�O���E���h���畜�A���� : false</param>
    private void CheckGoToBackground(bool flg)
    {
        if (flg)
        {
            Debug.Log("PlayFabManager.CheckGoToBackground : Go To Background");
            backGroundBefore = DateTime.Now;
        }
        else
        {
            Debug.Log("PlayFabManager.CheckGoToBackground : Back From Background");
            backGroundAfter = DateTime.Now;

            TimeSpan difference = backGroundAfter - backGroundBefore;
            Debug.Log($"PlayFabManager.CheckGoToBackground : Time (s) {difference.TotalSeconds}");

            if (difference.TotalSeconds >= BACKGROUND_TIME_SECOND)
            {
                Debug.Log("PlayFabManager.CheckGoToBackground : Long Time In Background!");
                if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
                {
                    //MenuSceneManager.Instance.ReceptionBackgroundLongerFromPlayFab();
                    Login();
                }
                else if (SceneManager.GetActiveScene().name == SCENE_NAME_GAME)
                {
                    GameSceneManager.Instance.ReceptionLongTimeBackGroundFromPlayFabManager();
                }
            }
        }
    }

    /// <summary>
    /// CustomId�̃Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetCustomId()
    {
        return _myCustomID;
    }

    /// <summary>
    /// PlayFabID�̃Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public string GetPlayFabID()
    {
        return myPlayFabID;
    }

    /// <summary>
    /// ���O�C�����Ă��邩�ǂ�����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetIsLogined()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }



}
