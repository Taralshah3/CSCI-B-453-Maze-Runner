using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Rigidbody rb;
    public static ArrayList AIPath = new ArrayList();
    public int current_index;
    bool canStart = false;
    public float speed = 1f;
    private Transform target;
    private int cellIndex;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;

        MazeCell current_cell = (MazeCell)AIPath[AIPath.Count-2];
        target = current_cell.transform;
    }

    void Update() {
        if(canStart) {
            Vector3 direction = target.position - transform.position;
            transform.Translate(direction.normalized*speed*Time.deltaTime, Space.World);

            if(Vector3.Distance(transform.position, target.position) <= 0.2f) {
                GetNextCell();
            }
        }
        

    }

    void GetNextCell() {

        if(cellIndex <= 0) {
            //Destroy(gameObject);
        } else {
            cellIndex--;
            MazeCell current_cell_2 = (MazeCell)AIPath[cellIndex];
            target = current_cell_2.transform;
        }

        

    }

  
    public void setPath(ArrayList path) {
        AIPath = path;
        //print(AIPath.Count);
        canStart = true;
        cellIndex = AIPath.Count-2;
        //current_index = AIPath.Count-2;
        
    }
    
}
