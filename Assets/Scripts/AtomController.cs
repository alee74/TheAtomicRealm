using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomController : MonoBehaviour {

    private Rigidbody2D atom;

    public int moveTimer = 5;
    public int ejectForFrames = 3;
    public float speed = 5f;
    public float scale = 0.3f;
    public float radius;
    public float ejectSpeedMultiplier = 1.5f;
    public bool justEjected;

    private Pair dir;
    private int ejectCount = 0;
    private float lastTime;

    public static Pair[] offsets = { new Pair(1, 1), new Pair(1, -1),
                                     new Pair(-1, 1), new Pair(-1, -1) };

	// Use this for initialization
	void Awake () {

        atom = GetComponent<Rigidbody2D>();
        atom.transform.localScale = new Vector3(scale, scale, 1);
        radius = GetComponent<CircleCollider2D>().radius * scale;
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time - lastTime >= moveTimer)
            SetDirection();

        atom.velocity = new Vector2(dir.x * speed, dir.y * speed);

        if (dir.x != 0 && dir.y != 0)
            atom.velocity = new Vector2(dir.x * Mathf.Cos(45) * speed, dir.y * Mathf.Sin(45) * speed);

        if (justEjected) {

            ejectCount++;
            atom.velocity = new Vector2(atom.velocity.x * ejectSpeedMultiplier, atom.velocity.y * ejectSpeedMultiplier);

            if (ejectCount >= ejectForFrames) {

                justEjected = false;
                ejectCount = 0;

            }

        }

        
	}

    void OnEnable() {

        justEjected = true;
        SetDirection();

    }

    void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag == "Player" && !justEjected) {

            Debug.Log("Atom absorbed by Player!");
            PlayerController.atomicNumber++;
            Debug.Log("atomicNumber = " + PlayerController.atomicNumber);
            atom.gameObject.SetActive(false);
            ObjectPool.SharedInstance.RemoveParticleFromPool(atom.gameObject);
            PlayerController.collectedAtoms.Add(atom.gameObject);

        }

        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Edge") {

            SetDirection();

        }

    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Photon") {

            Debug.Log("Atom annihilated by Photon!");
            atom.gameObject.SetActive(false);

        }

    }

    void SetDirection() {

        if (LevelManagement.RNG.Next() % 2 == 0)
            dir = PhotonController.offsets[LevelManagement.RNG.Next(PhotonController.offsets.Length)];
        else
            dir = offsets[LevelManagement.RNG.Next(offsets.Length)];

        lastTime = Time.time;

    }

}
