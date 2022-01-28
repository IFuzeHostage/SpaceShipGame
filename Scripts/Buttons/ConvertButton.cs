using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        PlayerShip.singleton.ConvertOre();
    }
}
