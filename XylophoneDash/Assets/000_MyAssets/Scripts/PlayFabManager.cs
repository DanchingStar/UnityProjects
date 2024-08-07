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
    /// <summary> アカウントを作成するかのフラグ </summary>
    private bool _shouldCreateAccount;

    /// <summary> ログイン時に使うID </summary>
    private string _myCustomID;

    /// <summary> プレイヤープロフィールの呼び出しに必要なPlayFabId </summary>
    private string myPlayFabID;

    /// <summary> 読み込んだプレイヤープロフィール </summary>
    private PlayerProfileModel myPlayerProfile;

    /// <summary> フレンドのリスト </summary>
    List<FriendInfo> _friends = null;

    /// <summary> IDを保存する時のKEY </summary>
    public static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    /// <summary> IDに使用する文字 </summary>
    private static readonly string ID_CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyz";

    /// <summary> セーブデータをPlayfabに保存するキー </summary>
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

    /// <summary> 仮想通貨交換結果を返すオブジェクト </summary>
    private GameObject memoryShopContentsPanelPrefab;

    /// <summary>
    /// フレンドのタグ名
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

    // アプリケーションが起動していれば true、そうでなければ false
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus && backGroundBefore == DateTime.MinValue) return;

        CheckGoToBackground(pauseStatus);
    }

#region ログイン処理

    /// <summary>
    /// ログイン実行
    /// </summary>
    public void Login()
    {
        _myCustomID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = _myCustomID, CreateAccount = _shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    /// <summary>
    /// ログイン成功
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result)
    {
        bool newPlayerFlg;

        //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
        if (_shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {_myCustomID} は既に使われています。");
            Login();//ログインしなおし
            return;
        }

        //アカウント作成時にIDを保存
        if (result.NewlyCreated)
        {
            SaveCustomID();
            newPlayerFlg = true;
        }
        else
        {
            newPlayerFlg = false;
        }

        Debug.Log($"PlayFabManager.OnLoginSuccess : PlayFabのログインに成功\n" +
            $"PlayFabId = {result.PlayFabId} , CustomId = {_myCustomID} , 新アカウント作成 = {result.NewlyCreated}");

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
    /// ログイン失敗
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");

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

#region 復旧ログイン

    /// <summary>
    /// 復旧コードのデータでログインする
    /// </summary>
    /// <param name="repairId"></param>
    public void RepairLogin(string repairId)
    {
        var request = new LoginWithCustomIDRequest { CustomId = repairId, CreateAccount = false };

        PlayFabClientAPI.LoginWithCustomID(request, OnRepairLoginSuccess, OnRepairLoginFailure);
    }

    /// <summary>
    /// 復旧ログイン成功
    /// </summary>
    /// <param name="result"></param>
    private void OnRepairLoginSuccess(LoginResult result)
    {
        Debug.Log("復旧ログイン : 成功");

        TitleSceneManager.Instance.AfterRepairLogin();
        //GetUserSaveData();
    }

    /// <summary>
    /// 復旧ログイン失敗
    /// </summary>
    /// <param name="result"></param>
    private void OnRepairLoginFailure(PlayFabError error)
    {
        Debug.Log("復旧ログイン : 失敗");

        // ここで存在しない場合の処理を実行
        TitleSceneManager.Instance.RepairFailure();
    }

#endregion

#region カスタムIDの取得

    /// <summary>
    /// IDを取得
    /// </summary>
    /// <returns></returns>
    private string LoadCustomID()
    {
        //IDを取得
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //保存されていなければ新規生成
        _shouldCreateAccount = string.IsNullOrEmpty(id);
        return _shouldCreateAccount ? GenerateCustomID() : id;
    }

    /// <summary>
    /// IDの保存
    /// </summary>
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, _myCustomID);
    }

    /// <summary>
    /// 復旧時に新しいIDの保存
    /// </summary>
    public void SaveNewCustomID(string newID)
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, newID);
    }

#endregion

#region カスタムIDの生成

    /// <summary>
    /// IDを生成する
    /// </summary>
    /// <returns></returns>
    private string GenerateCustomID()
    {
        int idLength = 32;//IDの長さ
        StringBuilder stringBuilder = new StringBuilder(idLength);
        var random = new System.Random();

        //ランダムにIDを生成
        for (int i = 0; i < idLength; i++)
        {
            stringBuilder.Append(ID_CHARACTERS[random.Next(ID_CHARACTERS.Length)]);
        }

        return stringBuilder.ToString();
    }

