using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JanKen
{
    public class Medal : MonoBehaviour
    {
        private Rigidbody rb;

        private bool disappearFlg1 = false;
        private bool disappearFlg2 = false;
        private bool disappearFlg3 = false;

        private void Start()
        {
            rb = this.GetComponent<Rigidbody>();

            int x = Random.Range(-2000, 2000);
            int y = Random.Range(0, 1000);
            rb.AddForce(new Vector3(x, y, 0));

            // Debug.Log($"x , y = {x} , {y}");
        }

        private void Update()
        {
            if (disappearFlg3) return;

            if (disappearFlg1 == false && disappearFlg2 == false && GameManager.Instance.GetAbleStartFlg() == false)
            {
                disappearFlg1 = true;
            }
            else if (disappearFlg1 == true && disappearFlg2 == false && GameManager.Instance.GetAbleStartFlg() == true)
            {
                disappearFlg2 = true;
            }
            else if (disappearFlg1 == true && disappearFlg2 == true && GameManager.Instance.GetAbleStartFlg() == false) 
            {
                disappearFlg3 = true;
                StartCoroutine(DisappearMedal());
            }
        }

        /// <summary>
        /// ƒƒ_ƒ‹‚ªÁ‚¦‚é‚Æ‚«‚Ì‰‰o
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisappearMedal()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            Color[] colors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                colors[i] = materials[i].color;
            }

            yield return new WaitForSeconds(1);

            float num = 100f;
            float firstNum = num;
            float value;

            while (num > 1f)
            {
                value = num / firstNum;

                for (int i = 0; i < materials.Length; i++)
                {
                    colors[i].a = value;
                    materials[i].color = colors[i];
                }

                // Debug.Log($"{materials.Length} : {num} , {value}");

                num = num - 1f;
                yield return new WaitForSeconds(0.01f);
            }

            Destroy(this.gameObject);
        }

    }
}