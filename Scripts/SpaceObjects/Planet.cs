using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : SpaceObject, IMineable
{
    public static string[] moonNames = { "Dione", "Enceladus", "Hyperion", "Iapetus", "Mimas", "Phoebe", "Rhea", "Tethys", "Titan" };
    public static System.Random rand = new System.Random();
    public float Mine(float amount)
    {
        //CallPirates
        var playerShip = PlayerShip.singleton;
        if (playerShip.SpendEnergy(playerShip.collectEfficency.y))
            if (rand.Next(0, 100) < 40)
                UI.singleton.UIStartBattle();
            return amount;
        return 0;
    }

    override public void GenerateName()
    {
        _name = moonNames[rand.Next(0, moonNames.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateName();
    }

    public string GetName()
    {
        return _name;
    }

    public float GetOre()
    {
        return -1;
    }

}
