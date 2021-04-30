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
    public void rotate() {
        transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }
    public void rotate2() {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
    public void rotate3() {
        transform.rotation = Quaternion.Euler(0f, 270f, 0f);
    }
    public void createWall(MazeCell  currentCell) {
        transform.parent = currentCell.transform;
        transform.localPosition = Vector3.zero;
    }
}
