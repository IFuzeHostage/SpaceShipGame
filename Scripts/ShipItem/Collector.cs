using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : ShipItem
{
    [SerializeField] private Vector2 collectEfficency;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.collectEfficency += collectEfficency;
        base.changeShipStats(ship);
    }
}
