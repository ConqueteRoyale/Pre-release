using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsManager : MonoBehaviour {

    public static RtsManager Current = null;

    public List<PlayerSetupDefinition> Players = new List<PlayerSetupDefinition>();

    public MeshCollider MapCollider;

    public Vector3? ScreenPointToMapPosition(Vector2 point)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        if (!MapCollider.Raycast(ray, out hit, Mathf.Infinity))
            return null;

        return hit.point;
    }

	// Use this for initialization
	void Start () {
        Current = this;

        foreach (var p in Players)
        {
            foreach (var u in p.StartingUnits)
            {
                var go = (GameObject)GameObject.Instantiate(u, p.Location.position, p.Location.rotation);
                var player = go.AddComponent<Player>();
                player.Info = p;
                if(!p.isAi)
                {
                    if (Player.Default == null) Player.Default = p;
                    go.AddComponent<RightClickNavigation>();
                    go.AddComponent<ActionSelect>();
                }
            }
        } 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
