using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pusher01
{
    public class Medal : MonoBehaviour
    {
        private Rigidbody rb;

        private bool isPointClear = false;
        private bool isChanceClear = false;

        private float velocity = 0;
        private float stopTime = 0;

        private int layer;
        string layerName;

        private float mass;

        //private bool disappearFlg1 = false;
        //private bool disappearFlg2 = false;

        private void Start()
        {
            rb = this.GetComponent<Rigidbody>();
            mass = rb.mass;

            layer = this.gameObject.layer;
            layerName = LayerMask.LayerToName(layer);

            if(layerName == "Default")
            {
                isPointClear = true;
            }
            else if (layerName == "Player")
            {
                int x = Random.Range(-2000, 2000);
                int y = Random.Range(0, -1000);
                rb.AddForce(new Vector3(x, y, 0) * mass);
            }
            else
            {
                Debug.Log("Medal.Start: layerName is Anything");
            }

            // Debug.Log($"x , y = {x} , {y}");
        }

        private void Update()
        {
            if (!isPointClear)
            {
                MedalStopStop();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.gameObject.tag == "Point" && isPointClear == false)
            {
                isPointClear = true;

                this.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else if (trigger.gameObject.tag == "Chance" && isChanceClear == false) 
            {
                // Debug.Log("Chance");

                isChanceClear = true;
                GameManager.Instance.IncrementHoryuu(false);
            }
            else if (trigger.gameObject.tag == "Out")
            {
                // Debug.Log("Out");

                StartCoroutine(DisappearMedal());
            }
            else if (trigger.gameObject.tag == "Goal")
            {
                // Debug.Log("Goal");

                PlayerInformationManager.Instance.AcquisitionMedal(1);
                PlayerInformationManager.Instance.IncrementField("Pusher01Data", "getMedalTotal");
                SoundManager.Instance.PlaySE(SoundManager.SoundSeType.MedalDrop);
                StartCoroutine(DisappearMedal());
            }

        }

        /// <summary>
        /// ƒƒ_ƒ‹‚ª~‚Ü‚é‚Ì‚ğ–h‚®
        /// </summary>
        private void MedalStopStop()
        {
            velocity = rb.velocity.magnitude;
            // Debug.Log(velocity);
            if (velocity < 0.001f)
            {
                stopTime += Time.deltaTime;
            }
            else
            {
                stopTime = 0;
            }
            if (stopTime > 0.1f)
            {
                AddPowerOnNail(MakeRandomBool(), 50);
                stopTime = 0;
            }
        }

        /// <summary>
        /// ƒ‰ƒ“ƒ_ƒ€‚Åbool‚Ì’l‚ğ•Ô‚·
        /// </summary>
        /// <returns></returns>
        private bool MakeRandomBool()
        {
            if (0.5 < (Random.Range(0f, 1f)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ƒƒ_ƒ‹‚ª“B‚Ìã‚Éæ‚Á‚½‚Æ‚«‚É—^‚¦‚é—Í
        /// </summary>
        /// <param name="b">³‚È‚ç‰EA•‰‚È‚ç¶</param>
        /// <param name="power">—^‚¦‚é—Í‚Ì‹­‚³(1000‚ª•’Ê)</param>
        private void AddPowerOnNail(bool b, int power)
        {
            if (b)
            {
                // Debug.Log("Right");
                rb.AddForce(Vector3.right * power * mass);
            }
            else
            {
                // Debug.Log("Left");
                rb.AddForce(Vector3.left * power * mass);
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

            // yield return new WaitForSeconds(1);

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

