using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipItem : MonoBehaviour{

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Camera _cam;
    private Vector3 _dragOffset;
    [SerializeField] protected Vector2 size;
    [SerializeField] private ShipItemSlot[] _slots;
    [SerializeField] public int tier;
    [SerializeField] protected float durability;
    [SerializeField] public float price;
    [SerializeField] public string Name;
    [SerializeField] public string[] cantPlaceNear;

    private void Awake(){
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _cam = Camera.main;
    }

    void OnMouseDown(){
        //Debug.Log(gameObject.name);
        MouseManager.singleton.draggedObject = gameObject;
        _dragOffset = transform.position - GetMousePosition();
    }

    void OnMouseDrag(){
        transform.position = GetMousePosition() + _dragOffset;
    }

    void OnMouseUp(){
        Place(transform.position);
    } 

    public void Place(Vector3 newPosition){
        if(_slots != null){
            foreach (var slot in _slots)
            {
                slot.RemoveItem();
            }
        }

        var newSlots = GetNewSlots(newPosition);
        transform.parent = null;
        if(newSlots != null){
            foreach (var slot in newSlots)
            {
                slot.PlaceItem(this);
            }
            transform.position = newSlots[0].transform.position;
            transform.parent = newSlots[0].transform;
        }

        _slots = newSlots;
        MouseManager.singleton.draggedObject = null;
        PlayerShip.singleton.GenerateShipStats();
    }

    virtual protected ShipItemSlot[] GetNewSlots(Vector3 newPosition){
        return ShipBuilder.singleton.GetSlotForAugment(newPosition, cantPlaceNear);
    }

    virtual public void changeShipStats(PlayerShip ship){
        ship.durabilityMax += durability;
    }

    public Vector3 GetMousePosition(){
        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }

    public ShipItemSlot[] GetOccupiedSlots() => _slots;
}
