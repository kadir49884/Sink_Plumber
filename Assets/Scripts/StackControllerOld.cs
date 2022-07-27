using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix;
using Sirenix.OdinInspector;

public class StackControllerOld : MonoBehaviour, IObstacleCollider
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
    private int startMinSpace;
    private int startMaxSpace;
    private bool pushDelay;
    private int obstacleIndexHelper = 0;
    private bool pullDelay;

    private Camera ortho;

    private Vector3 diff;
    private Vector3 firstPos;
    private Vector3 mousePos;

    private bool isMouseHold;
    private bool isFinishPush;
    private bool isMouseUp;
    private bool waitForPlunger;
    private bool isPlungerUp = true;
    private float waterSpeedTime = 0.01f;
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
        ortho = objectManager.OrthoCamera;
        plungerSkinnedMeshRenderer = plunger.GetComponent<SkinnedMeshRenderer>();
        waterParticle = objectManager.WaterParticle.GetComponent<ParticleSystem>();
        gameManager.PlungerActive += PlungerActive;
        waterSplash = objectManager.WaterSplash.GetComponent<ParticleSystem>();
        audioData = objectManager.WaterSound.GetComponent<AudioSource>();

        audioData.clip = waterAudioClip[0];
        GoStartPos();
        ObstacleGoStartPos();

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

        //if (isFinishPush && !pushDelay)
        //{
        //    pushDelay = true;
        //    plunger.DOMoveY(plunger.position.y + 5, 0.3f);
        //    objectManager.FinishWater.gameObject.SetActive(true);
        //    WaterPush();
        //}

        if (!gameManager.RunGame)
            return;

        if (pipeStackPosList.Count < obstacleIndex + 35)
        {
            waterSpeedTime = 0.0001f;
        }

        if (pipeStackPosList.Count < obstacleIndex + 24)
        {
            ThrowObstacle();
            return;
        }

        firstPos = Vector3.Lerp(firstPos, mousePos, .1f);

        if (Input.GetMouseButtonDown(0))
        {
            isMouseUp = false;
            MouseDown(Input.mousePosition);
        }

        if (isMouseUp)
        {
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }

        else if (Input.GetMouseButton(0))
        {
            if (!isMouseHold)
            {
                isMouseHold = true;
                MouseHold(Input.mousePosition);
                isMouseHold = false;
            }
        }

        if (diff.y < 0)
        {
            if (diff.y < -10)
            {
                diff.y = -10;
            }
            blandShape0 = 0;
            blandShape1 = -diff.y * plungerVolume;
        }
        else if (diff.y > 0)
        {
            if (diff.y > 10)
            {
                diff.y = 10;
            }
            blandShape0 = diff.y * plungerVolume;
            blandShape1 = 0;
        }

        if (diff.y < -0.1f)
        {

            if (!pushDelay /*&& !isCrashObstacle*/)
            {
                pullTimer = 0;
                pushTimer += Time.deltaTime;
                if (pushTimer > 1)
                {
                    pushTimer = 0;
                    isMouseUp = true;
                    MouseUp();
                }

                pushDelay = true;
                Execute.After(waterSpeedTime, () =>
                {
                    WaterPush();
                });
            }
        }
        else if (diff.y > 0.1f && !pullDelay)
        {
            pushTimer = 0;
            pullTimer += Time.deltaTime;
            if (pullTimer > 1)
            {
                pullTimer = 0;
                isMouseUp = true;
                MouseUp();
            }
            //isCrashObstacle = false;
            pullDelay = true;
            isPlungerUp = true;
            Execute.After(waterSpeedTime / 5, () =>
             {
                 WaterPull();
             });
        }

    }

    private void ThrowObstacle()
    {
        gameManager.GameWin();
        obstacle.GetComponent<CapsuleCollider>().enabled = false;
        waterParticle.Play();

        maxSpace = pipeStackPosList.Count - 1;
        isFinishPush = true;

        audioData.clip = waterAudioClip[1];
        audioData.Play();


        Execute.After(0.7f, () =>
        {
            DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.5f).SetEase(Ease.Linear);
            DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.5f).SetEase(Ease.Linear);
            audioData.clip = waterAudioClip[2];
            audioData.Play();
        });

        obstacle.GetComponent<Rigidbody>().AddForce(Vector3.down * 3000);
        DOVirtual.DelayedCall(3, () =>
        {
            //gameManager.NextLevel();
        });

    }


    private void FixedUpdate()
    {
        if (!waitForPlunger)
        {
            return;
        }

        if (isFinishPush && !pushDelay)
        {
            pushDelay = true;
            plunger.DOMoveY(plunger.position.y + 5, 0.3f);
            objectManager.FinishWater.gameObject.SetActive(true);
            WaterPush();
        }

    }

    private void WaterPush()
    {
        pushDelay = false;

        if (!gameManager.RunGame)
        {
            maxSpace = pipeStackPosList.Count - stackList.Count - 2;
            if (stackIndex >= maxSpace)
            {
                return;
            }
            stackIndex++;
        }
        else
        {
            if (waterPower > 3 && stepValue == 1)
            {
                stepValue = 2;
            }
            if (stackIndex < maxSpace)
            {
                stackIndex += stepValue;
            }
        }

        WaterMoveLoop();

    }


    private void WaterPull()
    {
        pullDelay = false;

        if (stackIndex > minSpace)
        {
            stackIndex -= stepValue;
        }

        WaterMoveLoop();
    }
    private void WaterMoveLoop()
    {

        for (int i = 0; i < stackList.Count; i++)
        {
            stackList[i].position = Vector3.Lerp(stackList[i].position, pipeStackPosList[i + stackIndex], 0.9f);
            stackList[i].rotation = Quaternion.Lerp(stackList[i].rotation, pipeStackRotList[i + stackIndex], 0.9f);

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

        stepValue = 1;
        minSpace = startMinSpace;
        maxSpace = startMaxSpace;
        //stackIndex = startStackIndex;
        plungerVolume = startPlungerVolume;
        //obstacleIndex = startObstacleIndex;
        waterPower = 1;

        DOTween.To(value => blandShape0 = value, blandShape0, 0, 0.5f).SetEase(Ease.Linear);
        DOTween.To(value => blandShape1 = value, blandShape1, 0, 0.5f).SetEase(Ease.Linear);

        GoStartPos();
        ObstacleGoStartPos();
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

    public void CollisionObstacle(Transform newObstacle)
    {
        if (!isPlungerUp)
        {
            return;
        }

        
        isPlungerUp = false;
        obstacle = newObstacle;
        //isCrashObstacle = true;
        waterPower++;
        maxSpace += stepValue;

        //obstacleStartIndex += stepValue;
        obstacleIndex += 2;

        if (pipeStackPosList.Count < obstacleIndex + 23)
        {

        }
        else
        {
            waterSplash.Play();
            audioData.Play(0);
        }

        newObstacle.DOMove(pipeStackPosList[obstacleIndex], 0.01f).SetUpdate(UpdateType.Fixed);
        newObstacle.DORotate(pipeStackRotList[obstacleIndex].eulerAngles, 0.01f).SetUpdate(UpdateType.Fixed);

        if (plungerVolume < 10)
        {
            plungerVolume += 1f;
        }

        if (minSpace > 1)
        {
            minSpace--;
        }
        if (minSpace < 4)
        {
            waterSpeedTime -= waterSpeedTime * 0.8f;
        }

    }
}
