using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Converter : ShipItem
{
    //X for energy, y for cost
    [SerializeField] private Vector2 converterEfficency;

    public override void changeShipStats(PlayerShip ship)
    {
        ship.converterEfficency += converterEfficency;
        base.changeShipStats(ship);
    }
}
