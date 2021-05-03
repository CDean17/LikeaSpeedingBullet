using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    private GameObject timeDisplay;
    private GameObject speedDisplay;
    private GameObject mainPanel;
    private GameObject player;
    public GameObject playerSpawnObj;
    private GameObject playerSpawn;

    private bool waitingToStart = true;
    private bool finished = false;
    //public float minStartVelocity = 2f;
    private float startTime = 0f;
    private float curTime = 0f;
    private float finalTime = 0f;

    //endcard variables
    private GameObject endOfLevelPanel;
    private GameObject finalTimeDisplay;
    private GameObject exitToMenuButton;
    private GameObject com1TimeDisplay;
    private GameObject com2TimeDisplay;
    private GameObject com3TimeDisplay;
    
    public LevelDataScriptable lData;

    //end of level smoothdamp
    public float smoothTime = 3f;
    private float yVel = 0f;
    private float curDispTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //get vars for end of level screen
        endOfLevelPanel = GameObject.FindGameObjectWithTag("EndOfLevel");
        finalTimeDisplay = GameObject.FindGameObjectWithTag("FinalTimeDisplay");
        exitToMenuButton = GameObject.FindGameObjectWithTag("ExitToMenu");
        com1TimeDisplay = GameObject.FindGameObjectWithTag("com1Time");
        com2TimeDisplay = GameObject.FindGameObjectWithTag("com2Time");
        com3TimeDisplay = GameObject.FindGameObjectWithTag("com3Time");

        com1TimeDisplay.GetComponent<UnityEngine.UI.Text>().text = lData.commendationTimes[0].ToString() + "s";
        com2TimeDisplay.GetComponent<UnityEngine.UI.Text>().text = lData.commendationTimes[1].ToString() + "s";
        com3TimeDisplay.GetComponent<UnityEngine.UI.Text>().text = lData.commendationTimes[2].ToString() + "s";

        endOfLevelPanel.SetActive(false);

        mainPanel = GameObject.FindGameObjectWithTag("MainPanel");
        timeDisplay = GameObject.FindGameObjectWithTag("TimeDisplay");
        speedDisplay = GameObject.FindGameObjectWithTag("SpeedDisplay");
        player = GameObject.FindGameObjectWithTag("Player");

        var g = Instantiate(playerSpawnObj, player.transform.position, player.transform.rotation);
        playerSpawn = g;

    }

    // Update is called once per frame
    void Update()
    {
        //Set speed number
        speedDisplay.GetComponent<UnityEngine.UI.Text>().text = Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString();

        //If the player hits R reset the run
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartRun();

        }

        //If the player hits Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }

        //Code for detecting when to start the timer and displaying the time
        if (waitingToStart)
        {
            if (Input.GetAxis("Horizontal")+ Input.GetAxis("Vertical") > 0)
            {
                waitingToStart = false;
                startTime = Time.time;
            }
        }
        else
        {
            if (finished && curDispTime < finalTime)
            {
                curDispTime = Mathf.SmoothDamp(curDispTime, finalTime, ref yVel, smoothTime);
                finalTimeDisplay.GetComponent<UnityEngine.UI.Text>().text = Round(curDispTime, 3).ToString();
                

                //Logic for displaying commendation checks;
                if(curDispTime > lData.commendationTimes[0])
                {
                    com1TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 100);
                }
                if (curDispTime > lData.commendationTimes[1])
                {
                    com2TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 100);
                }
                if (curDispTime > lData.commendationTimes[2])
                {
                    com3TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 100);
                }

            }
            else
            {
                curTime = Time.time - startTime;
                timeDisplay.GetComponent<UnityEngine.UI.Text>().text = Round(curTime, 3).ToString();
            }
            
        }
    }

    public void FinishLevel()
    {
        Debug.Log("Finished level!");
        finished = true;
        finalTime = curTime;
        curDispTime = 0f;
        endOfLevelPanel.SetActive(true);
        mainPanel.SetActive(false);
        setPlayerDisabled(true);

        com1TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 150, 0, 100);
        com2TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 150, 0, 100);
        com3TimeDisplay.GetComponentInChildren<Image>().color = new Color(0, 150, 0, 100);
        //restartRun();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void restartRun()
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Target"))
        {
            if (g.TryGetComponent(out HitTargetScript s))
            {
                s.resetTarget();
            }
        }

        player.transform.position = playerSpawn.transform.position;
        player.transform.rotation = playerSpawn.transform.rotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        setPlayerDisabled(false);
        waitingToStart = true;
        finished = false;
        endOfLevelPanel.SetActive(false);
        mainPanel.SetActive(true);
        timeDisplay.GetComponent<UnityEngine.UI.Text>().text = "0.000";
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void subtractTime(float time)
    {
        startTime += time;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public void setPlayerDisabled(bool b)
    {
        player.GetComponent<FPSPlayerController>().disabled = b;
    }
}
