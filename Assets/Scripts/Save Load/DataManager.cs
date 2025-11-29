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



    private Data saveData;



    private void Awake()

    {

        // --- 单例模式核心修改 ---

        if (instance == null)

        {

            instance = this;

            // 【关键】这一行让 DataManager 在切换场景时不被销毁

            // 这样你的 saveData 数据和 saveableList 才能带到下一关

            DontDestroyOnLoad(gameObject);



            // 数据初始化只做一次

            saveData = new Data();

        }

        else

        {

            // 如果已经有一个老大（instance）存在了，

            // 那么新场景里多出来的这个 DataManager 就要自我销毁

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

    }



    private void OnDisable()

    {

        saveDataEvent.OnEventRaised -= Save;

        loadDataEvent.OnEventRaised -= Load;

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



        // 调试打印

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