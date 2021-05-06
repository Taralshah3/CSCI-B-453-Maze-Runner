using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Rigidbody rb;

    //represents the path the AI needs to take to reach the ned of the maze
    public static ArrayList AIPath = new ArrayList();

    //represents if the AI has been spawned or not
    bool canStart = false;

    //represents the speed of the AI
    public float speed = 1f;

    //represents the next cell the AI should move towards
    private Transform target;

    //represents the index of the current cell the AI is at
    private int cellIndex;

    // Start is called before the first frame update
    void Start()
    {
        //creates the rigid body for the physics of the AI
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        //stops the AI From rotating
        rb.freezeRotation = true;

        //freezes the Y position of the AI so it doesn't go past the confines of the maze
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        MazeCell current_cell = (MazeCell)AIPath[AIPath.Count-2];
        target = current_cell.transform;
    }

    void Update() {
        //if AI is spawned, starts the process of having it follow the path to solve the maze
        if(canStart) {
            Vector3 direction = target.position - transform.position;
            transform.Translate(direction.normalized*speed*Time.deltaTime, Space.World);

            //if AI is in range of 0.2f to the next cell, switches pathfinding to the next cell in the path
            if(Vector3.Distance(transform.position, target.position) <= 0.2f) {
                GetNextCell();
            }
        }
        

    }


    //gets the next cell in the path to the end of the maze
    void GetNextCell() {
        
        if(cellIndex <= 0) {
            
        } else {
            cellIndex--;
            MazeCell current_cell_2 = (MazeCell)AIPath[cellIndex];
            target = current_cell_2.transform;
        }

        

    }

    
    //sets the path that the AI should take to reach the end of the maze
    public void setPath(ArrayList path) {
        AIPath = path;
        canStart = true;
        cellIndex = AIPath.Count-2;
        
    }
    
}
