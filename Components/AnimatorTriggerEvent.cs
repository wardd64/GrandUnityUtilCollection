using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides animator with an interface to fire different Unity events.
/// In other words, chain the less flexible animation events with the 
/// more flexible Unity events.
/// </summary>
public class AnimatorTriggerEvent : MonoBehaviour {

	public UnityEvent[] effects;

    public void InvokeAnimEvent(int index) {
        effects[index].Invoke();
    }
}
