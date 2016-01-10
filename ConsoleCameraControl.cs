using UnityEngine;
using System.Collections;

public class ConsoleCameraControl : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	GameManager gameManager;
	GameMenuUI gameMenuUI;
	public Camera consoleCamera;
	Vector3 roomPosition;
	Vector2 roomSize;

	KeyCode w, a, s, d, left, right, up, down;
	KeyCode num1, num2, spacebar;
	float scrollWheel;

	float cameraUpperBound, cameraLowerBound, cameraLeftBound, cameraRightBound;
	float zoomInBound = 20, zoomOutBound;

	Vector3 levelCentre;

	public LevelDetails levelDetails;

	/*---------------------------------------------------- LATE UPDATE ----------------------------------------------------*/

	void LateUpdate()
	{
		if(!gameMenuUI.menuOpen)
			CameraMovement();
	}

	/*---------------------------------------------------- CAMERA MOVEMENT ----------------------------------------------------*/

	void CameraMovement()
	{
		// Focus Room Controls
		if(Input.GetKeyDown(num1) && gameManager.myPlayerCurrentRoom)
			FocusRoom(gameManager.myPlayerCurrentRoom);
		else if(Input.GetKeyDown(num2) && gameManager.otherPlayerCurrentRoom)
			FocusRoom(gameManager.otherPlayerCurrentRoom);
		else if(Input.GetKeyDown(spacebar))
			FocusLevel();
		
		// Verticle Movement
		if(Input.GetKey(w)||Input.GetKey(up))
			transform.Translate(0, 2*consoleCamera.orthographicSize*Time.deltaTime, 0);
		else if(Input.GetKey(s)||Input.GetKey(down))
			transform.Translate(0, -2*consoleCamera.orthographicSize*Time.deltaTime, 0);
		
		// Horizontal Movement
		if(Input.GetKey(a)||Input.GetKey(left))
			transform.Translate(-2*consoleCamera.orthographicSize*Time.deltaTime, 0, 0);
		else if(Input.GetKey(d)||Input.GetKey(right))
			transform.Translate(2*consoleCamera.orthographicSize*Time.deltaTime, 0, 0);
		
		// Zoom Movement
		scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if(scrollWheel > 0)
			consoleCamera.orthographicSize-=3f;
		else if(scrollWheel < 0)
			consoleCamera.orthographicSize+=3f;

        CheckCameraBounds();
    }

	/*---------------------------------------------------- FOCUS ROOM ----------------------------------------------------*/

    public void FocusRoom(GameObject roomToFocus)
    {
		roomPosition = roomToFocus.transform.position;
		roomSize = roomToFocus.GetComponent<Collider2D>().bounds.size;

		Camera.main.ResetAspect();

		if((roomSize.x)*Camera.main.aspect > roomSize.y) // If the room needs to be bounded by the camera width-wise.
			consoleCamera.orthographicSize = ((roomSize.x)/Camera.main.aspect);
		else
			consoleCamera.orthographicSize = (roomSize.y);
		consoleCamera.transform.position = roomPosition + 10*Vector3.back;

        CheckCameraBounds();
    }

	/*---------------------------------------------------- FOCUS LEVEL ----------------------------------------------------*/

	void FocusLevel()
	{
		consoleCamera.orthographicSize = zoomOutBound; // Set zoom to minimum bound which includes the whole level.
		consoleCamera.transform.position = levelCentre + 10*Vector3.back; // Set position to centre of level.

		Camera.main.ResetAspect();
        CheckCameraBounds();
    }

	/*---------------------------------------------------- SET CAMERA BOUNDS ----------------------------------------------------*/

	public void SetCameraBounds()
	{
		Camera.main.ResetAspect();

		cameraUpperBound = levelDetails.levelCentre.y + levelDetails.levelDimensions.y/2;
		cameraLowerBound = levelDetails.levelCentre.y - levelDetails.levelDimensions.y/2;
		cameraLeftBound = levelDetails.levelCentre.x - levelDetails.levelDimensions.x/2;
		cameraRightBound = levelDetails.levelCentre.x + levelDetails.levelDimensions.x/2;

		// Check if the level needs to be bounded by the camera width-wise or height-wise.
		if((levelDetails.levelDimensions.x)*Camera.main.aspect > levelDetails.levelDimensions.y) 
			zoomOutBound = (levelDetails.levelDimensions.x/Camera.main.aspect)/2;
		else 
			zoomOutBound = levelDetails.levelDimensions.y/2;

        if(zoomOutBound < zoomInBound)
            zoomOutBound = zoomInBound;

		levelCentre = levelDetails.levelCentre;
	}

	/*---------------------------------------------------- CHECK CAMERA BOUNDS ----------------------------------------------------*/

	void CheckCameraBounds()
	{
		// Check orthographic size against zoom bounds.
		if (consoleCamera.orthographicSize < zoomInBound)
			consoleCamera.orthographicSize = zoomInBound;
		else if (consoleCamera.orthographicSize > zoomOutBound)
			consoleCamera.orthographicSize = zoomOutBound;

        // Check camera horizontal position against horizontal bounds.
		if (consoleCamera.transform.position.x  < cameraLeftBound)
			consoleCamera.transform.position = new Vector3(cameraLeftBound, consoleCamera.transform.position.y, consoleCamera.transform.position.z);
		else if (consoleCamera.transform.position.x > cameraRightBound)
			consoleCamera.transform.position = new Vector3(cameraRightBound, consoleCamera.transform.position.y, consoleCamera.transform.position.z);
		
        // Check camera verticle position against verticle bounds.
		if (consoleCamera.transform.position.y > cameraUpperBound)
			consoleCamera.transform.position = new Vector3(consoleCamera.transform.position.x, cameraUpperBound, consoleCamera.transform.position.z);
		else if (consoleCamera.transform.position.y < cameraLowerBound)
			consoleCamera.transform.position = new Vector3(consoleCamera.transform.position.x, cameraLowerBound, consoleCamera.transform.position.z);
	}

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	public void Initialisation()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		gameMenuUI = GameObject.Find("GameMenuUI").GetComponent<GameMenuUI>();

		w = KeyCode.W;
		a = KeyCode.A;
		s = KeyCode.S;
		d = KeyCode.D;
		
		left = KeyCode.LeftArrow;
		right = KeyCode.RightArrow;
		up = KeyCode.UpArrow;
		down = KeyCode.DownArrow;

		num1 = KeyCode.Alpha1;
		num2 = KeyCode.Alpha2;
		spacebar = KeyCode.Space;

		consoleCamera = GetComponent<Camera>();
		levelDetails = GameObject.Find("Level").GetComponent<LevelDetails>();

		levelDetails.consoleCameraControl = this;
    }
}
