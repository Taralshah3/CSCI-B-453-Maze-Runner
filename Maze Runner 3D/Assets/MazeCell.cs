using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] MazeWall wallPrefab;
    // Start is called before the first frame update

    //boolean variables that represent if we can spawn a wall in that direction
    public bool canNorth = true;
    public bool canEast = true;
    public bool canSouth = true;
    public bool canWest = true;

    public bool visited = false;

    //represents the x_value and z_value of that current cell
    public int x_value;
    public int z_value;
    
    //represents the wall objects themselves that are instantiated for each cell
    public MazeWall northWall;
    public MazeWall eastWall;
    public MazeWall southWall;
    public MazeWall westWall;

    //variables for A* pathfinding AI, these are used to compute the next optimal cell to go to 
    public int f = 0;
    public int g = 0;
    public int h = 0;
    

    //keeps the neighbors of the current cell
    public ArrayList neighbors;

    //keeps a pointer to the cell that came previously in the A* pathfinding path
    public MazeCell previous;


    //when the cell is spawned, instantiates a wall in each direction for the cell
    void Start()
    {
        if(canNorth) {
            //creates all the walls
            northWall = Instantiate(wallPrefab) as MazeWall;
            northWall.createWall(this);
        }
        if(canEast) {
            eastWall = Instantiate(wallPrefab) as MazeWall;
            eastWall.createWall(this);
            eastWall.rotate();
        }
        if(canSouth) {
            southWall = Instantiate(wallPrefab) as MazeWall;
            southWall.createWall(this);
            southWall.rotate2();
        }
        if(canWest) {
            westWall = Instantiate(wallPrefab) as MazeWall;
            westWall.createWall(this);
            westWall.rotate3();
        }
    
          
    }
    // Update is called once per frame
    void Update() {

    }
        
}
