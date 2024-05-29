using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pusher01
{
    public class UIAnimationPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI animationText;

        private GameObject animationTextObject;

        private const float OUT_OF_SCREEN_POSITION = 1000f;
        private Vector3 defaultTextPosition;
        private Vector3 startTextPosition;
        private Vector3 finishTextPosition;

        private void Start()
        {
            animationTextObject = animationText.gameObject;
            defaultTextPosition = animationTextObject.transform.position;
            startTextPosition = new Vector3(defaultTextPosition.x + OUT_OF_SCREEN_POSITION,
                defaultTextPosition.y, defaultTextPosition.z);
            finishTextPosition = new Vector3(defaultTextPosition.x - OUT_OF_SCREEN_POSITION,
                defaultTextPosition.y, defaultTextPosition.z);

            animationTextObject.SetActive(false);
        }


        /// <summary>
        /// 獲得メダル数表示のアニメーション
        /// </summary>
        public void AcquisitionMedalAnimation(int num)
        {
            animationText.fontSize = 200;
            animationText.color = Color.yellow;
            animationText.text = $"{num} 枚\n獲得!!";

            StartCoroutine(AcquisitionMedalAnimationCoroutine());
        }

        /// <summary>
        /// 獲得メダル数表示のアニメーションのコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator AcquisitionMedalAnimationCoroutine()
        {
            animationTextObject.SetActive(true);

            float speed = 1.5f;
            float timer = 0f;
            while (timer <= 1f)
            {
                animationTextObject.transform.position = Vector3.Lerp(startTextPosition, defaultTextPosition, timer);

                timer += Time.deltaTime * speed;

                yield return null;
            }

            yield return new WaitForSeconds(1f);

            timer = 0f;
            while (timer <= 1f)
            {
                animationTextObject.transform.position = Vector3.Lerp(defaultTextPosition, finishTextPosition, timer);

                timer += Time.deltaTime * speed;

                yield return null;
            }

            animationTextObject.SetActive(false);

            yield return null;
        }

        /// <summary>
        /// リーチのアニメーション
        /// </summary>
        public void ReachAnimation()
        {
            animationText.fontSize = 150;
            animationText.color = Color.red;
            animationText.text = "リーチ!!";

            StartCoroutine(ReachAnimationCoroutine());
        }

        /// <summary>
        /// リーチのアニメーションのコルーチン
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReachAnimationCoroutine()
        {
            animationTextObject.SetActive(true);

            float speed = 1.5f;
            float timer = 0f;
            while (timer <= 1f)
            {
                animationTextObject.transform.position = Vector3.Lerp(startTextPosition, defaultTextPosition, timer);

                timer += Time.deltaTime * speed;

                yield return null;
            }

            yield return new WaitForSeconds(1f);

            timer = 0f;
            while (timer <= 1f)
            {
                animationTextObject.transform.position = Vector3.Lerp(defaultTextPosition, finishTextPosition, timer);

                timer += Time.deltaTime * speed;

                yield return null;
            }

            animationTextObject.SetActive(false);

            GameManager.Instance.SetReachFlg(false);

            yield return null;
        }

    }
}
