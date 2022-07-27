using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace KikiExtension.Managers
{
	[CreateAssetMenu(menuName = "KikiExtension/Managers/LevelManager")]

	public class LevelManager : ScriptableBase
	{

        [ReadOnly] public LevelScriptable activeLevel;
        public GameObject level;

        //private string levelInfo;

        //private void Awake()
        //{
        //    levelInfo = Path.Combine(KikiStatics.LEVELS, KikiStatics.LEVELSTABIL + PlayerPrefs.GetInt(KikiStatics.ACTIVE_LEVEL, 1));
        //    LevelScriptable levelScriptable = Resources.Load<LevelScriptable>(levelInfo);
        //    Debug.Log(levelScriptable);
        //    Initialize();
        //}

        public void SetLevel()
        {
            activeLevel = GetLevel();
            TinySauce.OnGameStarted(activeLevel.ToString());
            Debug.Log("aaaaaaaaa" + activeLevel.ToString());
            InitializeLevel();
        }

        private void InitializeLevel()
        {
            if (activeLevel != null && activeLevel.levelPrefab != null)
            {
                level = Instantiate(activeLevel.levelPrefab);
            }
            else
            {
                PlayerPrefsManager.ResetLevel();
            }
        }

        private LevelScriptable GetLevel()
        {
            int levelIndex = PlayerPrefsManager.GetLevel;

            LevelScriptable level = Resources.Load(KikiStatics.LEVELS_PATH + levelIndex) as LevelScriptable;

            

            if (level == null)
            {
                PlayerPrefsManager.SetLevel(1);
                levelIndex = PlayerPrefsManager.GetLevel;
                level = Resources.Load(KikiStatics.LEVELS_PATH + levelIndex) as LevelScriptable;
            }
            return level;
        }

        public void Initialize()
        {
            var trashObjects = FindObjectsOfType<LevelController>().ToList();

            if (trashObjects.Count > 0)
                trashObjects.ForEach(trash => Destroy(trash.gameObject));

            SetLevel();
        }
    }
}