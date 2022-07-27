using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerController : MonoBehaviour
{

    [SerializeField]
    private List<Transform> plungerNewTransform = new List<Transform>();
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Transform dirtyWater;

    [SerializeField]
    private Transform stand;

    [SerializeField]
    private bool isLavabo;

    private AudioSource audioSource;


    void Start()
    {
        GameManager.Instance.GameStart += PlungerMove;
        skinnedMeshRenderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        dirtyWater = ObjectManager.Instance.DirtyWater;
        audioSource = transform.GetComponent<AudioSource>();
    }

    private void PlungerMove()
    {
        //transform.DOMove(plungerNewTransform.position, 1);
        //Execute.After(0.1f, () =>
        //{
        //    transform.DORotate(plungerNewTransform.eulerAngles, 0.5f).OnComplete(() =>
        //    {

        transform.DOMove(plungerNewTransform[0].position, 0.2f).SetEase(Ease.Linear);
        transform.DORotate(plungerNewTransform[0].eulerAngles, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            transform.DOMove(plungerNewTransform[1].position, 0.2f).SetEase(Ease.Linear);
            transform.DORotate(plungerNewTransform[1].eulerAngles, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {

                audioSource.Play();
                ObjectManager.Instance.DirtySplash.GetComponent<ParticleSystem>().Play();
                if (dirtyWater != null)
                {
                    if (isLavabo)
                    {
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            dirtyWater.gameObject.SetActive(false);
                        });
                    }
                    else
                    {
                        dirtyWater.DOMoveY(dirtyWater.position.y - 3, 0.5f).OnComplete(() =>
                        {
                            dirtyWater.gameObject.SetActive(false);
                        });
                    }

                }

                transform.DOMove(plungerNewTransform[2].position, 0.2f).SetEase(Ease.Linear);
                transform.DORotate(plungerNewTransform[2].eulerAngles, 0.2f).SetEase(Ease.Linear);
                skinnedMeshRenderer.SetBlendShapeWeight(1, 60);
                Execute.After(0.5f, () =>
                {
                    stand.DOMoveY(stand.transform.position.y - 50, 1);
                    GameManager.Instance.PlungerActive();
                });

            });
        });
        //    });
        //});
    }

}
