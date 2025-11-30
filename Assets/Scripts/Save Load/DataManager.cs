using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.InputSystem;



public class DataManager : MonoBehaviour

{

    private List<ISaveable> saveableList = new List<ISaveable>();

    public static DataManager instance;



    [Header("事件监听")]

    public VoidEventSO saveDataEvent;

    public VoidEventSO loadDataEvent;
    public VoidEventSO afterSceneLoadEvent;


    private Data saveData;

    // 在 DataManager.cs 中
    public void SaveOneObject(ISaveable saveable)
    {
        if (saveable != null)
        {
            // 只是更新内存数据，不写硬盘（效率高）
            saveable.GetSaveData(saveData);
        }
    }

    private void Awake()

    {

        if (instance == null)

        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            saveData = new Data();
        }

        else

        {
            Destroy(gameObject);
        }

    }

    private void OnApplicationQuit()

    {

        Save();

    }

    private void OnEnable()

    {

        saveDataEvent.OnEventRaised += Save;

        loadDataEvent.OnEventRaised += Load;
        if (afterSceneLoadEvent != null)
            afterSceneLoadEvent.OnEventRaised += Load;

    }



    private void OnDisable()

    {

        saveDataEvent.OnEventRaised -= Save;

        loadDataEvent.OnEventRaised -= Load;
        afterSceneLoadEvent.OnEventRaised -= Load;
    }



    private void Update()

    {

        // 测试按键

        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)

        {

            Load();

        }

    }



    public void RegisterSaveData(ISaveable saveable)

    {

        if (!saveableList.Contains(saveable))

        {

            saveableList.Add(saveable);
            // 【必须有这一步！】
            // 野猪一进门 (OnEnable)，立刻给它查账本。
            // 这样它才能在刚出生的时候就知道自己该死。
            if (saveData != null)
            {
                saveable.LoadData(saveData);
            }

        }

    }



    public void UnRegisterSaveData(ISaveable saveable)

    {

        saveableList.Remove(saveable);

    }



    public void Save()

    {

        // 【安全修复】使用副本遍历，防止报错

        foreach (var saveable in new List<ISaveable>(saveableList))

        {

            if (saveable != null)
            {
                saveable.GetSaveData(saveData);
            }

        }
        foreach (var item in saveData.characterPosDict)

        {

            Debug.Log(item.Key + "  " + item.Value);

        }

    }



    public void Load()

    {

        // 【安全修复】使用副本遍历，防止报错

        foreach (var saveable in new List<ISaveable>(saveableList))

        {

            if (saveable != null)

            {

                saveable.LoadData(saveData);

            }

        }

    }

}