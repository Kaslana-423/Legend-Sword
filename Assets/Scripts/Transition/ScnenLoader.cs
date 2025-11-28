using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScnenLoader : MonoBehaviour, ISaveable
{

    public Transform playertrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    public PlayerStatBar playerstatbar;


    [Header("�¼�����")]
    public PlayerController playercontroller;
    public GameObject gameoverpanel;
    public FadeEventSO fadeEvent;
    public VoidEventSO afterSceneLoadEvent;
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO backToMenuEvent;
    public VoidEventSO newGame;
    [Header("����")]
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
        backToMenuEvent.OnEventRaised += OnBackToMenu;
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGame.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnBackToMenu()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
        gameoverpanel.SetActive(false);
    }

    private void OnDisable()
    {
        backToMenuEvent.OnEventRaised -= OnBackToMenu;
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGame.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
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
        playercontroller.gameObject.SetActive(false);
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
        currentLoadScene = sceneToLoad;
        playertrans.position = positionToGo;
        playertrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeTime);
        }
        isLoading = false;

        if (currentLoadScene.sceneType == SceneType.Loacation)
        {
            playercontroller.isLoading = false;

            afterSceneLoadEvent.RaiseEvent();
        }
        playercontroller.exitStatus();
        playercontroller.gameObject.SetActive(true);
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playertrans.GetComponent<DataDefinition>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID];
            sceneToLoad = data.GetSavedScene();
            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
}