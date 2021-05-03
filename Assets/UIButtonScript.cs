using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButtonScript : MonoBehaviour
{
    public GameObject hostPanel;
    public GameObject targetPanel;
    public string targetScene;
    public bool sendsToTarget = false;
    public bool loadsScene = false;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        if(sendsToTarget)
        {
            targetPanel.SetActive(true);
            hostPanel.SetActive(false);
        }

        if (loadsScene)
            SceneManager.LoadScene(targetScene);

    }
}
