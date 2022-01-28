using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCell : MonoBehaviour
{
    [SerializeField] public SpaceObject spaceObject;

    private void OnMouseDown()
    {
        SpaceGrid.singleton.SelectCell(this);
    }
}
