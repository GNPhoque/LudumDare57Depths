using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed;
	[SerializeField] private float rotationSpeed;
	[SerializeField] private float maxXRotation;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;
	[SerializeField] private CameraController cameraController;
	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private PlayerUI playerUI;

	private bool canMove = true;
    public float gravity = 10f;
	private float xRotation;
	private Vector2 moveInput;
	private Vector2 lookInput;
	private Vector3 movement;

	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{

		#region Move
		Vector3 forward = transform.forward;
		Vector3 right = transform.right;

		float curSpeedX = canMove ? moveInput.x : 0;
		float curSpeedZ = canMove ? moveInput.y : 0;
		float movementDirectionY = movement.y;
		movement = (forward * curSpeedZ) + (right * curSpeedX);

		if (movement.magnitude > 1)
			movement.Normalize();

		movement = movement * moveSpeed;

		movement.y = movementDirectionY;
		#endregion

		//#region Jump
		//if (_input.jump && canMove && cController.isGrounded)
		//{
		//	moveDirection.y = jStrenght;
		//	_input.jump = false;
		//}
		//else
		//{
		//	moveDirection.y = movementDirectionY;
		//}

		if (!characterController.isGrounded)
		{
			movement.y -= gravity * Time.deltaTime;
		}

		//#endregion

		characterController.Move(movement * Time.deltaTime);

		#region Rotation
		if (canMove)
		{
			xRotation += -lookInput.y * rotationSpeed;
			xRotation = Mathf.Clamp(xRotation, -maxXRotation, maxXRotation);
			cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
			transform.rotation *= Quaternion.Euler(0, lookInput.x * rotationSpeed, 0);
		}
		#endregion
	}

	public void OnMove(InputValue value)
	{
		moveInput = value.Get<Vector2>();
	}

	public void OnLook(InputValue value)
	{
		lookInput = value.Get<Vector2>();
		if (playerInput.currentControlScheme == "Keyboard&Mouse")
		{
			lookInput *= .1f; 
		}
	}

	public void OnCrouch()
	{
		cameraController.CycleObjectives();
	}

	public void OnTakePhoto(InputValue value)
	{
		if (playerUI.isWaitingInput)
		{
			playerUI.ContinueUI();
		}
		else
		{
			StartCoroutine(cameraController.TakePhoto());
		}
	}

	public void OnAutoFocus(InputValue value)
	{
		cameraController.AutoFocus();
	}

	public void OnAperture(InputValue value)
	{
		cameraController.SelectAperture();
	}

	public void OnFocalLength(InputValue value)
	{
		cameraController.SelectFocalLength();
	}

	public void OnIncreaseSetting(InputValue value)
	{
		cameraController.IncreaseSetting();
	}

	public void OnDecreaseSetting(InputValue value)
	{
		cameraController.DecreaseSetting();
	}
}
