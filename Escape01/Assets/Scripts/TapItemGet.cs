using System;
using UnityEngine;

public class TapItemGet : TapCollider
{
    [SerializeField] private Item.Type itemType;
    [SerializeField] private AudioSource se = null;

    private Item item;

    protected new void Start()
    {
        base.Start();

        item = ItemGenerater.instance.Spawn(itemType);

        int prefsNum = PlayerPrefs.GetInt(Enum.GetName(typeof(Item.Type), itemType), 0);
        if (prefsNum > 0)
        {
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    protected override void OnTap()
    {
        if (ControlStopper.instance.GetIsControlStop()) return;

        base.OnTap();

        ItemBox.instance.SetItem(item);

        if (se != null)
        {
            se.Play();
        }

        ContinueManager.instance.SaveItemStatus(itemType, 1);

        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
