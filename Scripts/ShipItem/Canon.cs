using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : ShipItem
{
    [SerializeField] private float canonCost;
    [SerializeField] private float canonDamage;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.canonsCost += canonCost;
        ship.canonsDamage += canonDamage;
        base.changeShipStats(ship);
    }
}
