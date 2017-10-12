using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronController : MonoBehaviour {

    // TODO : Fix electrons affecting each other's speed

    private Rigidbody2D electron;

    public float speed = 10f;
    public float scale = 0.075f;
    public static float radius;

    private float xVelocity;
    private float yVelocity;

	// Use this for initialization
	void Awake () {

        electron = GetComponent<Rigidbody2D>();
        electron.transform.localScale = new Vector3(scale, scale, 1);
        radius = electron.GetComponent<CircleCollider2D>().radius * scale;
		
	}
	
	// Update is called once per frame
	void Update () {

        // To prevent electrons from affecting each other's velocity
        //electron.velocity = new Vector2(xVelocity, yVelocity);

        
        if (electron.velocity == Vector2.zero) {
            //Debug.Log("Electron death due to zero veloctiy");
            electron.gameObject.SetActive(false);
        }
        
		
	}

    void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Electron") {
            electron.gameObject.SetActive(false);
            //Debug.Log("Killing Electron from Collision");
        }

    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Photon")
            electron.gameObject.SetActive(false);

    }

    void OnEnable() {

        //xVelocity = electron.velocity.x;
        //yVelocity = electron.velocity.y;

    }

}