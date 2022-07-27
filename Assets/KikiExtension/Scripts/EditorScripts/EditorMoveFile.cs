using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
public class EditorMoveFile : EditorWindow
{

    #region Move File

    private int index;

    [MenuItem("KikiEditor/MoveFile")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorMoveFile), utility: false, "Move File");
    }
    static string[] options = new string[]
    {
         "Scripts", "Textures", "Materials","Models","Prefabs","Scenes","Sounds","ExternalTools","Resources","Animations",
        Path.Combine("Resources", "Levels"),Path.Combine("Materials", "PhysicMaterials")
    };

    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.fontStyle = FontStyle.Bold;
        gUIStyle.fontSize = 25;
        gUIStyle.normal.textColor = Color.green;
        gUIStyle.alignment = TextAnchor.MiddleCenter;
        gUIStyle.padding = new RectOffset() { bottom = 20, top = 10 };

        GUILayout.Label("Move File", gUIStyle);
        index = EditorGUILayout.Popup("File Type : ", index, options);

        if (GUILayout.Button("Move"))
            MoveFileFunc(options[index]);
        else if (GUILayout.Button("Move All"))
        {
            foreach (string item in options)
            {
                MoveFileFunc(item);
            }
        }
    }

    public static void MoveFileFunc(string fileType)
    {
        List<string> fileExtensions = new List<string>();
        CreateNecessaryDirectory();
        switch (fileType)
        {
            case "Scripts":
                fileExtensions.Add("*.cs");
                break;
            case "Textures":
                fileExtensions.Add("*.png");
                fileExtensions.Add("*.jpeg");
                fileExtensions.Add("*.jpg");
                break;
            case "Materials":
                fileExtensions.Add("*.mat");
                break;
            case "Models":
                fileExtensions.Add("*.obj");
                fileExtensions.Add("*.fbx");
                fileExtensions.Add("*.mesh");
                break;
            case "Prefabs":
                fileExtensions.Add("*.prefab");
                break;
            default:
                break;
        }
        string path = Application.dataPath;

        string[] filePaths = null;

        fileExtensions.ForEach(fileExtension =>
        {
            filePaths = Directory.GetFiles(path + "/", fileExtension);

            foreach (string item in filePaths)
            {
                string st = Path.GetFileName(item);
                FileUtil.MoveFileOrDirectory(item, Path.Combine(Path.Combine(path, fileType.ToString()), st));
                FileUtil.MoveFileOrDirectory(item + ".meta", Path.Combine(Path.Combine(path, fileType.ToString()), st + ".meta"));
            }
        });

        if (filePaths != null && filePaths.Length > 0)
        {
            Debug.Log(filePaths.Length + " files moved to selected directory!");
            AssetDatabase.Refresh();
        }
    }

    private static void CreateNecessaryDirectory()
    {
        string path = Application.dataPath;
        foreach (var item in options)
        {
            if (!Directory.Exists(Path.Combine(path, item)))
                Directory.CreateDirectory(Path.Combine(path, item));
            Directory.CreateDirectory(Path.Combine(path, item));

        }
        AssetDatabase.Refresh();
    }

    #endregion
}
#endif