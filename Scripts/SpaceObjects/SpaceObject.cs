using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObject : MonoBehaviour
{ 
    protected string _name;
    virtual public void GenerateName()
    {
        _name = null;
    }

}
