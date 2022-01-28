using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStation : SpaceObject
{
    private void Awake()
    {
        _name = "Space Sation";
    }
    public string GetName()
    {
        return _name;
    }


    public static float EstimateSellPrice(float amount, bool delivery)
    {
        if (delivery) return 0;
        if (amount > 1 && amount < 100)
            return amount * 0.12f;
        else if (amount < 500 && amount >= 100)
            return amount * 0.1f;
        else if (amount < 1500 && amount >= 500)
            return amount * 0.08f;
        else if (amount >= 1500)
            return amount * 0.06f;
        return 0;
    }

    public static float EstimateEnergyPrice(float amount, bool delivery)
    {
        if (amount > 1 && amount < 100)
            return delivery? 0 : amount * 0.05f;
        else if (amount < 500 && amount >= 100)
            return delivery? 0 : amount * 0.04f;
        else if (amount < 1500 && amount >= 500)
            return delivery? amount * 0.04f : amount * 0.03f;
        else if (amount >= 1500)
            return delivery ? amount * 0.03f : amount * 0.01f;
        return 0;
    }


}
