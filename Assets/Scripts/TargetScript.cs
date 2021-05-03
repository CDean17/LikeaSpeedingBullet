using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{

    public GameObject hitVersion;
    private GameObject gameManager;
    public float targetWorth = 5f;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    }
    
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Hit target!");
        if (other.gameObject.tag == "Bullet")
        {
            Instantiate(hitVersion, transform.position, transform.rotation);
            gameManager.GetComponent<GameManagerScript>().subtractTime(targetWorth);
            Destroy(gameObject);
        }
    }

}
