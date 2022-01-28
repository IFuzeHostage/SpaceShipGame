using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private PlayerShip playerShip;
    private PirateShip pirateShip;

    private bool inBattle = false;
    private bool shotFired = false;
    private bool playerTurn = false;
    private int battleCount = 1;
    private StringBuilder battleLog;

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject playerShipSprite;
    [SerializeField] private GameObject pirateShipSprite;



    public static BattleManager singleton;
    // Start is called before the first frame update

    private void Awake()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inBattle) return;
        var step = 3 * Time.deltaTime;
        Vector3 projDirection;
        if (!playerTurn)
        {
            projDirection = playerShipSprite.transform.position;
        }
        else
        {
            projDirection = pirateShipSprite.transform.position;
        }
        projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, projDirection, step);

        if(Vector3.Distance(projectile.transform.position, projDirection) < 0.001f)
        {
            String message;
            if (playerTurn)
            {
                pirateShip.TakeDamage(playerShip.canonsDamage);
                message = $"Pirate ship is hit for {playerShip.canonsDamage} damage! {pirateShip.GetDurability()} durability left!\n";
                playerTurn = false;
            }
            else
            {
                playerShip.TakeDamage(pirateShip.GetDamage());
                message = $"Player ship is hit for {playerShip.canonsDamage} damage! {playerShip.durabilityCurrent} durability left!\n";
                playerTurn = true;
            }
            battleLog.Append(message);
            UI.singleton.SendMessage(message);
        }
    }

    public void InitializeBattle()
    {
        playerShip = PlayerShip.singleton;
        pirateShip = new PirateShip(UI.singleton.shopTier);
        inBattle = true;
        battleLog = new StringBuilder();
        var message = $"Battle {battleCount} Log:\n";

        battleLog.Append(message);
        UI.singleton.SendMessage(message);
    }

    public void EndBattle(bool victory)
    {
        var winLine = victory ? "Player won!" : "Pirates won!";
        var color = victory ? Color.green : Color.red;
        var message = $"Battle ended! {winLine}";
        battleLog.Append(message);
        UI.singleton.SendMessage(message, color);

        File.WriteAllText($"Battle{battleCount}.txt", battleLog.ToString());

        if (victory)
        {
            playerShip.AddEnergy(100);
            playerShip.AddOre(1000);
            if(battleCount%3 == 0)
                SpaceGrid.singleton.SpawnAsteroids();
        }
        battleCount++;

        UI.singleton.UIEndBattle();
    }
}
