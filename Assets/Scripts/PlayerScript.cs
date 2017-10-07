using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float atomScale = 0.3f;
    public float electronScale = 0.075f;
    public float speed = 5f;
    public float eSpeed = 10f;
    public int shootDelay = 5;
    //public float eOffset = 1f;

    //private float electronScale;
    private float atomRadius;
    private float electronRadius;
    private float eOffset;
    private int shootCount = 0;
    private bool canShoot = true;

    private Rigidbody2D electron;
    private Rigidbody2D atom;
        
    // Use this for initialization
    void Start () {

        atom = GetComponent<Rigidbody2D>();
        electron = Resources.Load<Rigidbody2D>("Prefabs/Electron");

        atom.transform.localScale = new Vector3(atomScale, atomScale, 1);

        //electronScale = GameObject.Find("Electron").GetComponent<ElectronScript>().scale;
        atomRadius = atom.GetComponent<CircleCollider2D>().radius * atomScale;
        electronRadius = electron.GetComponent<CircleCollider2D>().radius * electronScale;
        eOffset = atomRadius + electronRadius;
		
	}

    // Update is called once per frame
    void Update() {
        // To control rate of fire
        shootCount++;
        if (shootCount == shootDelay) {
            canShoot = true;
            shootCount = 0;
        }

        // Get input from axes
        float moveX = Input.GetAxis("HorizontalMove");
        float moveY = Input.GetAxis("VerticalMove");
        float shootX = Input.GetAxis("HorizontalShoot");
        float shootY = Input.GetAxis("VerticalShoot");
        // Define vectors for (potential) projectile instantiation position and velocity
        Vector2 ePosition = Vector2.zero;
        Vector2 eVelocity = Vector2.zero;

        // Set atom velocity according to input
        atom.velocity = new Vector2(moveX * speed, atom.velocity.y);
        atom.velocity = new Vector2(atom.velocity.x, moveY * speed);
        // If at angle, adjust to maintain consistent velocity
        if (moveX != 0 && moveY != 0)
            atom.velocity = new Vector2(moveX * speed * Mathf.Cos(45), moveY * speed * Mathf.Sin(45));


        // Set eVelocity and ePosition vectors according to input (latter calculated to instantiate electron just beyond atom)
        if (shootX != 0 && shootY != 0) {
            ePosition = new Vector2(atom.position.x + (shootX * Mathf.Cos(45) * eOffset), atom.position.y + (shootY * Mathf.Sin(45) * eOffset));
            eVelocity = new Vector2(shootX * eSpeed * Mathf.Cos(45), shootY * eSpeed * Mathf.Sin(45));
        }
        else if (shootX != 0 || shootY != 0) {
            ePosition = new Vector2(atom.position.x + (shootX * eOffset), atom.position.y + (shootY * eOffset));
            eVelocity = new Vector2(shootX * eSpeed, shootY * eSpeed);
        }

        // Instantiate projectile, iff actually shot (ePosition remains zero if not)
        if (ePosition != Vector2.zero && canShoot) {

            Rigidbody2D elect = Instantiate(electron, ePosition, Quaternion.identity);
            elect.velocity = eVelocity;

            canShoot = false;

        }
         
    }
}
