using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] MazeWall wallPrefab;
    // Start is called before the first frame update
    public bool canNorth = true;
    public bool canEast = true;
    public bool canSouth = true;
    public bool canWest = true;

    public bool visited = false;

    public int x_value;
    public int z_value;
    
    public MazeWall northWall;
    public MazeWall eastWall;
    public MazeWall southWall;
    public MazeWall westWall;


    //don't know if current walls will be deleted or not 
    void Start()
    {
        if(canNorth) {
            //creates all the walls
            northWall = Instantiate(wallPrefab) as MazeWall;
            northWall.createWall(this);
        //northWall.transform.localPosition = Quaternion.Euler(0f, 90f, 0f);
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