#endregion

#region ユーザーデータ取得

    /// <summary>
    /// ユーザー(プレイヤー)データの取得
    /// </summary>
    public void GetUserSaveData()
    {
        //GetUserDataRequestのインスタンスを生成
        var request = new GetUserDataRequest();

        //ユーザー(プレイヤー)データの取得
        PlayFabClientAPI.GetUserData(request, OnSuccessGettingPlayerData, OnErrorGettingPlayerData);
        Debug.Log($"プレイヤー(ユーザー)データの取得開始");
    }

#endregion

#region ユーザーデータ取得結果

    /// <summary>
    /// ユーザー(プレイヤー)データの取得に成功
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessGettingPlayerData(GetUserDataResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessGettingPlayerData : ユーザー(プレイヤー)データの取得に成功しました");

        SaveData save = null;
        try
        {
            save = JsonUtility.FromJson<SaveData>(result.Data[KEY_SAVE_DATA].Value);
        }
        catch (Exception error)
        {
            Debug.LogWarning($"PlayFabManager.OnSuccessGettingPlayerData : saveの読み込み失敗\n{error.GetBaseException()}");

            PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(null);
        }

        PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(save);
    }

    /// <summary>
    /// ユーザー(プレイヤー)データの取得に失敗
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorGettingPlayerData(PlayFabError error)
    {
        Debug.LogError($"PlayFabManager.OnErrorGettingPlayerData : ユーザー(プレイヤー)データの取得に失敗しました\n{error.GenerateErrorReport()}");
        PlayerInformationManager.Instance.ReceptionSaveDataFromPlayFab(null);
    }

#endregion

#region ユーザーデータ更新

    /// <summary>
    /// 設定アイコンデータとセーブデータを更新する
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

        //更新するデータ
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
            Permission = UserDataPermission.Public //アクセス許可設定
        };

        //ユーザー(プレイヤー)データの更新
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerDataForIcon, OnErrorUpdatingPlayerDataForIcon);
        Debug.Log($"プレイヤー(ユーザー)データの更新開始");

    }

    /// <summary>
    /// 設定アイコンデータの更新
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

        //更新するデータ
        var updateDataDict = new Dictionary<string, string>()
        {
            { KEY_CHARACTER_NAME  , _CharactorName },
            { KEY_ICON_BACK_NAME  , _IconBackName  },
            { KEY_ICON_FRAME_NAME , _IconFrameName },
        };

        //UpdateUserDataRequestのインスタンスを生成
        var request = new UpdateUserDataRequest
        {
            Data = updateDataDict,
            Permission = UserDataPermission.Public //アクセス許可設定
        };

        //ユーザー(プレイヤー)データの更新
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerDataForIcon, OnErrorUpdatingPlayerDataForIcon);
        Debug.Log($"プレイヤー(ユーザー)データの更新開始");

    }

    /// <summary>
    /// セーブデータの更新
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

        //UpdateUserDataRequestのインスタンスを生成
        var request = new UpdateUserDataRequest
        {
            Data = saveData,
            Permission = UserDataPermission.Public //アクセス許可設定
        };

        //ユーザー(プレイヤー)データの更新
        PlayFabClientAPI.UpdateUserData(request, OnSuccessUpdatingPlayerData, OnErrorUpdatingPlayerData);
        Debug.Log($"プレイヤー(ユーザー)データの更新開始");

    }

#endregion

#region ユーザーデータ更新結果

    /// <summary>
    /// 設定アイコンデータの更新に成功
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
    /// 設定アイコンデータの更新に失敗
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
    /// セーブデータの更新に成功
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
    /// セーブデータの更新に失敗
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
    /// ログインしていないときに更新を強いられたとき
    /// </summary>
    private void OnErrorUpdatingPlayDataForNonLogIn()
    {
        Debug.LogWarning($"ログインしていないため、ユーザー(プレイヤー)データの更新に失敗しました");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            //Menu.MenuSceneManager.Instance.ReceptionUpdatePlayFab(false);
        }
    }

