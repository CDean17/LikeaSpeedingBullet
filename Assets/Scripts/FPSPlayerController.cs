using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerController : MonoBehaviour
{
    //vars for mouse look
    public float horizontalSens = 0.2f;
    public float verticalSens = 0.2f;
    public GameObject cam;
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public float defaultPlayerScale = 1.5f;

    //vars for movement
    private Rigidbody rb;
    public float groundMoveSpeed = 10f;
    public float crouchMoveSpeed = 5f;
    public float airMoveSpeed = 2f;
    private float moveSpeed = 5.0f;
    public float maxSpeed = 30f;
    public float sprintMod = 1.5f;
    public float jumpSpeed = 5.0f;
    public float slideBoost = 10f;
    public float slideCooldown = 5f;
    private float nextBoost = 0f;
    public float crouchThreshhold = 4f;
    public float groundedCheckDistance = 2f;
    public float aboveObjCheckDistance = 0.6f;
    private bool jumpNow = false;
    private bool canJump = true;
    public bool crouching = false;
    public bool sliding = false;
    public bool slideNow = false;

    //wallrun vars
    public float camTiltVel = 0.5f;
    public float jumpOffForce = 100f;
    public float wallStickForce = 5f;
    public float wallDownForce = 1f;
    public float wallMoveSpeed = 20f;
    public float maxWallrunTime = 8f;
    public float maxWallSpeed = 20f;
    private bool isWallRight, isWallLeft;
    private bool enableWallRun = true;
    private bool isWallRunning;
    public float camTilt = 5f;
    private float targetCamTilt;

    //clambering vars
    public bool clamberObj, autoClamberObj, autoClamberObj2;
    private bool clambering, autoClambering;
    private bool canInitClamber = true;
    private bool canInitRealClamber = true;
    public float autoClamberOffset = 0.5f;
    public float autoClamberMinSpeed = 5.0f;
    public float autoClamberTime = 0.5f;
    public float autoSquishFactor = 0.5f;


    public float clamberTimeMod = 0.3f;    //Reducing this value increases the max time of the clamber. Use desmos to callibrate this value to a time.
    public float clamberSpeed = 15f;
    public float clamberForwardSpeed = 10f;
    public float clamberCrestSpeed = 50f;
    public float clamberDetectionRange = 0.7f;
    public float autoClamberDetectionRange = 1.3f;

    public float clamberStartTime = 0f;
    public float clamberTime = 5f;
    //public float clamberActivatedTime = 2f;  //Alter this with the Time Mod in to get the clamber to end at the right time

    
    private float autoClamberRefresh;
    private float clamberRefresh;
   // private float clamberEndTime;
    


    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();

        //lock and hide cursor when player spawns
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        //mouse movement code and rotates player
        yaw += horizontalSens * Input.GetAxis("Mouse X");
        pitch -= verticalSens * Input.GetAxis("Mouse Y");

        cam.transform.rotation = Quaternion.Slerp(Quaternion.Euler(new Vector3(pitch, yaw, cam.transform.eulerAngles.z)), Quaternion.Euler(new Vector3(pitch, yaw, targetCamTilt)), camTiltVel); ;
        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);

        //check if touching ground and set whether can or cannot jump
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, groundedCheckDistance))
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }


        if (Input.GetKeyDown(KeyCode.Space) && canJump == true)
        {
            jumpNow = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isWallRunning)
        {
            jumpNow = true;

        }



        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            sliding = true;
            crouching = true;
            slideNow = true;
            transform.position = transform.position - new Vector3(0f, 0.2f, 0f);
            
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            crouching = true;


        }
        else
        {
            RaycastHit hit1;
            Ray ray1 = new Ray(transform.position, Vector3.up);
            if (Physics.Raycast(ray1, out hit1, aboveObjCheckDistance) && crouching)
            {
                //Debug.Log("Hit!");
                crouching = true;
                
            }
            else
            {
                crouching = false;
                sliding = false;
                
            }

        }

        if(crouching == true)
        {
            transform.localScale = new Vector3(1, 0.7f, 1);
        }
        else
        {
            if (autoClambering)
            {
                transform.localScale = new Vector3(1, autoClamberOffset, 1);
            }
            else
            {
                transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);
            }
            
        }


        //wallrunning based code
        //check for wall
        isWallRight = Physics.Raycast(transform.position, transform.right, 1f);
        isWallLeft = Physics.Raycast(transform.position, -transform.right, 1f);

        if((isWallRight || isWallLeft) && Input.GetAxis("Vertical") > 0 && enableWallRun)
        {
            isWallRunning = true;
        }
        else
        {
            isWallRunning = false;
            
        }

        if (!(isWallRight || isWallLeft))
            enableWallRun = true;

        //rotate cam based on wall currently on
        if (isWallRight && isWallRunning)
            targetCamTilt = camTilt;
        if (isWallLeft && isWallRunning)
            targetCamTilt = -camTilt;
        if (!isWallRunning)
            targetCamTilt = 0f;

        //code for ledge hoping and clambering
        //make sure to make ledge climbing override wallrunning
        clamberObj = Physics.Raycast(transform.position, transform.forward, clamberDetectionRange);
        autoClamberObj = Physics.Raycast(transform.position - new Vector3(0, autoClamberOffset, 0), transform.forward, autoClamberDetectionRange);
        autoClamberObj2 = Physics.Raycast(transform.position - new Vector3(0, -1, 0), transform.forward, autoClamberDetectionRange);



        if (clamberObj && !canJump && Input.GetKey(KeyCode.Space) && canInitRealClamber && !clambering)
        {
            clambering = true;
            canInitRealClamber = false;

            //clamberEndTime = Time.time + clamberActivatedTime;
            clamberStartTime = Time.time;
            clamberRefresh = Time.time + 100;
        }
        else if(autoClamberObj && !canJump && canInitClamber && !clambering && !clamberObj && !autoClamberObj2)
        {
            autoClambering = true;
            canInitClamber = false;
            autoClamberRefresh = Time.time + autoClamberTime;
            transform.position = transform.position + new Vector3(0, autoClamberOffset, 0);
            rb.AddForce(transform.forward * clamberCrestSpeed);
        }

        if(autoClamberRefresh <= Time.time)
        {
            canInitClamber = true;

            autoClambering = false;
        }

        //reset for proper clambering
        if(clamberRefresh <= Time.time)
        {
            canInitRealClamber = true;
        }
    }

    private void FixedUpdate()
    {
        Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
        Vector3 jumpVect = new Vector3(0, jumpSpeed, 0);
        Vector3 forwardMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 strafingMove = transform.right * Input.GetAxis("Horizontal");


        //determine if in the air or not
        if (canJump == true)
        {

            //set move speed based on whether crouching or moving normally
            if(crouching == true)
            {
                moveSpeed = crouchMoveSpeed;
            }
            else
            {
                moveSpeed = groundMoveSpeed;
            }

            //check if the player is sliding
            if(sliding == true)
            {
                //if the player just started sliding try to boost them
                if(slideNow == true && Time.time > nextBoost)
                {
                   //If the player is moving forward any amount boost them.
                    if (transform.InverseTransformDirection(rb.velocity).z > 0)
                    {
                        rb.velocity += (transform.forward * slideBoost);
                    }
                    //either way dont do this check again till the boost cooldown is expired.
                    slideNow = false;
                    nextBoost = Time.time + 3;

                }

                //if the player is set to jump now add velocity then set jumpnow to false
                if(jumpNow == true)
                {
                    rb.velocity += jumpVect;
                    jumpNow = false;
                }

                //if the player slows down too much while sliding transition them to crouching and set the boost cooldown
                if(rb.velocity.magnitude <= crouchThreshhold)
                {
                    sliding = false;
                    nextBoost = Time.time + slideCooldown;
                }
            }
            else
            {
                //if the player is either standing normally or crouching without sliding use this code to calculate movement vectors and jumping
                if (jumpNow == true)
                {
                    rb.velocity = Vector3.ClampMagnitude((forwardMove + strafingMove) * moveSpeed, moveSpeed) + jumpVect;
                    jumpNow = false;
                }
                else
                {
                    rb.velocity = Vector3.ClampMagnitude((forwardMove + strafingMove) * moveSpeed, moveSpeed) + yVelFix;
                }
            }
            

        }   //Things to while in the air
        else
        {
            //First check for either type of clambering. If clambering can't wallrun.
            if (autoClambering || clambering)
            {
                if (autoClambering)
                {
                    
                }

                if (clambering && Input.GetKey(KeyCode.Space))
                {
                    float curClamberSpeed = clamberSpeed * Mathf.Cos(clamberTimeMod * (Time.time - clamberStartTime));


                    rb.velocity = (transform.forward * clamberForwardSpeed) + (transform.up * curClamberSpeed);


                    if(curClamberSpeed <= 1.0F || clamberObj == false)
                    {
                        clambering = false;

                        clamberRefresh = Time.time + clamberTime;

                        if (!clamberObj)
                        {
                            rb.AddForce(transform.forward * clamberCrestSpeed);

                        }
                    }
                }
                else
                {
                    clambering = false;

                    clamberRefresh = Time.time + clamberTime;
                }
            }
            else
            {
                if (isWallRunning)
                {
                    rb.useGravity = false;
                    //wallrunning code will probably go here as wallrunning requires being in the air.


                    if (isWallRight)
                    {
                        rb.velocity = (transform.forward * wallMoveSpeed) + (transform.right * wallStickForce) + yVelFix + (-transform.up * wallDownForce);
                        if (jumpNow == true)
                        {
                            rb.AddForce((transform.up + -transform.right) * jumpOffForce);
                            jumpNow = false;
                            enableWallRun = false;
                        }

                    }

                    if (isWallLeft)
                    {
                        rb.velocity = (transform.forward * wallMoveSpeed) + (-transform.right * wallStickForce) + yVelFix + (-transform.up * wallDownForce);

                        if (jumpNow == true)
                        {
                            rb.AddForce((transform.up + transform.right) * jumpOffForce);
                            jumpNow = false;
                            enableWallRun = false;
                        }
                    }

                }
                else
                {
                    rb.useGravity = true;
                    //if in the air and not wallrunning add velocity and clamp the vector to a maximum value
                    if (rb.velocity.magnitude < maxSpeed)
                    {
                        rb.AddForce(forwardMove * airMoveSpeed);
                    }
                    rb.AddForce(strafingMove * airMoveSpeed);
                }
            }
            
            



        }

    }
}
