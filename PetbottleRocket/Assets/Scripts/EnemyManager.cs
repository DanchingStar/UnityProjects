using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �G�Ǘ��pManager
/// </summary>
public class EnemyManager : MonoBehaviour
{
    #region �G�̎��
    // �v���p�e�B����EnemyType�ƕK�����킹�邱��
    #region �_
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

    #region ��
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

    #region ��s�@(��)
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

    #region ��s�@(�E)
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

    #region 覐�
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

    #region �F���S�~
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

    // Enemy�𐶐�����Ԋu
    private float _CreateSpawnTime = 1.0f;

    // �O��Enemy�𐶐����Ă���o�߂�������
    private float _ElapsedTime = 0.0f;

    // Enemy�̍ő�o����
    //[SerializeField]
    private int _MaxExistCount = 3;

    // Enemy�����ݑ��݂��Ă��鐔
    //[SerializeField]
    private int _NowExistCount = 0;

    // Enemy�����󂳂��鎞��Player�Ƃ�Y�������̋���
    private float _UnderMergin = 5.0f;

    private List<GameObject> _Enemys;
    private List<int> _RemovedEnemyIndexs;


    private int _EnemyTypeCount = 0;
    private int _MaxEnemyTypeCount = 0;


    private Dictionary<EnemyType, GameObject> _TypeObjectDictionary;
    enum EnemyType
    {
        Cloud = 0, // �_
        Bird, // ��
        PlaneLeft, // JAL
        PlaneRight, // ANA
        Meteorite, // 覐�
        //SpaceDebri, // �F���S�~
    }

    #region GameSystem
    private void Start()
    {
        // EnumType����Ή�����GameObject���擾��Dictionary�𐶐�
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

        // Enemy�̎�ނ�r���ŕύX����Ȃ炢���Ȃ�Length�����Ȃ���������
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

        // TODO:���t���[���ĂԕK�v�͂Ȃ�
        TryDestoryEnemy();
        UpdateEnemyFrequency();
    }
    #endregion

    #region �G�̏o�������ύX����

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

    #region �G�����p����
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
                // �����ɗ���ꍇ��GameObject���w��ł��Ă��Ȃ��\��������
                // D&D��Prefab���w�肵�����ǂ����m�F����
                Debug.LogError($"[EnemyManager] {newEnemyType}��GameObject��null");
            }
        }
        else
        {
            // ���O����GameObject���擾�ł��Ă��Ȃ�
            // GameObject�̃v���p�e�B�̖�����EnumType�Ɠ����ɂ���K�v������
            Debug.LogError($"[EnemyManager] {newEnemyType}��GameObject�擾�ł��Ȃ�");
        }
    }

    private EnemyType GetRandomEnemyType()
    {
        int rand = UnityEngine.Random.Range(0, _EnemyTypeCount);
        return (EnemyType)rand;
    }
    #endregion

    #region �G����p����

    /// <summary>
    /// Player��艺�̓G������Ƃ��A�G���̏�����F�X����
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
