using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percussion : MonoBehaviour
{
    [SerializeField] private SoundManager.SoundSeType[] seType;

    private CinemachineImpulseSource vcamImpulseSource;

    private void Start()
    {
        vcamImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    /// <summary>
    /// 自身のオブジェクトを叩いた時
    /// </summary>
    public void TapMyObject()
    {
        if (MenuSceneManager.Instance.GetNowSelectMode() != MenuSceneManager.SelectMode.FirstMenu) return;

        if (vcamImpulseSource != null)
        {
            vcamImpulseSource.GenerateImpulse();
        }

        if (seType.Length > 0)
        {
            int num = Random.Range(0, seType.Length);
            SoundManager.Instance.PlaySE(seType[num]);
        }

    }
}
