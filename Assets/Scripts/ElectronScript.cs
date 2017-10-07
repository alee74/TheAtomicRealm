using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronScript: MonoBehaviour {

    public float speed = 7f;
    public float scale = 0.075f;

    private float xVelocity;
    private float yVelocity;

    private Rigidbody2D electron;

	// Use this for initialization
	void Start () {

        electron = GetComponent<Rigidbody2D>();
        electron.transform.localScale = new Vector3(scale, scale, 1);

        xVelocity = electron.velocity.x;
        yVelocity = electron.velocity.y;
		
	}
	
	// Update is called once per frame
	void Update () {

        // To prevent electrons from affecting each other's velocity
        electron.velocity = new Vector2(xVelocity, yVelocity);

        if (electron.velocity == Vector2.zero)
            Destroy(electron.gameObject);
		
	}

    void OnCollisionEnter2D(Collision2D obj) {
        if (obj.gameObject.layer != LayerMask.NameToLayer("Player"))
            Destroy(electron.gameObject);
        //Debug.Log("Destroying Electron");
    }
    

    public void Shoot (int dir) {

        float xVel = 0f;
        float yVel = 0f;

        // Set velocities based on direction fired
        switch (dir) {
            case 0: // north
                yVel = speed;
                break;
            case 1: // east
                xVel = speed;
                break;
            case 2: // south
                yVel = -speed;
                break;
            case 3: // west
                xVel = -speed;
                break;
            case 4: // northeast
                xVel = Mathf.Cos(speed);
                yVel = Mathf.Sin(speed);
                break;
            case 5: // southeast
                xVel = Mathf.Cos(speed);
                yVel = -Mathf.Sin(speed);
                break;
            case 6: // southwest
                xVel = -Mathf.Cos(speed);
                yVel = -Mathf.Sin(speed);
                break;
            case 7: // northwest
                xVel = -Mathf.Cos(speed);
                yVel = Mathf.Sin(speed);
                break;
        }

        electron.velocity = new Vector2(xVel, yVel);

    }
}
