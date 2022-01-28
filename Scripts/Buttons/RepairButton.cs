using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        PlayerShip.singleton.Repair();
    }
}
