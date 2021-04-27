using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagementScript : MonoBehaviour
{

    private GameObject lastCheckpoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setCheckpoint(GameObject g)
    {
        lastCheckpoint = g;
    }

    public void resetToCheckpoint()
    {
        transform.position = lastCheckpoint.transform.position;
    }
}
