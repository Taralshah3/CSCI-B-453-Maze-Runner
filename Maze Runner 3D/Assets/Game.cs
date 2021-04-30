using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] MazeCell cellPrefab;
    int x_size, z_size;
    MazeCell[,] mazeCurrent;
    MazeCell currentCell;
    Stack<MazeCell> cellStack = new Stack<MazeCell>();
    // Start is called before the first frame update
    void Start()
    {
        createMaze(10,10);

        //sets the initial current cell after maze is created
        currentCell = mazeCurrent[0,0];
        
        
    }

    // Update is called once per frame
    //supposed to update the current cell, idk if it is working or not though
    //might need to change where this block of code is
    void Update()
    {   
        currentCell.visited = true;

        currentCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;

        MazeCell currentNeighbor = getRandomNeighbor(currentCell.x_value,currentCell.z_value);
        if(currentNeighbor != null) {
            MazeCell nextCell = getRandomNeighbor(currentCell.x_value,currentCell.z_value);
            nextCell.visited = true;

            nextCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;

            cellStack.Push(currentCell);
            //nextCell.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1);

            removeWall(currentCell,nextCell);
            //Destroy(currentCell.gameObject);
            //Debug.Log(currentCell);

            currentCell = nextCell;
    
            //Debug.Log("X value: " + currentNeighbor.x_value  + "Y value: " + currentNeighbor.z_value);
        } else if(cellStack.Count > 0) {
            currentCell = cellStack.Pop();
        }


    }

    void removeWall(MazeCell currentCell, MazeCell nextCell) {
        int xDiff = currentCell.x_value - nextCell.x_value;
        int zDiff = currentCell.z_value - nextCell.z_value;

        //Debug.Log("XDIFF: " + xDiff);
        //Debug.Log("ZDiff: " + zDiff);

        if(xDiff > 0) {
            currentCell.canWest = false;
            nextCell.canEast = false;
            Destroy(currentCell.westWall.gameObject);
            Destroy(nextCell.eastWall.gameObject);


        } else if(xDiff < 0) {
            currentCell.canEast = false;
            nextCell.canWest = false;
            
            Destroy(currentCell.eastWall.gameObject);
            Destroy(nextCell.westWall.gameObject);


        }
        if(zDiff < 0) {
            currentCell.canNorth = false;
            nextCell.canSouth = false;

            Destroy(currentCell.northWall.gameObject);
            Destroy(nextCell.southWall.gameObject);


        } else if(zDiff > 0) {
            currentCell.canSouth = false;
            nextCell.canNorth = false;

            Destroy(currentCell.southWall.gameObject);
            Destroy(nextCell.northWall.gameObject);

        }

    }
    
    void createMaze(int x_size, int z_size) {
        this.x_size = x_size;
        this.z_size = z_size;
        mazeCurrent = new MazeCell[x_size, z_size];
        for(int x = 0; x < x_size; x++) {
            for(int z = 0; z < z_size; z++) {
                MazeCell currentCell = Instantiate(cellPrefab) as MazeCell;
                currentCell.transform.localPosition = new Vector3(x-x_size*0.5f+0.5f, 0f, z-z_size*0.5f+0.5f);
                currentCell.x_value = x;
                currentCell.z_value = z;
                mazeCurrent[x,z] = currentCell;

            }
        }
    }

    MazeCell getRandomNeighbor(int x_point, int z_point) {
        List<MazeCell> neighbors = new List<MazeCell>();

        //isInGrid makes sure that the points are within the size of the maze
        if(isInGrid(x_point-1, z_point)) {
            MazeCell west = mazeCurrent[x_point-1,z_point];

            //checks to see if that cell has already been visited
            if(west.visited == false) {
                neighbors.Add(west);
            }
        }
        //might be south
        if(isInGrid(x_point, z_point+1)) {
            MazeCell north = mazeCurrent[x_point, z_point+1];
            if(north.visited == false) {
                neighbors.Add(north);
            }
        }
        if(isInGrid(x_point, z_point-1)) {
            MazeCell south = mazeCurrent[x_point, z_point-1];
            if(south.visited == false) {
                neighbors.Add(south);
            }
        }
        if(isInGrid(x_point+1, z_point)) {
            MazeCell east = mazeCurrent[x_point+1, z_point];
            if(east.visited == false) {
                neighbors.Add(east);
            }
        }

        if(neighbors.Count > 0) {
            System.Random rnd = new System.Random();
            int randomInt = rnd.Next(neighbors.Count);
            MazeCell randomNeighbor = neighbors[randomInt];
            return randomNeighbor;

        } else {
            return null;
        }
        

    }
    bool isInGrid(int x_point, int z_point) {
        if((x_point >= 0 & x_point < x_size) & (z_point >= 0 & z_point < z_size)) {
            return true;
        } else {
            return false;
        }
    }
}
