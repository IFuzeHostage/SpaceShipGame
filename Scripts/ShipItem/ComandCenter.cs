using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComandCenter : ShipItem
{
    [SerializeField] private int hullLimit;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.centeres++;
        ship.hullLimit = hullLimit;
        base.changeShipStats(ship);
    }
}
