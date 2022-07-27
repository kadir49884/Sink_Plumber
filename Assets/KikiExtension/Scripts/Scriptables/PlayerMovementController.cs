using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementController : MonoBehaviour
{
	[SerializeField] private float playerSpeed = 5;
	[SerializeField] private float sensitivity = 5;
	[SerializeField] private float lerpValue = 0.1f;

	[SerializeField] private Vector2 clampX = new Vector2(-5, 5);
	private Camera ortho;

	private Vector3 diff;
	private Vector3 firstPos;
	private Vector3 mousePos;

	private Rigidbody body;

	private GameManager gameManager;

	private void Awake()
	{
		body = GetComponent<Rigidbody>();
	}
	private void Start()
	{
		gameManager = GameManager.Instance;
		ortho = ObjectManager.Instance.OrthoCamera;
	}

	void Update()
	{
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, clampX.x, clampX.y),
			transform.position.y,
			transform.position.z);

		if (!gameManager.RunGame)
			return;

		firstPos = Vector3.Lerp(firstPos, mousePos, .1f);

		if (Input.GetMouseButtonDown(0))
			MouseDown(Input.mousePosition);

		else if (Input.GetMouseButtonUp(0))
			MouseUp();

		else if (Input.GetMouseButton(0))
			MouseHold(Input.mousePosition);
	}


	private void FixedUpdate()
	{
		if (!gameManager.RunGame)
		{
			body.velocity = new Vector3(0, body.velocity.y, 0);
			return;
		}

		body.velocity = Vector3.Lerp(body.velocity, new Vector3(diff.x, body.velocity.y, playerSpeed), lerpValue);
	}

	private void MouseDown(Vector3 inputPos)
	{
		mousePos = ortho.ScreenToWorldPoint(inputPos);
		firstPos = mousePos;
	}

	private void MouseHold(Vector3 inputPos)
	{
		mousePos = ortho.ScreenToWorldPoint(inputPos);
		diff = mousePos - firstPos;
		diff *= sensitivity;
	}

	private void MouseUp()
	{
		diff = Vector3.zero;
	}
}
