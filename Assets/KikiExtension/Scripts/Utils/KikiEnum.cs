using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
	Idle = 0,
	Running = 1,
}
[System.Serializable]

public static class KikiEnum
{
	public static bool IsRunOrIdle(this AnimationType type) => type == AnimationType.Idle || type == AnimationType.Running;

}
