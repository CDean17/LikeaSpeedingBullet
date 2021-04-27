using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    private Rigidbody rb;
    public float projectileSpeed = 100f;
    public float maxLifetime = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * projectileSpeed;
        StartCoroutine(destroyAfterTime(maxLifetime));
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    IEnumerator destroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
