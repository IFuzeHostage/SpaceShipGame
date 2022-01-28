using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    [SerializeField] private GameObject buildGrid;
    [SerializeField] private GameObject spaceGrid;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject actionList;
    [SerializeField] private GameObject playerShip;
    [SerializeField] private Text moveCostText;
    [SerializeField] private TextMeshPro shipResourcesText;
    [SerializeField] private GameObject planetWindow;
    [SerializeField] private GameObject tradeWindow;
    [SerializeField] private Slider buySlider;
    [SerializeField] private TextMeshProUGUI buyText;
    [SerializeField] private Slider sellSlider;
    [SerializeField] private TextMeshProUGUI sellText;
    [SerializeField] private GameObject tradeButton;
    [SerializeField] private GameObject mineButton;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private List<GameObject> shopTiers;
    [SerializeField] private GameObject endgameWindow;

    public int shopTier = 1;
    private int selectedTier = 1;
    public static UI singleton;

    public int maxMessages = 100;
    [SerializeField]
    private List<Message> messageList = new List<Message>();
    [SerializeField]
    GameObject messagePanel, messagePrefab;

    public object SceneLoader { get; private set; }

    public enum UIState { 
        Builder,
        Space
    }

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetUI();
        ChangeUIState(UIState.Builder);
        SendMessage("Collect 5000 crypto or place all tier 3 items on your ship to win!", Color.yellow);
    }

    public void ChangeUIState(UIState state)
    {
        ResetUI();
        switch (state)
        {
            case UIState.Builder:
                buildGrid.SetActive(true);
                shop.SetActive(true);
                break;
            case UIState.Space:
                spaceGrid.SetActive(true);
                actionList.SetActive(true);
                playerShip.GetComponent<SpriteRenderer>().enabled = true;
                break;

        }
    }

    private void ResetUI()
    {
        buildGrid.SetActive(false);
        spaceGrid.SetActive(false);
        shop.SetActive(false);
        actionList.SetActive(false);
        playerShip.GetComponent<SpriteRenderer>().enabled = false;

    }

    public void ResetActionUI()
    {
        buySlider.gameObject.SetActive(false);
        buyText.gameObject.SetActive(false);
        sellSlider.gameObject.SetActive(false);
        sellText.gameObject.SetActive(false); ;
        tradeButton.SetActive(false);
        mineButton.SetActive(false);
    }

    #region UpdateUI
    public void UpdateUIMoveCost(float moveCost)
    {
        moveCostText.text = $"Estimated Cost: {moveCost}MW";
    } 

    public void UpdateUIshipResources()
    {
        var playerShip = PlayerShip.singleton;

        var shipText = new StringBuilder();
        shipText.Append($"Player Ship\n");
        shipText.Append($"Hull Durability:\n");
        shipText.Append($"{playerShip.durabilityCurrent} / {playerShip.durabilityMax}\n");
        shipText.Append($"Ore Storage:\n");
        shipText.Append($"{playerShip.oreCurrent} / {playerShip.oreMax}\n");
        shipText.Append($"Energy Storage:\n");
        shipText.Append($"{playerShip.energyCurrent} / {playerShip.energyMax}MW\n");

        shipResourcesText.text = shipText.ToString();

    }

    public void UpdateSellText(float value)
    {
        var stringBuilder = new StringBuilder();
        var ore = PlayerShip.singleton.oreCurrent;

        bool delivery = !SpaceGrid.singleton.ShipOnSelected();

        stringBuilder.Append($"Sell Ore:\n");
        stringBuilder.Append($"{SpaceStation.EstimateSellPrice(ore * value, delivery):N}$/{ore * value:N} ore");

        sellText.text = stringBuilder.ToString();
    }

    public void UpdateBuyText(float value)
    {
        var stringBuilder = new StringBuilder();
        var crypto = PlayerShip.singleton.GetCrypto();

        bool delivery = !SpaceGrid.singleton.ShipOnSelected();

        stringBuilder.Append($"Buy Energy:\n");
        stringBuilder.Append($"{SpaceStation.EstimateEnergyPrice(5000f * value, delivery):N}$/{(5000f * value):N} MW");

        buyText.text = stringBuilder.ToString();
    }
    #endregion

    #region UpdateSelectedUI
    public void UpdateSelectedUI(Planet planet)
    {
        planetWindow.SetActive(true);
        var text = planetWindow.GetComponentInChildren<TextMeshPro>();

        var builder = new StringBuilder();
        builder.Append($"{planet.GetName()}\n");
        builder.Append($"Infinte amounts of ore left\n");

        text.text = builder.ToString();
        ResetActionUI();
        ShowMineUI();
    }

    public void UpdateSelectedUI(Asteroids asteroids)
    {
        planetWindow.SetActive(true);
        var text = planetWindow.GetComponentInChildren<TextMeshPro>();

        var builder = new StringBuilder();
        builder.Append($"{asteroids.GetName()}\n");
        builder.Append($"{asteroids.GetOre()} ore left\n");

        text.text = builder.ToString();
        ResetActionUI();
        ShowMineUI();
    }

    public void UpdateSelectedUI(SpaceStation station)
    {
        planetWindow.SetActive(true);
        var text = planetWindow.GetComponentInChildren<TextMeshPro>();

        var builder = new StringBuilder();
        builder.Append($"{station.GetName()}\n");

        text.text = builder.ToString();
        ResetActionUI();
        ShowTradeUI();
    }

    public void UpdateSelectedUI()
    {
        planetWindow.SetActive(true);
        ResetActionUI();    
        planetWindow.GetComponentInChildren<TextMeshPro>().text = "";
    }

    public void UpdateSelectedUI(SpaceObject spaceObj)
    {
        if (spaceObj == null) UpdateSelectedUI();
        if(spaceObj is SpaceStation)
            UpdateSelectedUI((SpaceStation)spaceObj);
        else if (spaceObj is Planet)
            UpdateSelectedUI((Planet)spaceObj);
        else if (spaceObj is Asteroids)
            UpdateSelectedUI((Asteroids)spaceObj);
    }
    #endregion

    #region ShowUI
    public void ShowTradeUI()
    {
        buySlider.gameObject.SetActive(true);
        buyText.gameObject.SetActive(true);
        sellSlider.gameObject.SetActive(true);
        sellText.gameObject.SetActive(true); ;
        tradeButton.SetActive(true);
        UpdateSellText(sellSlider.value);
        UpdateBuyText(buySlider.value);
    }

    public void ShowMineUI()
    {
        mineButton.SetActive(true);
    }

    public void DisplayShopTier(int tier)
    {
        if (tier < 1 || tier > shopTiers.Count) return;
        foreach (var item in shopTiers)
        {
            item.SetActive(false);
        }
        shopTiers[tier - 1].SetActive(true);
    }
    #endregion 

    #region ButtonPressed
    public void ShopButtonPressed()
    {
        ResetUI();
        buildGrid.SetActive(true);
        shop.SetActive(true);
    }

    public void SpaceButtonPressed()
    {
        ResetUI();
        spaceGrid.SetActive(true);
        actionList.SetActive(true);
        playerShip.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void RepairButtonPressed()
    {
        PlayerShip.singleton.Repair();
    }

    public void ConvertButtonPressed()
    {
        PlayerShip.singleton.ConvertOre();
    }



    public void MineButtonPressed()
    {
        SpaceGrid.singleton.Mine();
    }

    public void MoveButtonPressed()
    {
        SpaceGrid.singleton.MovePlayer(SpaceGrid.singleton.GetSelectedCell());
    }

    public void TradeButtonPressed()
    {
        SpaceGrid.singleton.Trade(sellSlider.value, buySlider.value);
    }

    public void UpgradeButtonPressed()
    {
        if(shopTier < 3 && PlayerShip.singleton.SpendCrypto(1000))
        {
            shopTier++;
            SendMessage($"You've upgraded to Shop Tier {shopTier}", Color.green);
        }
        else  SendMessage($"Can't upgrade shop! Not enough crypto or Shop at Max Lvl", Color.red);
    }

    public void TierButtonPressed()
    {
        selectedTier = selectedTier+1>shopTier? 1 : selectedTier+1;
        DisplayShopTier(selectedTier);
    }
    public void RetryButtonPressed()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void ExitButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region BattleUI
    public void UIStartBattle()
    {
        battleManager.gameObject.SetActive(true);
        battleManager.InitializeBattle();
    }

    public void UIEndBattle()
    {
        battleManager.gameObject.SetActive(false);
    }
    #endregion
    #region Messager
    [System.Serializable]
    public class Message
    {
        public string text;
        public Text textObject;
    }

    public void SendMessage(string text) {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(messagePrefab, messagePanel.transform);
        
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
    }

    public void SendMessage(string text, Color color)
    {
        SendMessage(text);
        messageList[messageList.Count-1].textObject.color = color;
    }
    #endregion

    #region Endgame
    public bool CheckFullUpgrades()
    {
        var augments = ShipBuilder.singleton.GetPlacedAugments();
        HashSet<string> uniqueAugmetns = new HashSet<string>();
        foreach(var augment in augments)
        {
            if(augment.tier == 3)
            {
                uniqueAugmetns.Add(augment.Name);
            }
        }
        if (uniqueAugmetns.Count >= 10) return true;
        return false;
    }

    public void CheckEndgame()
    {
        if(PlayerShip.singleton.GetCrypto() >= 5000 || CheckFullUpgrades())
        {
            GameWon();
        }
        else if (!SpaceGrid.singleton.CanShipReachStation())
        {
            GameLost();
        }
    }

    public void GameLost()
    {
        endgameWindow.SetActive(true);
        endgameWindow.GetComponentInChildren<Text>().text = "Game Lost!";
    }

    public void GameWon()
    {
        endgameWindow.SetActive(true);
        endgameWindow.GetComponentInChildren<Text>().text = "Game Won!";
    }
    #endregion
}
