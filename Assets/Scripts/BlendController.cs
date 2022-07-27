using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendController : MonoBehaviour
{

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private float blandShape0;
    private float blandShape1;

    void Start()
    {

        skinnedMeshRenderer = transform.GetComponent<SkinnedMeshRenderer>();
        blandShape0 = skinnedMeshRenderer.GetBlendShapeWeight(0);
        blandShape1 = skinnedMeshRenderer.GetBlendShapeWeight(1);
        BlendLoop();
    }

    private void BlendLoop()
    {
        DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.2f).SetEase(Ease.Linear).OnComplete(()=>
        {
                DOTween.To(value => blandShape1 = value, blandShape1, 100, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.2f).SetEase(Ease.Linear);
                    DOTween.To(value => blandShape0 = value, blandShape0, 100, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        BlendLoop();
                    });
                });
            
        });
    }

    private void FixedUpdate()
    {
        skinnedMeshRenderer.SetBlendShapeWeight(0, blandShape0);
        skinnedMeshRenderer.SetBlendShapeWeight(1, blandShape1);
    }

}
