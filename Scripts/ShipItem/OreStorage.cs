using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreStorage : ShipItem
{
    [SerializeField] private float oreMax;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.oreMax += oreMax;
        base.changeShipStats(ship);
    }
}