#endregion

#region ユーザーデータ削除

    /// <summary>
    /// ユーザーデータ削除
    /// </summary>
    public void DeletePlayerData()
    {
        var request = new PlayFab.AdminModels.DeleteMasterPlayerAccountRequest { PlayFabId = myPlayFabID };

        PlayFabAdminAPI.DeleteMasterPlayerAccount(request, OnDeleteSuccess, OnDeleteFailure);
    }

    /// <summary>
    /// ユーザーデータ削除の成功時に呼ぶ
    /// </summary>
    /// <param name="result"></param>
    private void OnDeleteSuccess(PlayFab.AdminModels.DeleteMasterPlayerAccountResult result)
    {
        Debug.Log("ユーザーデータの削除に成功しました。");

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionDeletePlayFab(true);
        }
    }

    /// <summary>
    /// ユーザーデータ削除の失敗時に呼ぶ
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

#region プレイヤープロフィールの呼び出し

    /// <summary>
    /// 自身のプレイヤープロフィールの呼び出し(未使用)
    /// </summary>
    private void GetMyPlayerProfile()
    {
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = myPlayFabID,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                // その他の情報を開示するには、ブラウザからタイトルの設定でクライアントプロフィールオプションを変更する
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
    /// 誰かプレイヤープロフィールの呼び出し
    /// </summary>
    /// <param name="playFabId">呼び出すID</param>
    public void GetOtherPlayerProfile(string playFabId, FriendInfo friendInfo)
    {
        if (playFabId == myPlayFabID)
        {
            GetOtherPlayerProfileFailure("これは自分のIDです");
            return;
        }

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                // その他の情報を開示するには、ブラウザからタイトルの設定でクライアントプロフィールオプションを変更する
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
            GetOtherPlayerProfileFailure("ユーザーデータが\n見つかりませんでした");
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// 誰かのセーブデータの呼び出し
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
            GetOtherPlayerProfileFailure("セーブデータが\n読み込めませんでした");
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// 誰かの呼び出しが失敗したとき
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

#region フレンド機能

    /// <summary>
    /// フレンド情報を全て取得する
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
    /// 指定したタグのフレンドリストを全て表示する
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
            GetOtherPlayerProfileFailure("フォローしているユーザが\nひとりもいません");
        }
    }

    /// <summary>
    /// 指定したユーザーをフレンドにする
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
            DisplayFriendDialogText("このユーザは\nすでにフレンドです");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// 指定したフレンドを削除する
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

            DisplayFriendDialogText("このユーザを\nフレンドから\n削除しました");
        },
        error =>
        {
            DisplayFriendDialogText("このユーザは\nすでにフレンドでは\nありません");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// 指定したフレンドにタグを追加する
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
            DisplayFriendDialogText("このユーザを\nフレンドに\n追加しました");
        },
        error =>
        {
            DisplayFriendDialogText("このユーザの\nタグ編集に\n失敗しました");
            DisplayPlayFabError(error);
        });
    }

    /// <summary>
    /// エラー発生時にログを表示する
    /// </summary>
    /// <param name="error"></param>
    private void DisplayPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    /// <summary>
    /// FriendDetailのダイアログにテキストを表示する
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

#region 表示名の更新

    /// <summary>
    /// 新しい表示名に更新する
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
                Debug.Log("表示名の更新成功");
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
    }

    /// <summary>
    /// 入力された名前が有効かどうかをチェックする
    /// </summary>
    /// <param name="inputName"></param>
    /// <returns></returns>
    public bool IsValidName(TMP_InputField inputName)
    {
        // 表示名は、３文字以上２５文字以下
        // 今回は3文字以上12文字以内に設定
        string playerName = inputName.text;
        bool isValidName = !string.IsNullOrWhiteSpace(playerName);
        bool isNameLengthValid = playerName.Length >= 3 && playerName.Length <= 12;

        return isValidName && isNameLengthValid;
    }

#endregion

