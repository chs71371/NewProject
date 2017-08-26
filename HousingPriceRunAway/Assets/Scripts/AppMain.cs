using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
 
 
public class AppMain : MonoBehaviour
{

    public static AppMain Instance;


    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 30;
 
 
    }


 
}