using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private Vector3 _dragOffset;
    private Camera _cam;
    private SpriteRenderer _image;
    private TextMeshPro _text;
    [SerializeField] public ShipItem item;
    private ShipItem boughtItem;


    // Start is called before the first frame update
    private void Awake()
    {
        _cam = Camera.main;
        _image = GetComponentInChildren<SpriteRenderer>();
        var itemSpriteRrenderer = item.GetComponentInChildren<SpriteRenderer>();
        _image.sprite = itemSpriteRrenderer.sprite;
        _text = GetComponentInChildren<TextMeshPro>();
        _text.text = $"{item.Name}\nPrice: {item.price}$";
        _image.transform.position += itemSpriteRrenderer.transform.position;
        _image.transform.localScale = itemSpriteRrenderer.transform.localScale;
        _image.color = itemSpriteRrenderer.color;
        GetComponent<BoxCollider2D>().offset = item.GetComponent<BoxCollider2D>().offset;
        GetComponent<BoxCollider2D>().size = item.GetComponent<BoxCollider2D>().size;
    }
    void OnMouseDown()
    {
        if (SpaceGrid.singleton.ShipOnStation() && PlayerShip.singleton.SpendCrypto(item.price))
        {
            var newItem = Instantiate(item.gameObject, transform.position, Quaternion.identity);
            MouseManager.singleton.draggedObject = newItem;
            boughtItem = newItem.GetComponent<ShipItem>();
            boughtItem.transform.parent = transform.parent;
            boughtItem.transform.localScale *= ShipBuilder.singleton._gridScale;
            _dragOffset = transform.position - GetMousePosition();

        }
    }

    void OnMouseDrag()
    {
        if (boughtItem == null) return;
        boughtItem.transform.position = GetMousePosition() + _dragOffset;
    }

    void OnMouseUp()
    {
        if (boughtItem == null) return;
        boughtItem.Place(boughtItem.transform.position);
        boughtItem = null;
    }

    public Vector3 GetMousePosition()
    {
        var mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }
   

}
