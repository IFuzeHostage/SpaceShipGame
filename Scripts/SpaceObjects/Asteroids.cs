using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Asteroids : SpaceObject, IMineable
{
    private float ore;

    public static string[] moonNames = {"Dione", "Enceladus" ,"Hyperion", "Iapetus", "Mimas", "Phoebe", "Rhea", "Tethys", "Titan" };
                                       
    public float Mine(float amount)
    {
        if (amount > ore)
        {
            var oreLeft = ore;
            ore = 0;
            Destroy(this.gameObject);
            UI.singleton.UpdateSelectedUI();
            return oreLeft;
        }
        ore -= amount;
        UI.singleton.UpdateSelectedUI(this);
        return amount;

    }

    public float GetOre()
    {
        return ore;
    }

    override public void GenerateName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var builder = new StringBuilder();
        var rand = new System.Random();

        for(int i = 0; i < 8; i++)
        {
            builder.Append(chars[rand.Next(0, chars.Length)]);
        }

        _name = builder.ToString();
      
    }
    public string GetName()
    {
        return _name;
    }

    void Awake()
    {
        GenerateName();
        ore = 100 +new System.Random().Next(0, 900);
    }

}
