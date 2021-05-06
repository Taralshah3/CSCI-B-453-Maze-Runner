using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //function to rotate the east wall of a cell in the right direction
    public void rotate() {
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }

    //function to rotate the south wall of a cell in the right direction
    public void rotate2() {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    //function to rotate the west wall of a cell in the right direction
    public void rotate3() {
        transform.rotation = Quaternion.Euler(0f, 270f, 0f);
    }

    //creates the wall and sets its position
    public void createWall(MazeCell  currentCell) {
        transform.parent = currentCell.transform;
        transform.localPosition = Vector3.zero;
    }
}
