using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : ShipItem
{
    [SerializeField] public float energyCostMove;
    [SerializeField] public float energyCostBattle;
    public override void changeShipStats(PlayerShip ship)
    {
        ship.engineCount++;
        ship.energyCostMove += energyCostMove;
        ship.energyCostBattle += energyCostBattle;
        base.changeShipStats(ship);
    }
}
