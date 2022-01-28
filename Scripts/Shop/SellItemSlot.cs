using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellItemSlot : ShipItemSlot
{

    override public bool PlaceItem(ShipItem item)
    {
        if (item == null) return false;
        Destroy(item.gameObject);
        return true;
    }
}
