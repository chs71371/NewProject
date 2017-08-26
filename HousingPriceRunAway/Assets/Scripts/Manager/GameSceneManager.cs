using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;
using UISystem;
using UnityEngine.Events;

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
 
    public string sceneName;

    public UnityAction loadOverEvent;
 
    public void LoadScene(string rSceneName ,UnityAction rfunc= null)
    {
        sceneName = rSceneName;
        loadOverEvent = rfunc;
        StartCoroutine(LoadSceneResource());
    }
 
 
    private IEnumerator LoadSceneResource( )
    {
        AsyncOperation asyncO = SceneManager.LoadSceneAsync(sceneName);

        asyncO.allowSceneActivation = false;

        yield return 0;

        while (asyncO.progress < 0.9f)
        {
            yield return 0;
        }

        asyncO.allowSceneActivation = true;

        while (asyncO.progress < 1)
        {
            yield return 0;
        }

 
        if (loadOverEvent != null)
        {
            loadOverEvent();
        }



    }
 
}
