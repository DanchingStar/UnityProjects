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
    private const string SAVE_DATA_KEY = "save";

    private const string SCENE_NAME_TITLE = "Title";
    private const string SCENE_NAME_MENU = "Menu";

    private const string FRIEND_TAG_FOLLOW = "Follow";
    private const string FRIEND_TAG_FOLLOWER = "Follower";

    private GameObject friendPanelObject;
    private GameObject friendDetailObject;

    public List<CatalogItem> CatalogItems { get; private set; }
    public List<StoreItem> StoreItems { get; private set; }

    private const string CATALOG_VERSION_TEXT = "�K�`��";
    private const string GACHA_STORE_ID_TEXT = "StoreGacha";
    private const string VIRTUAL_CURRENCY_TEXT = "GC";

    private int haveGachaTicket;

    /// <summary> �K�`���`�P�������ʂ�Ԃ��I�u�W�F�N�g </summary>
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
        Debug.Log($"PlayFab�̃��O�C���ɐ���\nPlayFabId : {result.PlayFabId}, CustomId : {_myCustomID}\n�A�J�E���g���쐬������ : {result.NewlyCreated}");

        myPlayFabID = result.PlayFabId;

        //GetPlayerProfile();

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            Title.TitleManager.Instance.AfterLogIn(newPlayerFlg);
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Menu.MenuSceneManager.Instance.AfterCheckPlayFabLogin(true);
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
            Title.TitleManager.Instance.FailureLogIn();
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Menu.MenuSceneManager.Instance.AfterCheckPlayFabLogin(false);
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

        Title.TitleManager.Instance.AfterRepairLogin();
        GetUserData();
    }

    /// <summary>
    /// �������O�C�����s
    /// </summary>
    /// <param name="result"></param>
    private void OnRepairLoginFailure(PlayFabError error)
    {
        Debug.Log("�������O�C�� : ���s");

        // �����ő��݂��Ȃ��ꍇ�̏��������s
        Title.TitleManager.Instance.RepairFailure();
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
    public void GetUserData()
    {
        //GetUserDataRequest�̃C���X�^���X�𐶐�
        var request = new GetUserDataRequest();

        //���[�U�[(�v���C���[)�f�[�^�̎擾
        PlayFabClientAPI.GetUserData(request, OnSuccessGettingPlayerData, OnErrorGettingPlayerData);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̎擾�J�n");
    }

#endregion

#region ���[�U�[�f�[�^�擾����

    //���[�U�[(�v���C���[)�f�[�^�̎擾�ɐ���
    private void OnSuccessGettingPlayerData(GetUserDataResult result)
    {
        Debug.Log($"���[�U�[(�v���C���[)�f�[�^�̎擾�ɐ������܂���");

        SaveData save = JsonUtility.FromJson<SaveData>(result.Data[SAVE_DATA_KEY].Value);

        Title.TitleManager.Instance.AfterRepairSaveData(save);
    }

    //���[�U�[(�v���C���[)�f�[�^�̎擾�Ɏ��s
    private void OnErrorGettingPlayerData(PlayFabError error)
    {
        Debug.LogWarning($"���[�U�[(�v���C���[)�f�[�^�̎擾�Ɏ��s���܂��� : {error.GenerateErrorReport()}");
    }

#endregion

#region ���[�U�[�f�[�^�X�V

    /// <summary>
    /// ���[�U�[(�v���C���[)�f�[�^�̍X�V
    /// </summary>
    public void UpdateUserData()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            OnErrorUpdatingPlayDataForNonLogIn();
            return;
        }

        ////�X�V����f�[�^
        //var updateDataDict = new Dictionary<string, string>()
        //{
        //    {"Name"  , "Kan"},
        //    {"LV"    , "58"},
        //    {"SageNo", "39"},
        //};
        ////�폜����L�[
        //List<string> removeKeyList = new List<string>() 
        //{
        //  "Key"
        //};

        SaveData save = PlayerInformationManager.Instance.GetSaveData();

        var saveData = new Dictionary<string, string>()
        {
            { SAVE_DATA_KEY , PlayFabSimpleJson.SerializeObject(save) }
        };

        //UpdateUserDataRequest�̃C���X�^���X�𐶐�
        var request = new UpdateUserDataRequest
        {
            //Data = updateDataDict,
            //KeysToRemove = removeKeyList,
            Data = saveData,
            Permission = UserDataPermission.Public //�A�N�Z�X���ݒ�
        };

        //���[�U�[(�v���C���[)�f�[�^�̍X�V
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnErrorUpdatingPlayerData);
        Debug.Log($"�v���C���[(���[�U�[)�f�[�^�̍X�V�J�n");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            SetPlayerDisplayName();
        }
    }