#region ストア

    /// <summary>
    /// カタログデータの取得(ガチャ)
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
    /// ストアデータの取得(ガチャ)
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
    /// ガチャの価格を返すゲッター
    /// </summary>
    /// <param name="gachaTimes">ガチャ回数(1or11)</param>
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
    /// ガチャの購入
    /// </summary>
    /// <param name="gachaTimes">ガチャ回数(1or11)</param>
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
            // キャラクターを使う場合は CharacterId のセットも必要
        },
        result =>
        {
            Debug.Log($"PlayFabManager.PurchaseItem : {result.Items[0].DisplayName}購入成功！" +
                $"\n{PlayFabSimpleJson.SerializeObject(result.Items)}");

            List<string> itemNames = new List<string>();
            foreach (var item in result.Items)
            {
                if (item.ItemId != itemId)
                {
                    itemNames.Add(item.ItemId);
                }
            }

            foreach (var item in itemNames) //ガチャの結果をログ出力
            {
                Debug.Log($"name : {item}");
            }

            MenuSceneManager.Instance.ReceptionGachaResult(itemNames);

            UpdateHaveGachaTicketNum();
        },
        error =>
        {
            // 金額不足
            if (error.Error == PlayFabErrorCode.InsufficientFunds)
            {
                MenuSceneManager.Instance.ReceptionGachaFailure("SPコインが\n不足しています。");
            }
            else
            {
                MenuSceneManager.Instance.ReceptionGachaFailure("エラーが発生しました。");
            }

            Debug.LogError("PlayFabManager.PurchaseGacha : " + error.GenerateErrorReport());

        });
    }

    /// <summary>
    /// 仮想通貨枚数の情報更新を要求
    /// </summary>
    public void UpdateHaveGachaTicketNum()
    {
        //GetUserInventoryRequestのインスタンスを生成
        var userInventoryRequest = new GetUserInventoryRequest();

        //インベントリの情報の取得
        PlayFabClientAPI.GetUserInventory(userInventoryRequest, OnSuccessUpdateHaveGachaTicketNum, OnErrorUpdateHaveGachaTicketNum);
    }

    /// <summary>
    /// 仮想通貨枚数の情報の取得に成功
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessUpdateHaveGachaTicketNum(GetUserInventoryResult result)
    {
        haveGachaTicket = result.VirtualCurrency[VIRTUAL_CURRENCY_TEXT];
    }

    /// <summary>
    /// 仮想通貨枚数の情報の取得に失敗
    /// </summary>
    /// <param name="result"></param>
    private void OnErrorUpdateHaveGachaTicketNum(PlayFabError error)
    {
        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.SendInformationText("SPコインの所持枚数を\n取得できませんでした。");
        }

        Debug.LogError("PlayFabManager.OnErrorUpdateHaveGachaTicketNum : " + error.GenerateErrorReport());
    }

    /// <summary>
    /// 仮想通貨枚数を返す(情報が古い場合アリ)
    /// </summary>
    /// <returns></returns>
    public int GetHaveGachaTicket()
    {
        return haveGachaTicket;
    }

    /// <summary>
    /// 仮想通貨追加の申請
    /// </summary>
    /// <param name="num">追加する枚数</param>
    public void AddGachaTicket(int num)
    {
        var addUserVirtualCurrencyRequest = new AddUserVirtualCurrencyRequest
        {
            Amount = num,   //追加する金額
            VirtualCurrency = VIRTUAL_CURRENCY_TEXT, //仮想通貨のコード
        };

        PlayFabClientAPI.AddUserVirtualCurrency(addUserVirtualCurrencyRequest, OnSuccessAddGachaTicket, OnErrorAddGachaTicket);
    }

    /// <summary>
    /// 仮想通貨の追加に成功
    /// </summary>
    /// <param name="result"></param>
    private void OnSuccessAddGachaTicket(ModifyUserVirtualCurrencyResult result)
    {
        Debug.Log($"PlayFabManager.OnSuccessAddGachaTicket : 仮想通貨の追加に成功\n" +
            $"変更した仮想通貨のコード = {result.VirtualCurrency}\n" +
            $"変更後の残高 = {result.Balance}\n" +
            $"加算額 = {result.BalanceChange}\n");

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionAddSpCoin(false ,result.BalanceChange);
        }
    }

    /// <summary>
    /// 仮想通貨の追加に失敗
    /// </summary>
    /// <param name="error"></param>
    private void OnErrorAddGachaTicket(PlayFabError error)
    {
        Debug.LogError($"OnErrorAddGachaTicket : 仮想通貨の追加に失敗\n{error.GenerateErrorReport()}");

        UpdateHaveGachaTicketNum();

        if (SceneManager.GetActiveScene().name == SCENE_NAME_MENU)
        {
            MenuSceneManager.Instance.ReceptionAddSpCoin(true, 0);
        }
    }

