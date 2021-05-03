using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "addLevel")]
public class LevelDataScriptable : ScriptableObject
{
    public string levelName;
    public float[] commendationTimes = new float[] { 30f, 60f, 120f };
}
