using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private Transform mainCam;
    public Transform gunVisualT;
    private GameObject player;
    public Vector3 playerOffset = new Vector3(0.46f, 0.471f, 0.269f);
    public Vector3 targetRot;
    private Vector3 targetPos;
    private Vector3 posVel = Vector3.zero;
    private Vector3 prevAngle;
    public float smoothTime;
    public float posSmoothTime;

    //function vars
    public float fireDelay = 0.2f;
    private float nextFire = 0.0f;

    //recoil vars
    public float zRecoil = 0.4f;
    public float xRecoil = 5f;
    public float yRecoil = 10f;
    public float recoveryTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera").transform;
        player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {
        targetRot = new Vector3(mainCam.transform.rotation.eulerAngles.x, mainCam.transform.rotation.eulerAngles.y, 15f);
        targetPos = player.transform.position + (playerOffset.x * player.transform.right) + (playerOffset.y * player.transform.up) + (playerOffset.z * player.transform.forward);
        Quaternion targetQuat = Quaternion.Euler(targetRot);

        if(gunVisualT.localRotation.x != 0 || gunVisualT.localRotation.y != 0)
        {
            gunVisualT.localRotation = Quaternion.Slerp(gunVisualT.localRotation, Quaternion.Euler(Vector3.zero), fireDelay);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, smoothTime);
        transform.position = targetPos;

        //code for firing and recoil
        if(Input.GetMouseButton(0) && nextFire < Time.time)
        {
            gunVisualT.localPosition -= new Vector3(0 , 0, zRecoil);

            gunVisualT.localRotation = Quaternion.Euler(new Vector3(xRecoil, yRecoil, 0));

            nextFire = Time.time + fireDelay;
        }

        gunVisualT.localPosition = Vector3.SmoothDamp(gunVisualT.localPosition, Vector3.zero, ref posVel, recoveryTime);
    }
}
