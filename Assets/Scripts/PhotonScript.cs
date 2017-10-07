using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonScript : MonoBehaviour {

    public int speed;

    private int[,] map;

	// Use this for initialization
	void Start () {

        map = GameObject.Find("LevelManager").GetComponent<LevelGenerator>().map;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
