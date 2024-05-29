using UnityEngine;

public class TapEffect : MonoBehaviour
{
    public GameObject effectPrefab; // 2Dエフェクトのプレハブ
    public Canvas canvas; // UIのCanvas

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            // UI座標に変換
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out uiPosition);

            SpawnEffect(uiPosition);
        }
    }

    void SpawnEffect(Vector2 position)
    {
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        effect.transform.SetParent(canvas.transform, false); // UIオブジェクトの子要素として設定
        // エフェクトのカスタマイズ（サイズや色など）が必要な場合はここで行う
        // 例: effect.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}