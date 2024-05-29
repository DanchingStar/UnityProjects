using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] private Text versionText;
    [SerializeField] private GameObject myStartConfirmationPanel;
    [SerializeField] private GameObject myOptionPanel;
    [SerializeField] private GameObject myCreditPanel;

    [SerializeField] private Slider myBGMSlider;
    [SerializeField] private Slider mySESlider;
    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    [SerializeField] private Button continueButton;

    [SerializeField] private ScrollRect creditTextScrollRect;

    [SerializeField] private AudioSource seYes = null;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 beforePosition;
    [SerializeField] private Vector3 beforeRotation;
    [SerializeField] private Vector3 afterPosition;
    [SerializeField] private Vector3 afterRotation;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 startRotation;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;
    private Transform cameraTransform;

    private bool tapGameStart = false;

    private bool isTapDisable = false;

    private bool isContinue;

    private enum CameraTiming
    {
        Awake,
        GameStart,
    }

    void Start()
    {
        //myBanner.RequestBanner();

        versionText.text = $"Ver {Application.version}";

        InitCamera();
        InitSlider();
        InitContinueButton();
    }

    private void InitCamera()
    {
        cameraTransform = mainCamera.gameObject.GetComponent<Transform>();
        StartCoroutine(CameraMove(CameraTiming.Awake));
    }

    private IEnumerator CameraMove(CameraTiming timing)
    {
        float presentLocation = 0f;
        float speed;

        if (timing == CameraTiming.Awake)
        {
            speed = 0.25f;
            while (presentLocation < 1 && tapGameStart == false)
            {
                //presentLocation += (Time.deltaTime * speed);
                //cameraTransform.position = Vector3.Slerp(beforePosition, afterPosition, presentLocation);
                //cameraTransform.rotation = Quaternion.Lerp(Quaternion.Euler(beforeRotation), Quaternion.Euler(afterRotation), presentLocation);

                float curveValue = Mathf.Sin(presentLocation * Mathf.PI * 0.5f);
                cameraTransform.position = Vector3.Lerp(beforePosition, afterPosition, curveValue);
                cameraTransform.rotation = Quaternion.Lerp(Quaternion.Euler(beforeRotation), Quaternion.Euler(afterRotation), curveValue);
                presentLocation += Time.deltaTime * speed;

                yield return null;
            }
        }
        else if (timing == CameraTiming.GameStart)
        {
            bool flg = false;
            Vector3 nowPosition = cameraTransform.position;
            Quaternion nowRotation = cameraTransform.rotation;
            speed = 0.75f;
            while (presentLocation < 1)
            {
                presentLocation += (Time.deltaTime * speed);
                cameraTransform.position = Vector3.Slerp(nowPosition, startPosition, presentLocation);
                cameraTransform.rotation = Quaternion.Lerp(nowRotation, Quaternion.Euler(startRotation), presentLocation);

                if (flg == false && presentLocation > 0.75f) 
                {
                    flg = true;
                    FadeManager.Instance.LoadScene("Game", 0.3f);
                }

                yield return null;
            }
        }
    }

    private void InitSlider()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myBGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        mySESlider.value = PlayerPrefs.GetFloat("SEVolume", 0.5f);

        myAudioBGM.volume = myBGMSlider.value;
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = mySESlider.value;
        }
        //captionSpeed = mySpeedSlider.value;

        myBGMSlider.onValueChanged.AddListener(value => myAudioBGM.volume = value);
        foreach (Transform childTF in myAudioTF)
        {
            mySESlider.onValueChanged.AddListener(value => childTF.gameObject.GetComponent<AudioSource>().volume = value);
        }
        //mySpeedSlider.onValueChanged.AddListener(value => captionSpeed = value);
    }

    private void SetSliderPrefs()
    {
        PlayerPrefs.SetFloat("BGMVolume", myBGMSlider.value);
        PlayerPrefs.SetFloat("SEVolume", mySESlider.value);
        PlayerPrefs.Save();
    }

    private void InitContinueButton()
    {
        isContinue = ContinueManager.instance.GetIsContinue();
        if (isContinue)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    public void OnStartButton()
    {
        if (isTapDisable) return;

        if (seYes != null)
        {
            seYes.Play();
        }

        if (isContinue)
        {
            isTapDisable = true;
            myStartConfirmationPanel.SetActive(true);
        }
        else
        {
            ToGameScene();
        }
    }

    public void OnStartConfirmationYesButton()
    {
        if (seYes != null)
        {
            seYes.Play();
        }

        myStartConfirmationPanel.SetActive(false);

        ContinueManager.instance.StartFromTheBeginning();
        ToGameScene();
    }

    public void OnStartConfirmationNoButton()
    {
        isTapDisable = false;

        if (seYes != null)
        {
            seYes.Play();
        }

        myStartConfirmationPanel.SetActive(false);
    }

    public void OnContinueButton()
    {
        if (isTapDisable) return;

        if (seYes != null)
        {
            seYes.Play();
        }

        PlayerPrefs.SetInt("IsContinue", 2);
        PlayerPrefs.Save();
        ToGameScene();
    }

    private void ToGameScene()
    {
        tapGameStart = true;
        isTapDisable = true;
        StartCoroutine(CameraMove(CameraTiming.GameStart));
    }

    public void OnOptionButton()
    {
        if (isTapDisable) return;

        isTapDisable = true;

        if (seYes != null)
        {
            seYes.Play();
        }

        myOptionPanel.SetActive(true);
    }

    public void OnCreditButton()
    {
        if (isTapDisable) return;

        isTapDisable = true;

        if (seYes != null)
        {
            seYes.Play();
        }

        myCreditPanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(creditTextScrollRect.verticalScrollbar.gameObject);
        creditTextScrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnOptionBackButton()
    {
        isTapDisable = false;

        if (seYes != null)
        {
            seYes.Play();
        }

        SetSliderPrefs();
        myOptionPanel.SetActive(false);
    }

    public void OnCreditBackButton()
    {
        isTapDisable = false;

        if (seYes != null)
        {
            seYes.Play();
        }

        myCreditPanel.SetActive(false);
    }

}
