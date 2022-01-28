using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGrid : MonoBehaviour
{
    [SerializeField] public float _gridScale;
    [SerializeField] private int _width;
    [SerializeField] private int _hight;

    [SerializeField] private SpaceCell _selectedCell;
    [SerializeField] private SpaceCell _shipPosition;
    [SerializeField] private GameObject cellSelection;

    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private SpaceStation _spaceStationPrefab;
    [SerializeField] private Planet _planetPrefab;
    [SerializeField] private Asteroids _asteroidsPrefab;

    private SpaceCell[,] _spaces;
    private PlayerShip _playerShip;

    public static SpaceGrid singleton;

    private void Awake()
    {
        singleton = this;
        cellSelection.transform.localScale *= _gridScale;
        _spaces = new SpaceCell[_width, _hight];
    }

    void Start()
    {
        _playerShip = PlayerShip.singleton;
        BuildGrid();
        InitializeSpace();
    }

    void OnDrawGizmos()
    {
        //if(!_drawGizmos || Application.isPlaying) return;
        foreach (var cell in EvaluateGrid())
        {
            Gizmos.DrawWireCube(cell, new Vector3(1, 1, 0) * _gridScale);
        }
    }

    void BuildGrid()
    {
        var slotPositions = EvaluateGrid();
        for (int x = 0; x < slotPositions.GetLength(0); x++)
        {
            for (int y = 0; y < slotPositions.GetLength(1); y++)
            {
                var newCell = Instantiate(_cellPrefab, slotPositions[x, y], Quaternion.identity);
                newCell.transform.localScale *= _gridScale;
                newCell.transform.parent = transform;
                _spaces[x, y] = newCell.GetComponent<SpaceCell>();
            }
        }
    }

    void InitializeSpace()
    {
        SpaceObject[] spaceObjects = {_planetPrefab, _planetPrefab };
        var rand = new System.Random();


        var center = _spaces[_width / 2, _hight / 2];
        PlaceSpaceObject(center, _spaceStationPrefab);
        PlayerShip.singleton.transform.position = center.transform.position;
        _shipPosition = center;

        foreach (var obj in spaceObjects)
        {
            while (true)
            {
                var cell = _spaces[rand.Next(0, 40), rand.Next(0, 40)];
                if (cell.spaceObject != null) continue;
                PlaceSpaceObject(cell, obj);
                break;
            }
        }

    }

    public void SpawnAsteroids()
    {
        var rand = new System.Random();
        while (true)
        {
            var cell = _spaces[rand.Next(0, 40), rand.Next(0, 40)];
            if (cell.spaceObject != null) continue;
            PlaceSpaceObject(cell, _asteroidsPrefab);
            UI.singleton.SendMessage($"New Asteroids appeared on Space Map!");
            break;
        }
    }

    void PlaceSpaceObject(SpaceCell cell, SpaceObject obj)
    {
        cell.spaceObject = Instantiate(obj, cell.transform.position, Quaternion.identity);
        cell.spaceObject.transform.localScale = cell.transform.localScale;
        cell.spaceObject.transform.parent = cell.transform;
    }

    Vector3[,] EvaluateGrid()
    {
        Vector3[,] arr = new Vector3[_width, _hight];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _hight; y++)
            {
                arr[x, y] = new Vector3(x, y, 0) * _gridScale + transform.position;
            }
        }
        return arr;
    }

    public void MovePlayer(SpaceCell spaceCell)
    {
        if (_playerShip.ready == false) return;
        if (spaceCell == null) return;

        var distance = EstimateDistance(spaceCell.transform.position, _playerShip.transform.position);
        var energyToSpend = _playerShip.EstimateEnergyCost(distance);

        if (!_playerShip.SpendEnergy(energyToSpend)) return;
        _playerShip.transform.position = spaceCell.transform.position;
        _shipPosition = spaceCell;
        _playerShip.GenerateEnergy(distance);
        if (_playerShip.firstMove == true) _playerShip.firstMove = false;

        UI.singleton.SendMessage($"Player ship moves, spending {energyToSpend} MW");
        UI.singleton.UpdateUIMoveCost(0);
    }

    public void SelectCell(SpaceCell spaceCell)
    {
        _selectedCell = spaceCell;
        cellSelection.transform.position = _selectedCell.transform.position;
        var distanceToShip = EstimateDistance(spaceCell.transform.position, _playerShip.transform.position);
        var energyCost = _playerShip.EstimateEnergyCost(distanceToShip);
        var spaceObject = spaceCell.spaceObject;
        UI.singleton.UpdateUIMoveCost(energyCost);
        UI.singleton.UpdateSelectedUI(spaceObject);
    }

    public SpaceCell GetSelectedCell()
    {
        return _selectedCell;
    }

    public bool ShipOnSelected()
    {
        return _selectedCell == _shipPosition;
}

    public void Mine()
    {
        if (!ShipOnSelected()) return;

        var mineable = _selectedCell.spaceObject.GetComponent<IMineable>();
        _playerShip.AddOre(mineable.Mine(_playerShip.collectEfficency.x));
    }

    public void Trade(float sellValue, float buyValue)
    {
        var playerShip = PlayerShip.singleton;
        var delivery = !ShipOnSelected();
        //Try sell
        var ore = playerShip.oreCurrent;
        var orePrice = SpaceStation.EstimateSellPrice(ore * sellValue, delivery);
        if (orePrice!= 0 && PlayerShip.singleton.SpendOre(ore * sellValue))
        {
            playerShip.AddCrypto(orePrice);
        }

        //Try Buy
        var energy = buyValue * 5000f;
        var energyPrice = SpaceStation.EstimateEnergyPrice(energy, delivery);
        if (energyPrice != 0 && PlayerShip.singleton.SpendCrypto(energyPrice))
        {
            playerShip.AddEnergy(energy);
        }
    }

    public float EstimateOre()
    {
        return _selectedCell.spaceObject.GetComponent<IMineable>().GetOre();
    }

    public float EstimateDistance(Vector3 first, Vector3 second)
    {
        return (first - second).magnitude / _gridScale * 1000;
    }

    public bool CanShipReachStation()
    {
        var distance = EstimateDistance(_playerShip.transform.position, _spaces[_width / 2, _hight / 2].transform.position);
        var energyRequired = _playerShip.EstimateEnergyCost(distance);
        var energyCanBeProducedByConverter = _playerShip.GetOre() / _playerShip.converterEfficency.y * _playerShip.converterEfficency.x;
        var energyToDeliver = energyRequired - _playerShip.energyCurrent - energyCanBeProducedByConverter;
        if(energyToDeliver <= 0)
        {
            return true;
        }
        else if (SpaceStation.EstimateEnergyPrice(energyToDeliver, true) > _playerShip.GetCrypto())
        {
            return false;
        }
        return true;
    }

    public bool ShipOnStation()
    {
        if (_shipPosition == _spaces[_width / 2, _hight / 2])
            return true;
        return false;
    }
}
