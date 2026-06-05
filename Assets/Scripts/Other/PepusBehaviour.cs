using UnityEngine;
using UnityEngine.AI;

public abstract class PepusBehaviour : MonoBehaviour
{
    [Saveable][HideInInspector] public Vector3 savedPosition;
    [Saveable][HideInInspector] public Quaternion savedRotation;
    [Saveable][HideInInspector] public bool savedActiveState;

    public virtual void OnBeforeSave()
    {
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        savedActiveState = gameObject.activeSelf;
    }
    public virtual void OnAfterLoad()
    {
        CharacterController characterController = gameObject.GetComponent<CharacterController>();
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

        if (characterController != null)
        {
            bool cCState = characterController.enabled;
            characterController.enabled = false;

            transform.position = savedPosition;
            transform.rotation = savedRotation;

            characterController.enabled = cCState;
        }
        else if (navMeshAgent != null)
        {
            if (!navMeshAgent.Warp(savedPosition))
                transform.position = savedPosition;
            transform.rotation = savedRotation;
        }
        else if (rigidbody != null)
        {
            bool isKinematic = rigidbody.isKinematic;
            rigidbody.isKinematic = true;

            transform.position = savedPosition;
            transform.rotation = savedRotation;

            rigidbody.isKinematic = isKinematic;
        }
        else
        {
            transform.position = savedPosition;
            transform.rotation = savedRotation;
        }

        gameObject.SetActive(savedActiveState);
    }
}