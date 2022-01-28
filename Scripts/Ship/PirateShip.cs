using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PirateShip : MonoBehaviour
{
    private float canonDamage;
    private float durability;
    private System.Random rand = new System.Random();

    public PirateShip(int tier)
    {
        for (int i = 0; i < rand.Next(1, 4 * tier); i++)
        {
            durability += 100 * tier;
        }

        for (int i = 0; i < rand.Next(1, 2 * tier); i++)
        {
            canonDamage += 50;
        }
    }

    public void TakeDamage(float damage)
    {
        durability -= damage;
        if (durability <= 0)
        {
            Die();
        }
    }

    public float GetDamage()
    {
        return canonDamage;
    }

    public float GetDurability()
    {
        return durability;
    }

    private void Die()
    {
        BattleManager.singleton.EndBattle(true);
    }
}