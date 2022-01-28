using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ShipStatWindow : MonoBehaviour
{
    [SerializeField] PlayerShip ship;
    private TextMeshPro textMesh;

    public static ShipStatWindow singleton;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshPro>();
        singleton = this;
    }
    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        var newText = new StringBuilder();
        newText.Append($"Crypto: {ship.GetCrypto()}$\n");
        newText.Append($"Command Centeres: {ship.centeres}\n");
        newText.Append($"Hull limit: {ship.hullLimit}\n");
        newText.Append($"Max Durability: {ship.durabilityMax}\n");
        newText.Append($"Engines: {ship.engineCount}\n");
        newText.Append($"Energy Costs:\n");
        newText.Append($"Battle: {ship.energyCostBattle}\n");
        newText.Append($"Move: {ship.energyCostMove}/100km\n");
        newText.Append($"Max Energy: {ship.energyMax}\n");
        newText.Append($"Max Ore: {ship.oreMax}\n");
        newText.Append($"Canons:\n");
        newText.Append($"Damage: {ship.canonsDamage}/shot\n");
        newText.Append($"Cost: {ship.canonsCost}/shot\n");
        newText.Append($"Collector Efficency:\n");
        newText.Append($"{ship.collectEfficency.x} Ore/{ship.collectEfficency.y} MW\n");
        newText.Append($"Converter Efficency:\n");
        newText.Append($"{ship.converterEfficency.x} MW/{ship.converterEfficency.y} Ore\n");
        newText.Append($"Generator Efficency:\n");
        newText.Append($"{ship.generatorEfficency.x} MW/{ship.generatorEfficency.y} km of flight\n");
        newText.Append($"Repair Efficency:\n");
        newText.Append($"{ship.repairEfficency.x} Dur./{ship.repairEfficency.y} MW\n");
        newText.Append(ship.ready ? "Ship is Ready for flight!" : "Ship is not ready!");
        textMesh.text = newText.ToString();
    }
}
