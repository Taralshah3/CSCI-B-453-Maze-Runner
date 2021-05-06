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
    //used to instantiate the maze cells
    [SerializeField] MazeCell cellPrefab;

    //used to instantiate the AI
    [SerializeField] AI AIPrefab;

    //used to hold the text that displays "Game Over" when the player loses the game
    [SerializeField] GameObject _gameOverText;

    //used to instantiate the player
    [SerializeField] Player playerPrefab;


    //check to see if the AI is created
    bool AICreated = false;

    //check to see if the player is created
    bool playerCreated = false;

    //holds the current AI object
     AI currentAI;
    
    //holds the current player object
    Player currentPlayer;

    //holds the size of the maze you want to create
    int x_size, z_size;

    //two dimensional array of maze cells in the current maze
    MazeCell[,] mazeCurrent;
    MazeCell currentCell;

    //stack used for the creation of the maze 
    Stack<MazeCell> cellStack = new Stack<MazeCell>();

    //stacks used for A* path-finding
    ArrayList openStack = new ArrayList();
    ArrayList closedStack = new ArrayList();

    //used to represent the starting point for the AI in the maze
    
    public MazeCell start;

    //used to represent the ending point for the AI in the maze
    public MazeCell end;

    //used to represent the ending point for the player in the maze
    public MazeCell playerEnd;
    
    public int numberRows = 10;
    public int numberCols = 10;

    //used to store the path that the AI should travel to complete the maze
    public ArrayList finalPath = new ArrayList();
    
    void Start()
    {
        createMaze(numberRows,numberCols);

        //sets the initial current cell after maze is created
        currentCell = mazeCurrent[0,0];
        playerEnd = mazeCurrent[0, numberCols-1];

        createPlayer();
    }

    //provides the path for the AI to follow to the goal using A*
    //current start point is 0,0 whereas the end point is numberRows-1, numberCols-1 
    void AIPathfinding() {
        //starting index of AI
        start = mazeCurrent[0,0];
        start.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;

        openStack.Add(start);
        
        //ending index of AI 
        end = mazeCurrent[numberRows-1, numberCols-1];
        
        AddNeighborsToCells();
        while(openStack.Count > 0) {
            int highestNeighbor = 0;

            for(int i = 0; i < openStack.Count; i++) {
                MazeCell current = (MazeCell)openStack[i];
                MazeCell neighbor = (MazeCell)openStack[highestNeighbor];

                if(current.f < neighbor.f) {
                    highestNeighbor = i;
                }


            }

            MazeCell currentCell = (MazeCell)openStack[highestNeighbor];

            if(currentCell == end) {
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


    //used as the heuristic for the A* algorithm
    public int ManhattanDistance(MazeCell neighbor, MazeCell end) {
        return Math.Abs(neighbor.x_value - end.x_value) + Math.Abs(neighbor.z_value - end.z_value);
    }

    //adds the relevant neighbors of each cell to each maze cell object
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
             Application.LoadLevel(0);
         }

        //initiates game over if the AI wins the level
        if(AICreated) {
            if(didAIWin()) {
                 initiateGameOver();
            }
        }

        //creates a new level if the player beats the AI in the level
        if(playerCreated) {
            if(didPlayerWin()) {
                Application.LoadLevel(0);
            }

        }

        //checks to see if AI won
        currentCell.visited = true;
        

        //As maze is being built, makes the base color of the maze red
        currentCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;


        //iteratively goes throughout the process of removing walls and making a "maze" after the base maze (no walls removed) has been created
        MazeCell currentNeighbor = getRandomNeighbor(currentCell.x_value,currentCell.z_value);
        if(currentNeighbor != null) {
            MazeCell nextCell = getRandomNeighbor(currentCell.x_value,currentCell.z_value);
            nextCell.visited = true;

            nextCell.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;

            cellStack.Push(currentCell);
           

            removeWall(currentCell,nextCell);
           
            currentCell = nextCell;
    
        } else if(cellStack.Count > 0) {
            currentCell = cellStack.Pop();
            
        } else {
            //calls AI Pathfinding once building the maze is complete
            AIPathfinding();
        }

          

    }

    //checks to see if a set of walls should be removed between two cells, and if so, removes them based on the directions they are in.
    //used for maze creation
    void removeWall(MazeCell currentCell, MazeCell nextCell) {
        int xDiff = currentCell.x_value - nextCell.x_value;
        int zDiff = currentCell.z_value - nextCell.z_value;


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
    
    //initializes the process to create the maze 
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

    //gets random neighbor of a current cell for maze creation
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
    //checks to see if a certain x and z coordinate is within the confines of the maze
    bool isInGrid(int x_point, int z_point) {
        if((x_point >= 0 & x_point < x_size) & (z_point >= 0 & z_point < z_size)) {
            return true;
        } else {
            return false;
        }
    }
    //creates the AI and starts its pathfinding toward the end of the maze
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


