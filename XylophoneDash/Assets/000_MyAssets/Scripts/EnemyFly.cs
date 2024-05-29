using UnityEngine;

public class EnemyFly : EnemyBase
{
    [SerializeField] private DealDamageComponent dealDamageComponent;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    private const float MATERIAL_VALUE = 2f;

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

        skinnedMeshRenderer = dealDamageComponent.transform.Find("Eaglle_Normal").GetComponent<SkinnedMeshRenderer>();
        myMaterial = skinnedMeshRenderer.material;

        gameObject.name = $"EnemyFly{enemyNumber}(Clone)";
    }

    /// <summary>
    /// Move‚Ì‹ï‘Ì“I‚È“à—e
    /// </summary>
    private void MoveContents()
    {
        transform.position = Vector3.MoveTowards(transform.position, destinationTransforms[1], moveSpeed * Time.deltaTime);

        if (transform.position.z <= destinationTransforms[1].z)
        {
            transform.position = destinationTransforms[0];
        }
        else if (transform.position.z > destinationTransforms[0].z - MATERIAL_VALUE)
        {
            float value = (destinationTransforms[0].z - transform.position.z) / MATERIAL_VALUE;
            skinnedMeshRenderer.material.color = new Color(myMaterial.color.r, myMaterial.color.b, myMaterial.color.b, value);
        }
        else if (transform.position.z < destinationTransforms[1].z + MATERIAL_VALUE)
        {
            float value = (transform.position.z - destinationTransforms[1].z) / MATERIAL_VALUE;
            skinnedMeshRenderer.material.color = new Color(myMaterial.color.r, myMaterial.color.b, myMaterial.color.b, value);
        }
        else if (meshRenderer.material.color.a != 1f)
        {
            skinnedMeshRenderer.material.color = new Color(myMaterial.color.r, myMaterial.color.b, myMaterial.color.b, 1f);
        }

        


    }

}
