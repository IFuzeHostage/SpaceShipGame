using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        UI.singleton.TierButtonPressed();
    }
}
