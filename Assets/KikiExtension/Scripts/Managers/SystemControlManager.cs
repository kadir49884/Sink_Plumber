using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KikiExtension.Managers
{
	[CreateAssetMenu(menuName = "KikiExtension/Managers/SystemControlManager")]
	public class SystemControlManager : ScriptableBase
	{
		[FoldoutGroup("Debug"), SerializeField, ShowInInspector] private bool isIpadDebug;
		[FoldoutGroup("Debug"), SerializeField, ShowInInspector] private bool isAboveDebug;

		public bool IsIpad { get => CheckIsIpad(); }

		public bool AboveIphone8 { get => CheckAboveIphone8(); }

		private bool CheckIsIpad()
		{
#if UNITY_EDITOR
			return isIpadDebug ? true : SystemInfo.deviceModel.ToString().IndexOf("iPad") > -1;
#else
			return SystemInfo.deviceModel.ToString().IndexOf("iPad") > -1;
#endif
		}

		private bool CheckAboveIphone8()
		{

#if UNITY_EDITOR
			if (!isAboveDebug)
			{
				double screenRatio = (1.0 * Screen.height) / (1.0 * Screen.width);

				if (1.7f < screenRatio && screenRatio < 1.8f)   // iphone 8+ ve altı
					return false;
				else
					return true;
			}
			else
				return true;
#else

		double screenRatio = (1.0 * Screen.height) / (1.0 * Screen.width);

			if (1.7f < screenRatio && screenRatio < 1.8f)   // iphone 8+ ve altı
				return false;
			else
				return true;
#endif
		}
	}
}