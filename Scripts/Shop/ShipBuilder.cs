using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class ShipBuilder : MonoBehaviour
{
    [SerializeField] private bool _drawGizmos;
    [SerializeField] private int _width;
    [SerializeField] private int _hight;
    [SerializeField] public float _gridScale;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private LayerMask shipItemMask;
    [SerializeField] private SellItemSlot sellSlot;

    private ShipItemSlot[,] slots;
    public static ShipBuilder singleton;

    // Start is called before the first frame update

    void Awake(){
        singleton = this;
        slots = new ShipItemSlot[_width, _hight];
    }
    void Start()
    {
        BuildGrid();
    }

    void OnDrawGizmos(){
        //if(!_drawGizmos || Application.isPlaying) return;
        foreach(var cell in EvaluateGrid()){
            Gizmos.DrawWireCube(cell, new Vector3(1,1,0)*_gridScale);
        }
    }

    void BuildGrid(){
        var slotPositions = EvaluateGrid();
        for(int x = 0; x < slotPositions.GetLength(0); x++){
            for(int y = 0; y < slotPositions.GetLength(1); y++){
                var newCell = Instantiate(_cellPrefab, slotPositions[x, y], Quaternion.identity);
                newCell.transform.localScale *= _gridScale;
                newCell.transform.parent = transform;
                slots[x,y] = newCell.GetComponent<ShipItemSlot>();
            }
        }
    }

    Vector3[,] EvaluateGrid(){
        Vector3[,] arr = new Vector3[_width, _hight];
        for(int x = 0; x < _width; x++){
            for(int y = 0; y < _hight; y++){
                arr[x,y] = new Vector3(x, y, 0)*_gridScale + transform.position;
            }
        }
        return arr;
    }

    public ShipItemSlot[] GetSlots(){
        return GetComponentsInChildren<ShipItemSlot>();
    }

    public ShipItemSlot[] GetSlotForHull(Vector3 position, Vector2 size){
        float minDistance = _gridScale*4;
        ShipItemSlot closestSlot = null;
        Vector2 extension = size - Vector2.one;
        Vector2 closestSlotPos = Vector2.zero;
        float distance;

        for (int x = 0; x < slots.GetLength(0); x++){
            for(int y = 0; y < slots.GetLength(1); y++){
                //Check if form size would go out of grid bounds
                if(x+extension.x >= slots.GetLength(0) || y+extension.y >= slots.GetLength(1)) continue; 
                
                //Check if cells are not occupied
                bool availible = true;
                for(int i = 0; i < size.x; i++){
                    for(int j = 0; j < size.y; j++){
                        if(slots[x+i,y+j].GetItem() != null) availible = false;
                    }
                }
                if(!availible) continue;

                var slot = slots[x,y];

                distance = (slot.transform.position - position).magnitude;
                if(distance < minDistance){
                    //New closest cell
                    minDistance = distance;
                    closestSlot = slot;
                    closestSlotPos = new Vector2(x,y);
                }
            }
        }

        //Check sell slot
        distance = (sellSlot.transform.position - position).magnitude;
        if (distance < minDistance)
        {
            return new ShipItemSlot[] { sellSlot };
        }

        //Fill result array with slots to be occupied with item
        ShipItemSlot[] occupiedSlots = new ShipItemSlot[(int)size.x * (int)size.y];

        for(int x = 0; x < size.x; x++){
            for(int y = 0; y < size.y; y++){
                occupiedSlots[x+y] = slots[(int)closestSlotPos.x + x, (int)closestSlotPos.y + y];
            }
        }
        return occupiedSlots;
    }
    public ShipItemSlot[] GetSlotForAugment(Vector3 position, string[] cantPlaceNear){
        ShipItemSlot closestSlot = null;
        float minDistance = _gridScale*1;

        //Check sell slot
        var distance = (sellSlot.transform.position - position).magnitude;
        if (distance < minDistance)
        {
            closestSlot = sellSlot;
            minDistance = distance;
        }

        foreach (var slot in GetHullSlots())
        {
            if(slot.GetItem() != null) continue;
            distance = (slot.transform.position - position).magnitude;

            if(distance < minDistance){
                if(!neigborsAllowed(slot, cantPlaceNear)) continue;

                closestSlot = slot;
                minDistance = distance;
            }
        }


        if (closestSlot == null) return null;
        else return new ShipItemSlot[]{closestSlot};
    }

    public bool neigborsAllowed(ShipItemSlot slot, string[] cantPlaceNear){
        var neighbors = Physics2D.OverlapBoxAll(slot.transform.position, slot.transform.localScale*2, 0, shipItemMask);
        foreach (var neighbor in neighbors)
        {
            if (cantPlaceNear.Contains(neighbor.GetComponent<ShipItem>().Name)) {
                var newSt = new StringBuilder();
                foreach(var item in cantPlaceNear)
                {
                    newSt.Append($"{item}, ");
                }

                UI.singleton.SendMessage($"Can't place near {newSt.ToString()}", Color.red);
                return false;
            }
        }
        return true;
    }

    public ShipItem[] GetPlacedAugments(){
        Queue<ShipItem> augments = new Queue<ShipItem>();
        //Add augments placed on hulls
        foreach (var slot in GetHullSlots())
        {
            var placed = slot.GetItem();
            if(placed != null)
                augments.Enqueue(placed);
        }
        //Add hulls
        foreach (var hull in GetHulls())
        {
            augments.Enqueue(hull);
        }
        return augments.ToArray<ShipItem>();

    }
 
    public Hull[] GetHulls(){
        HashSet<Hull> hulls = new HashSet<Hull>();
        foreach (var slot in slots)
        {
            var item = slot.GetItem();

            if(item == null) continue;
            if(item.GetComponent<Hull>() != null){
                hulls.Add(item.GetComponent<Hull>());
            }
        }
        return hulls.ToArray<Hull>();
    }
    
    public ShipItemSlot[] GetHullSlots(){
        Queue<ShipItemSlot> hullSlots = new Queue<ShipItemSlot>();
        foreach (var hull in GetHulls())
        {
            foreach (var hullSlot in hull.GetComponentsInChildren<ShipItemSlot>())
            {
                hullSlots.Enqueue(hullSlot);
            }
        }
        return hullSlots.ToArray();
    }
}
