using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// ‰æ–Ê‚ğ—h‚ç‚·ŠÖ”
    /// </summary>
    /// <param name="duration">—h‚ê‚éŠÔ</param>
    /// <param name="magnitude">—h‚ê‚Ì‹K–Í</param>
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        var pos = transform.localPosition;

        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = pos;
    }
}