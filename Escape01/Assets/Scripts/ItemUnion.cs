using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUnion : MonoBehaviour
{
    [SerializeField] private UnionItemListEntity unionItemListEntity;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private GameObject unionPanel;
    [SerializeField] private GameObject newItemImageObjects;
    [SerializeField] private GameObject[] unionImageObjects;

    [SerializeField] private AudioSource seDecideUnion = null;
    [SerializeField] private AudioSource seSucceseUnion = null;

    private float defaultUnionImagePosition = 300f;
    private float defaultNewItemImageSize = 450f;

    public static ItemUnion instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < unionImageObjects.Length; i++)
        {
            unionImageObjects[i].GetComponent<Image>().sprite = defaultSprite;
            unionImageObjects[i].GetComponent<Image>().color = Color.white;
            unionImageObjects[i].SetActive(false);

            if (i == 0)
            {
                unionImageObjects[i].GetComponent<Transform>().localPosition = new Vector3(-defaultUnionImagePosition, 0, 0);
            }
            else if (i == 1)
            {
                unionImageObjects[i].GetComponent<Transform>().localPosition = new Vector3(defaultUnionImagePosition, 0, 0);
            }
            else if (i == 2)
            {
                unionImageObjects[i].GetComponent<Transform>().localPosition = new Vector3(0, -defaultUnionImagePosition, 0);
            }
            else if (i == 3)
            {
                unionImageObjects[i].GetComponent<Transform>().localPosition = new Vector3(0, defaultUnionImagePosition, 0);
            }
            else
            {
                Debug.Log("Union Start Error");
            }
        }
        newItemImageObjects.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        newItemImageObjects.GetComponent<Image>().sprite = defaultSprite;
        newItemImageObjects.SetActive(false);
        unionPanel.SetActive(false);
    }

    /// <summary>
    /// �����̃A�C�e������ɍ��̂�����֐�
    /// </summary>
    /// <param name="item">�I�����ꂽ�A�C�e��</param>
    public void Union(Item item)
    {
        if (item.unionGroup == UnionItemStatus.Group.NoGroup) return;

        UnionItemStatus thisUnionGroup = null;

        foreach (var i in unionItemListEntity.UnionItemList)
        {
            if (item.unionGroup == i.group)
            {
                thisUnionGroup = i;
            }
        }

        if (thisUnionGroup == null)
        {
            Debug.Log("Union Error 01");
            return;
        }

        int count = 0;
        foreach (var itemslot in ItemBox.instance.itemSlots)
        {
            if (itemslot.GetHaveItem() != null)
            {
                if (itemslot.GetHaveItem().unionGroup == thisUnionGroup.group)
                {
                    count++;
                }

            }
        }

        Item unionItem = ItemGenerater.instance.StringToItem(thisUnionGroup.group.ToString());

        if (unionItem == null)
        {
            Debug.Log("Union Error 02");
            return;
        }

        //�A�C�e�����̂����������Ƃ�
        if (count == thisUnionGroup.num)
        {
            if (seDecideUnion != null)
            {
                seDecideUnion.Play();
            }
            StartCoroutine(UnionStaging(unionItem, thisUnionGroup));
        }

    }

    /// <summary>
    /// �A�C�e�������̂�����Ƃ��̉��o�������ǂ�֐�
    /// </summary>
    /// <param name="_UnionItem">���̐�̃A�C�e��</param>
    /// <param name="_UnionGroup">���̌��̃O���[�v</param>
    /// <returns></returns>
    private IEnumerator UnionStaging(Item _UnionItem, UnionItemStatus _UnionGroup)
    {
        Sprite[] spriteArray = new Sprite[_UnionGroup.num];
        int imgCount = 0;

        unionPanel.SetActive(true);
        newItemImageObjects.GetComponent<Image>().sprite = _UnionItem.sprite;

        foreach (var itemslot in ItemBox.instance.itemSlots)
        {
            if (itemslot.GetHaveItem() != null)
            {
                if (itemslot.GetHaveItem().unionGroup == _UnionGroup.group)
                {
                    unionImageObjects[imgCount].GetComponent<Image>().sprite = itemslot.GetHaveItem().sprite;
                    unionImageObjects[imgCount].SetActive(true);

                    StartCoroutine(MoveUp(unionImageObjects[imgCount]));

                    imgCount++;
                    ContinueManager.instance.SaveItemStatus(itemslot.GetHaveItem().type, 2);
                    ItemBox.instance.DeleteItem(itemslot);
                }
            }
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < _UnionGroup.num; i++)
        {
            //obj.SetActive(false);
            StartCoroutine(Transparent(unionImageObjects[i].GetComponent<Image>()));

        }

        yield return new WaitForSeconds(1.5f);
        if (seSucceseUnion != null)
        {
            seSucceseUnion.Play();
        }
        newItemImageObjects.SetActive(true);
        StartCoroutine(SizeBigger(newItemImageObjects.GetComponent<RectTransform>()));

        yield return new WaitForSeconds(2.0f);

        Start();

        ContinueManager.instance.SaveItemStatus(_UnionItem.type, 1);

        ItemBox.instance.SetItem(_UnionItem);
    }

    /// <summary>
    /// �I�u�W�F�N�g��(0,0,0)�֓������֐�
    /// </summary>
    /// <param name="obj">�����������I�u�W�F�N�g</param>
    /// <returns></returns>
    private IEnumerator MoveUp(GameObject obj)
    {
        Vector3 startPosition = obj.transform.localPosition;

        float presentLocation = 0f;
        float speed = 0.6f;

        while (presentLocation < 1f)
        {
            presentLocation += (Time.deltaTime * speed);

            obj.transform.localPosition = Vector3.Slerp(startPosition, Vector3.zero, presentLocation);

            yield return null;
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�����X�ɓ����ɂ���֐�
    /// </summary>
    /// <param name="img">�����ɂ������I�u�W�F�N�g��Image</param>
    /// <returns></returns>
    private IEnumerator Transparent(Image img)
    {
        for (int i = 0; i < 26; i++)
        {
            float alpha = img.color.a - (10f / 255f);
            img.color = new Color(1.0f, 1.0f, 1.0f, alpha);

            yield return new WaitForSeconds(0.02f);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�����X�ɑ傫������֐�
    /// </summary>
    /// <param name="tra">�傫���������I�u�W�F�N�g��RectTransform</param>
    /// <returns></returns>
    private IEnumerator SizeBigger(RectTransform tra)
    {
        float size = 0f;
        int roopTimes = 30;

        for (int i = 0; i < roopTimes; i++)
        {
            size = (defaultNewItemImageSize / roopTimes) * (i + 1);
            tra.sizeDelta = new Vector2(size, size);

            yield return new WaitForSeconds(0.02f);
        }
    }
}
