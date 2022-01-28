using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    public bool ready = false;
    [SerializeField] private float crypto = 0;
    public bool firstMove = true;

    #region All Ship Stats (Variables) 
    //ComandCenter
    public int centeres;
    public int hullLimit;
    public int hullCount;
    //Durability
    public float durabilityCurrent;
    public float durabilityMax;
    //Engine
    public int engineCount;
    public float energyCostMove;
    public float energyCostBattle;
    //EnergyStorage
    public float energyCurrent;
    public float energyMax;
    //OreStorage
    public float oreCurrent;
    public float oreMax;
    //Canons
    public float canonsCost;
    public float canonsDamage;
    //Collector
    public bool hasCollector;
    public Vector2 collectEfficency;//X for produced, y for cost
    //Converter
    public bool hasConverter;
    public Vector2 converterEfficency;//X for produced, y for cost
    //Generator
    public bool hasGenerator;
    public Vector2 generatorEfficency;//X for produced, y for cost
    //Repairer
    public bool hasRepairer;
    public Vector2 repairEfficency;//X for produced, y for cost
    #endregion

    public static PlayerShip singleton;


    void Awake()
    {
        singleton = this;
        ResetShipStats();
    }
    #region Ship Stats Generation
    public void GenerateShipStats()
    {
        ResetShipStats();
        ShipItem[] shipItems = ShipBuilder.singleton.GetPlacedAugments();
        foreach (var item in shipItems)
        {
            item.changeShipStats(this);
        }

        if (firstMove )
        {
            durabilityCurrent = durabilityMax;
        }
        else {
            oreCurrent = oreCurrent > oreMax ? oreMax : oreCurrent;
            energyCurrent = energyCurrent > energyMax ? energyMax : energyCurrent;
            durabilityCurrent = durabilityCurrent > durabilityMax ? durabilityMax : durabilityCurrent;
        }

        ready = isShipReady();
        ShipStatWindow.singleton.UpdateText();
        UI.singleton.UpdateUIshipResources();
        UI.singleton.CheckEndgame();
    }

    //VERY BAD CODE
    private void ResetShipStats()
    {
        centeres = 0;
        hullLimit = 0;
        hullCount = 0;
        //Durability
        durabilityMax = 0;
        //Engine
        engineCount = 0;
        energyCostMove = 0;
        energyCostBattle = 0;
        //EnergyStorage
        energyMax = 0;
        //OreStorage
        oreMax = 0;
        //Canons
        canonsCost = 0;
        canonsDamage = 0;
        //Collector
        hasCollector = false;
        collectEfficency = Vector2.zero;
        //Converter
        hasConverter = false;
        converterEfficency = Vector2.zero;
        //Generator
        hasGenerator = false;
        generatorEfficency = Vector2.zero;
        //Repairer
        hasRepairer = false;
        repairEfficency = Vector2.zero;
    }

    public bool isShipReady()
    {
        if (centeres != 1)
        {
            UI.singleton.SendMessage("Ship's not ready! Exactly one comand center must ne placed!", Color.red);
            return false;
        }
        else if (energyMax <= 0)
        {
            UI.singleton.SendMessage("Ship's not ready! Place a Battery!", Color.red);
            return false;
        }
        else if(canonsDamage <= 0)
        {
            UI.singleton.SendMessage("Ship's not ready! Place a Canon!", Color.red);
            return false;
        }
        else if(oreMax <= 0)
        {
            UI.singleton.SendMessage("Ship's not ready! Place an Ore Storage!", Color.red);
            return false;
        }
        else if (collectEfficency == Vector2.zero)
        {
            UI.singleton.SendMessage("Ship's not ready! Place a collector!", Color.red);
            return false;
        }
        else if (engineCount < hullCount / 2)
        {
            UI.singleton.SendMessage($"Ship's not ready! Need at least {hullCount/2} Engines!", Color.red);
            return false;
        }
        else if (hullCount > hullLimit)
        {
            UI.singleton.SendMessage($"Ship's not ready! You placed more than {hullLimit} hulls!", Color.red);
            return false;
        }

        ShipBuilder shipBuilder = ShipBuilder.singleton;
        foreach (var augment in shipBuilder.GetPlacedAugments())
        {
            if (!shipBuilder.neigborsAllowed(augment.GetOccupiedSlots()[0], augment.cantPlaceNear))
            {
                var cantPlaceNearString = new StringBuilder();
                foreach(var item in augment.cantPlaceNear)
                {
                    cantPlaceNearString.Append($"{item}, ");
                }
                UI.singleton.SendMessage($"Ship's not ready! {augment.Name} can't be placed near {cantPlaceNearString.ToString()}", Color.red);
                return false;
            }
        }
        UI.singleton.SendMessage("Ship is ready!", Color.green);
        return true;
    }
    #endregion
    #region Crypto
    public void AddCrypto(float value)
    {
        crypto += value;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You recieved {value} crypto", Color.green);

        ShipStatWindow.singleton.UpdateText();
        UI.singleton.CheckEndgame();
    }

    public bool SpendCrypto(float value)
    {
        if (value > crypto)
        {
            UI.singleton.SendMessage($"Not enough Crypto! You're {value - crypto} off on crypto", Color.red);

            return false;
        }
        crypto -= value;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You spent {value} crypto");

        ShipStatWindow.singleton.UpdateText();

        UI.singleton.CheckEndgame();
        return true;
    }

    public float GetCrypto()
    {
        return crypto;
    }
    #endregion
    #region Ore
    public void AddOre(float ore)
    {
        oreCurrent += ore;
        oreCurrent = oreCurrent > oreMax ? oreMax : oreCurrent;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You recieved {ore} ore", Color.green);
    }

    public bool SpendOre(float value)
    {
        if (value > oreCurrent)
        {
            UI.singleton.SendMessage($"Not enough Ore! You're {value - oreCurrent} off on ore", Color.red);

            return false;
        }
        oreCurrent -= value;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You spent {value} ore");

        UI.singleton.CheckEndgame();
        return true;
    }

    public float GetOre()
    {
        return oreCurrent;
    }
    #endregion
    #region Energy
    public bool SpendEnergy(float energyRequired)
    {
        if (energyRequired > energyCurrent)
        {
            UI.singleton.SendMessage($"Not enough energy! You're {energyRequired-energyCurrent} off on energy", Color.red);

            return false;
        }
        energyCurrent -= energyRequired;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You spent {energyRequired} energy");

        UI.singleton.CheckEndgame();
        return true;

    }

    public void AddEnergy(float energy)
    {
        energyCurrent += energy;
        energyCurrent = energyCurrent > energyMax ? energyMax : energyCurrent;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You recieved {energy} energy", Color.green);

    }
    #endregion
    #region Durability
    public void TakeDamage(float amount)
    {
        durabilityCurrent -= amount;
        UI.singleton.UpdateUIshipResources();
        if (durabilityCurrent <= 0)
        {
            BattleManager.singleton.EndBattle(false);
            UI.singleton.GameLost();
        }
    }

    public void RepairDamage(float amount)
    {
        durabilityCurrent += amount;
        durabilityCurrent = durabilityCurrent > durabilityMax ? durabilityMax : durabilityCurrent;

        UI.singleton.UpdateUIshipResources();
        UI.singleton.SendMessage($"You Repaired {amount} of damage. Now tour ship is at {durabilityCurrent} durability!");
    }
    #endregion


    #region Augment methods
    public float EstimateEnergyCost(float distance)
    {
        return distance / 100 * energyCostMove;
    }

    public void ConvertOre()
    {
        UI.singleton.SendMessage($"Converting {converterEfficency.y} ore into {converterEfficency.x} MW...");

        if (SpendOre(converterEfficency.y))
        {
            AddEnergy(converterEfficency.x);
        }
    }

    public void Repair()
    {
        UI.singleton.SendMessage($"Spending {repairEfficency.y} MW to repair {repairEfficency.x} durability...");

        if (SpendEnergy(repairEfficency.y))
        {
            RepairDamage(repairEfficency.x);
        }
    }

    public void GenerateEnergy(float distance)
    {
        UI.singleton.SendMessage($"Generators produce some energy during flight...");

        AddEnergy((distance/100) * generatorEfficency.x);
    }
    #endregion
}
