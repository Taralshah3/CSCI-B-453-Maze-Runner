using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] MazeCell cellPrefab;
    [SerializeField] AI AIPrefab;
    [SerializeField] GameObject _gameOverText;

    bool AICreated = false;
    bool playerCreated = false;
     AI currentAI;
    [SerializeField] Player playerPrefab;
    Player currentPlayer;
    int x_size, z_size;
    MazeCell[,] mazeCurrent;
    MazeCell currentCell;
    Stack<MazeCell> cellStack = new Stack<MazeCell>();

    //stacks used for A* path-finding
    ArrayList openStack = new ArrayList();
    ArrayList closedStack = new ArrayList();
    
    public MazeCell start;
    public MazeCell end;
    public MazeCell playerEnd;
    //public int levelsSurvived = 0;
    // Start is called before the first frame update

    public int numberRows = 10;
    public int numberCols = 10;
    public ArrayList finalPath = new ArrayList();
    
    void Start()
    {
        createMaze(numberRows,numberCols);

        //sets the initial current cell after maze is created
        currentCell = mazeCurrent[0,0];
        playerEnd = mazeCurrent[0, numberCols-1];

        //AIPathfinding();
        createPlayer();
        
        
        

        
        
    }
    //provides the path for the AI to follow to the goal using A*
    //current start point is 0,0 whereas the end point is numberRows-1, numberCols-1 
    void AIPathfinding() {
        //Debug.Log("AI Pathfinding start");
        //Task.Delay(100000);
        
        //starting index of AI
        start = mazeCurrent[0,0];
        start.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;

        //stops at this line of code for some reason?
        openStack.Add(start);
        
        //ending index of AI 
        end = mazeCurrent[numberRows-1, numberCols-1];
        
        AddNeighborsToCells();
        while(openStack.Count > 0) {
            int highestNeighbor = 0;

            //MazeCell test = openStack.Pop();
            for(int i = 0; i < openStack.Count; i++) {
                MazeCell current = (MazeCell)openStack[i];
                MazeCell neighbor = (MazeCell)openStack[highestNeighbor];

                if(current.f < neighbor.f) {
                    highestNeighbor = i;
                }


            }

            MazeCell currentCell = (MazeCell)openStack[highestNeighbor];

            if(currentCell == end) {
                //Debug.Log("DONE");
                finalPath = new ArrayList();
                MazeCell tempCell = currentCell;
                finalPath.Add(tempCell);
                
                

                //traces back the path and adds it to the Final Path array
                while(tempCell.previous) {
                    finalPath.Add(tempCell.previous);
                    tempCell = tempCell.previous;
                }

                //traces back the final path for the AI and makes it yellow so I can see it 
                for(int i = 0; i < finalPath.Count; i++) {
                    MazeCell pathCell = (MazeCell)finalPath[i];
                    pathCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                }

                startAI();
                //createPlayer();

            }


            openStack.Remove(currentCell);
            closedStack.Add(currentCell);

            ArrayList currentNeighbors = currentCell.neighbors;
            for(int i = 0; i < currentNeighbors.Count; i++) {
                MazeCell neighbor = (MazeCell)currentNeighbors[i];

                if(!(closedStack.Contains(neighbor))) {
                    int modifiedG = currentCell.g + 1;
                    if(openStack.Contains(neighbor)) {
                        if(modifiedG < neighbor.g) {
                            neighbor.g = modifiedG;
                        }
                    } else {
                        neighbor.g = modifiedG;
                        openStack.Add(neighbor);
                    }

                    neighbor.h =  ManhattanDistance(neighbor, end);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = currentCell;
                }
                
            }
        }
        
    }

    public int ManhattanDistance(MazeCell neighbor, MazeCell end) {
        return Math.Abs(neighbor.x_value - end.x_value) + Math.Abs(neighbor.z_value - end.z_value);
    }

    public void AddNeighborsToCells() {

        for(int i = 0; i < numberRows; i++) {
            for(int j = 0; j < numberCols; j++) {

                ArrayList currentNeighbors = new ArrayList();
                if(isInGrid(i-1, j)) {
                    MazeCell west = mazeCurrent[i-1,j];
                    
                    if(west.eastWall == null) {
                        currentNeighbors.Add(west);
                    }
                    
                       
                } 
                if(isInGrid(i, j+1)) {
                    MazeCell north = mazeCurrent[i, j+1];
                    if(north.southWall == null) {
                        currentNeighbors.Add(north);
                    }
                    
                }
                if(isInGrid(i, j-1)) {
                    MazeCell south = mazeCurrent[i, j-1];
                    if(south.northWall == null) {
                        currentNeighbors.Add(south);
                    }
                    

                }
                if(isInGrid(i+1, j)) {
                    MazeCell east = mazeCurrent[i+1,j];
                    if(east.westWall == null) {
                        currentNeighbors.Add(east);
                    }
                    
                    
                    
                }
                mazeCurrent[i,j].neighbors = currentNeighbors;
                
            }
        }
        /*ArrayList startNeighbors = mazeCurrent[0,0].neighbors;
        for(int v = 0; v < startNeighbors.Count; v++) {
            MazeCell mazeA = (MazeCell)startNeighbors[v];
            Debug.Log(mazeA.x_value + ", " + mazeA.z_value);
        }*/
        
    }

    // Update is called once per frame
    //supposed to update the current cell, idk if it is working or not though
    //might need to change where this block of code is
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
             Debug.Log("RESTART");
             //_gameOverText.SetActive(false);
             Application.LoadLevel(0);
         }


        if(AICreated) {
            if(didAIWin()) {
                 Debug.Log("AI WON!");
                 initiateGameOver();
            }
        }
        if(playerCreated) {
            if(didPlayerWin()) {
                Debug.Log("PLAYER WON!");
                //levelsSurvived += 1;
                Application.LoadLevel(0);
            }

        }
        //checks to see if AI won
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
            
        } else {
            //calls AI Pathfinding once building the maze is complete
            AIPathfinding();
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

        //Debug.Log("Create Maze Done");
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
    public void startAI() {
        currentAI = Instantiate(AIPrefab) as AI;
        AICreated = true;
        currentAI.setPath(finalPath);
    }
    public void createPlayer() {
        currentPlayer = Instantiate(playerPrefab) as Player;
        playerCreated = true;

    }
    public bool didAIWin() {
        if(Vector3.Distance(currentAI.transform.position, end.transform.position) <= 0.2f) {
            return true;
        }
        return false;
    }
    public bool didPlayerWin() {
        float playerDistToEnd = Vector3.Distance(currentPlayer.transform.position, playerEnd.transform.position);
        if(playerDistToEnd <= 0.35f) {
            return true;
        }
        return false;
    }
    public void initiateGameOver() {
        _gameOverText.SetActive(true);
        
        //stops player movement once the game is over
        currentPlayer.isGameOver = true;
    }

}


