using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipItemSlot : MonoBehaviour
{
    [SerializeField] private ShipItem _placedItem;

    virtual public bool PlaceItem(ShipItem item){
        if(item == null) return false;
        _placedItem = item;
        return true;
    }

    public void RemoveItem(){
        _placedItem = null;
    }

    public ShipItem GetItem(){
        return _placedItem;
    }
}
