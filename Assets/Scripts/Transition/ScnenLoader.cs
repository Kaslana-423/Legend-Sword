using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScnenLoader : MonoBehaviour
{
    
    public Transform playertrans;
    public Vector3 firstPosition;

    [Header("ÊÂ¼þ¼àÌý")]
    public FadeEventSO fadeEvent;
    public VoidEventSO afterSceneLoadEvent;
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;
    public GameSceneSO currentLoadScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeTime;
    
    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        OnLoadRequestEvent(sceneToLoad,firstPosition, true);
    }
    private void Start()
    {
        NewGame();
    }
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading) return;
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        if (currentLoadScene != null) 
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
        
        
    }
    private IEnumerator UnloadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEvent.FadeIn(fadeTime);
        }
        yield return new WaitForSeconds(fadeTime);
        if (currentLoadScene != null)
        {
            yield return currentLoadScene.sceneReference.UnLoadScene();
        }
        playertrans.gameObject.SetActive(false);
        LoadNewScene();
    }

    private void LoadNewScene()
    {
       var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnloadCompleted;
    }

    private void OnloadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadScene=sceneToLoad;
        playertrans.position=positionToGo;
        playertrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeTime);
        }
        isLoading = false;
        afterSceneLoadEvent?.RaiseEvent();
    }
}