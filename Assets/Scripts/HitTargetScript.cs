using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitTargetScript : MonoBehaviour
{
    public GameObject hitVersion;

    public void resetTarget()
    {
        Instantiate(hitVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
