using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("�㲥")]
    public VoidEventSO SaveEventSO;
    [Header("��������")]
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
        var components = globalVolume.profile.components;

        foreach (var component in components)
        {
            if (component is Bloom bloom)
            {
                _cachedBloom = bloom;
                _cachedBloom.intensity.overrideState = true;
                Debug.Log("��ʼ���ɹ��������� Bloom ���");
                break;
            }
        }

        if (_cachedBloom == null) Debug.LogError("���ˣ�Profile ��û�� Bloom��");
    }
    private void OnEnable()
    {

        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }


    public void TriggerAction()
    {
        StartCoroutine(exbls(duration));
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            //���ݱ���
            SaveEventSO.RaiseEvent();
            this.gameObject.tag = "Untagged";
        }
    }

    private IEnumerator exbls(float duration)
    {

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0, 1, t / duration);
            _cachedBloom.intensity.value = a * 500;
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
