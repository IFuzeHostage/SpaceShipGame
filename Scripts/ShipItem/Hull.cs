using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull : ShipItem
{
    void OnMouseUp(){
        Place(transform.position);
    } 

    override protected ShipItemSlot[] GetNewSlots(Vector3 newPosition){
        return ShipBuilder.singleton.GetSlotForHull(newPosition, size);
    }

    public override void changeShipStats(PlayerShip ship)
    {
        ship.hullCount++;
        base.changeShipStats(ship);
    }
}
