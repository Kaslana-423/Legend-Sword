using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class SavePoint : MonoBehaviour,IInteractable
{
    [Header("广播")]
    public VoidEventSO LoadEventSO;
    [Header("变量参数")]
    public Volume globalVolume;
    public Bloom _cachedBloom;
    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;
    public Sprite darkSprite;
    public Sprite lightSprite;
    public bool isDone;
    public float duration;

    void Start()
    {
        if (globalVolume == null) globalVolume = GetComponent<Volume>();

        // --- 在游戏开始时，只找一次 ---
        var components = globalVolume.profile.components; // 只要访问 .profile，Unity就会自动克隆，不用担心

        foreach (var component in components)
        {
            if (component is Bloom bloom)
            {
                _cachedBloom = bloom; // 找到了！存起来

                // 顺便把覆盖开关打开，免得后面还要操心
                _cachedBloom.intensity.overrideState = true;
                Debug.Log("初始化成功：已锁定 Bloom 组件");
                break; // 找到了就停止循环
            }
        }

        if (_cachedBloom == null) Debug.LogError("坏了，Profile 里没加 Bloom！");
    }
    private void OnEnable()
    {
     
        spriteRenderer.sprite=isDone ? lightSprite :darkSprite ;
        lightObj.SetActive(isDone);
    }
   

    public void TriggerAction()
    {
        StartCoroutine(exbls(duration));
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite=lightSprite;
            lightObj.SetActive(true);
            //数据保存

            this.gameObject.tag = "Untagged";
        }
    }
    
    private IEnumerator exbls(float duration)
    {
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0,1, t / duration);
            _cachedBloom.intensity.value = a*500;
            yield return null;
        }
        while (t > 0)
        {
            t -= Time.deltaTime;
            float a = Mathf.Lerp(0, 1, t / duration);
            _cachedBloom.intensity.value = a * 500;
            yield return null;
        }

    }
}
