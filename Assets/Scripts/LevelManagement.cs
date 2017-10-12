using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManagement : MonoBehaviour {

    // TODO : Instantiate Player programmatically
    // TODO : Add Canvas and Text to display Atomic number / Atom type
    // TODO : Add Title Screen
    // TODO : Eliminate unreachable areas from map

        // TODO : SOUNDS!!!!

    public static LevelManagement LvlObj;

    public static int elemSize;
    public static System.Random RNG;

    public int lvlNum;
    private int lvlSize;
    private int numElems;
    private int[,] map;

    public const int FLOOR = 0;
    public const int WALL = 1;
    private const int BASE_NUM_ELEMS = 20;
    private const int NUM_ELEMS_MULT = 5;

    public Sprite floorSprite;
    public GameObject floorObj;
    public GameObject wallObj;
    public GameObject edge;
    public Text numberText;
    public Text elementText;
    public Text symbolText;
    public Text massText;
    public Text levelEnd;
    public GameObject player;
    public Button playAgainButton;
    public Button continueButton;

    public int GetLevelNumber() { return lvlNum; }
    public int GetLevelSize() { return lvlSize; }
    public int GetNumElems() { return numElems; }
    public int[,] GetLevelMap() { return map; }

    // Static Constructor
    static LevelManagement() {

        // Create Random Number Generator - doing so here maintains seed throughout game execution
        RNG = new System.Random();

    }


    void Awake () {

        // Set static instance to current level manager, allowing access to level specific values 
        LvlObj = this;

        // Get size of map elements (floorPattern and wallPattern Sprites are same size)
        elemSize = (int)(floorSprite.bounds.extents.x * 2);
        numElems = BASE_NUM_ELEMS + ((lvlNum - 1) * NUM_ELEMS_MULT);
        lvlSize = numElems * elemSize;

        map = GenerateLevel();

    }

    // Use this for initialization
    void Start () {

        levelEnd.text = "";
        playAgainButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        GameObject photon = ObjectPool.SharedInstance.GetPooledParticle("Photon");
        GameObject atom = ObjectPool.SharedInstance.GetPooledParticle("Atom");

        while (photon != null) {
            photon.SetActive(true);
            photon = ObjectPool.SharedInstance.GetPooledParticle("Photon");
        }

        while (atom != null) {
            Pair spawn = RandomSpawnLocation();
            atom.transform.position = new Vector2(PhotonController.xTransform(spawn.x), PhotonController.yTransform(spawn.y));
            atom.SetActive(true);
            atom = ObjectPool.SharedInstance.GetPooledParticle("Atom");
        }

        numberText.text = PlayerController.atomicNumber.ToString();
        elementText.text = PlayerController.element;
        symbolText.text = PlayerController.symbol;
        massText.text = PlayerController.atomicMass;


        if (!player.gameObject.activeInHierarchy) {

            levelEnd.text = "Game Over";
            playAgainButton.gameObject.SetActive(true);

        } else if (!ObjectPool.SharedInstance.HasParticleOfType("Atom")) {

            levelEnd.text = "You have outgrown your surroundings!";
            continueButton.gameObject.SetActive(true);

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

    public Pair RandomSpawnLocation() {

        Pair spawnAt;

        do {

            spawnAt = new Pair(RNG.Next(numElems), RNG.Next(numElems));

        } while (map[spawnAt.y, spawnAt.x] != FLOOR);

        return spawnAt;

    }

}
