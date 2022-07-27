using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelScriptable : ScriptableObject
{
	[AssetsOnly, AssetList(Path = "Prefabs/LevelPrefabs")] public GameObject levelPrefab;
}
