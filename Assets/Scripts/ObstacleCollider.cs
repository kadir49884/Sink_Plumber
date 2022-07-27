using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCollider : MonoBehaviour
{

    private ObjectManager objectManager;
    private StackController stackController;

    private void Start()
    {
        stackController = ObjectManager.Instance.PlayerObject.GetComponent<StackController>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerCollider"))
        {
            stackController.GetComponent<IObstacleCollider>()?.CollisionObstacle(transform);
        }
    }
}
