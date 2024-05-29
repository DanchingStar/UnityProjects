using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : ManagerParent
{
    public struct ItemInformationString
    {
        public string itemName;
        public string itemGetInformation;
        public string itemNoneInformation;
    }

    [SerializeField] private GameObject BGMObject;
    [SerializeField] private GameObject SEObjectParent;

    [SerializeField] private Text textAchievementRate;

    [SerializeField] private GameObject informationPanel;
    [SerializeField] private Image informationItemImage;
    [SerializeField] private Text itemImageNameText;
    [SerializeField] private Text itremImageInformationText;

    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Image[] itemImage;

    private AudioSource myAudioBGM;
    private Transform myAudioTF;

    private int[] recordItemFlags = new int[Enum.GetNames(typeof(RecordItemEnum)).Length];

    private bool informationFlg = false;

    private int itemCount;
    private int totalItemCount;
    private float achievementRate;

    private ItemInformationString[] itemInformationString = new ItemInformationString[Enum.GetNames(typeof(RecordItemEnum)).Length];


    // Start is called before the first frame update
    void Start()
    {
        {
            myBanner.RequestBanner();

            LoadPrefabs();
            InformationSetting();
            DisplayItemImage();

            AchievementRateTextSetting();

            InitSound();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void InitSound()
    {
        myAudioBGM = BGMObject.GetComponent<AudioSource>();
        myAudioTF = SEObjectParent.GetComponent<Transform>();

        myAudioBGM.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        foreach (Transform childTF in myAudioTF)
        {
            childTF.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
        }

    }

    private void LoadPrefabs()
    {
        itemCount = 0;
        totalItemCount = 0;
        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            recordItemFlags[(int)i] = PlayerPrefs.GetInt(i.ToString(), 0);
            totalItemCount++;
            if (recordItemFlags[(int)i]==1)
            {
                itemCount++;
            }

            Debug.Log($"{i.ToString()} : {recordItemFlags[(int)i]}");
        }
        achievementRate = (float)itemCount / (float)totalItemCount * 100f;
    }

    private void AchievementRateTextSetting()
    {
        textAchievementRate.text = "�B���� : " + achievementRate.ToString("F0") + "%";

        if (achievementRate >= 99)
        {
            StartCoroutine(_rainbow(textAchievementRate, 3f));
        }
        else if (achievementRate >= 80)
        {
            textAchievementRate.color = Color.red;
        }
        else if (achievementRate >= 50)
        {
            textAchievementRate.color = new Color(0.5f,0f,1f,1f);
        }
        else
        {
            textAchievementRate.color = Color.blue;
        }
    }

    private void DisplayItemImage()
    {
        foreach (var i in Enum.GetValues(typeof(RecordItemEnum)))
        {
            if (recordItemFlags[(int)i] == 1)
            {
                itemImage[(int)i].sprite = itemSprites[(int)i];
            }
            else
            {
                itemImage[(int)i].sprite = noneSprite;
            }
        }
    }
    public void OnImageObject(int n)
    {
        if (informationFlg) return;
     
        if (recordItemFlags[n] ==1)
        {
            informationItemImage.sprite = itemSprites[n];
            itemImageNameText.text = itemInformationString[n].itemName;
            itremImageInformationText.text = itemInformationString[n].itemGetInformation;
        }
        else
        {
            informationItemImage.sprite = noneSprite;
            itemImageNameText.text = "�H�H�H";
            itremImageInformationText.text = itemInformationString[n].itemNoneInformation;
        }

        informationFlg = true;
        informationPanel.SetActive(true);

    }

    public void OnToBackButton()
    {
        informationFlg = false;
        informationPanel.SetActive(false);
    }

    public void OnSNSButton()
    {
        string url = "";
        //string image_path = "";
        string text = "";

        string str1 = "�R���N�V�����B���� : " + achievementRate.ToString("F0") + "%!\n";

        string str2 = "";
        if (achievementRate >= 99f)
        {
            str2 = "�R���v���[�g���߂łƂ�!!!\n";
        }
        else if(achievementRate >= 80f)
        {
            str2 = "�R���v���[�g�܂ł��Ə���!!\n";
        }
 
        string str9 = "#���N�ʃA�h�x���`���[";

        text = str1 + str2 + str9;

        if (Application.platform == RuntimePlatform.Android)
        {
            url = "https://play.google.com/store/apps/details?id=com.DanchingStar.Otoshidama";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + " #Android\n";
            Debug.Log("Android");
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            text = text + " #iPhone\n";
            Debug.Log("iPhone");
        }
        else
        {
            url = "https://www.google.com";
            //image_path = Application.persistentDataPath + "/SS.png";
            //text = text + "���̑��̋@��\n";
            Debug.Log("Other OS");
        }

        SocialConnector.SocialConnector.Share(text, url); //��1����:�e�L�X�g,��2����:URL,��3����:�摜
    }
    public void OnToMenuButton()
    {
        FadeManager.Instance.LoadScene("Menu", 0.3f);
    }

    IEnumerator _rainbow(Text text, float speed)
    {
        //�������[�v
        while (true)
        {
            //�J���[��ω������鏈��
            text.color = Color.HSVToRGB(Time.time / speed % 1, 1, 1);
            //1�t���[���҂�
            yield return new WaitForFixedUpdate();
        }
    }

    private void InformationSetting()
    {
        itemInformationString[0].itemName = "�h�{�h�����N";
        itemInformationString[1].itemName = "�A�������";
        itemInformationString[2].itemName = "�Â����N�M";
        itemInformationString[3].itemName = "�V���n�̃`�P�b�g";
        itemInformationString[4].itemName = "�L���L��������";
        itemInformationString[5].itemName = "�V�����_�[�o�b�O";
        itemInformationString[6].itemName = "�H�q��";
        itemInformationString[7].itemName = "���΂�";
        itemInformationString[8].itemName = "�싅�̃O���[�u";
        itemInformationString[9].itemName = "���܂̂ʂ������";
        itemInformationString[10].itemName = "��l���ۂ����z";
        itemInformationString[11].itemName = "���X���[�̉���������";
        itemInformationString[12].itemName = "�~���������Q�[���@";
        itemInformationString[13].itemName = "�p�p�����������Q�[���@";
        itemInformationString[14].itemName = "�}�}���҂�ł��ꂽ�}�t���[";
        itemInformationString[15].itemName = "�n�Y���n��";
        itemInformationString[16].itemName = "�����̂�����";
        itemInformationString[17].itemName = "���킢���a�َq";

        itemInformationString[0].itemNoneInformation = "12��31���ɃQ�b�g�\";
        itemInformationString[1].itemNoneInformation = "12��31���ɃQ�b�g�\";
        itemInformationString[2].itemNoneInformation = "1��1���ɃQ�b�g�\";
        itemInformationString[3].itemNoneInformation = "1��1���ɃQ�b�g�\";
        itemInformationString[4].itemNoneInformation = "1��1���ɃQ�b�g�\";
        itemInformationString[5].itemNoneInformation = "1��2���ɃQ�b�g�\";
        itemInformationString[6].itemNoneInformation = "1��2���ɃQ�b�g�\";
        itemInformationString[7].itemNoneInformation = "1��2���ɃQ�b�g�\";
        itemInformationString[8].itemNoneInformation = "1��2���ɃQ�b�g�\";
        itemInformationString[9].itemNoneInformation = "1��2���ɃQ�b�g�\";
        itemInformationString[10].itemNoneInformation = "1��3���ɃQ�b�g�\";
        itemInformationString[11].itemNoneInformation = "1��4���ɃQ�b�g�\";
        itemInformationString[12].itemNoneInformation = "�m�[�}���G���f�B���O���B�ŃQ�b�g�\";
        itemInformationString[13].itemNoneInformation = "�m�[�}���G���f�B���O�A�i�U�[���B�ŃQ�b�g�\";
        itemInformationString[14].itemNoneInformation = "�O�b�h�G���f�B���O���B�ŃQ�b�g�\";
        itemInformationString[15].itemNoneInformation = "�o�b�h�G���f�B���O���B�ŃQ�b�g�\";
        itemInformationString[16].itemNoneInformation = "���ʃG���f�B���O1���B�ŃQ�b�g�\";
        itemInformationString[17].itemNoneInformation = "���ʃG���f�B���O2���B�ŃQ�b�g�\";

        itemInformationString[0].itemGetInformation = "�c���������������ݕ��B���̋@�Ƃ��ł��܂Ɍ������邯�ǁA���̈��ݕ��Ɣ�ׂē��e�ʂ����Ȃ��̂ɒl�i���ς��Ȃ��̂Ȃ񂾂����������Ȃ��������ˁB";
        itemInformationString[1].itemGetInformation = "�c�ꂩ���������܂�܂�L�����f�B�B�A�������ƌĂԂ̂͊֐����̐l�ɑ����C���[�W�B���̃Q�[���̒��ł����Ƃ������ł����炦�����ȕ��͂��ꂩ������Ȃ��ˁB";
        itemInformationString[2].itemGetInformation = "�c�������������M�L�p��B�ŋ߂ł͖��N�M�����ۂɌ������ƂȂ��l�������̂ł́H���i����g�����Ȃ��Ă���l���ăG���K���g�ŃJ�b�R������ۂ��������Ⴂ�܂��B";
        itemInformationString[3].itemGetInformation = "�c�ꂩ���������`�P�b�g�B�����łȂ��Ȃ��ł��I���̒��ɂ͂����ȃ`�P�b�g�����邯�ǁA���l�ɔz����S����������`�P�b�g���Ăȁ`�񂾁H������[�G�`�P�b�g]�ł����I";
        itemInformationString[4].itemGetInformation = "�c����̉ƂɗV�тɗ����O�єL�����������P���΁B��ӂȂǂɂ���΂��ΓX�Ŏ�舵���Ă���΂̈Ⴂ�͉��Ȃ̂��낤���B�����ځH���A�x�H�����Đ搶(^^)/";
        itemInformationString[5].itemGetInformation = "�f�ꂩ���������o�b�O�B�o�b�O�ɂ����낢��Ȏ�ނ����邯�ǁA�قډ�������Ȃ��傫���̃}�C�N���o�b�O�Ƃ������̑��݂ɂ͋������ˁB���̊T�O�𕢂����z�͂ɒE�X�B";
        itemInformationString[6].itemGetInformation = "�Z�Ǝo�����������H�q�B�H�q�̎g�����Ƃ����ΉH��������ʓI���Ǝv���Ă������ǁA�����̏���Ƃ��ėp����Ƃ����g����������炵���ˁB�E�`�ɂ͖��������ȁB";
        itemInformationString[7].itemGetInformation = "�����������������΂��B��Ԃ�[������]��[�Ђ���Ƃ�]���ȁH�F�B��Z��ȂǁA�m�l�̊�ŕ��΂�������ėV�Ԃ̂��ʔ�����I�����A�{�l�ɂ͋�������Ċy�����ˁI";
        itemInformationString[8].itemGetInformation = "�Z�����������싅�̃O���[�u�B�������΂���ŐV�i�̃O���[�u���Ǝ�ɓ���܂Ȃ��Ďg���Â炢��ˁB�g���₷���O���[�u�͂���������K���Ă����؂Ȃ̂����H";
        itemInformationString[9].itemGetInformation = "�o�������������܂̂ʂ�����݁B�Q�[���Z���^�[�Ȃǂ̃N���[���Q�[���ł͍��ł��ʂ�����݂���Ԃ��ȁH����A������Ȃ����H��x���炢�l���Ă݂����Ȃ��c�B";
        itemInformationString[10].itemGetInformation = "�����������������z�B��l�ɂȂ��Ă���ƃJ�[�h�ނ������Ă��č��z���傫���Ȃ肪���B�L���b�V�����X������J�[�h���X�����i�ނƍ��z���������Ȃ��Ă����̂��ȁH";
        itemInformationString[11].itemGetInformation = "������������������B�u�A�C�h���̉����ł͂悭�������邯�ǁA�v�����X�̉����ł�������Ȃ�Ďg���́H�v�Ǝv���Ă��邻���̂��Ȃ��I���ۂɎg���l�������ł���I";
        itemInformationString[12].itemGetInformation = "���N�ʂŔ������Q�[���@�B�ŋ߂̓X�}�z�����y���āA�g�уQ�[���@�����Ȃ����X���B����̎q�ǂ����񂩂炷���[�ʐM�P�[�u��]�Ƃ��M�����Ȃ��񂾂낤�Ȃ��c�B";
        itemInformationString[13].itemGetInformation = "���������Ă��ꂽ�Q�[���@�B�Q�[���@�Ɍ��炸�Ԃ�X�}�z�ȂǁA���g�͑S�������@�\�ł��A�J���[�̃o���G�[�V�������������邾���ŁA���[�U�[�͑I�Ԃ̃��N���N�����ˁB";
        itemInformationString[14].itemGetInformation = "�ꂪ��Ȃׂ��ĕ҂�ł��ꂽ�}�t���[�B�ꂩ�牽�����������u�܂������Ă�邩�v���ĂȂ邩�Ȃƍl���ďo�Ă����̂��}�t���[�������B���Ȃ݂ɍ�҂Ȃ��΂ɋ����Ȃ��B";
        itemInformationString[15].itemGetInformation = "�����牟���t����ꂽ�S�~�B���[�X�̌��ʎ���ł͂���ɂȂ肦��B�����������ŉ��l�������܂ő傫���ς����̂͑��ɂ��邾�낤���H����A�Ȃ��B�i�Õ��̔���I�ȕ��j";
        itemInformationString[16].itemGetInformation = "�m��Ȃ��j������������������B�Ȃ񂾂��񂾍ŋ߂ł͖{���f�[�^���������A������������Ă���l�������Ă����̂ł́H����ɂ��Ă����̂���ςł��ˁB";
        itemInformationString[17].itemGetInformation = "�m��Ȃ����������������a�َq�B�������ł͂Ȃ������ڂɂ��Â��Ă���a�َq����������ƁA�E�l�̏�M���������ˁB�܂�����͘a�َq�Ɍ������b����Ȃ����B";
    }
}
