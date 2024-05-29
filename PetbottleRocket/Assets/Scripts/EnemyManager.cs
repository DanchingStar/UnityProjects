using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵管理用Manager
/// </summary>
public class EnemyManager : MonoBehaviour
{
    #region 敵の種類
    // プロパティ名はEnemyTypeと必ず合わせること
    #region 雲
    [SerializeField]
    private GameObject _Cloud;

    public GameObject Cloud
    {
        get
        {
            return _Cloud;
        }
    }
    #endregion

    #region 鳥
    [SerializeField]
    private GameObject _Bird;

    public GameObject Bird
    {
        get
        {
            return _Bird;
        }
    }
    #endregion

    #region 飛行機(左)
    [SerializeField]
    private GameObject _PlaneLeft;

    public GameObject PlaneLeft
    {
        get
        {
            return _PlaneLeft;
        }
    }
    #endregion

    #region 飛行機(右)
    [SerializeField]
    private GameObject _PlaneRight;

    public GameObject PlaneRight
    {
        get
        {
            return _PlaneRight;
        }
    }
    #endregion

    #region 隕石
    [SerializeField]
    private GameObject _Meteorite;

    public GameObject Meteorite
    {
        get
        {
            return _Meteorite;
        }
    }
    #endregion

    #region 宇宙ゴミ
    //[SerializeField]
    //private GameObject _SpaceDebri;

    //public GameObject SpaceDebri
    //{
    //    get
    //    {
    //        return _SpaceDebri;
    //    }
    //}
    #endregion

    #endregion

    [SerializeField]
    private GameObject _Player;

    // Enemyを生成する間隔
    private float _CreateSpawnTime = 1.0f;

    // 前回Enemyを生成してから経過した時間
    private float _ElapsedTime = 0.0f;

    // Enemyの最大出現数
    //[SerializeField]
    private int _MaxExistCount = 3;

    // Enemyが現在存在している数
    //[SerializeField]
    private int _NowExistCount = 0;

    // Enemyを自壊させる時のPlayerとのY軸方向の距離
    private float _UnderMergin = 5.0f;

    private List<GameObject> _Enemys;
    private List<int> _RemovedEnemyIndexs;


    private int _EnemyTypeCount = 0;
    private int _MaxEnemyTypeCount = 0;


    private Dictionary<EnemyType, GameObject> _TypeObjectDictionary;
    enum EnemyType
    {
        Cloud = 0, // 雲
        Bird, // 鳥
        PlaneLeft, // JAL
        PlaneRight, // ANA
        Meteorite, // 隕石
        //SpaceDebri, // 宇宙ゴミ
    }

    #region GameSystem
    private void Start()
    {
        // EnumTypeから対応するGameObjectを取得しDictionaryを生成
        _TypeObjectDictionary = new Dictionary<EnemyType, GameObject>();
        foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
        {
            System.Reflection.PropertyInfo goInfo = typeof(EnemyManager).GetProperty(type.ToString());
            var value = goInfo.GetValue(this);
            var obj = value as GameObject;

            _TypeObjectDictionary.Add(type, goInfo.GetValue(this) as GameObject);
        }

        _Enemys = new List<GameObject>();
        _RemovedEnemyIndexs = new List<int>();

        // Enemyの種類を途中で変更するならいきなりLengthを取らない方がいい
        _MaxEnemyTypeCount = Enum.GetValues(typeof(EnemyType)).Length;
        _EnemyTypeCount = 1;
    }

    private void Update()
    {
        _ElapsedTime += Time.deltaTime;

        if (_ElapsedTime > _CreateSpawnTime && _NowExistCount <= _MaxExistCount)
        {
            CreateEnemey();
            _ElapsedTime = 0.0f;
        }

        // TODO:毎フレーム呼ぶ必要はない
        TryDestoryEnemy();
        UpdateEnemyFrequency();
    }
    #endregion

    #region 敵の出現割合変更処理

    private int _NowLevel = 0;
    private int _MaxLevel = 10;

    private void UpdateEnemyFrequency()
    {
        if (_MaxLevel <= _NowLevel)
        {
            return;
        }

        var playerY = _Player.transform.position.y;

        if (_NowLevel * 40.0f < playerY && playerY <= (_NowLevel + 1) * 40.0f)
        {
            if (_EnemyTypeCount < _MaxEnemyTypeCount)
            {
                _EnemyTypeCount++;
            }
            else
            {
                _MaxExistCount++;
            }
            _NowLevel++;
        }
    }

    #endregion

    #region 敵生成用処理
    private void CreateEnemey()
    {
        EnemyType newEnemyType = GetRandomEnemyType();
        if (_TypeObjectDictionary.TryGetValue(newEnemyType, out GameObject enemyObject))
        {
            if (enemyObject != null)
            {
                var comp = enemyObject.GetComponent<Enemy>();
                var enemyPos = comp.GenerateEnemyPos() + _Player.transform.position;
                var enemy = Instantiate(enemyObject, enemyPos, Quaternion.identity);
                _Enemys.Add(enemy);
                _NowExistCount++;
            }
            else
            {
                // ここに来る場合はGameObjectが指定できていない可能性がある
                // D&DでPrefabを指定したかどうか確認する
                Debug.LogError($"[EnemyManager] {newEnemyType}のGameObjectがnull");
            }
        }
        else
        {
            // 名前からGameObjectが取得できていない
            // GameObjectのプロパティの命名はEnumTypeと同名にする必要がある
            Debug.LogError($"[EnemyManager] {newEnemyType}のGameObject取得できない");
        }
    }

    private EnemyType GetRandomEnemyType()
    {
        int rand = UnityEngine.Random.Range(0, _EnemyTypeCount);
        return (EnemyType)rand;
    }
    #endregion

    #region 敵自壊用処理

    /// <summary>
    /// Playerより下の敵がいるとき、敵回りの処理を色々する
    /// </summary>
    private void TryDestoryEnemy()
    {
        for (int i = 0; i < _Enemys.Count; i++)
        {
            var enemy = _Enemys[i];

            if (IsUnderPlayer(enemy))
            {
                _RemovedEnemyIndexs.Add(i);
            }
        }

        for (int i = _RemovedEnemyIndexs.Count - 1; i > 0; i--)
        {
            var destroryEnemy = _Enemys[_RemovedEnemyIndexs[i]];
            _Enemys.RemoveAt(_RemovedEnemyIndexs[i]);
            DestroyEnemy(destroryEnemy);
        }
        _RemovedEnemyIndexs.Clear();
    }

    private void DestroyEnemy(GameObject enemy)
    {
        Destroy(enemy);
        _NowExistCount--;
    }

    private bool IsUnderPlayer(GameObject enemy)
    {
        var playerPosY = _Player.transform.position.y;
        var enemyPosY = enemy.transform.position.y;

        if (playerPosY > enemyPosY + _UnderMergin)
        {
            return true;
        }

        return false;
    }
    #endregion
}
