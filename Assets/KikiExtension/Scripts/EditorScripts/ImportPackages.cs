using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;


#if UNITY_EDITOR
public class ImportPackages : EditorWindow
{

    private List<bool> showBtn = new List<bool>();
    private List<string> names = new List<string>();
    private List<string[]> fileList = new List<string[]>();
    private List<string> newFileList = new List<string>();
    private List<string> packageList = new List<string>();
    private List<string> assetString = new List<string>();
    private List<string> selectedAsset = new List<string>();
    private string packageStatus;

    private string title;

    [MenuItem("KikiEditor/ImportPackages")]
    public static void ShowStartNewProjectWindow()
    {
        EditorWindow.GetWindow(typeof(ImportPackages), utility: false, "Import Packages");
    }

    public void Awake()
    {
        ImportAssetSettings();
        title = " Import Packages";
    }

    public void StartNewProject()
    {

        for (int i = 0; i < showBtn.Count; i++)
        {
            UpdateAssetPackage(i, showBtn[i]);
        }

        ImportAssets();

        title = "Packages Are Importing...  Sit Back And Be Relax :)";

    }





    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();

        GUIStyle assetGUIStyle = new GUIStyle();


        gUIStyle.fontStyle = FontStyle.Bold;
        gUIStyle.fontSize = 15;
        gUIStyle.normal.textColor = Color.green;
        gUIStyle.alignment = TextAnchor.MiddleCenter;
        gUIStyle.padding = new RectOffset() { bottom = 10, top = 10 };

        assetGUIStyle.fontStyle = FontStyle.Bold;
        assetGUIStyle.fontSize = 15;
        assetGUIStyle.normal.textColor = Color.white;
        assetGUIStyle.alignment = TextAnchor.UpperLeft;
        assetGUIStyle.padding = new RectOffset() { bottom = 10, top = 10 };

        GUILayout.Label(title, gUIStyle);
        GUILayout.Label(" Select Assets Package", assetGUIStyle);


       

        for (int i = 0; i < showBtn.Count; i++)
        {
            showBtn[i] = EditorGUILayout.Toggle(newFileList[i], showBtn[i]);
        }
        if (GUILayout.Button("START IMPORT ASSETS!  "))
        {
            StartNewProject();
        }

    }

    public void ImportAssetSettings()
    {
        if (packageList != null)
            packageList.Clear();
        if (names != null)
            names.Clear();
        if (showBtn != null)
            showBtn.Clear();
        if (newFileList != null)
            newFileList.Clear();
        if (selectedAsset != null)
            selectedAsset.Clear();

        packageStatus = Path.Combine("/KikiExtension", "StarterPackage");
        names = Directory.GetFiles(Application.dataPath + packageStatus).ToList();



        for (int i = 0; i < names.Count; i++)
        {
            fileList.Add(names[i].Split('\\'));
            if (i % 2 == 0)
            {
                newFileList.Add(fileList[i][fileList[i].Length - 1]);
            }
        }

        if (fileList != null)
        {
            for (int i = 0; i < newFileList.Count; i++)
            {
                if (newFileList[i] == "Odin - Inspector and Serializer v3.0.4.unitypackage" ||
                    newFileList[i] == "DoTween_v1.2.632.unitypackage")
                {
                    showBtn.Add(true);
                    assetString.Add((Path.Combine("/KikiExtension", "StarterPackage", newFileList[i])));
                }
                else
                {
                    showBtn.Add(false);
                }
            }
        }
    }

    private void UpdateAssetPackage(int index, bool isOpen)
    {

        if (isOpen)
        {
            string status = (Path.Combine("/KikiExtension", "StarterPackage", newFileList[index]));
            if (selectedAsset.Contains(status))
            {
                return;
            }
            selectedAsset.Add(status);
        }

    }

    private void ImportAssets()
    {

        foreach (string package in selectedAsset)
        {
            AssetDatabase.ImportPackage(Application.dataPath + package, false);
        }
    }


}
#endif