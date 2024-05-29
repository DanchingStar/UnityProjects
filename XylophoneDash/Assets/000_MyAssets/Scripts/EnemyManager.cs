using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform[] walkUnderCheckPoints;
    [SerializeField] private Transform[] walkOverCheckPoints;

    private float[] flyCheckPointsPositionX;
    private Transform[] flyAppearanceCheckPoints;
    private Transform[] flyExtinctionCheckPoints;

    [SerializeField] private GameObject enemyWalkPrefab;
    [SerializeField] private GameObject enemyFlyPrefab;

    private Transform checkPointsTransform;

    private const float MOVE_SPEED_WALK = 10f;
    private const float MOVE_SPEED_FLY = 5f;

    private const float ENEMY_POWOR = 5f;

    /// <summary>
    /// 出現させるエネミーの通し番号
    /// </summary>
    private int enemyNumber = 0;

    private readonly Vector3 DEFAULT_POSITION = new Vector3(100f,100f,100f);

    public static EnemyManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        checkPointsTransform = transform.Find("CheckPoints");
        InitEnemyFlyValues();
    }

    /// <summary>
    /// 飛ぶエネミーの出現位置の初期化
    /// </summary>
    private void InitEnemyFlyValues()
    {
        flyCheckPointsPositionX = new float[] { -8f, -4f, 0f, 4f, 8f };
        flyAppearanceCheckPoints = new Transform[flyCheckPointsPositionX.Length];
        flyExtinctionCheckPoints = new Transform[flyCheckPointsPositionX.Length];

        for (int i = 0; i < flyCheckPointsPositionX.Length; i++)
        {
            flyAppearanceCheckPoints[i] = new GameObject().transform;
            flyExtinctionCheckPoints[i] = new GameObject().transform;

            flyAppearanceCheckPoints[i].position = new Vector3(flyCheckPointsPositionX[i], 1.5f, 10f);
            flyExtinctionCheckPoints[i].position = new Vector3(flyCheckPointsPositionX[i], 1.5f, -10f);

            flyAppearanceCheckPoints[i].rotation = Quaternion.identity;
            flyExtinctionCheckPoints[i].rotation = Quaternion.identity;

            flyAppearanceCheckPoints[i].localScale = Vector3.one;
            flyExtinctionCheckPoints[i].localScale = Vector3.one;

            flyAppearanceCheckPoints[i].gameObject.name = $"Point_Fly{(i * 2)}";
            flyExtinctionCheckPoints[i].gameObject.name = $"Point_Fly{(i * 2) + 1}";

            flyAppearanceCheckPoints[i].parent = checkPointsTransform;
            flyExtinctionCheckPoints[i].parent = checkPointsTransform;
        }
    }

    /// <summary>
    /// 鍵盤(下)を歩くエネミーを出現させる
    /// </summary>
    /// <param name="index">配置させる番号(0,1,2,3)</param>
    /// <param name="distance">チェックポイントとの距離(0.0f～1.0f)</param>
    public void SpawnWalkUnderEnemy(int index, float distance)
    {
        EnemyWalk enemy = Instantiate(enemyWalkPrefab, DEFAULT_POSITION, Quaternion.identity, transform).GetComponent<EnemyWalk>();

        int beforeIndex = index - 1 < 0 ? walkUnderCheckPoints.Length - 1 : index - 1;

        //Debug.Log($"index = {index} , beforeIndex = {beforeIndex}");

        enemy.SetMyStartIndex(index);
        enemy.SetMyIsAbleFly(false);
        enemy.SetMyMoveSpeed(MOVE_SPEED_WALK);
        enemy.SetMyAttackPower(ENEMY_POWOR);
        enemy.SetMySpawnPositon(Vector3.Lerp(walkUnderCheckPoints[index].position, walkUnderCheckPoints[beforeIndex].position, distance));
        enemy.SetMyDestinationTransforms(walkUnderCheckPoints);

        enemy.SetUp(enemyNumber);
        enemyNumber++;
    }

    /// <summary>
    /// 鍵盤(上)を歩くエネミーを出現させる
    /// </summary>
    /// <param name="index">配置させる番号(0,1,2,3)</param>
    /// <param name="distance">チェックポイントとの距離(0.0f～1.0f)</param>
    public void SpawnWalkOverEnemy(int index, float distance)
    {
        EnemyWalk enemy = Instantiate(enemyWalkPrefab, DEFAULT_POSITION, Quaternion.identity, transform).GetComponent<EnemyWalk>();

        int beforeIndex = index - 1 < 0 ? walkOverCheckPoints.Length - 1 : index - 1;

        //Debug.Log($"index = {index} , beforeIndex = {beforeIndex}");

        enemy.SetMyStartIndex(index);
        enemy.SetMyIsAbleFly(false);
        enemy.SetMyMoveSpeed(MOVE_SPEED_WALK);
        enemy.SetMyAttackPower(ENEMY_POWOR);
        enemy.SetMySpawnPositon(Vector3.Lerp(walkOverCheckPoints[index].position, walkOverCheckPoints[beforeIndex].position, distance));
        enemy.SetMyDestinationTransforms(walkOverCheckPoints);

        enemy.SetUp(enemyNumber);
        enemyNumber++;
    }

    /// <summary>
    /// 飛ぶエネミーを出現させる
    /// </summary>
    /// <param name="index">配置させる番号(0,1,2,3,4)</param>
    /// <param name="distance">チェックポイントとの距離(0.0f～1.0f)</param>
    public void SpawnFlyEnemy(int index, float distance)
    {
        EnemyFly enemy = Instantiate(enemyFlyPrefab, DEFAULT_POSITION, Quaternion.identity, transform).GetComponent<EnemyFly>();

        enemy.SetMyStartIndex(index);
        enemy.SetMyIsAbleFly(true);
        enemy.SetMyMoveSpeed(MOVE_SPEED_FLY);
        enemy.SetMyAttackPower(ENEMY_POWOR);
        enemy.SetMySpawnPositon(Vector3.Lerp(flyAppearanceCheckPoints[index].position, flyExtinctionCheckPoints[index].position, distance));
        Transform[] destinationTransforms = new Transform[] { flyAppearanceCheckPoints[index], flyExtinctionCheckPoints[index] };
        enemy.SetMyDestinationTransforms(destinationTransforms);

        enemy.SetUp(enemyNumber);
        enemyNumber++;

    }


}
