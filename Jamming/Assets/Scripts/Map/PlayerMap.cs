using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMap : MonoBehaviour
{
    [SerializeField]
    List<GameObject> waypoints;
    bool stopMove;
    int currentWaypoint;
    Vector3 target;
    float currentDir;
    Vector3 startLoc;
    // Start is called before the first frame update
    private void Start()
    {
        currentWaypoint = 0; //TODO: need to fix on exiting lvl 2
        stopMove = true;
        target = waypoints[currentWaypoint].transform.position;
        startLoc = target;
        currentDir = 0; 
    }
    // Update is called once per frame
    void Update()
    {
        float LR = Input.GetAxis("Horizontal"); //-1 = left?
        if(LR != 0 && stopMove)
        {
            currentDir = LR;
            stopMove = false;
        }
        if (!stopMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 5f * Time.deltaTime);
            if (Vector2.Distance(transform.position, target) < 0.2f) //magic number, can adjust as needed
            {
                if (waypoints[currentWaypoint].GetComponent<MapNode>().type == MapNode.pointType.stop && target != startLoc)
                {
                    startLoc = target;
                    stopMove = true;
                }
                else if (currentDir < 0 && currentWaypoint < (waypoints.Count - 1))
                {
                    currentWaypoint += 1;
                    target = waypoints[currentWaypoint].transform.position;
                }
                else if (currentDir > 0 && currentWaypoint > 0)
                {
                    currentWaypoint -= 1;
                    target = waypoints[currentWaypoint].transform.position;
                }
            }
        }
    }
}
