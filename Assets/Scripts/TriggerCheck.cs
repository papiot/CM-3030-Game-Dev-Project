using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterCheck : MonoBehaviour
{
    [SerializeField] string targetTag;
    public UnityEvent<GameObject> OnEnterEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == targetTag)
        {
            OnEnterEvent.Invoke(other.gameObject);
        }

    }

}
