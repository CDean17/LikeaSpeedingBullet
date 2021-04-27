using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathplaneScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out PlayerManagementScript p))
        {
            p.resetToCheckpoint();
        }
    }
}