#endregion

#region ���[�U�[�f�[�^�X�V����

    //���[�U�[(�v���C���[)�f�[�^�̍X�V�ɐ���
    private void OnSuccessUpdatingPlayerData(UpdateUserDataResult result)
    {
        Debug.Log($"���[�U�[(�v���C���[)�f�[�^�̍X�V�ɐ������܂���");

        //result.ToJson��json�Ō`���Ō��ʂ��m�F�\(result.Data�͂Ȃ�)

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {
            Title.TitleManager.Instance.AfterUpdateUserData();
        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Menu.MenuSceneManager.Instance.ReceptionUpdatePlayFab(true);
        }
    }

    //���[�U�[(�v���C���[)�f�[�^�̍X�V�Ɏ��s
    private void OnErrorUpdatingPlayerData(PlayFabError error)
    {
        Debug.LogWarning($"���[�U�[(�v���C���[)�f�[�^�̍X�V�Ɏ��s���܂��� : {error.GenerateErrorReport()}");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_TITLE)
        {

        }
        else if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Menu.MenuSceneManager.Instance.ReceptionUpdatePlayFab(false);
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
            Menu.MenuSceneManager.Instance.ReceptionUpdatePlayFab(false);
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
            Menu.MenuSceneManager.Instance.ReceptionDeletePlayFab(true);
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
            Menu.MenuSceneManager.Instance.ReceptionDeletePlayFab(false);
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
                ShowDisplayName = true ,
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
            GetOtherUserData(playFabId, (DateTime)result.PlayerProfile.LastLogin , friendInfo);

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
            SaveData save = JsonUtility.FromJson<SaveData>(result.Data[SAVE_DATA_KEY].Value);

            if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
            {
                if (friendPanelObject == null)
                {
                    friendPanelObject = GameObject.Find("FriendPanel");
                }

                Menu.FriendPanel friendPanel = friendPanelObject.GetComponent<Menu.FriendPanel>();

                friendPanel.ReceptionSearchIdSuccess(save, playFabId, lastLogin, friendInfo);
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

            Menu.FriendPanel friendPanel = friendPanelObject.GetComponent<Menu.FriendPanel>();

            friendPanel.ReceptionSearchIdFailure(str);
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
            DisplayFriends(_friends,tag);
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
                if (item == Enum.GetName(typeof(FriendTag),tag))
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

        Menu.FriendDetail friendDetail = friendDetailObject.GetComponent<Menu.FriendDetail>();

        friendDetail.ActiveDialogPanel(str);
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
    /// Json����\�������m�F���āA�K�v�Ȃ�X�V����
    /// </summary>
    public void SetPlayerDisplayName()
    {
        string str = PlayerInformationManager.Instance.GetPlayerName();

        //if (str == playerProfile.DisplayName) return;

        PlayFabClientAPI.UpdateUserTitleDisplayName(
           new UpdateUserTitleDisplayNameRequest
           {
               DisplayName = str
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
            Debug.Log($"PlayFabManager.GetCatalogData : success {CATALOG_VERSION_TEXT}");
            CatalogItems = result.Catalog;

            GetGachaStoreData();
        }
        , error =>
        {
            Debug.Log(error.GenerateErrorReport());
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
            Debug.Log($"PlayFabManager.GetStoreData : success {GACHA_STORE_ID_TEXT}");
            StoreItems = result.Store;
        }
        , (error) =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// �K�`���̍w��
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="price"></param>
    public void PurchaseGacha(string itemId, int price)
    {
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
            foreach ( var item in result.Items)
            {
                //Debug.Log($"{item.ItemId}");

                if (item.ItemId != itemId)
                {
                    itemNames.Add(item.ItemId);
                }
            }

            foreach ( var item in itemNames) //�K�`���̌��ʂ����O�o��
            {
                Debug.Log($"name : {item}");
            }

            Menu.MenuSceneManager.Instance.ReceptionGachaResult(itemNames);

            UpdateHaveGachaTicketNum();
        },
        error =>
        {
            // ���z�s��
            if (error.Error == PlayFabErrorCode.InsufficientFunds)
            {
                Menu.MenuSceneManager.Instance.ReceptionGachaFailure("�K�`���`�P�b�g��\n�s�����Ă��܂��B");
                Debug.Log($"PlayFabManager.PurchaseItem : ���z�s��");
            }
            else
            {
                Menu.MenuSceneManager.Instance.ReceptionGachaFailure("�G���[���������܂����B");
            }

            Debug.Log(error.GenerateErrorReport());

        });
    }

    /// <summary>
    /// �K�`���`�P�����̏��X�V��v��
    /// </summary>
    public void UpdateHaveGachaTicketNum()
    {
        //GetUserInventoryRequest�̃C���X�^���X�𐶐�
        var userInventoryRequest = new GetUserInventoryRequest();

        //�C���x���g���̏��̎擾
        //Debug.Log($"�C���x���g���̏��̎擾�J�n");
        PlayFabClientAPI.GetUserInventory(userInventoryRequest, OnSuccessUpdateHaveGachaTicketNum, OnErrorUpdateHaveGachaTicketNum);
    }

    /// <summary>
    /// �K�`���`�P�����̏��̎擾�ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessUpdateHaveGachaTicketNum(GetUserInventoryResult result)
    {
        ////result.Inventory���C���x���g���̏��
        //Debug.Log($"�C���x���g���̏��̎擾�ɐ���");

        ////�������Ă��鉼�z�ʉ݂̏������O�ŕ\��
        //foreach (var virtualCurrency in result.VirtualCurrency)
        //{
        //    Debug.Log($"���z�ʉ� {virtualCurrency.Key} : {virtualCurrency.Value}");
        //}

        haveGachaTicket = result.VirtualCurrency[VIRTUAL_CURRENCY_TEXT];
    }

    /// <summary>
    /// �K�`���`�P�����̏��̎擾�Ɏ��s
    /// </summary>
    /// <param name="result"></param>
    private void OnErrorUpdateHaveGachaTicketNum(PlayFabError error)
    {
        //Debug.LogError($"�C���x���g���̏��̎擾�Ɏ��s\n{error.GenerateErrorReport()}");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            Menu.MenuSceneManager.Instance.SendInformationText("�K�`���`�P�b�g�̏���������\n�擾�ł��܂���ł����B");
        }
    }

    /// <summary>
    /// �K�`���`�P������Ԃ�(��񂪌Â��ꍇ�A��)
    /// </summary>
    /// <returns></returns>
    public int GetHaveGachaTicket()
    {
        return haveGachaTicket;
    }

    /// <summary>
    /// �K�`���`�P�ǉ��̐\��
    /// </summary>
    /// <param name="num">�ǉ����閇��</param>
    public void AddGachaTicket(int num,GameObject shopContentsPanelPrefab)
    {
        var addUserVirtualCurrencyRequest = new AddUserVirtualCurrencyRequest
        {
            Amount = num,   //�ǉ�������z
            VirtualCurrency = VIRTUAL_CURRENCY_TEXT, //���z�ʉ݂̃R�[�h
        };

        memoryShopContentsPanelPrefab = shopContentsPanelPrefab;
        PlayFabClientAPI.AddUserVirtualCurrency(addUserVirtualCurrencyRequest, OnSuccessAddGachaTicket, OnErrorAddGachaTicket);
    }

    /// <summary>
    /// �K�`���`�P�̒ǉ��ɐ���
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessAddGachaTicket(ModifyUserVirtualCurrencyResult result)
    {
        //Debug.Log($"���z�ʉ݂̒ǉ��ɐ���");
        //Debug.Log($"�ύX�������z�ʉ݂̃R�[�h : {result.VirtualCurrency}");
        //Debug.Log($"�ύX��̎c�� : {result.Balance}");
        //Debug.Log($"���Z�z : {result.BalanceChange}");

        memoryShopContentsPanelPrefab.GetComponent<Menu.ShopContentsPanelPrefab>().ReceptionGachaTicketExchange(true);
        memoryShopContentsPanelPrefab = null;

        UpdateHaveGachaTicketNum();
    }

    /// <summary>
    /// �K�`���`�P�̒ǉ��Ɏ��s
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorAddGachaTicket(PlayFabError error)
    {
        Debug.LogError($"OnErrorAddGachaTicket : ���z�ʉ݂̒ǉ��Ɏ��s\n{error.GenerateErrorReport()}");

        memoryShopContentsPanelPrefab.GetComponent<Menu.ShopContentsPanelPrefab>().ReceptionGachaTicketExchange(false);
        memoryShopContentsPanelPrefab = null;

        UpdateHaveGachaTicketNum();
    }


    #endregion


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

    /// <summary>
    /// ���O�C�����Ă��邩�ǂ�����Ԃ��Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public bool GetIsLogined()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }



}
