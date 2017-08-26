using UnityEngine;

public class MonoBehaviourWrapped : MonoBehaviour
{
    private Transform cachedTransform = null;

    public Transform Transform
    {
        get
        {
            if (cachedTransform == null)
            {
                cachedTransform = transform;
            }
            return cachedTransform;
        }
    }

    private GameObject cachedGO = null;

    public GameObject GameObject
    {
        get
        {
            if (cachedGO == null)
            {
                cachedGO = gameObject;
            }
            return cachedGO;
        }
    }
}