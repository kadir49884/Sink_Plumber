using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix;
using Sirenix.OdinInspector;
using Lofelt.NiceVibrations;

public class StackController : MonoBehaviour, IObstacleCollider
{
    private GameManager gameManager;
    [SerializeField]
    private Transform pipe;

    private List<Vector3> pipeStackPosList = new List<Vector3>();
    private List<Quaternion> pipeStackRotList = new List<Quaternion>();

    [SerializeField] private float playerSpeed = 5;
    [SerializeField] private float sensitivity = 5;

    [SerializeField]
    private int stackIndex = 5;
    private int startStackIndex;
    [SerializeField]
    private int obstacleIndex = 15;
    private int startObstacleIndex;
    private int stepValue = 1;
    [SerializeField]
    private int minSpace = 4;
    [SerializeField]
    private int maxSpace = 6;

    [SerializeField]
    private int targetIndex = 9;
    private int startTargetIndex = 9;
    private int startMinSpace;
    private int startMaxSpace;
    private int obstacleIndexHelper = 0;

    private bool pullDelay = true;
    private bool pushDelay;
    private bool waitForPush = true;
    private bool waitForPull = true;
    private int perfectValue;


    [SerializeField]
    private bool isInTarget;


    private Camera ortho;

    private Vector3 diff;
    private Vector3 firstPos;
    private Vector3 mousePos;
    private Transform targetObject;


    private bool isMouseHold;
    private bool isFinishPush;
    private bool isMouseUp;
    private bool waitForPlunger;
    private bool isPlungerUp = true;
    [SerializeField]
    private float waterSpeedTime = 0.1f;
    private float blandShape0;
    private float blandShape1;
    private float plungerVolume = 2f;
    private float startPlungerVolume;
    private float pullTimer;
    private float pushTimer;
    private float waterPower = 0;
    private ParticleSystem waterParticle;
    private ObjectManager objectManager;

    private Transform obstacle;

    [SerializeField]
    private Transform plunger;
    private SkinnedMeshRenderer plungerSkinnedMeshRenderer;
    [SerializeField]
    private List<Transform> stackList = new List<Transform>();

    private ParticleSystem waterSplash;

    private AudioSource audioData;

    [SerializeField]
    private List<AudioClip> waterAudioClip = new List<AudioClip>();

    private Material targetMat;
    private Color targetMatColor;

    private float lerpValue = 0.7f;
    private ParticleSystem targetParticle1;
    private ParticleSystem targetParticle2;
    private int waterScore = 0;
    private Transform perfectObject;

    private VibrationController vibrationController;



    void Start()
    {
        for (int i = 0; i < pipe.childCount; i++)
        {
            pipeStackPosList.Add(pipe.GetChild(i).position);
            pipeStackRotList.Add(pipe.GetChild(i).rotation);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            stackList.Add(transform.GetChild(i));
        }


        gameManager = GameManager.Instance;
        objectManager = ObjectManager.Instance;
        startMinSpace = minSpace - 2;
        startMaxSpace = maxSpace - 2;
        startStackIndex = stackIndex - 2;
        startObstacleIndex = obstacleIndex - 2;
        startPlungerVolume = plungerVolume;
        startTargetIndex = targetIndex;
        ortho = objectManager.OrthoCamera;
        plungerSkinnedMeshRenderer = plunger.GetComponent<SkinnedMeshRenderer>();
        waterParticle = objectManager.WaterParticle.GetComponent<ParticleSystem>();
        gameManager.PlungerActive += PlungerActive;
        waterSplash = objectManager.WaterSplash.GetComponent<ParticleSystem>();
        audioData = objectManager.WaterSound.GetComponent<AudioSource>();
        targetObject = objectManager.TargetObject;
        audioData.clip = waterAudioClip[0];
        targetMat = objectManager.TargetObject.GetChild(0).GetComponent<MeshRenderer>().materials[1];
        targetParticle1 = objectManager.TargetObject.GetChild(1).GetComponent<ParticleSystem>();
        targetParticle2 = objectManager.TargetObject.GetChild(2).GetComponent<ParticleSystem>();
        targetMatColor = targetMat.color;
        perfectObject = objectManager.PerfectObject;
        vibrationController = VibrationController.Instance;
        HapticController.Init();

    }
    private void PlungerActive()
    {
        waitForPlunger = true;
    }

