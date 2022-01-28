using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : ShipItem
{
    [SerializeField] private Vector2 generatorEfficency;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.generatorEfficency += generatorEfficency;
        base.changeShipStats(ship);
    }
}
