using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : ShipItem
{
    [SerializeField] private float energyMax;

    public override void changeShipStats(PlayerShip ship)
    {
        ship.energyMax += energyMax;
        base.changeShipStats(ship);
    }
}
