using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerParent : MonoBehaviour
{
    [SerializeField] protected AdMobBanner myBanner;
    [SerializeField] protected AdMobInterstitial myInterstitial;

    protected enum MyParameterEnum
    {
        HaveMoney,
        MotherMoney,
        HeartfulPoint,
        IsUncle,
        IsAunt,
        IsBrother,
        IsSister,
        IsCat,
        HatsuYume,
        Asobi,
        IsHorseLuck,
        IsGrandPaLuck,
        IsGrandMaLuck,
        FlgHorseClear,
    }
    protected enum MyItemFlagsEnum
    {
        ItemDrink,
        ItemCandy,
        ItemPen,
        ItemTicket,
        ItemStone,
        ItemBag,
        ItemBoard,
        ItemFace,
        ItemGlove,
        ItemDoll,
        ItemWallet,
        ItemFan,
        ItemEndingNormal,
        ItemEndingAnother,
        ItemEndingGood,
        ItemEndingBad,
        ItemEndingSP1,
        ItemEndingSP2,
    }
    protected enum RecordItemEnum
    {
        RecordItemDrink,
        RecordItemCandy,
        RecordItemPen,
        RecordItemTicket,
        RecordItemStone,
        RecordItemBag,
        RecordItemBoard,
        RecordItemFace,
        RecordItemGlove,
        RecordItemDoll,
        RecordItemWallet,
        RecordItemFan,
        RecordItemEndingNormal,
        RecordItemEndingAnother,
        RecordItemEndingGood,
        RecordItemEndingBad,
        RecordItemEndingSP1,
        RecordItemEndingSP2,
    }
}
