using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform cameraPlayPos;
    [SerializeField]
    private Transform tutorial;
    void Start()
    {
        GameManager.Instance.PlungerActive += PlungerMove;
        cameraPlayPos = ObjectManager.Instance.CameraPlayPos;
    }

    private void PlungerMove()
    {
        transform.DORotate(cameraPlayPos.eulerAngles, 1).SetEase(Ease.Linear);
        transform.DOMove(cameraPlayPos.position, 1).SetEase(Ease.Linear);
        DOVirtual.DelayedCall(1f, () =>
        {
            tutorial.gameObject.SetActive(true);

        });
        DOVirtual.DelayedCall(5f, () =>
        {
            tutorial.gameObject.SetActive(false);
        });
    }
}
