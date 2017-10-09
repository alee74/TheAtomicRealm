using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonController : MonoBehaviour {

    // TODO : Adjust starting position to be radius distance from edge, if possible

    #region Property and Constant Declarations

    private Rigidbody2D photon;     // Photon's Rigidbody

    public float speed = 15f;
    public float scale = 0.015f;
    public float radius;

    // Descriptive Constants
    private const int endAtX = 0;
    private const int endAtY = 1;
    private const int FLOOR = 0;
    private const int NORTH = 0;
    private const int EAST = 1;
    private const int SOUTH = 2;
    private const int WEST = 3;

    // Values from LevelGenerator script
    private int numMapElems;    // Number of Wall/Floor spaces per row/column (height/width of map in map elements)
    private int mapElemSize;    // height/width of Wall/Floor in game units
    private int lvlOffset;      // Half of height/width of map in game units
    System.Random rng;          // Random Number Generator

    private Pair dir;
    private Pair nextPoint;
    private Pair currentPoint;
    private List<Pair> path;
    // Offsets used in calculating shortest path and setting Photon velocity
    private Pair[] offsets = { new Pair(0, 1), new Pair(1, 0),
                                   new Pair(0, -1), new Pair(-1, 0) };
    #endregion

    // Use this for initialization
    void Awake() {

        photon = GetComponent<Rigidbody2D>();                               // Grab Photon's Rigidbody
        photon.transform.localScale = new Vector3(scale, scale, 1);         // Set render scale of Photon
        radius = photon.GetComponent<CircleCollider2D>().radius * scale;    // Calculate actual radius of rendered Photon

        // Grab values from LevelGenerator script
        numMapElems = LevelManagement.SharedInstance.numElems;
        mapElemSize = LevelManagement.SharedInstance.elemSize;
        lvlOffset = LevelManagement.SharedInstance.lvlSize / 2;
        rng = LevelManagement.SharedInstance.RNG;

    }

    // Update is called once per frame
    void Update() {

        // Ensure Photon is active
        if (photon.gameObject.activeInHierarchy) {

            // Check if need to update location and travel direction
            if ((dir == offsets[EAST] && transform.position.x >= xTransform(nextPoint.x)) || 
                (dir == offsets[WEST] && transform.position.x <= xTransform(nextPoint.x)) ||
                (dir == offsets[NORTH] && transform.position.y >= yTransform(nextPoint.y)) || 
                (dir == offsets[SOUTH] && transform.position.y <= yTransform(nextPoint.y)))
            {

                // Only update location and direction if not at end of path
                if (path.Count > 0) {
                    advancePoints();
                    dir = getDirection();
                }

            }

            // Set Photon's velocity
            photon.velocity = new Vector2(dir.x * speed, dir.y * speed);
        }

    }

    void OnCollisionEnter2D(Collision2D other) {

        Debug.Log("Photon death due to collision with " + other.gameObject.tag);
        photon.gameObject.SetActive(false);

    }


    #region Pooled Photon Management

    /// <summary>
    /// Called when Photon becomes active
    /// </summary>
    void OnEnable() {

        #region Generate random starting location and path for Photon to follow

        if (rng.Next() % 2 == 0) {  // Spawn on left/right edge (traverse x)

            if (rng.Next() % 2 == 0)    // Spawn on left edge
                path = shortestPath(new Pair(0, rng.Next(numMapElems)), numMapElems - 1, endAtX);
            else                        // Spawn on right edge
                path = shortestPath(new Pair(numMapElems - 1, rng.Next(numMapElems)), 0, endAtX);

        } else {    // Spawn on top/bottom edge (traverse y)

            if (rng.Next() % 2 == 0)    // Spawn on top edge
                path = shortestPath(new Pair(rng.Next(numMapElems), 0), numMapElems - 1, endAtY);
            else                        // Spawn on bottom edge
                path = shortestPath(new Pair(rng.Next(numMapElems), numMapElems - 1), 0, endAtY);
        }
        #endregion

        if (path == null) {     // Photon is trapped

            // Set inactive to be reused
            photon.gameObject.SetActive(false);

        } else {    // path exists

            // Get starting location and direction
            nextPoint = path[0];
            path.RemoveAt(0);
            advancePoints();
            dir = getDirection();

            // Set location of Photon
            transform.position = new Vector3(xTransform(currentPoint.x), yTransform(currentPoint.y), transform.position.z);

        }

    }

    /// <summary>
    /// Called when Photon becomes inactive
    /// </summary>
    void OnDisable() {

        if (path != null)
            path.Clear();

    }
    #endregion

    #region Functions for map traversal

    #region Shortest Path Algorithm
    /// <summary>
    /// Calculate shortest path photon can take from starting location to opposite edge of map
    /// </summary>
    /// <param name="start"> Pair (xy-coordinate) indicating photon's starting position, relative to map </param>
    /// <param name="end"> int corresponding to index of destination row/column </param>
    /// <param name="endType"> int that describes traversal as horizontal or vertical </param>
    /// <returns> list of Pairs (xy-coordinates) that form a path from one edge to the opposite edge </returns>
    List<Pair> shortestPath(Pair start, int end, int endType) {

        int[,] map = LevelManagement.SharedInstance.map;         // 2D array of 0s and 1s representing Wall and Floor elements
        Queue<QueueType> queue = new Queue<QueueType>();
        HashSet<Pair> visited = new HashSet<Pair>();            // Track points already visited by algorithm
        queue.Enqueue(new QueueType(start, new List<Pair>()));  // Enqueue starting location, empty list, and empty set
        visited.Add(start);

        while (queue.Count > 0) {

            QueueType current = queue.Dequeue();
            // Step in each direction (up, down, left, right) and
            foreach (Pair offset in offsets) {

                Pair newPoint = new Pair(current.point.x + offset.x, current.point.y + offset.y);
                // If current location is not a wall and is the desired ending location, add points to path and return it
                if (((endType == endAtX && newPoint.x == end) || (endType == endAtY && newPoint.y == end)) && map[newPoint.y, newPoint.x] == FLOOR) {

                    current.path.Add(current.point);
                    current.path.Add(newPoint);
                    return current.path;

                }
                // Otherwise, check that next location is within map bounds, is not a wall, and has not already been visited
                if (newPoint.x >= 0 && newPoint.x < numMapElems && newPoint.y >= 0 && newPoint.y < numMapElems && map[newPoint.y, newPoint.x] == FLOOR && !visited.Contains(newPoint)) {

                    List<Pair> newPath = new List<Pair>(current.path);
                    newPath.Add(current.point);
                    queue.Enqueue(new QueueType(newPoint, newPath));
                    visited.Add(newPoint);

                }
            }
        }
        // If queue is empty, there is no path from starting location to opposite edge of map
        return null;
    }
    #endregion

    /// <summary>
    /// Get the direction the photon should be traveling, according to the calculated path
    /// </summary>
    /// <returns> a Pair (xy-pair) representing the direction in which the photon should travel </returns>
    Pair getDirection() {

        if (currentPoint.y > nextPoint.y)
            return offsets[NORTH];
        if (currentPoint.x < nextPoint.x)
            return offsets[EAST];
        if (currentPoint.y < nextPoint.y)
            return offsets[SOUTH];
        if (currentPoint.x > nextPoint.x)
            return offsets[WEST];

        return new Pair(0, 0);
    }

    /// <summary>
    /// Step forward in Photon's path
    /// </summary>
    void advancePoints() {

        currentPoint = nextPoint;

        if (path.Count > 0) {
            nextPoint = path[0];
            path.RemoveAt(0);
        }
    }
    #endregion

    #region Coordinate Transformation Functions

    /// <summary>
    /// Transform 2D array x-coordinate to game unit coordinate
    /// </summary>
    /// <param name="inX"> 2D array x-coordinate, corresponds to column of map used for photon's path </param>
    /// <returns> x-coordinate corresponding to center of floor/wall tile in game </returns>
    float xTransform(int inX) {

        if (inX < numMapElems / 2) // column is left of origin, value should be negative
            return -(lvlOffset - (inX * mapElemSize)) + (mapElemSize / 2f);
        else // column is right of origin, value should be positive
            return ((inX - (numMapElems / 2)) * mapElemSize) + (mapElemSize / 2f);
    }


    /// <summary>
    /// Transform 2D array y-coordinate to game unit coordinate
    /// </summary>
    /// <param name="inY"> 2D array y-coordinate, corresponds to row of map used for photon's path </param>
    /// <returns> y-coordinate corresponding to center of floor/wall tile in game </returns>
    float yTransform(int inY) {

        if (inY < numMapElems / 2) // row is above origin, value should be positive
            return lvlOffset - (inY * mapElemSize) - (mapElemSize / 2f);
        else // row is below origin, value should be negative
            return -((inY - (numMapElems / 2)) * mapElemSize) - (mapElemSize / 2f);
    }
    #endregion

    #region Structs

    /// <summary>
    /// Defines a tuple (x, y) analogous to an xy coordinate
    /// </summary>
    private struct Pair {

        public int x;
        public int y;

        public Pair(int xx, int yy) {
            x = xx;
            y = yy;
        }

        public static bool operator==(Pair that, Pair other) { return other.x == that.x && other.y == that.y; }
        public static bool operator!=(Pair that, Pair other) { return !(that == other); }

    };

    /// <summary>
    /// Defines a 3-tuple type (Pair, List<Pair>, HashSet<Pair>) for use within queue in shortestPath algorithm
    /// </summary>
    private struct QueueType {

        public Pair point;              // Current point
        public List<Pair> path;         // Accumulated path

        public QueueType(Pair poi, List<Pair> pat) {
            point = poi;
            path = pat;
        }
    };
    #endregion

}


//Debug.Log("Photon instantiated at map (" + currentPoint.x + ", " + currentPoint.y + "), game (" + xTransform(currentPoint.x) + ", " + yTransform(currentPoint.y) + ")");