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
    public Vector3 menuPosition;
    public PlayerStatBar playerstatbar;
   

    [Header("ÊÂ¼þ¼àÌý")]
    public PlayerController playercontroller;
    public FadeEventSO fadeEvent;
    public VoidEventSO afterSceneLoadEvent;
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGame;
    [Header("³¡¾°")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    public GameSceneSO currentLoadScene;
    private GameSceneSO sceneToLoad;
    public Camera mainca;
    private Vector3 positionToGo;
    private bool fadeScreen;
    private bool isLoading;
    public float fadeTime;
    
    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGame.OnEventRaised +=NewGame;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGame.OnEventRaised -= NewGame;
    }

    private void Awake()
    {
        
    }
    private void NewGame()
    {
        mainca.gameObject.SetActive(true);
        sceneToLoad = firstLoadScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);

    }
    private void Start()
    {
        //NewGame();
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
    }
    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        
        if (locationToLoad.sceneType == SceneType.Menu)
        {
            playerstatbar.gameObject.SetActive(false);
        }
        else
        {
            playerstatbar.gameObject.SetActive(true);
        }
        
        playercontroller.isLoading = true;
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

        if (currentLoadScene.sceneType == SceneType.Loacation) {
            playercontroller.isLoading = false;
            
            afterSceneLoadEvent.RaiseEvent();
        }
        playercontroller.exitStatus();
    }
}