    private void Update()
    {

        if (!waitForPlunger)
        {
            return;
        }

        plungerSkinnedMeshRenderer.SetBlendShapeWeight(0, blandShape0);
        plungerSkinnedMeshRenderer.SetBlendShapeWeight(1, blandShape1);





        if (!gameManager.RunGame)
            return;

        //if (pipeStackPosList.Count < obstacleIndex + 35)
        //{
        //    waterSpeedTime = 0.0001f;
        //}


        if (pipeStackPosList.Count < obstacleIndex + 24)
        {
            ThrowObstacle();
            return;
        }



        firstPos = Vector3.Lerp(firstPos, mousePos, .1f);

        if (Input.GetMouseButtonDown(0))
        {
            //isMouseUp = false;
            MouseDown(Input.mousePosition);
            if (isInTarget)
            {
                TargetMoving();
            }
            else if (startTargetIndex > targetIndex)
            {
                TargetMovingReduce();
            }
        }

        //if (isMouseUp)
        //{
        //    return;
        //}

        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }

        else if (Input.GetMouseButton(0))
        {
            MouseHold(Input.mousePosition);
        }

    }

    private void FixedUpdate()
    {


        if (/*isFinishPush && */ !pullDelay && pushDelay && waitForPush)
        {
            waitForPush = false;
            DOVirtual.DelayedCall(waterSpeedTime, () =>
            {
                waitForPush = true;
            });
            WaterPush();
        }
        else if (isFinishPush)
        {
            waitForPush = false;
            DOVirtual.DelayedCall(waterSpeedTime, () =>
            {
                waitForPush = true;
            });
            WaterPush();
            plunger.DOMoveY(plunger.position.y + 5, 0.3f);
            objectManager.FinishWater.gameObject.SetActive(true);
            return;
        }
        else if (/*isFinishPush && */ !pushDelay && pullDelay && waitForPull)
        {
            waitForPull = false;
            DOVirtual.DelayedCall(waterSpeedTime, () =>
            {
                waitForPull = true;
            });
            WaterPull();
        }



        //blandShape1 = -diff.y * plungerVolume;

        //blandShape0 = diff.y * plungerVolume;

    }

    private void TargetMoving()
    {

        if (minSpace > 1)
        {
            minSpace--;
            targetIndex--;
        }

        targetParticle1.Play();
        targetParticle2.Play();

        audioData.Play(0);

        vibrationController.Vibrate(Lofelt.NiceVibrations.HapticPatterns.PresetType.HeavyImpact);

        maxSpace++;
        waterSpeedTime -= 0.02f;
        if (waterSpeedTime <= 0)
        {
            waterSpeedTime = 0.03f;
        }

        targetObject.GetComponent<BoxCollider>().enabled = false;
        DOVirtual.DelayedCall(0.5f, () =>
         {
             targetObject.GetComponent<BoxCollider>().enabled = true;
         });


        DOVirtual.DelayedCall(0.15f, () =>
         {
             targetObject.DOMove(pipeStackPosList[targetIndex], 0.01f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
             targetObject.DORotate(new Vector3(targetObject.eulerAngles.x, pipeStackRotList[targetIndex].eulerAngles.y, targetObject.eulerAngles.z), 0.01f)
                 .SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
         });

        if (pipeStackPosList.Count < obstacleIndex + 25)
        {
            lerpValue = 1f;
            waterSpeedTime = 0.00001f;
        }
        waterScore++;
        if (waterScore > 2)
        {
            waterScore = 3;
            perfectObject.gameObject.SetActive(true);
            PerfectScale();
        }
        PushWithMouseDown();
    }

    private void PerfectScale()
    {
        perfectValue++;
        if (perfectValue > 2)
        {
            perfectObject.gameObject.SetActive(false);
            perfectValue = 0;
            return;
        }
        perfectObject.DOScale(perfectObject.localScale * 1.3f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            perfectObject.DOScale(perfectObject.localScale / 1.3f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                PerfectScale();
            });
        });
    }

    private void TargetMovingReduce()
    {
        targetMat.color = Color.red;
        DOVirtual.DelayedCall(0.5f, () =>
        {
            targetMat.color = targetMatColor;
        });
        minSpace++;
        targetIndex++;
        waterSpeedTime += 0.02f;
        if (waterSpeedTime > 0.2f)
        {
            waterSpeedTime = 0.2f;
        }
        targetObject.DOMove(pipeStackPosList[targetIndex], 0.01f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        targetObject.DORotate(new Vector3(targetObject.eulerAngles.x, pipeStackRotList[targetIndex].eulerAngles.y, targetObject.eulerAngles.z), 0.01f);
        if (waterScore > 0)
        {
            waterScore--;
        }
    }

    private void ThrowObstacle()
    {
        gameManager.GameWin();
        obstacle.GetComponent<BoxCollider>().enabled = false;
        targetObject.gameObject.SetActive(false);
        waterParticle.Play();

        maxSpace += 20;
        isFinishPush = true;


        audioData.clip = waterAudioClip[1];
        audioData.Play();

        vibrationController.ContinuousHaptics(1, 1, 3);


        Execute.After(0.7f, () =>
        {
            DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.5f).SetEase(Ease.Linear);
            DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.5f).SetEase(Ease.Linear);
            audioData.clip = waterAudioClip[2];
            audioData.Play();
        });

        Vector3 throwVector = pipeStackPosList[pipeStackPosList.Count - 5] - pipeStackPosList[pipeStackPosList.Count - 6];

        obstacle.GetComponent<Rigidbody>().AddForce(throwVector * 3000);
        DOVirtual.DelayedCall(3, () =>
        {
            gameManager.WinPanelActive();
        });

    }



    private void WaterPush()
    {
        //maxSpace = pipeStackPosList.Count - stackList.Count - 2;
        if (stackIndex > maxSpace)
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                DOTween.To(value => blandShape0 = value, blandShape0, 50, 0.3f).SetEase(Ease.Linear);
                DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.3f).SetEase(Ease.Linear);
            });

            pushDelay = false;
            pullDelay = true;
        }
        else
        {

            stackIndex++;
        }

        //if (!gameManager.RunGame)
        //{
        //    maxSpace = pipeStackPosList.Count - stackList.Count - 2;
        //    if (stackIndex >= maxSpace)
        //    {
        //        return;
        //    }
        //    stackIndex++;
        //}
        //else
        //{
        //    if (waterPower > 3 && stepValue == 1)
        //    {
        //        stepValue = 2;
        //    }
        //    if (stackIndex < maxSpace)
        //    {
        //        stackIndex += stepValue;
        //    }
        //}

        WaterMoveLoop();

    }

    private void PushWithMouseDown()
    {
        isInTarget = false;

        targetObject.DOScale(new Vector3(targetObject.localScale.x, targetObject.localScale.y, targetObject.localScale.z) * 1.2f, 0.1f).OnComplete(() =>
          {
              targetObject.DOScale(new Vector3(targetObject.localScale.x, targetObject.localScale.y, targetObject.localScale.z) / 1.2f, 0.1f);
          });

        DOVirtual.DelayedCall(0.1f, () =>
        {
            DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.3f).SetEase(Ease.Linear);
            DOTween.To(value => blandShape1 = value, blandShape1, 100, 0.3f).SetEase(Ease.Linear);
        });

        pullDelay = false;
        pushDelay = true;
    }


    private void WaterPull()
    {

        if (stackIndex < minSpace)
        {

            DOVirtual.DelayedCall(0.1f, () =>
            {
                DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.3f).SetEase(Ease.Linear);
                DOTween.To(value => blandShape1 = value, blandShape1, 100, 0.3f).SetEase(Ease.Linear);
            });


            pullDelay = false;
            pushDelay = true;
        }
        else
        {
            stackIndex--;
        }

        WaterMoveLoop();
    }
    private void WaterMoveLoop()
    {


        for (int i = 0; i < stackList.Count; i++)
        {
            stackList[i].position = Vector3.Lerp(stackList[i].position, pipeStackPosList[i + stackIndex], lerpValue);
            stackList[i].rotation = Quaternion.Lerp(stackList[i].rotation, pipeStackRotList[i + stackIndex], lerpValue);

            //stackList[i].DOMove(startStackPosList[i + stackIndex], 0.1f).SetUpdate(UpdateType.Fixed);
            //stackList[i].DORotate(startStackRotList[i + stackIndex].eulerAngles, 0.01f).SetUpdate(UpdateType.Fixed);
        }
    }

    private void MouseDown(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        firstPos = mousePos;
        diff = mousePos - firstPos;
    }

    private void MouseHold(Vector3 inputPos)
    {
        mousePos = ortho.ScreenToWorldPoint(inputPos);
        diff = mousePos /*- firstPos*/;
        diff *= sensitivity;
    }

    private void MouseUp()
    {
        diff = Vector3.zero;

        //stepValue = 1;
        //minSpace = startMinSpace;
        //maxSpace = startMaxSpace;
        ////stackIndex = startStackIndex;
        //plungerVolume = startPlungerVolume;
        ////obstacleIndex = startObstacleIndex;
        //waterPower = 1;

        //DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.5f).SetEase(Ease.Linear);
        //DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.5f).SetEase(Ease.Linear);

        //GoStartPos();
        //ObstacleGoStartPos();
    }

    private void GoStartPos()
    {
        if (stackIndex <= startStackIndex)
        {
            stackIndex = startStackIndex;
            return;
        }

        stackIndex--;
        for (int i = 0; i < stackList.Count; i++)
        {
            stackList[i].DOMove(pipeStackPosList[i + stackIndex], 0.012f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
            stackList[i].DORotate(pipeStackRotList[i + stackIndex].eulerAngles, 0.012f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        }
        DOVirtual.DelayedCall(0.012f, () =>
        {
            GoStartPos();
        });
    }

    private void ObstacleGoStartPos()
    {
        if (obstacleIndex == startObstacleIndex)
        {
            return;
        }

        obstacle.DOMove(pipeStackPosList[obstacleIndex], 0.01f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);
        obstacle.DORotate(pipeStackRotList[obstacleIndex].eulerAngles, 0.01f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
          {
              obstacleIndex--;
              ObstacleGoStartPos();
          });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TargetCollider"))
        {
            isInTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TargetCollider"))
        {
            isInTarget = false;
        }
    }


    public void CollisionObstacle(Transform newObstacle)
    {

        newObstacle.GetComponent<BoxCollider>().enabled = false;
        DOVirtual.DelayedCall(0.5f, () =>
         {
             newObstacle.GetComponent<BoxCollider>().enabled = true;
         });
        obstacleIndex++;
        newObstacle.DOMove(pipeStackPosList[obstacleIndex], 0.01f).SetUpdate(UpdateType.Fixed);
        newObstacle.DORotate(pipeStackRotList[obstacleIndex].eulerAngles, 0.01f).SetUpdate(UpdateType.Fixed);

        waterSplash.Play();


        //if (!isPlungerUp)
        //{
        //    return;
        //}


        //isPlungerUp = false;
        obstacle = newObstacle;
        ////isCrashObstacle = true;
        //waterPower++;
        //maxSpace += stepValue;

        ////obstacleStartIndex += stepValue;
        //obstacleIndex += 2;

        //if (pipeStackPosList.Count < obstacleIndex + 23)
        //{

        //}
        //else
        //{
        //    waterSplash.Play();
        //    audioData.Play(0);
        //}

        //newObstacle.DOMove(pipeStackPosList[obstacleIndex], 0.01f).SetUpdate(UpdateType.Fixed);
        //newObstacle.DORotate(pipeStackRotList[obstacleIndex].eulerAngles, 0.01f).SetUpdate(UpdateType.Fixed);

        //if (plungerVolume < 10)
        //{
        //    plungerVolume += 1f;
        //}

        //if (minSpace > 1)
        //{
        //    minSpace--;
        //}
        //if (minSpace < 4)
        //{
        //    waterSpeedTime -= waterSpeedTime * 0.8f;
        //}

    }
}
