using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidController : MonoBehaviour
{
    public GameObject boidPrefab;
    public GameObject goalMarker;
    int boidCount = 7;
    float detection_radius = 1.5f;

    private class BoidStatus
    {
        public Vector3 position;
        public GameObject boidObject;
        public Vector3 velocity;
    }

    private List<BoidStatus> statusList;

    // Start is called before the first frame update
    void Start()
    {
        statusList = new List<BoidStatus>();
        for (int i = 0; i < boidCount; ++i)
        {
            for (int j = 0; j < boidCount; ++j)
            {
                GameObject newBoid = new GameObject();
                newBoid.transform.parent = gameObject.transform;
                newBoid.name = "Boid No." + (i * boidCount + j).ToString();

                GameObject instPrefab = Instantiate(boidPrefab);
                instPrefab.transform.parent = newBoid.transform;

                Vector3 startingPos = new Vector3((float)j - 4, (float)-3, (float)i - 4);
                BoidStatus status = new BoidStatus();                
                status.position = startingPos + goalMarker.transform.position;
                newBoid.transform.position = status.position;
                status.boidObject = newBoid;
                status.boidObject.transform.localScale *= .25f;
                // status.velocity = new Vector3(0, 0, 0);
                statusList.Add(status);
            }
        }
        boidPrefab.SetActive(true);
    }
   
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < statusList.Count; ++i)
        {
            BoidStatus status = statusList[i];
            status.velocity = Vector3.zero;
            Vector3 alignment = align_velocity(status);
            Vector3 cohesion = calculate_cohesion(status);
            Vector3 separation = calculate_separation(status);
            Vector3 leader = goalMarker.transform.position - status.position;

            status.velocity = alignment * 1.5f + cohesion * 1.5f + separation * 4f + leader;
            status.velocity = status.velocity.normalized;
            status.position += status.velocity * .15f;
            
            //terrain collision prevention
            if(status.position.x < terrainMap.subdivision && status.position.z < terrainMap.subdivision && status.position.x >= 0 && status.position.z >= 0) {
                if(status.position.y <= terrainMap.perlinHeight[(int)status.position.z, (int)status.position.x]+1f) {
                    status.position.y = status.position.y + terrainMap.perlinHeight[(int)status.position.z, (int)status.position.x];
                    //Debug.Log("got here");
                }
            }            

            status.boidObject.transform.LookAt(status.position);
            status.boidObject.transform.Rotate(0, 90, 90);
            status.boidObject.transform.position = status.position;

            //status.boidObject.transform.forward = Vector3.Normalize(goalMarker.transform.position - status.boidObject.transform.position);
            statusList[i] = status;
        }
    }

    // next 3 functions inspired by: https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
    Vector3 align_velocity (BoidStatus boid) {
        Vector3 point = boid.position;
        int neighbour_count = 0;
        Vector3 velocity = new Vector3(0, 0, 0);
        for(int i = 0; i < statusList.Count; ++i) {
            if(statusList[i] != boid) {
                if(Vector3.Distance(statusList[i].position, point) < detection_radius) {
                    velocity += statusList[i].velocity;
                    neighbour_count++;
                }
            }
        }
        if( neighbour_count != 0){
            velocity /= neighbour_count;
        }
        return velocity.normalized;
    }
    
    Vector3 calculate_cohesion (BoidStatus boid) {
        int neighbour_count = 0;
        Vector3 point = boid.position;
        Vector3 velocity = new Vector3(0, 0, 0);
        for(int i = 0; i < statusList.Count; ++i) {
            if(statusList[i] != boid) {
                if(Vector3.Distance(statusList[i].position, point) < detection_radius) {
                    velocity += statusList[i].position;
                    neighbour_count++;
                }
            }
        }
        if(neighbour_count != 0) {
            velocity /= neighbour_count;
            velocity -= point;
        }       
        return velocity.normalized;
    }

    Vector3 calculate_separation (BoidStatus boid) {
        int neighbour_count = 0;
        Vector3 point = boid.position;
        Vector3 velocity = new Vector3(0, 0, 0);
        for(int i = 0; i < statusList.Count; ++i) {
            if(statusList[i] != boid) {
                if(Vector3.Distance(statusList[i].position, point) < detection_radius) {
                    velocity += statusList[i].position - point;
                    neighbour_count++;
                }
            }
        }
        if(neighbour_count != 0) {
            velocity /= neighbour_count;
            velocity *= -1;
        }
        return velocity.normalized;
    }
}
