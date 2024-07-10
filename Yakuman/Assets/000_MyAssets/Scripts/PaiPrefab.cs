using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PaiPrefab : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleFire;
    [SerializeField] private ParticleSystem particleAura;

    private MahjongManager.PaiKinds thisKind;
    private int totalNumber;
    /// <summary>MahjongManagerのpaiYamaの配列のインデックス</summary>
    private int arrayNumber;

    private MahjongManager.PlayerKind playerTehaiFlg;

    private float movingTimer;

    private MeshRenderer meshRendererOfGara;
    private MeshRenderer meshRendererOfPai;

    private const float MOVE_TIME = 0.25f;

    private Color COLOR_DEFAULT;
    private Color COLOR_GREY = Color.grey;

    private bool interactableTapFlg;

    private bool startFinifhFlg = false;

    private void Start()
    {
        if (startFinifhFlg) return;
        startFinifhFlg = true;

        meshRendererOfGara = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        meshRendererOfPai = GetComponent<MeshRenderer>();
        COLOR_DEFAULT = meshRendererOfPai.material.color;
        interactableTapFlg = true;
    }

    /// <summary>
    /// 自身のステータスを設定する
    /// </summary>
    /// <param name="_thisKind"></param>
    /// <param name="_totalNumber"></param>
    /// <param name="_arrayNumber"></param>
    /// <param name="_tehaiFlg"></param>
    public void SetStatus(MahjongManager.PaiKinds _thisKind, int _totalNumber, int _arrayNumber, MahjongManager.PlayerKind _tehaiFlg)
    {
        Start();

        thisKind = _thisKind;
        totalNumber = _totalNumber;
        arrayNumber = _arrayNumber;

        playerTehaiFlg = _tehaiFlg;

        gameObject.name = $"Pai_{arrayNumber}({totalNumber})";
        meshRendererOfGara.material = MahjongManager.Instance.GetGaraMaterial(thisKind);
    }

    /// <summary>
    /// Positionを設定する
    /// </summary>
    /// <param name="_position"></param>
    public void ChangeTransformPosition(Vector3 _position)
    {
        transform.localPosition = _position;
    }

    /// <summary>
    /// Rotationを設定する
    /// </summary>
    /// <param name="_rotate"></param>
    public void ChangeTransformRotate(Vector3 _rotate)
    {
        transform.localRotation = Quaternion.Euler(_rotate);
    }

    /// <summary>
    /// 自身のオブジェクトを移動させる
    /// </summary>
    /// <param name="_positionNumber"></param>
    public void MoveThisPaiOnTehai(int _positionNumber)
    {
        movingTimer = 0f;
        StartCoroutine(MoveThisPaiOnTehaiCoroutine(_positionNumber));
    }

    /// <summary>
    /// 自身のオブジェクトを移動させるコルーチン
    /// </summary>
    /// <param name="_positionNumber"></param>
    /// <returns></returns>
    private IEnumerator MoveThisPaiOnTehaiCoroutine(int _positionNumber)
    {
        movingTimer = 0;
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = MahjongManager.Instance.GetPositionTehai(playerTehaiFlg, _positionNumber, false);

        yield return null;

        while (movingTimer < MOVE_TIME)
        {
            movingTimer += Time.deltaTime;

            var rate = movingTimer / MOVE_TIME;

            transform.localPosition = Vector3.Lerp(startPosition, endPosition, rate);

            yield return null;
        }
    }

    /// <summary>
    /// この牌のオブジェクトをタップしたとき
    /// </summary>
    public void TapThisObject()
    {
        if (playerTehaiFlg != MahjongManager.PlayerKind.Player) return;
        if (!interactableTapFlg) return;
        if (MahjongManager.Instance.GetPlayerTehaiComponent(MahjongManager.PlayerKind.Player).GetReachTurn() != MahjongManager.INDEX_NONE &&
            MahjongManager.Instance.GetPlayerTehaiComponent(MahjongManager.PlayerKind.Player).GetPaiStatusTehaiOfRight().totalNumber != totalNumber) return;

        MahjongManager.Instance.ReceptionPaiPrefab(totalNumber);
    }

    /// <summary>
    /// タップ可能かどうかを変更する
    /// </summary>
    /// <param name="_flg"></param>
    public void ChangeInteractableTap(bool _flg)
    {
        interactableTapFlg = _flg;
        ChangeColorGrey(!interactableTapFlg);
    }

    /// <summary>
    /// 牌の色をグレーにする
    /// </summary>
    /// <param name="_greyFlg"></param>
    public void ChangeColorGrey()
    {
        if (meshRendererOfPai.material.color == COLOR_DEFAULT)
        {
            ChangeColorGrey(true);
        }
        else
        {
            ChangeColorGrey(false);
        }
    }

    /// <summary>
    /// 牌の色をグレーにする
    /// </summary>
    /// <param name="_greyFlg"></param>
    public void ChangeColorGrey(bool _greyFlg)
    {
        if (_greyFlg)
        {
            meshRendererOfPai.material.color = COLOR_GREY;
            meshRendererOfGara.material.color = COLOR_GREY;
        }
        else
        {
            meshRendererOfPai.material.color = COLOR_DEFAULT;
            meshRendererOfGara.material.color = COLOR_DEFAULT;
        }
    }

    /// <summary>
    /// 炎のエフェクトを再生する
    /// </summary>
    /// <param name="_flg"></param>
    public void PlayParticleFire(bool _flg)
    {
        if (_flg)
        {
            particleFire.Play();
        }
        else
        {
            particleFire.Stop();
        }
    }

    /// <summary>
    /// オーラのエフェクトを再生する
    /// </summary>
    /// <param name="_flg"></param>
    public void PlayParticleAura(bool _flg)
    {
        if (_flg)
        {
            particleAura.Play();
        }
        else
        {
            particleAura.Stop();
        }
    }

    /// <summary>
    /// 設定されている牌種を返すゲッター
    /// </summary>
    /// <returns></returns>
    public MahjongManager.PaiKinds GetThisKind()
    {
        return thisKind;
    }

    /// <summary>
    /// トータルナンバーを返すゲッター
    /// </summary>
    /// <returns></returns>
    public int GetTotalNumber()
    {
        return totalNumber;
    }


    /// <summary>
    /// インスペクターのボタンを押したとき
    /// </summary>
    public void InspectorButtonFunction()
    {
        Debug.Log($"InspectorButtonFunction : Display PaiPrefab Status\n" +
            $"thisKind = {thisKind}\ntotalNumber = {totalNumber}\narrayNumber = {arrayNumber}");
    }
}

[CustomEditor(typeof(PaiPrefab))] // PaiPrefabを拡張する
public class PaiPrefabDisplayLog : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Display Log of Status"))
        {
            PaiPrefab yourScript = (PaiPrefab)target;
            yourScript.InspectorButtonFunction();
        }

        if (GUILayout.Button("Change Color"))
        {
            PaiPrefab yourScript = (PaiPrefab)target;
            yourScript.ChangeColorGrey();
        }
    }
}
