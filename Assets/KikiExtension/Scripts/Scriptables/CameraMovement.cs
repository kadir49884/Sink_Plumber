using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KikiExtension;

public class CameraMovement : MonoBehaviour
{

	[SerializeField] private Transform target;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float lerpTime = .1f;

	private Vector3 newPos;


	private void Start()
	{
		GameManager gameManager = GameManager.Instance;

		gameManager.GameWin += GameWin;
		gameManager.GameFail += GameFail;

	}

	public Transform Target { get => target; set => target = value; }

	void FixedUpdate()
	{
		newPos = Target.localPosition + offset;
		newPos.x = 0;
		newPos.z = Target.localPosition.z + offset.z;
		transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, lerpTime);
	}


	private void GameWin()
	{
		//	TODO 
	}
	private void GameFail()
	{
		// TODO
	}
}
