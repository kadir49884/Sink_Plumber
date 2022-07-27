using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerPrefsManager
{
	public static int GetLevel => PlayerPrefs.GetInt(KikiStatics.ACTIVE_LEVEL, 1);
	public static void SetLevel(int value) => PlayerPrefs.SetInt(KikiStatics.ACTIVE_LEVEL, value);

	public static void ResetLevel() => SetLevel(1);


	public static void LevelNext()
	{
		int level = GetLevel;
		SetLevel(level + 1);
	}
	public static void PreviousLevel()
	{
		int level = GetLevel;
		SetLevel(level - 1);
	}

}
