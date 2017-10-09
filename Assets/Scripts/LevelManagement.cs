using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagement : MonoBehaviour {

    // TODO : Instantiate Player programmatically
    // TODO : Add Canvas and Text to display Atomic number / Atom type
    // TODO : Add Title Screen
    // TODO : Eliminate unreachable areas from map

        // TODO : SOUNDS!!!!

    public static LevelManagement SharedInstance;

    public int lvlNum;
    public int lvlSize;
    public int elemSize;
    public int numElems;
    public int[,] map;
    public System.Random RNG;

    public const int FLOOR = 0;
    public const int WALL = 1;
    private const int baseNumMapElems = 20;
    private const int lvlMultiplier = 5;

    public GameObject floorObj;
    public GameObject wallObj;
    public GameObject edge;


    void Awake () {

        SharedInstance = this;

        floorObj = Resources.Load<GameObject>("Prefabs/FloorTile");
        wallObj = Resources.Load<GameObject>("Prefabs/WallTile");
        edge = Resources.Load<GameObject>("Prefabs/Edge");

        // floorPattern and wallPattern Sprites are same size
        elemSize = (int)(Resources.Load<Sprite>("Sprites/floorPattern").bounds.extents.x * 2);
        numElems = baseNumMapElems + ((lvlNum - 1) * lvlMultiplier);
        lvlSize = numElems * elemSize;
        RNG = new System.Random();

        map = GenerateLevel();

    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

        GameObject photon = ObjectPool.SharedInstance.GetPooledParticle("Photon");

        while (photon != null) {
            photon.SetActive(true);
            photon = ObjectPool.SharedInstance.GetPooledParticle("Photon");
        }
		
	}

    int[,] GenerateLevel () {

        int k, h; // 2D array coords
        int[,] mapp = new int[numElems, numElems];
        float start = -(lvlSize / 2f) + (elemSize / 2f);

        // Traverse y - i (game coords) & k (2D array coords)
        for (float i = start - elemSize; i <= -(start - elemSize); i += elemSize) {
            k = numElems - ((int)(i - start) / elemSize) - 1;
            // Traverse x - j (game coords) & h (2D array coords)
            for (float j = start - elemSize; j <= -(start - elemSize); j += elemSize) {
                h = (int)(j - start) / elemSize;
                // Build boundary of level
                if (i < start || i > -start || j < start || j > -start)
                    Instantiate(edge, new Vector3(j, i, 0f), Quaternion.identity);
                // Build maze of level
                else {
                    if (RNG.Next(75) < 49) {
                        Instantiate(floorObj, new Vector3(j, i, 0f), Quaternion.identity);
                        mapp[k, h] = FLOOR;
                    }
                    else {
                        Instantiate(wallObj, new Vector3(j, i, 0f), Quaternion.identity);
                        mapp[k, h] = WALL;
                    }
                }
            }
        }
        return mapp;
    }

}
