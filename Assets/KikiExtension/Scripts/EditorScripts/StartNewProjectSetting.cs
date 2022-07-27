
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
public class StartNewProjectSetting : EditorWindow
{
    #region Start New Project Settings

    private string title;

    [MenuItem("KikiEditor/Start New Project Settings")]
    public static void ShowStartNewProjectWindow()
    {
        EditorWindow.GetWindow(typeof(StartNewProjectSetting), utility: false, "Start New Project Settings");
    }

    public void Awake()
    {
        title = " Start New Project Settings";
    }

    public void StartNewProject()
    {
        SetPlayerSettings();
        PrepareScene();
        title = " Project is Ready";
    } 


    private static void SetPlayerSettings()
    {
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
    }

    private static void PrepareScene()
    {
        Camera ortoCam;

        if (!GameObject.Find("OrtoCam"))
            ortoCam = new GameObject("OrtoCam").AddComponent<Camera>();
        else
            ortoCam = GameObject.Find("OrtoCam").GetComponent<Camera>();

        if (ortoCam.GetComponent<AudioListener>())
            DestroyImmediate(ortoCam.GetComponent<AudioListener>());

        ortoCam.orthographic = true;
        ortoCam.depth = 1;
        ortoCam.clearFlags = CameraClearFlags.Depth;
        ortoCam.cullingMask = 5 << 5;

        ortoCam.cullingMask = LayerMask.GetMask("UI");

        Canvas mainCanvas;

        if (!FindObjectOfType<Canvas>())
            mainCanvas = new GameObject("Canvas").AddComponent<Canvas>();
        else
            mainCanvas = FindObjectOfType<Canvas>();

        if (mainCanvas.GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler canvasScaler = mainCanvas.gameObject.AddComponent<CanvasScaler>();

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080, 1920);
        }

        if (!mainCanvas.GetComponent<GraphicRaycaster>())
            mainCanvas.gameObject.AddComponent<GraphicRaycaster>();


        mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mainCanvas.worldCamera = ortoCam;

        mainCanvas.gameObject.layer = 5;


        if (!FindObjectOfType<EventSystem>())
        {
            EventSystem eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }

        //GameManager gameManager;

        //if (!GameObject.Find("GameManager"))
        //    gameManager = new GameObject("GameManager").AddComponent<GameManager>();
        //else
        //    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();


        string gameManagerPath = Path.Combine("Assets", "Prefabs", "GameManager", "GameManager.prefab");
        Object gameManagerObject = AssetDatabase.LoadAssetAtPath(gameManagerPath, typeof(GameObject));

        if (!GameObject.Find("GameManager"))
        {
            GameObject newGameManagerObject = Instantiate((GameObject)gameManagerObject);
            newGameManagerObject.name = "GameManager";
        }
        else
        {
            gameManagerObject = GameObject.Find("GameManager(Clone)");
        }


        ObjectManager objectManager;

        if (!GameObject.Find("ObjectManager"))
            objectManager = new GameObject("ObjectManager").AddComponent<ObjectManager>();
        else
            objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        objectManager.OrthoCamera = ortoCam;


        string UISettingsPath = Path.Combine("Assets","Prefabs", "UISettings", "UISettings.prefab");
        Object UISettings = AssetDatabase.LoadAssetAtPath(UISettingsPath, typeof(GameObject));

        if (!GameObject.Find("UISettings"))
        {
            GameObject newUIPrefab = Instantiate((GameObject)UISettings, mainCanvas.transform);
            newUIPrefab.name = "UISettings";
        }
        else
        {
            UISettings = GameObject.Find("UISettingsPrefab(Clone)");
        }

        


        //Image tapToPlay;
        //if (!GameObject.Find("TapToPlay"))
        //    tapToPlay = new GameObject("TapToPlay").AddComponent<Image>();
        //else
        //    tapToPlay = GameObject.Find("TapToPlay").GetComponent<Image>();

        //Vector3 tapToPlayPos = tapToPlay.rectTransform.localPosition;
        //tapToPlay.transform.SetParent(mainCanvas.transform);
        //tapToPlay.rectTransform.anchorMin = new Vector2(0, 0);
        //tapToPlay.rectTransform.anchorMax = new Vector2(1, 1);
        //tapToPlay.gameObject.AddComponent<StarterEvent>();
        //tapToPlay.color = new Color(255, 255, 255, 0);
        //tapToPlay.rectTransform.localScale = Vector3.one;
        //tapToPlayPos.z = 0;
        //tapToPlay.rectTransform.localPosition = tapToPlayPos;


    }


    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();
        GUIStyle assetGUIStyle = new GUIStyle();

        gUIStyle.fontStyle = FontStyle.Bold;
        gUIStyle.fontSize = 15;
        gUIStyle.normal.textColor = Color.green;
        gUIStyle.alignment = TextAnchor.MiddleCenter;
        gUIStyle.padding = new RectOffset() { bottom = 10, top =10 };

        assetGUIStyle.fontStyle = FontStyle.Bold;
        assetGUIStyle.fontSize = 15;
        assetGUIStyle.normal.textColor = Color.white;
        assetGUIStyle.alignment = TextAnchor.UpperLeft;
        assetGUIStyle.padding = new RectOffset() { bottom = 10, top = 10 };

        GUILayout.Label(title, gUIStyle);

        if (GUILayout.Button("START NEW PROJECT!  "))
        {
            StartNewProject();
        }

    }

    #endregion
}
#endif