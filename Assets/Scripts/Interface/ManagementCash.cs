using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Financial resources management
public class ManagementCash : MonoBehaviour
{

// Tower manager's starting money level
    public float startCoins = 100;

// Starting level of the enemy manager
    public float startEnemyCoins = 150;

    private float coins = 0;

    private float coinsEnemy = 0;

// An object that displays the tower manager's money
    public TextMeshProUGUI coinsInfo;

// An object that displays the enemies manager's money
    public TextMeshProUGUI enemyCoinsInfo;

    // Get information about the financial resources of the tower manager
    // Returns information about the tower manager's financial resources
    public float GetCoins()
    {
        return coins;
    }

// Set the amount of money for the tower manager
// coins_ - new value of the tower manager's money amount
    public void SetCoins(float coins_)
    {
        coins = coins_;
        if (coinsInfo != null)
        {
            coinsInfo.text = coins.ToString();
        }
    }

    // Change the amount of money the tower manager has
    // coins_ - the value by which the amount of money the tower manager will change
    public void AddCoins(float coins_)
    {
        coins += coins_;
        if (coinsInfo != null)
        {
            coinsInfo.text = coins.ToString();
        }
    }

    // Retrieving information about the financial resources of the enemies manager
    // Returns information about the enemies manager's financial resources
    public float GetEnemyCoins()
    {
        return coinsEnemy;
    }

    // Set the amount of money for the enemies manager
    // coins_ - new value for the amount of money of the enemies manager
    public void SetEnemyCoins(float coins_)
    {
        coinsEnemy = coins_;
        if (coinsInfo != null)
        {
            enemyCoinsInfo.text = coinsEnemy.ToString();
        }
    }

    // Change the amount of money of the enemies manager
    // coins_ - the value by which the amount of money of the enemies manager will change
    public void AddEnemyCoins(float coins_)
    {
        coinsEnemy += coins_;
        if (enemyCoinsInfo != null)
        {
            enemyCoinsInfo.text = coinsEnemy.ToString();
        }
    }

    void Start()
    {
        if (coinsInfo != null)
        {
            coinsInfo.text = startCoins.ToString();
        }
        coins = startCoins;

        if (enemyCoinsInfo != null)
        {
            enemyCoinsInfo.text = startEnemyCoins.ToString();
        }
        coinsEnemy = startEnemyCoins;
    }
}
