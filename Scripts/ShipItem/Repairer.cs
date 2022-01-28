using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repairer : ShipItem
{
    [SerializeField] private Vector2 repairEfficency;

    public override void changeShipStats(PlayerShip ship)
    {
        ship.repairEfficency += repairEfficency;
        base.changeShipStats(ship);
    }
}
