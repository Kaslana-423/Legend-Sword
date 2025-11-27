using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataManager : MonoBehaviour
{
    private List<ISaveable> saveableList = new List<ISaveable>();
    public static DataManager instance;
    [Header("ÊÂ¼þ¼àÌý")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;
    private Data saveData;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        saveData = new Data();
    }
    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
        
    }
    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }
    private void Update()
    {
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            Load();
        }
    }
    public void RegisterSaveData(ISaveable saveable)
    {
        if(!saveableList.Contains(saveable)) saveableList.Add(saveable);
    }
    public void UnRegisterSaveData(ISaveable saveable)
    {
        saveableList.Remove(saveable);
    }
    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        foreach(var item in saveData.characterPosDict)
        {
            Debug.Log(item.Key+"  "+item.Value);
        }
    }
    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }
}