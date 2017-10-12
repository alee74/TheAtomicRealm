using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour {

    public Transform target;

    private static float offset;
    private static float hOff;
    private static float wOff;


	// Use this for initialization
	void Start () {

        // distance (game units) from center to each edge (map is square)
        offset = LevelManagement.LvlObj.GetLevelSize() / 2f;
        // Vertical distance from center to edge of camera
        hOff = Camera.main.orthographicSize;
        // Horizontal distance from center to edge of camera
        wOff = hOff * Camera.main.aspect;

    }
	
	// Update is called once per frame
	void Update () {

        // Camera stays centered on player
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Calculate distance between player and edges
        float leftDist = (-offset) - target.position.x;
        float rightDist = offset - target.position.x;
        float topDist = offset - target.position.y;
        float bottomDist = (-offset) - target.position.y;

        // Pin camera to edge(s) if player is too close

        // Check x boundaries
        if (Mathf.Abs(leftDist) < wOff)
            transform.position = new Vector3(wOff - offset, transform.position.y, transform.position.z);
        else if (rightDist < wOff)
            transform.position = new Vector3(offset - wOff, transform.position.y, transform.position.z);
        
        // Check y boundaries
        if (topDist < hOff) 
            transform.position = new Vector3(transform.position.x, offset - hOff, transform.position.z);
        else if (Mathf.Abs(bottomDist) < hOff)
            transform.position = new Vector3(transform.position.x, hOff - offset, transform.position.z);

	}

}

// offset = GameObject.Find("LevelManager").GetComponent<LevelManagement>().lvlSize / 2f;

