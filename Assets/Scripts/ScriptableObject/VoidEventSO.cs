using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidEventSO", menuName = "Event/VoidEventSO")]
public class VoidEventSO :ScriptableObject
{
    public UnityAction OnEventRaised;
    
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}
