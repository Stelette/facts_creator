using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New WindowConfig", menuName = "Window/WindowConfig")]
public class WindowConfigWrapper : ScriptableObject
{
    //public GameObject UIRoot;

    public List<WindowConfig> windowConfigs = new List<WindowConfig>();
}