using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        UI.singleton.UpgradeButtonPressed();
    }
}
