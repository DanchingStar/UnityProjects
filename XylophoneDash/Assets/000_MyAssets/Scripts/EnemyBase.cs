using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected int myEnemyNumber;

    protected float moveSpeed;
    protected float attackPower;
    protected bool isAbleFly;
    protected int startIndex;
    protected Vector3 spawnPositon;
    protected Vector3[] destinationTransforms;

    protected MeshRenderer meshRenderer;
    protected Material myMaterial;

    protected Collider[] myColliders;

    protected int nowIndex;
    protected bool isMove;

    protected bool isEnabled;
    protected float disableTimer;

    protected float disableTimeSetting;

    protected const float DEFAULT_DISABLE_TIME = 3f;

    private void Update()
    {
        if (GameSceneManager.Instance.GetPauseFlg()) return;

        if (isMove)
        {
            if (GameSceneManager.Instance.GetNowPhase() != GameSceneManager.NowPhase.GameClear)
            {
                Move();
            }
        }
        else 
        {
            if(GameSceneManager.Instance.GetGameInformation().mode == PlayerInformationManager.GameMode.FreeStyle)
            {
                isMove = true;
                ChangeEnable(true);
            }
            else
            {
                if (GameSceneManager.Instance.GetCountDownValue() <= 0)
                {
                    isMove = true;
                    ChangeEnable(true);
                }
            }
        }

        if (disableTimer > 0f)
        {
            DisableTime();
        }
    }

    public virtual void SetUp(int enemyNumber)
    {
        myEnemyNumber = enemyNumber;

        isMove = false;
        transform.position = spawnPositon;
        meshRenderer = GetComponent<MeshRenderer>();
        myMaterial = meshRenderer.material;
        disableTimeSetting = DEFAULT_DISABLE_TIME;

        myColliders = GetComponentsInChildren<Collider>();
    }

    protected virtual void Move()
    {

    }

    private void DisableTime()
    {
        disableTimer -= Time.deltaTime;

        if ((int)(disableTimer * 10f) % 2 == 0)
        {
            FlashingMaterial(true);
        }
        else
        {
            FlashingMaterial(false);
        }

        if (disableTimer < 0f)
        {
            ChangeEnable(true);
            disableTimer = 0f;
            FlashingMaterial(true);
        }
    }

    private void FlashingMaterial(bool flg)
    {
        if (flg)
        {
            myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, 1f);
        }
        else
        {
            myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, 0.1f);
        }
        meshRenderer.material = myMaterial;
    }

    public void SetMyMoveSpeed(float value)
    {
        moveSpeed = value;
    }

    public void SetMyAttackPower(float value)
    {
        attackPower = value;
    }

    public void SetMyIsAbleFly(bool value)
    {
        isAbleFly = value;
    }

    public void SetMyStartIndex(int value)
    {
        startIndex = value;
        nowIndex = startIndex;
    }

    public void SetMySpawnPositon(Vector3 value)
    {
        spawnPositon = value;
    }

    public void SetMyDestinationTransforms(Transform[] values)
    {
        destinationTransforms = new Vector3[values.Length];

        for (int i = 0; i < values.Length; i++) 
        {
            destinationTransforms[i] = values[i].position;
        }
    }

    /// <summary>
    /// é©êgÇÃìñÇΩÇËîªíËÇïœçXÇ∑ÇÈ
    /// </summary>
    /// <param name="flg"></param>
    private void ChangeEnable(bool flg)
    {
        isEnabled = flg;
        foreach (var col in myColliders)
        {
            col.enabled = flg;
        }
    }

}
