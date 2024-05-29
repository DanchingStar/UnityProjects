using UnityEngine;

public class TapEffect : MonoBehaviour
{
    public GameObject effectPrefab; // 2D�G�t�F�N�g�̃v���n�u
    public Canvas canvas; // UI��Canvas

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;

            // UI���W�ɕϊ�
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out uiPosition);

            SpawnEffect(uiPosition);
        }
    }

    void SpawnEffect(Vector2 position)
    {
        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        effect.transform.SetParent(canvas.transform, false); // UI�I�u�W�F�N�g�̎q�v�f�Ƃ��Đݒ�
        // �G�t�F�N�g�̃J�X�^�}�C�Y�i�T�C�Y��F�Ȃǁj���K�v�ȏꍇ�͂����ōs��
        // ��: effect.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}