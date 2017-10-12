using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5f;
    public float scale = 0.3f;
    public static float radius;
    public int shootDelay = 5;

    public static int atomicNumber = 1;
    public static string symbol;
    public static string element;
    public static string classification;
    public static string atomicMass;
    public static string state;

    private int shootCount = 0;
    private bool canShoot = true;
    private bool isHit = false;
    private float eSpeed;
    private float eOffset;

    public Rigidbody2D electron;
    private Rigidbody2D player;
    private Animator playerAnimator;
    private AnimatorStateInfo stateInfo;
    public static List<GameObject> collectedAtoms;


    // Use this for initialization
    void Start () {

        player = GetComponent<Rigidbody2D>();
        player.transform.localScale = new Vector3(scale, scale, 1);
        radius = player.GetComponent<CircleCollider2D>().radius * scale;

        eSpeed = electron.GetComponent<ElectronController>().speed;
        eOffset = radius + ElectronController.radius;

        playerAnimator = GetComponent<Animator>();
        collectedAtoms = new List<GameObject>();
		
	}

    // Update is called once per frame
    void Update() {

        // Set animator's condition
        playerAnimator.SetBool("playerIsHit", isHit);

        // Null isHit if playerHit animation has finished
        stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Base.playerHit"))     
            isHit = false;

        // To control rate of fire
        shootCount++;
        if (shootCount == shootDelay) {

            canShoot = true;
            shootCount = 0;

        }

        Move();
        Shoot();

        UpdateElementInfo();
                 
    }

    void Move() {

        // Get input values from axes
        float moveX = Input.GetAxis("HorizontalMove");
        float moveY = Input.GetAxis("VerticalMove");

        // Set atom velocity according to input
        player.velocity = new Vector2(moveX * speed, player.velocity.y);
        player.velocity = new Vector2(player.velocity.x, moveY * speed);

        // If at angle, adjust to maintain consistent velocity
        if (moveX != 0 && moveY != 0)
            player.velocity = new Vector2(moveX * speed * Mathf.Cos(45), moveY * speed * Mathf.Sin(45));

    }

    void Shoot() {

        // Define vectors for (potential) projectile instantiation position and velocity
        Vector2 ePosition = Vector2.zero;
        Vector2 eVelocity = Vector2.zero;
        
        // Get input from axes
        float shootX = Input.GetAxis("HorizontalShoot");
        float shootY = Input.GetAxis("VerticalShoot");

        // Set eVelocity and ePosition vectors according to input (latter calculated to instantiate electron just beyond atom)
        if (shootX != 0 && shootY != 0) {

            ePosition = new Vector2(player.position.x + (shootX * Mathf.Cos(45) * eOffset), player.position.y + (shootY * Mathf.Sin(45) * eOffset));
            eVelocity = new Vector2(shootX * eSpeed * Mathf.Cos(45), shootY * eSpeed * Mathf.Sin(45));

        } else if (shootX != 0 || shootY != 0) {

            ePosition = new Vector2(player.position.x + (shootX * eOffset), player.position.y + (shootY * eOffset));
            eVelocity = new Vector2(shootX * eSpeed, shootY * eSpeed);
        }

        // Instantiate projectile, iff actually shot (ePosition remains zero if not)
        if (ePosition != Vector2.zero && canShoot) {

            Rigidbody2D elect = ObjectPool.SharedInstance.GetPooledParticle("Electron").GetComponent<Rigidbody2D>();
            elect.gameObject.SetActive(true);
            elect.transform.position = ePosition;
            elect.velocity = eVelocity;

            canShoot = false;

        }

    }

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Photon" && !isHit)
        {

            Debug.Log("Collision with Photon! AtomicNumber--");
            isHit = true;
            atomicNumber--;

            SpawnAtom();

            if (atomicNumber <= 0)
            {

                UpdateElementInfo();
                player.gameObject.SetActive(false);
                Debug.Log("Player died. Game Over.");

            }

            other.gameObject.SetActive(false);

        }

    }

    void SpawnAtom() {

        if (collectedAtoms.Count > 0)
        {

            GameObject atom = collectedAtoms[0];                // Grab first atom from collected list
            collectedAtoms.RemoveAt(0);                         // Remove it from player's list
            ObjectPool.SharedInstance.PutParticleInPool(atom);  // Put it back in object pool
            atom.transform.position = player.position;          // Set its locatioin
            atom.SetActive(true);                               // Activate it

        }

    }

    public static void DecrementAtomicNumber() {

        atomicNumber--;

    }

    public static void IncrementAtomicNumber() {

        atomicNumber++;

    }

    void UpdateElementInfo() {

        switch (atomicNumber) {

            case 1:
                symbol = "H";
                element = "Hydrogen";
                atomicMass = "1.008";
                state = "Gas";
                classification = "Nonmetal";
                break;
            case 2:
                symbol = "He";
                element = "Helium";
                atomicMass = "4.003";
                state = "Gas";
                classification = "Noble Gas";
                break;
            case 3:
                symbol = "Li";
                element = "Lithium";
                atomicMass = "6.941";
                state = "Solid";
                classification = "Alkali Metal";
                break;
            case 4:
                symbol = "Be";
                element = "Beryllium";
                atomicMass = "9.012";
                state = "Solid";
                classification = "Alkaline Earth Metal";
                break;
            case 5:
                symbol = "B";
                element = "Boron";
                atomicMass = "10.811";
                state = "Solid";
                classification = "Metalloid";
                break;
            case 6:
                symbol = "C";
                element = "Carbon";
                atomicMass = "12.011";
                state = "Solid";
                classification = "Nonmetal";
                break;
            case 7:
                symbol = "N";
                element = "Nitrogen";
                atomicMass = "14.007";
                state = "Gas";
                classification = "Nonmetal";
                break;
            case 8:
                symbol = "O";
                element = "Oxygen";
                atomicMass = "15.999";
                state = "Gas";
                classification = "Nonmetal";
                break;
            case 9:
                symbol = "F";
                element = "Fluorine";
                atomicMass = "18.998";
                state = "Gas";
                classification = "Nonmetal";
                break;
            case 10:
                symbol = "Ne";
                element = "Neon";
                atomicMass = "20.180";
                state = "Gas";
                classification = "Noble Gas";
                break;
            case 11:
                symbol = "Na";
                element = "Sodium";
                atomicMass = "22.990";
                state = "Solid";
                classification = "Alkali Metal";
                break;
            case 12:
                symbol = "Mg";
                element = "Magnesium";
                atomicMass = "24.305";
                state = "Solid";
                classification = "Alkaline Earth Metal";
                break;
            case 13:
                symbol = "Al";
                element = "Aluminum";
                atomicMass = "26.982";
                state = "Solid";
                classification = "Post-Transition Metal";
                break;
            case 14:
                symbol = "Si";
                element = "Silicon";
                atomicMass = "28.086";
                state = "Solid";
                classification = "Metalloid";
                break;
            case 15:
                symbol = "P";
                element = "Phosphorus";
                atomicMass = "30.974";
                state = "Solid";
                classification = "Nonmetal";
                break;
            case 16:
                symbol = "S";
                element = "Sulfur";
                atomicMass = "32.056";
                state = "Solid";
                classification = "Nonmetal";
                break;
            case 17:
                symbol = "Cl";
                element = "Chlorine";
                atomicMass = "35.453";
                state = "Gas";
                classification = "Nonmetal";
                break;
            case 18:
                symbol = "Ar";
                element = "Argon";
                atomicMass = "39.948";
                state = "Gas";
                classification = "Noble Gas";
                break;
            case 19:
                symbol = "K";
                element = "Potassium";
                atomicMass = "39.098";
                state = "Solid";
                classification = "Alkali Metal";
                break;
            case 20:
                symbol = "Ca";
                element = "Calcium";
                atomicMass = "40.078";
                state = "Solid";
                classification = "Alkaline Earth Metal";
                break;
            case 21:
                symbol = "Sc";
                element = "Scandium";
                atomicMass = "44.956";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 22:
                symbol = "Ti";
                element = "Titanium";
                atomicMass = "47.867";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 23:
                symbol = "V";
                element = "Vanadium";
                atomicMass = "50.942";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 24:
                symbol = "Cr";
                element = "Chromium";
                atomicMass = "51.996";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 25:
                symbol = "Mn";
                element = "Manganese";
                atomicMass = "54.938";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 26:
                symbol = "Fe";
                element = "Iron";
                atomicMass = "55.845";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 27:
                symbol = "Co";
                element = "Cobalt";
                atomicMass = "58.933";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 28:
                symbol = "Ni";
                element = "Nickel";
                atomicMass = "58.693";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 29:
                symbol = "Cu";
                element = "Copper";
                atomicMass = "63.546";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 30:
                symbol = "Zn";
                element = "Zinc";
                atomicMass = "65.38";
                state = "Solid";
                classification = "Transition Meral";
                break;
            case 31:
                symbol = "Ga";
                element = "Gallium";
                atomicMass = "69.723";
                state = "Solid";
                classification = "Post-Transition Metal";
                break;
            case 32:
                symbol = "Ge";
                element = "Germanium";
                atomicMass = "72.631";
                state = "Solid";
                classification = "Metalloid";
                break;
            case 33:
                symbol = "As";
                element = "Arsenic";
                atomicMass = "74.922";
                state = "Solid";
                classification = "Metalloid";
                break;
            case 34:
                symbol = "Se";
                element = "Selenium";
                atomicMass = "78.971";
                state = "Solid";
                classification = "Nonmetal";
                break;
            case 35:
                symbol = "Br";
                element = "Bromine";
                atomicMass = "79.904";
                state = "Liquid";
                classification = "Nonmetal";
                break;
            case 36:
                symbol = "Kr";
                element = "Krypton";
                atomicMass = "83.798";
                state = "Gas";
                classification = "Noble Gas";
                break;
            case 37:
                symbol = "Rb";
                element = "Rubidium";
                atomicMass = "85.468";
                state = "Solid";
                classification = "Alkali Metal";
                break;
            case 38:
                symbol = "Sr";
                element = "Strontium";
                atomicMass = "87.62";
                state = "Solid";
                classification = "Alkaline Earth Metal";
                break;
            case 39:
                symbol = "Y";
                element = "Yttrium";
                atomicMass = "88.906";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 40:
                symbol = "Zr";
                element = "Zirconium";
                atomicMass = "91.224";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 41:
                symbol = "Nb";
                element = "Niobium";
                atomicMass = "92.906";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 42:
                symbol = "Mo";
                element = "Molybdenur";
                atomicMass = "95.95";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 43:
                symbol = "Tc";
                element = "Technetium";
                atomicMass = "(98)";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 44:
                symbol = "Ru";
                element = "Ruthenium";
                atomicMass = "101.07";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 45:
                symbol = "Rh";
                element = "Rhodium";
                atomicMass = "102.91";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 46:
                symbol = "Pd";
                element = "Palladium";
                atomicMass = "106.42";
                state = "Solid";
                classification = "Tansition Metal";
                break;
            case 47:
                symbol = "Ag";
                element = "Silver";
                atomicMass = "107.87";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 48:
                symbol = "Cd";
                element = "Cadmium";
                atomicMass = "112.41";
                state = "Solid";
                classification = "Transition Metal";
                break;
            case 49:
                symbol = "In";
                element = "Indium";
                atomicMass = "114.82";
                state = "Solid";
                classification = "Post-Transition Metal";
                break;
            case 50:
                symbol = "Sn";
                element = "Tin";
                atomicMass = "118.71";
                state = "Solid";
                classification = "Post-Transition Metal";
                break;

            default:
                symbol = "XX";
                element = "nonexistent";
                atomicMass = "????";
                state = "error";
                classification = "unknown";
                break;

        }

    }

}
