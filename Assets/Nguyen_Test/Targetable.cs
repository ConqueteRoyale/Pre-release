using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Factions
{
    RED,
    BLUE,
    GREEN,
    YELLOW
}

public class Targetable : MonoBehaviour {

    public Factions faction;

    public void OnMouseStart(/*TouchData data*/)
    {
        print("touch" + gameObject.name);
    }
}
