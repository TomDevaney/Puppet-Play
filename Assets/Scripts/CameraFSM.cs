using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFSM : StateMachine
{
	// Use this to indicate what lerp speed should be used
	enum CameraMoveSpeed
	{
		Slow,
		Average,
		Fast,
	}

	CameraMoveSpeed moveSpeed;

	// Camera cannot go past these limits
	public float levelBoundLeft;
	public float levelBoundRight;
	public float levelBoundTop;
	public float levelBoundBottom;

	// Is the camera close enouogh to target position?
	const float distanceThreshold = 1.5f;

	// Start is called before the first frame update
	void Start()
	{
		// Set to the default state
		SetCurrentState(new FollowState(this, GameManager.instance.GetPlayerPuppet().gameObject, false));
	}

	public override void Reset()
	{
		SetCurrentState(new FollowState(this, GameManager.instance.GetPlayerPuppet().gameObject, false));
	}


	/* Make states needed for CameraFSM */
	public class CameraState : State
	{
		CameraFSM cameraFSM;

		public CameraState(CameraFSM camera) : base()
		{
			cameraFSM = camera;
		}

		public void SetCameraFSM(CameraFSM camera)
		{
			cameraFSM = camera;
		}

		public CameraFSM GetCameraFSM()
		{
			return cameraFSM;
		}
	}

	public class FollowState : CameraState
	{
		GameObject objectToFollow;
		bool notifyEventManager;

		public FollowState(CameraFSM controller, GameObject gameObject, bool eventBased) : base(controller)
		{
			objectToFollow = gameObject;
			notifyEventManager = eventBased;

			print("Camera FollowState");
		}

		override public State Update()
		{
			float levelBoundLeft = GetCameraFSM().levelBoundLeft;
			float levelBoundRight = GetCameraFSM().levelBoundRight;
			float levelBoundTop = GetCameraFSM().levelBoundTop;
			float levelBoundBottom = GetCameraFSM().levelBoundBottom;

			Transform followTransform = objectToFollow.transform;
			Transform cameraTransform = GetCameraFSM().transform;

			// Check x bounds
			float targetX = followTransform.position.x;
			if (followTransform.position.x > levelBoundRight)
			{
				targetX = levelBoundRight;
			}
			else if (followTransform.position.x < levelBoundLeft)
			{
				targetX = levelBoundLeft;
			}

			// Check y bounds
			float targetY = followTransform.position.y;
			if (followTransform.position.y > levelBoundBottom)
			{
				targetY = levelBoundTop;
			}
			else if (objectToFollow.transform.position.y < levelBoundBottom)
			{
				targetY = levelBoundBottom;
			}

			// Get new position off of target position
			float nextX = Mathf.Lerp(cameraTransform.position.x, targetX, Time.deltaTime);
			float nextY = Mathf.Lerp(cameraTransform.position.y, targetY, Time.deltaTime);

			cameraTransform.position = new Vector3(nextX, nextY, cameraTransform.position.z);

			// See if the camera is close enough to notify eventmanager
			float totalDistance = Mathf.Abs(cameraTransform.position.x - targetX) + Mathf.Abs(cameraTransform.position.y - targetY);

			if (notifyEventManager && totalDistance < distanceThreshold)
			{
				notifyEventManager = false;
				EventManager.instance.MarkEventAsDone();
			}

			return CheckForTransition();
		}

		override public void OnStateEnter()
		{

		}

		override public void OnStateExit()
		{

		}
	}

	public class MoveState : CameraState
	{
		float targetX;
		float targetY;

		bool notifyEventManager;

		public MoveState(CameraFSM controller, float xPosition, float yPosition, bool eventBased) : base(controller)
		{
			targetX = xPosition;
			targetY = yPosition;
			notifyEventManager = eventBased;

			print("Camera MoveState");
		}

		override public State Update()
		{
			float levelBoundLeft = GetCameraFSM().levelBoundLeft;
			float levelBoundRight = GetCameraFSM().levelBoundRight;
			float levelBoundTop = GetCameraFSM().levelBoundTop;
			float levelBoundBottom = GetCameraFSM().levelBoundBottom;

			Transform cameraTransform = GetCameraFSM().transform;

			// Check x bounds
			if (targetX > levelBoundRight)
			{
				targetX = levelBoundRight;
			}
			else if (targetX < levelBoundLeft)
			{
				targetX = levelBoundLeft;
			}

			// Check y bounds
			if (targetY > levelBoundBottom)
			{
				targetY = levelBoundTop;
			}
			else if (targetY < levelBoundBottom)
			{
				targetY = levelBoundBottom;
			}

			// Get new position off of target position
			float nextX = Mathf.Lerp(cameraTransform.position.x, targetX, Time.deltaTime);
			float nextY = Mathf.Lerp(cameraTransform.position.y, targetY, Time.deltaTime);

			cameraTransform.position = new Vector3(nextX, nextY, cameraTransform.position.z);

			// See if the camera is close enough to notify eventmanager
			float totalDistance = Mathf.Abs(cameraTransform.position.x - targetX) + Mathf.Abs(cameraTransform.position.y - targetY);

			if (notifyEventManager && totalDistance < distanceThreshold)
			{
				notifyEventManager = false;
				EventManager.instance.MarkEventAsDone();
			}

			return CheckForTransition();
		}

		override public void OnStateEnter()
		{

		}

		override public void OnStateExit()
		{

		}
	}
}