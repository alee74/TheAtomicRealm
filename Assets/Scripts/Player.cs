using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 5f;

    private Rigidbody2D atom;
    private Animator atomSpin;
    
    // Use this for initialization
    void Start () {

        atom = GetComponent<Rigidbody2D>();
        atomSpin = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {

        float inX = Input.GetAxis("HorizontalMove");
        atom.velocity = new Vector2(inX * speed, atom.velocity.y);

        float inY = Input.GetAxis("VerticalMove");
        atom.velocity = new Vector2(atom.velocity.x, inY * speed);


	}
}
