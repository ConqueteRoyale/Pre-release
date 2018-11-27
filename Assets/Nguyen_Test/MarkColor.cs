using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkColor : MonoBehaviour {

    public MeshRenderer[] Renderer;

	// Use this for initialization
	void Start () {
        var color = GetComponent<Player>().Info.AccentColor;
        foreach (var r in Renderer)
        {
            r.material.color = color;
        }
	}

}