#endregion

#region ランキング

    /// <summary>
    /// ランキングに値を提出し、表示用のスコアを返す
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
    /// 順位上位のランキング(リーダーボード)を取得する
    /// </summary>
    /// <param name="rankingName"></param>
    public void RankingGetLeaderboardAroundTop(string rankingName, ShowRankingPanelPrefab showRankingPanelPrefab)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = rankingName, //ランキング名(統計情報名)
            MaxResultsCount = RANKING_ITEM_NUM //何件取得するか
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

                Debug.Log($"{rankPosition}位 : {playerName} ({playerId}) : スコア {displayScoreTime}");
                showRankingPanelPrefab.ReceptionLeaderboardAroundTop(rankPosition, playerName, playerId, displayScoreTime);
            }
            if (counter == 0)
            {
                showRankingPanelPrefab.ReceptionFailureFromLeaderboardAroundTop("まだ記録がありません");
                Debug.Log($"RankingGetLeaderboardAroundTop : Ranking Data is Nothing !");
            }
        }, error =>
        {
            showRankingPanelPrefab.ReceptionFailureFromLeaderboardAroundTop("エラーが発生しました");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    /// <summary>
    /// 自分の順位周辺のランキング(リーダーボード)を取得する
    /// </summary>
    /// <param name="rankingName"></param>
    public void RankingGetLeaderboardAroundPlayer(string rankingName, ShowRankingPanelPrefab showRankingPanelPrefab)
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = rankingName, //ランキング名(統計情報名)
            MaxResultsCount = RANKING_ITEM_NUM //自分を含め前後何件取得するか
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

                Debug.Log($"{rankPosition}位 : {playerName} ({playerId}) : スコア {displayScoreTime}");
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
    /// タイムとミス回数をランキングに送るスコアに変換する
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
    /// ランキングに送られたスコアを見た目上のスコアに変える
    /// </summary>
    /// <param name="rankingScore"></param>
    /// <returns></returns>
    private float ConversionRankingScoreToDisplayScore(int rankingScore)
    {
        float displayScore = -1f * (float)rankingScore / 100f;

        return displayScore;
    }

    /// <summary>
    /// ランキングの人のアイコンデータを受信する
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

#region アイテム

    /// <summary>
    /// 文字列のアイテムを所持しているかチェックして結果を返す
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
    /// アイテムインベントリにアクセス成功したとき
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

        Debug.Log($"PlayFabManager.OnSuccessGetUserInventory : Have Item 「{_itemId}」 = {haveFlg}");

        if (_itemId == "NoAdMob")
        {
            AdMobManager.Instance.ReceptionPlayFabForItem(haveFlg);
        }
    }

    /// <summary>
    /// アイテムインベントリにアクセス失敗したとき
    /// </summary>
    /// <param name="error"></param>
    /// <param name="_itemId"></param>
    private void OnErrorGetUserInventory(PlayFabError error, string _itemId)
    {
        Debug.LogError($"PlayFabManager.OnErrorGetUserInventory : アイテムインベントリの取得に失敗しました {_itemId}\n{error.GenerateErrorReport()}");

        if (_itemId == "NoAdMob")
        {
            AdMobManager.Instance.ReceptionPlayFabForItem(false);
        }
    }

    #endregion

    /// <summary>
    /// バックグラウンドの移行・復帰をチェックする
    /// </summary>
    /// <param name="flg">バックグラウンドに行った : true , バックグラウンドから復帰した : false</param>
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
    /// CustomIdのゲッター
    /// </summary>
    /// <returns></returns>
    public string GetCustomId()
    {
        return _myCustomID;
    }

    /// <summary>
    /// PlayFabIDのゲッター
    /// </summary>
    /// <returns></returns>
    public string GetPlayFabID()
    {
        return myPlayFabID;
    }

    /// <summary>
    /// ログインしているかどうかを返すゲッター
    /// </summary>
    /// <returns></returns>
    public bool GetIsLogined()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }



}
