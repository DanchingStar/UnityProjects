using UnityEngine;

public class EnemyWalk : EnemyBase
{
    [SerializeField] private RobotFreeAnim robotFreeAnim;

    protected override void Move()
    {
        base.Move();

        if (GameSceneManager.Instance.GetGameInformation().mode == PlayerInformationManager.GameMode.FreeStyle)
        {
            if (GameSceneManager.Instance.GetPlayTime() <= 2f) return;

            MoveContents();
        }
        else
        {
            MoveContents();
        }
    }

    public override void SetUp(int enemyNumber)
    {
        base.SetUp(enemyNumber);
        disableTimeSetting = 1f;
        robotFreeAnim.SetMyMaterials(enemyNumber);
        gameObject.name = $"EnemyWalk{enemyNumber}(Clone)";
    }

    /// <summary>
    /// Move‚Ì‹ï‘Ì“I‚È“à—e
    /// </summary>
    private void MoveContents()
    {
        transform.position = Vector3.MoveTowards(transform.position, destinationTransforms[nowIndex], moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, destinationTransforms[nowIndex]) < 0.01f)
        {
            nowIndex = destinationTransforms.Length <= nowIndex + 1 ? 0 : nowIndex + 1;
        }
    }

    //protected override void AttackPlayer(Rigidbody rb, Vector3 vector)
    //{
    //    base.AttackPlayer(rb, vector);
    //    Vector3 vec = new Vector3(vector.x, 0.45f, vector.z);
    //    rb.AddForce(vector * attackPower, ForceMode.VelocityChange);
    //    //Debug.Log($"EnemyWalk.AttackPlayer : vector = {vector} , attackPower = {attackPower}");
    //}

    //public override Vector3 HitPlayer(Player player)
    //{
    //    return base.HitPlayer(player);
    //}

}
