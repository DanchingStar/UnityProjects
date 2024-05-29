using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DropBalls
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Slider myCameraHeightSlider;
        [SerializeField] private Slider myCameraDistanceSlider;

        [SerializeField] private Vector3 zoomHeightMaxPosition;
        [SerializeField] private Vector3 zoomHeightMidPosition;
        [SerializeField] private Vector3 zoomHeightMinPosition;
        [SerializeField] private Vector3 zoomDistanceMinPosition;

        [SerializeField] private GameObject cameraPanel;

        private Vector3 zoomDistanceMaxPosition;

        private float cameraHeightPosition;
        private float oldCameraHeightPosition = 0f;
        private const float DEFAULT_CAMERA_HEIGHT_POSITION = 0.9f;

        private float cameraDistancePosition;
        private float oldCameraDistancePosition = 0f;
        private const float DEFAULT_CAMERA_DISTANCE_POSITION = 0.9f;

        public static UIManager Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitSlider();
            cameraPanel.SetActive(false);
        }


        private void Update()
        {
            CameraPosition();
        }

        private void InitSlider()
        {
            myCameraHeightSlider.value = DEFAULT_CAMERA_HEIGHT_POSITION;
            cameraHeightPosition = DEFAULT_CAMERA_HEIGHT_POSITION;
            myCameraHeightSlider.onValueChanged.AddListener(value => cameraHeightPosition = value);

            myCameraDistanceSlider.value = DEFAULT_CAMERA_DISTANCE_POSITION;
            cameraDistancePosition = DEFAULT_CAMERA_DISTANCE_POSITION;
            myCameraDistanceSlider.onValueChanged.AddListener(value => cameraDistancePosition = value);
        }

        private void CameraPosition()
        {
            if ((oldCameraDistancePosition == cameraDistancePosition) && (oldCameraHeightPosition == cameraHeightPosition)) return;

            oldCameraDistancePosition = cameraDistancePosition;
            oldCameraHeightPosition = cameraHeightPosition;

            float _heightNum;
            float _heightCenter = 0.75f;
            if (cameraHeightPosition > _heightCenter)
            {
                _heightNum = (cameraHeightPosition - _heightCenter) / (1 - _heightCenter);
                zoomDistanceMaxPosition = Vector3.Lerp(zoomHeightMidPosition, zoomHeightMaxPosition, _heightNum);
            }
            else
            {
                _heightNum = cameraHeightPosition / _heightCenter;
                zoomDistanceMaxPosition = Vector3.Lerp(zoomHeightMinPosition, zoomHeightMidPosition, _heightNum);
            }

            mainCamera.transform.position = Vector3.Lerp(zoomDistanceMinPosition, zoomDistanceMaxPosition, cameraDistancePosition);
        }

        /// <summary>
        /// カメラのズーム変更パネルの表示非表示を変える
        /// </summary>
        public void ChangeCameraPanel()
        {
            cameraPanel.SetActive(!cameraPanel.activeSelf);
        }

        /// <summary>
        /// カメラのズーム変更パネルの表示非表示を変える
        /// </summary>
        /// <param name="flg">表示・非表示</param>
        public void ChangeCameraPanel(bool flg)
        {
            cameraPanel.SetActive(flg);
        }

    }
}