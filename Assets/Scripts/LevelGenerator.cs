using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public int lvlNum;
    public int lvlSize;
    public GameObject floor;
    public GameObject wall;
    public GameObject edge;
    public int[,] map;

    private int elemSize = 5;
    private int numElems = 20;

    void Awake () {
        // To ensure lvlSize is initialized before Camera script's Start method is called
        lvlSize = lvlNum * numElems * elemSize;
    }

	// Use this for initialization
	void Start () {

        floor = Resources.Load<GameObject>("Prefabs/FloorTile");
        wall = Resources.Load<GameObject>("Prefabs/WallTile");
        edge = Resources.Load<GameObject>("Prefabs/Edge");
        map = new int[numElems, numElems];

        GenerateLevel();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void GenerateLevel () {

        float start = -(lvlSize / 2f) + (elemSize / 2f);
        System.Random rnd = new System.Random();

        for (float i = start - elemSize; i <= -(start - elemSize); i += elemSize) {

            for (float j = start - elemSize; j <= -(start - elemSize); j += elemSize) {
                // Build boundary of level
                if (i < start || i > -start || j < start || j > -start)
                    Instantiate(edge, new Vector3(j, i, 0f), Quaternion.identity);
                // Build maze of level
                else {
                    if (rnd.Next(75) < 49) {
                        Instantiate(floor, new Vector3(j, i, 0f), Quaternion.identity);
                        map[(int)j, (int)i] = 0;
                    }
                    else {
                        Instantiate(wall, new Vector3(j, i, 0f), Quaternion.identity);
                        map[(int)j, (int)i] = 1;
                    }
                }
            }
        }

    }
}
