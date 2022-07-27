using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
	public Camera OrthoCamera;
	[SerializeField]
	private Transform playerObject;
	[SerializeField]
	private Transform waterParticle;
	[SerializeField]
	private Transform finishWater;
	[SerializeField]
	private Transform cameraPlayPos;
	[SerializeField]
	private Transform dirtyWater;
	[SerializeField]
	private Transform waterSplash;
	[SerializeField]
	private Transform waterSound;
	[SerializeField]
	private Transform dirtySplash;
	[SerializeField]
	private Transform targetObject;
	[SerializeField]
	private Transform perfectObject;



	public Transform PlayerObject { get => playerObject; set => playerObject = value; }
	public Transform WaterParticle { get => waterParticle; set => waterParticle = value; }
	public Transform FinishWater { get => finishWater; set => finishWater = value; }
    public Transform CameraPlayPos { get => cameraPlayPos; set => cameraPlayPos = value; }
    public Transform DirtyWater { get => dirtyWater; set => dirtyWater = value; }
    public Transform WaterSplash { get => waterSplash; set => waterSplash = value; }
    public Transform WaterSound { get => waterSound; set => waterSound = value; }
    public Transform DirtySplash { get => dirtySplash; set => dirtySplash = value; }
    public Transform TargetObject { get => targetObject; set => targetObject = value; }
    public Transform PerfectObject { get => perfectObject; set => perfectObject = value; }
}