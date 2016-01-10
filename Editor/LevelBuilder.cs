using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LevelBuilder : EditorWindow
{
    //Objects
    GameObject level, room, lastCreatedRoom, selectedRoom, roomToAlignTo;
    GameObject topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;
    GameObject lightsButton, consoleViewCanvas, consoleViewRoom;

    //Room Details
    int roomTypeSelection = 0, roomNumSelection = 0;
    string roomName;
    int roomWidth = 1, roomHeight = 1;

    //Background
    int roomBackground;
    GameObject background;

    //Doors
	public struct DoorProperties
	{
        public int wall;
        public int position;
	}

    public int doorCount;

    public DoorProperties[] doorProperties = new DoorProperties[0];

    //Room Alignment
    int alignRoomSelection = 0;

    //Editor Prefs
    int roomNum = 1, corridorNum = 1;

    //GUI
    GUIStyle boldFont = new GUIStyle();
    
    float maxFieldWidth = 180;

    int toolbarFilterSelection = 0, toolbarAlignToRoom = 0;
    string[] toolbarFilterOptions = new string[] {"None", "Room", "Door"};
    string[] toolbarAlignToRoomOptions = new string[] { "Top", "Bottom", "Left", "Right"};

    //Options
    bool showOptions = true, resetSelection, debugWalls;
    bool automaticRoomNumbering = true;

    //UNITY MENU ITEM
    // Adds menu item named "Level Builder" to the NMA Tool menu
    [MenuItem("NMA Tools/Level Builder %l")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.        
        EditorWindow levelBuilder = EditorWindow.GetWindow(typeof(LevelBuilder), false, "Level Builder", false);
        levelBuilder.minSize = new Vector2(275, 400);     
    }

    void OnEnable()
    {
        LoadEditorPrefs();
        boldFont.fontStyle = FontStyle.Bold;
    }

    void OnDisable()
    {
        SaveEditorPrefs();
    }

    //LEVEL BUILDER GUI
    void OnGUI()
    {
        //Main Layout
        GUILayout.BeginVertical();

        ///ROOM///

        //Room label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Room Details", boldFont, GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Room layout
        GUILayout.BeginVertical("box");

        //Room Name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Room Name");
        string[] roomNames = new string[] { "Room", "Corridor", "Start Room", "Other" };
        string[] roomNumbers = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(maxFieldWidth));

        roomTypeSelection = EditorGUILayout.Popup(roomTypeSelection, roomNames, GUILayout.MaxWidth(maxFieldWidth));

        if (roomTypeSelection != 3)
        {
            if(!automaticRoomNumbering)
                roomNumSelection = EditorGUILayout.Popup(roomNumSelection, roomNumbers);

            if (roomTypeSelection == 0) //"Room"
            {
                if(automaticRoomNumbering)
                    roomName = roomNames[roomTypeSelection] + " " + roomNum;

                else
                    roomName = roomNames[roomTypeSelection] + " " + roomNumSelection;
            }

            else if (roomTypeSelection == 1) //"Corridor"
            {
                if (automaticRoomNumbering)
                    roomName = roomNames[roomTypeSelection] + " " + corridorNum;

                else
                    roomName = roomNames[roomTypeSelection] + " " + roomNumSelection;
            }

            else
                roomName = roomNames[roomTypeSelection];
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (roomTypeSelection == 3)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("");

            roomName = GUILayout.TextField(roomName, GUILayout.MaxWidth(maxFieldWidth));
            GUILayout.EndHorizontal();
        }

        //Background
        GUILayout.BeginHorizontal();
        GUILayout.Label("Background");
        string[] roomArtList = new string[] { "Blue Metal", "Green Metal", "Yellow Metal", "Orange Metal" };
        roomBackground = EditorGUILayout.Popup(roomBackground, roomArtList, GUILayout.MaxWidth(maxFieldWidth));
        GUILayout.EndHorizontal();

        //Dimensions
        GUILayout.BeginHorizontal();
        GUILayout.Label("Dimensions");
        GUILayout.BeginHorizontal((GUILayout.MaxWidth(maxFieldWidth)));
        string[] roomWidthList = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        string[] roomHeightList = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        roomWidth = EditorGUILayout.Popup(roomWidth, roomWidthList);
        roomHeight = EditorGUILayout.Popup(roomHeight, roomHeightList);
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        ///DOORS///

        //Doors Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Doors", boldFont, GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Doors Layout
        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("No. of Doors");
        string[] doorCountList = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8" };
        doorCount = EditorGUILayout.Popup(doorCount, doorCountList, (GUILayout.MaxWidth(maxFieldWidth)));
        GUILayout.EndHorizontal();

        System.Array.Resize(ref doorProperties, doorCount);

        GUILayout.Space(5);

        for (int i = 0; i < doorCount; i++)
        {
            GUILayout.BeginHorizontal();
            string[] wall = new string[] { "Top Wall", "Bottom Wall", "Left Wall", "Right Wall"};
            string[] positionHorizontalWall = new string[] { "Center", "Left", "Right"};
            string[] positionVerticalWall = new string[] { "Bottom", "Center", "Top"};

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(maxFieldWidth));
            GUILayout.Label("Wall");
            doorProperties[i].wall = EditorGUILayout.Popup(doorProperties[i].wall, wall);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.MaxWidth(maxFieldWidth));
            GUILayout.Label("Position");
            if (doorProperties[i].wall > 1)
                doorProperties[i].position = EditorGUILayout.Popup(doorProperties[i].position, positionVerticalWall);

            else
                doorProperties[i].position = EditorGUILayout.Popup(doorProperties[i].position, positionHorizontalWall);
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        ///ROOM ALIGNMENT///

        //Room Alignment Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Room Alignment", boldFont, GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Room Alignment Layout
        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Align to:");
        string[] alignRoomOptions = new string[] { "Last Created Room", "Selected Room", "Don't Align"};
        alignRoomSelection = EditorGUILayout.Popup(alignRoomSelection, alignRoomOptions, GUILayout.MaxWidth(maxFieldWidth));
        GUILayout.EndHorizontal();

        if (alignRoomSelection == 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Last Created Room:");

            if (lastCreatedRoom)
                GUILayout.Label(lastCreatedRoom.name, GUILayout.MaxWidth(maxFieldWidth));

            else
                GUILayout.Label("No Rooms Created", GUILayout.MaxWidth(maxFieldWidth));

            GUILayout.EndHorizontal();

            roomToAlignTo = lastCreatedRoom;
        }

        else if (alignRoomSelection == 1)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Selected Room:");

            if (selectedRoom)
                GUILayout.Label(selectedRoom.name, GUILayout.MaxWidth(maxFieldWidth));

            else
                GUILayout.Label("No Room Selected", GUILayout.MaxWidth(maxFieldWidth));

            GUILayout.EndHorizontal();

            roomToAlignTo = selectedRoom;
        }

        else
            roomToAlignTo = null;

        toolbarAlignToRoom = GUILayout.Toolbar(toolbarAlignToRoom, toolbarAlignToRoomOptions);

        GUILayout.EndVertical();

        //Build Room Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Build Room", boldFont, GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Build Room Button
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Room"))
            BuildRoom(true);

        //Build Room W/O Walls Button
        if (GUILayout.Button("Build Room w/o Walls"))
            BuildRoom(false);
        GUILayout.EndHorizontal();

        //Filter Selection Toolbar
        GUILayout.Space(15);
        
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selection Filter", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        toolbarFilterSelection = GUILayout.Toolbar(toolbarFilterSelection, toolbarFilterOptions);
        

        //Selected Room
        if (selectedRoom && Selection.activeGameObject)
        {
            DispaySelectedRoom(selectedRoom);

            //Sets gameobject in hierarchy
            if (toolbarFilterSelection == 1)
            {
                if (selectedRoom != null)
                    Selection.activeGameObject = selectedRoom;
            }
        }

        //Debug Options
        GUILayout.Space(15);

        showOptions = EditorGUILayout.Foldout(showOptions, "Options");

        if (showOptions)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Selection Options"))
                setDefaultFields();

            //Build Room W/O Walls Button
            if (GUILayout.Button("Reset Room Numbering"))
                ResetRoomNumbering();

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box");
            automaticRoomNumbering = GUILayout.Toggle(automaticRoomNumbering, "Automatic Room Numbering");
            resetSelection = GUILayout.Toggle(resetSelection, "Reset selection options after building room");
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            GUILayout.Label("Debug Messages", boldFont);
            debugWalls = GUILayout.Toggle(debugWalls, "Walls");
            GUILayout.EndVertical();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Notes", boldFont);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Automatic room numbering wiil not work if rooms are not deleted with delete room button. Press reset room Numbering to reset to one", MessageType.Info);
            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();
    } 

    //LEVEL BUILDER FUNCTIONS

    //Builds Room
    void BuildRoom(bool buildWalls)
    {
        CreateRoomObject();

        //Increment and save room numbers
        if (automaticRoomNumbering)
        {
            if (roomTypeSelection == 0) //"Room"
            {
                room.GetComponent<RoomProperties>().roomNum = roomNum;
                roomNum++;
            }

            else if (roomTypeSelection == 1)// "Corridor"
            {
                room.GetComponent<RoomProperties>().roomNum = corridorNum;
                corridorNum++;
            }
        }

        SetSize();
        SetBackground();

        if(doorProperties.Length > 0)
            CreateDoors(room);

        if (buildWalls)
            CreateWalls(room);

        if (roomToAlignTo)
            SetPosition();

        if (resetSelection)
            setDefaultFields();

		SetPlayerTriggers(room);
               
        lastCreatedRoom = room;
        Selection.activeGameObject = room;
    }

    //Creates Room Object
    void CreateRoomObject()
    {
        //Variables
        GameObject levelObject;

        //Create room object
        Object roomPrefab = Resources.Load("Room");
        room = (GameObject)PrefabUtility.InstantiatePrefab(roomPrefab);

        //Set room name
        if (string.IsNullOrEmpty(roomName))
            room.name = "Unnamed Room";

        else
            room.name = roomName;

        //Checks for Level object, and creates one if not found.
        if (!GameObject.Find("Level"))
        {
            levelObject = new GameObject();
            levelObject.name = "Level";
            levelObject.transform.position = Vector3.zero;
            levelObject.AddComponent<LevelDetails>();
        }

        else
            levelObject = GameObject.Find("Level");

        //Set Level as parent
        room.transform.SetParent(levelObject.transform); //Sets Level as parent object.      
    }

    //Create Room Shape
    void SetSize()
    {
        room.GetComponent<BoxCollider2D>().size = new Vector2(10 * (roomWidth + 1), 10 * (roomHeight + 1));
    }

    //Sets Room Position
    void SetPosition()
    {
        Vector2 roomAlignCenter;
        float roomAlignEdge, roomExtents;

        if (roomToAlignTo)
        {
            roomAlignCenter = roomToAlignTo.transform.position;

            switch (toolbarAlignToRoom)
            {
                //Order - Top, Bottom, Left, Right
                case 0:
                roomAlignEdge = roomToAlignTo.GetComponent<BoxCollider2D>().bounds.max.y + 1;
                roomExtents = room.GetComponent<BoxCollider2D>().bounds.extents.y;
                room.transform.position = new Vector2(roomAlignCenter.x, roomAlignEdge + roomExtents);
                break;

                case 1:
                roomAlignEdge = roomToAlignTo.GetComponent<BoxCollider2D>().bounds.min.y - 1;
                roomExtents = room.GetComponent<BoxCollider2D>().bounds.extents.y;
                room.transform.position = new Vector2(roomAlignCenter.x, roomAlignEdge - roomExtents);
                break;

                case 2:
                roomAlignEdge = roomToAlignTo.GetComponent<BoxCollider2D>().bounds.min.x - 1;
                roomExtents = room.GetComponent<BoxCollider2D>().bounds.extents.x;
                room.transform.position = new Vector2(roomAlignEdge - roomExtents, roomAlignCenter.y);
                break;

                case 3:
                roomAlignEdge = roomToAlignTo.GetComponent<BoxCollider2D>().bounds.max.x + 1;
                roomExtents = room.GetComponent<BoxCollider2D>().bounds.extents.x;
                room.transform.position = new Vector2(roomAlignEdge + roomExtents, roomAlignCenter.y);
                break;

                default:
                break;
            }
        }
    }

    //Creates the background of the room.
    void SetBackground()
    {
        Object backgroundPrefab = Resources.Load("RoomBackground");
        background = (GameObject)PrefabUtility.InstantiatePrefab(backgroundPrefab);
        background.transform.SetParent(room.transform);
        background.transform.localScale = new Vector3(1, 1, 1);

        Sprite[] backgroundSprites = Resources.LoadAll<Sprite>("Room_Background");

        switch (roomBackground)
        {
            case 0:
            background.transform.FindChild("Background_Lit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[0];
            background.transform.FindChild("Background_Unlit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[0];
            break;
            case 1:
            background.transform.FindChild("Background_Lit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[1];
            background.transform.FindChild("Background_Unlit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[1];
            break;
            case 2:
            background.transform.FindChild("Background_Lit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[2];
            background.transform.FindChild("Background_Unlit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[2];
            break;
            default:
            background.transform.FindChild("Background_Lit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[3];
            background.transform.FindChild("Background_Unlit").GetChild(0).GetComponent<Image>().sprite = backgroundSprites[3];
            break;
        }

        //Scale Background Canvas
        foreach (Transform canvas in background.transform)
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(10 * (roomWidth + 1), 10 * (roomHeight + 1));
    }
    
	void SetPlayerTriggers(GameObject r)
	{
        Bounds roomBounds = r.GetComponent<Collider2D>().bounds;

        room.transform.FindChild("Player Triggers").GetComponent<BoxCollider2D>().size = new Vector2(20 + roomBounds.size.x, 20 + roomBounds.size.y);
		room.transform.FindChild("Visibility Overlay").GetComponent<RectTransform>().sizeDelta = new Vector2(roomBounds.size.x, roomBounds.size.y);
	}

    void CreateDoors(GameObject r)
	{
        Bounds roomBounds = r.GetComponent<Collider2D>().bounds;

        GameObject doorsParent = new GameObject("Doors");
        doorsParent.transform.transform.SetParent(room.transform);
        doorsParent.transform.localScale =  new Vector3(1 , 1 , 1);

        Object doorVPrefab = Resources.Load("DoorV");
        Object doorHPrefab = Resources.Load("DoorH");
        float doorOffset = 0;

        for (int i = 0; i < doorProperties.Length; i++) //Cycles through all doors defined in the editor.
        {
            GameObject door;

            //Set door vetical or horizontal based on wall. Wall Order - Top, Bottom, Left, Right
            if (doorProperties[i].wall == 0 || doorProperties[i].wall == 1)
                door = (GameObject)PrefabUtility.InstantiatePrefab(doorHPrefab);

            else
                door = (GameObject)PrefabUtility.InstantiatePrefab(doorVPrefab);

            door.name += " " + (i + 1);
            door.transform.SetParent(doorsParent.transform);
            door.transform.localScale = new Vector3(1 / room.transform.localScale.x, 1 / room.transform.localScale.y, 1);

            if (doorProperties[i].wall == 0)
            {
                //Position Order - Center, Left, Right
                if (doorProperties[i].position == 1)
                    doorOffset = roomBounds.min.x + door.GetComponent<BoxCollider2D>().bounds.size.x / 2;

                else if (doorProperties[i].position == 2)
                    doorOffset = roomBounds.max.x - door.GetComponent<BoxCollider2D>().bounds.size.x / 2;

                else
                    doorOffset = 0;

                door.transform.position = new Vector3(doorOffset, roomBounds.extents.y + door.GetComponent<BoxCollider2D>().bounds.extents.y, -1);
            }

            else if (doorProperties[i].wall == 1)
            {
                //Position Order - Center, Left, Right
                if (doorProperties[i].position == 1)
                    doorOffset = roomBounds.min.x + door.GetComponent<BoxCollider2D>().bounds.size.x / 2;

                else if (doorProperties[i].position == 2)
                    doorOffset = roomBounds.max.x - door.GetComponent<BoxCollider2D>().bounds.size.x / 2;

                 else
                    doorOffset = 0;

                door.transform.position = new Vector3(doorOffset, -roomBounds.extents.y - door.GetComponent<BoxCollider2D>().bounds.extents.y, -1);
            }
            	
            else if (doorProperties[i].wall == 2)
            {
                //Position Order - Bottom, Center, Top
                if (doorProperties[i].position == 2)
					doorOffset = roomBounds.max.y - door.GetComponent<BoxCollider2D>().bounds.size.y / 2 - door.GetComponent<BoxCollider2D>().offset.y;

                else if (doorProperties[i].position == 0)
					doorOffset = roomBounds.min.y + door.GetComponent<BoxCollider2D>().bounds.size.y / 2 - door.GetComponent<BoxCollider2D>().offset.y;

                else
					doorOffset =  -door.GetComponent<BoxCollider2D>().offset.y;

				door.transform.position = new Vector3(-roomBounds.extents.x - door.GetComponent<BoxCollider2D>().bounds.extents.x, doorOffset, -1);
            }
            	
            else if (doorProperties[i].wall == 3)
            {
                 if (doorProperties[i].position == 2)
					doorOffset = roomBounds.max.y - door.GetComponent<BoxCollider2D>().bounds.size.y / 2 - door.GetComponent<BoxCollider2D>().offset.y;

                else if (doorProperties[i].position == 0)
					doorOffset = roomBounds.min.y + door.GetComponent<BoxCollider2D>().bounds.size.y / 2 - door.GetComponent<BoxCollider2D>().offset.y;

                 else
					doorOffset =  -door.GetComponent<BoxCollider2D>().offset.y;

				door.transform.position = new Vector3(roomBounds.extents.x + door.GetComponent<BoxCollider2D>().bounds.extents.x, doorOffset, -1);
            }
        }
    }

    void CreateWalls(GameObject r)
	{
        Bounds roomBounds = r.GetComponent<Collider2D>().bounds;

        List<GameObject> doors = new List<GameObject>();

        if (r.transform.FindChild("Doors"))
        {
            foreach (Transform child in r.transform.FindChild("Doors"))
                doors.Add(child.gameObject);
        }

        GameObject wallsParent = new GameObject("Walls");
        wallsParent.transform.SetParent(r.transform);
        wallsParent.transform.localScale =  new Vector3(1 , 1 , 1);

        Object wallPrefab = Resources.Load ("Wall");
        Sprite wallSprite = Resources.Load<Sprite>("Blue Metal Wall");
		Sprite wallEndSprite = Resources.Load<Sprite>("Blue Metal Wall Corner");
		Sprite consoleCornerSprite = Resources.Load<Sprite>("CV Corner");

        //CREATE CORNERS
        //Corners are used for the placement and sizing of walls. 
        //Corners on walls that are not selected are deleted after walls are created 

        topLeftCorner = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
        topLeftCorner.GetComponent<SpriteRenderer>().sprite = wallEndSprite;
		topLeftCorner.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = consoleCornerSprite;
        topLeftCorner.name = "Wall Top-Left Corner";
        topLeftCorner.transform.SetParent(wallsParent.transform);
        topLeftCorner.transform.position = new Vector2(r.transform.position.x -roomBounds.extents.x - 0.5f, r.transform.position.y + roomBounds.extents.y + 0.5f);
        topLeftCorner.transform.localScale = new Vector3(1 / r.transform.localScale.x, 1 / r.transform.localScale.y, 1);
       
        topRightCorner = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
        topRightCorner.GetComponent<SpriteRenderer>().sprite = wallEndSprite;
		topRightCorner.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = consoleCornerSprite;
        topRightCorner.name = "Wall Top-Right Corner";
        topRightCorner.transform.SetParent(wallsParent.transform);
        topRightCorner.transform.position = new Vector2(r.transform.position.x + roomBounds.extents.x + 0.5f, r.transform.position.y + roomBounds.extents.y + 0.5f);
        topRightCorner.transform.localScale = new Vector3(-1 / r.transform.localScale.x, 1 / r.transform.localScale.y, 1);
       
        bottomLeftCorner = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
        bottomLeftCorner.GetComponent<SpriteRenderer>().sprite = wallEndSprite;
		bottomLeftCorner.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = consoleCornerSprite;
        bottomLeftCorner.name = "Wall Bottom-Left Corner";
        bottomLeftCorner.transform.SetParent(wallsParent.transform);
        bottomLeftCorner.transform.position = new Vector2(r.transform.position.x - roomBounds.extents.x - 0.5f, r.transform.position.y - roomBounds.extents.y - 0.5f);
        bottomLeftCorner.transform.localScale = new Vector3(1 / r.transform.localScale.x, -1 / r.transform.localScale.y, 1);
        
        bottomRightCorner = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
        bottomRightCorner.GetComponent<SpriteRenderer>().sprite = wallEndSprite;
		bottomRightCorner.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = consoleCornerSprite;
		bottomRightCorner.name = "Wall Bottom-Right Corner";
        bottomRightCorner.transform.SetParent(wallsParent.transform);
        bottomRightCorner.transform.position = new Vector2(r.transform.position.x + roomBounds.extents.x + 0.5f, r.transform.position.y - roomBounds.extents.y - 0.5f);
        bottomRightCorner.transform.localScale = new Vector3(-1 / r.transform.localScale.x, -1 / r.transform.localScale.y, 1);       

        //CREATE WALLS

        //Top Wall
        //Build wall if there is no aligned room selected or if room alignment is not bottom
        if (!roomToAlignTo ||  (roomToAlignTo && toolbarAlignToRoom !=1))
        {
            //Get walls on top and order by position
            GameObject[] orderedGameObjects = new GameObject[1];
            orderedGameObjects[0] = topLeftCorner;
            int objArrayPos = 1;

            if (doors.Count > 0)
            {
                //if door is on top wall add to array
                foreach (GameObject door in doors)
                {
                    if (door.transform.position.y > roomBounds.max.y)
                    {
                        System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
                        orderedGameObjects[objArrayPos] = door;
                        objArrayPos++;
                    }
                }
            }

            System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
            orderedGameObjects[objArrayPos] = topRightCorner;

            // Ordered from left to right
            orderedGameObjects = orderedGameObjects.OrderBy(go => go.transform.position.x).ToArray();

            if (debugWalls)
            {
                Debug.Log("Objects ordered from bottom to top");
                foreach (GameObject orderedObject in orderedGameObjects)
                    Debug.Log(orderedObject);
            }

            // Create walls in between doors and corners
            for (int i = 0; i < orderedGameObjects.Length - 1; i++)
            {
                float objAPosX = orderedGameObjects[i].GetComponent<Collider2D>().bounds.max.x;
                float objBPosX = orderedGameObjects[i + 1].GetComponent<Collider2D>().bounds.min.x;

                float difference = objBPosX - objAPosX;
                float midPoint = objAPosX + difference * 0.5f;

                if (debugWalls)
                {
                    Debug.Log("Distance between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + difference);
                    Debug.Log("Midpoint between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + midPoint);
                }

                // If there is space between doors or corners
                if (difference > 0)
                {
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
                    wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
                    wall.name = "Top Wall " + (i+1);
                    wall.transform.SetParent(wallsParent.transform);
                    wall.transform.position = new Vector2(midPoint, r.transform.position.y + roomBounds.extents.y + 0.5f);
                    wall.transform.localScale = new Vector3(difference / r.transform.localScale.x, 1 / r.transform.localScale.y, 1);
                }
            }
        }

        // Bottom Wall
        if (!roomToAlignTo || (roomToAlignTo && toolbarAlignToRoom != 0))
        {
            GameObject[] orderedGameObjects = new GameObject[1];
            orderedGameObjects[0] = bottomRightCorner;
            int objArrayPos = 1;

            foreach (GameObject door in doors)
            {
                if (door.transform.position.y < roomBounds.min.y)
                {
                    System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
                    orderedGameObjects[objArrayPos] = door;
                    objArrayPos++;
                }
            }

            System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
            orderedGameObjects[objArrayPos] = bottomLeftCorner;

            orderedGameObjects = orderedGameObjects.OrderBy(go => go.transform.position.x).ToArray();

            if (debugWalls)
            {
                Debug.Log("Objects ordered from bottom to top");
                foreach (GameObject orderedObject in orderedGameObjects)
                    Debug.Log(orderedObject);
            }

            // Create walls in between doors and corners
            for (int i = 0; i < orderedGameObjects.Length - 1; i++)
            {
                float objAPosX = orderedGameObjects[i].GetComponent<Collider2D>().bounds.max.x;
                float objBPosX = orderedGameObjects[i + 1].GetComponent<Collider2D>().bounds.min.x;

                float difference = objBPosX - objAPosX;
                float midPoint = objAPosX + difference * 0.5f;

                if (debugWalls)
                {
                    Debug.Log("Distance between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + difference);
                    Debug.Log("Midpoint between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + midPoint);
                }

                // If there is space between doors or corners
                if (difference > 0)
                {
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
                    wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
                    wall.name = "Bottom Wall " + (i + 1);
                    wall.transform.SetParent(wallsParent.transform);
                    wall.transform.position = new Vector2(midPoint, r.transform.position.y - roomBounds.extents.y - 0.5f);
                    wall.transform.localScale = new Vector3(difference / r.transform.localScale.x, -1 / r.transform.localScale.y, 1);
                }
            }
        }

        // Left Wall
        if (!roomToAlignTo || (roomToAlignTo && toolbarAlignToRoom != 3))
        {
            GameObject[] orderedGameObjects = new GameObject[1];
            orderedGameObjects[0] = topLeftCorner;

            int objArrayPos = 1;

            foreach (GameObject door in doors)
            {
                if (door.transform.position.x < roomBounds.min.x)
                {
                    System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
                    orderedGameObjects[objArrayPos] = door;
                    objArrayPos++;
                }
            }

            System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
            orderedGameObjects[objArrayPos] = bottomLeftCorner;
            
            // Ordered from bottom to top
            orderedGameObjects = orderedGameObjects.OrderBy(go => go.transform.position.y).ToArray();
            
            if (debugWalls)
            {
                Debug.Log("Objects ordered from bottom to top");
                foreach (GameObject orderedObject in orderedGameObjects)
                    Debug.Log(orderedObject);
            }

            // Create walls in between doors and corners
            for (int i = 0; i < orderedGameObjects.Length - 1; i++)
            {
                float objAPosY = orderedGameObjects[i].GetComponent<Collider2D>().bounds.max.y;
                float objBPosY = orderedGameObjects[i + 1].GetComponent<Collider2D>().bounds.min.y;

                float difference = objBPosY - objAPosY;
                float midPoint = objAPosY + difference * 0.5f;
                
                if (debugWalls)
                {
                    Debug.Log("Distance between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + difference);
                    Debug.Log("Midpoint between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + midPoint);
                }

                // If there is space between doors or corners
                if (difference > 0)
                {
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
                    wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
                    wall.name = "Left Wall " + (i + 1);
                    wall.transform.SetParent(wallsParent.transform);
                    wall.transform.Rotate(0, 0, -90);
                    wall.transform.position = new Vector2(r.transform.position.x - roomBounds.extents.x - 0.5f, midPoint);
                    wall.transform.localScale = new Vector3(difference / r.transform.localScale.x, 1 / r.transform.localScale.y, 1);
                }
            }
        }

        // Right Wall
        if (!roomToAlignTo || (roomToAlignTo && toolbarAlignToRoom != 2))
        {
            GameObject[] orderedGameObjects = new GameObject[1];
            orderedGameObjects[0] = topRightCorner;
            int objArrayPos = 1;

            foreach (GameObject door in doors)
            {
                if (door.transform.position.x > roomBounds.max.x)
                {
                    System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
                    orderedGameObjects[objArrayPos] = door;
                    objArrayPos++;
                }
            }

            System.Array.Resize(ref orderedGameObjects, orderedGameObjects.Length + 1);
            orderedGameObjects[objArrayPos] = bottomRightCorner;

            // Ordered from top to bottom
            orderedGameObjects = orderedGameObjects.OrderBy(go => go.transform.position.y).ToArray();

            if (debugWalls)
            {
                Debug.Log("Objects ordered from bottom to top");
                foreach (GameObject orderedObject in orderedGameObjects)
                    Debug.Log(orderedObject);
            }

            // Create walls in between doors and corners
            for (int i = 0; i < orderedGameObjects.Length - 1; i++)
            {
                float objAPosY = orderedGameObjects[i].GetComponent<Collider2D>().bounds.max.y;
                float objBPosY = orderedGameObjects[i + 1].GetComponent<Collider2D>().bounds.min.y;

                float difference = objBPosY - objAPosY;
                float midPoint = objAPosY + difference * 0.5f;

                if (debugWalls)
                {
                    Debug.Log("Distance between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + difference);
                    Debug.Log("Midpoint between " + orderedGameObjects[i] + " + " + orderedGameObjects[i + 1] + " = " + midPoint);
                }

                // If there is space between doors or corners
                if (difference > 0)
                {
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab);
                    wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
                    wall.name = "Right Wall " + (i + 1);
                    wall.transform.SetParent(wallsParent.transform);
                    wall.transform.Rotate(0, 0, 90);
                    wall.transform.localScale = new Vector3(difference / r.transform.localScale.x, 1 / r.transform.localScale.y, 1);
                    wall.transform.position = new Vector2(r.transform.position.x + roomBounds.extents.x + 0.5f, midPoint);
                }
            }
        }

        // Delete corners on walls that are not selected
        // Top Wall
        if (roomToAlignTo && toolbarAlignToRoom == 1)
        {
            GameObject.DestroyImmediate(topLeftCorner);
            GameObject.DestroyImmediate(topRightCorner);
        }

        // Bottom Wall
        if (roomToAlignTo && toolbarAlignToRoom == 0)
        {
            GameObject.DestroyImmediate(bottomLeftCorner);
            GameObject.DestroyImmediate(bottomRightCorner);
        }

        // Left Wall
        if (roomToAlignTo && toolbarAlignToRoom == 3)
        {
            GameObject.DestroyImmediate(topLeftCorner);
            GameObject.DestroyImmediate(bottomLeftCorner);
        }

        //Right Wall
        if (roomToAlignTo && toolbarAlignToRoom == 2)
        {
            GameObject.DestroyImmediate(topRightCorner);
            GameObject.DestroyImmediate(bottomRightCorner);
        }
    }

    //EDITOR FUNCTIONS

    void OnSelectionChange()
    {
        //Room Selection
        if (Selection.activeGameObject)
        {
            // Check if object has parent
            if (Selection.activeGameObject.transform.parent != null)
            {
                //Check if root parent is Level
                if (Selection.activeGameObject.transform.parent.root.name == "Level")
                {
                    selectedRoom = Selection.activeGameObject;

                    //Jump up through parents until Room is found
                    while (selectedRoom.tag != "Room")
                        selectedRoom = selectedRoom.transform.parent.gameObject;
                }
            }
        }

        else
            selectedRoom = null;

        Repaint();
    }

    //DISPLAY ROOM PROPERTIES
    void DispaySelectedRoom(GameObject r)
    {
        GUILayout.Space(15);

        //RoomProperties roomProperties = room.GetComponent<RoomProperties>();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected Room", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Room Name: " + selectedRoom.name);

        //ReBuild Walls Button
        if (r.transform.FindChild("Walls"))
        {
            if (GUILayout.Button("ReBuild Walls"))
            {
                //Destroy old walls
                GameObject.DestroyImmediate(r.transform.FindChild("Walls").gameObject);

                CreateWalls(r);
            }
        }

        //Build Walls Button
        else
        {

            if (GUILayout.Button("Build Walls"))
                CreateWalls(r);
        }

        //Delete Room Button
        if (GUILayout.Button("Delete Selected Room"))
            DeleteRoom();

        GUILayout.EndVertical();
    }

    public void DeleteRoom()
    {
        if(selectedRoom != null)
        {
            GameObject roomToBeDestroyed = selectedRoom;

            if (automaticRoomNumbering)
            {
                foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
                {
                    if (roomToBeDestroyed.name.Contains("Room"))
                    {
                        if (room.name.Contains("Room"))
                        {
                            if (room.GetComponent<RoomProperties>().roomNum > roomToBeDestroyed.GetComponent<RoomProperties>().roomNum)
                            {
                                //Change room number
                                room.name = "Room " + (room.GetComponent<RoomProperties>().roomNum - 1);
                                room.GetComponent<RoomProperties>().roomNum--;
                            }
                        }
                    }

                    else if (roomToBeDestroyed.name.Contains("Corridor"))
                    {
                        if (room.name.Contains("Corridor"))
                        {
                            if (room.GetComponent<RoomProperties>().roomNum > roomToBeDestroyed.GetComponent<RoomProperties>().roomNum)
                            {
                                //Change room number
                                room.name = "Corridor " + (room.GetComponent<RoomProperties>().roomNum - 1);
                                room.GetComponent<RoomProperties>().roomNum--;
                            }
                        }
                    }
                }

                if (roomTypeSelection == 0) //"Room"
                    roomNum--;

                else if (roomTypeSelection == 1)//"Corridor"
                    corridorNum--;
            }

            //Destroy Room
            GameObject.DestroyImmediate(selectedRoom);
        }
    }

    void setDefaultFields()
    {
        roomName = "";
        roomTypeSelection = 0;
        roomNumSelection = 0;
        roomBackground = 0;
        roomWidth = 1;
        roomHeight = 1;
        doorCount = 0;
    }

    void SaveEditorPrefs()
    {
        EditorPrefs.SetInt("RoomNum", roomNum);
        EditorPrefs.SetInt("CorridorNum", corridorNum);
    }

    void LoadEditorPrefs()
    {
        roomNum = EditorPrefs.GetInt("RoomNum");
        corridorNum = EditorPrefs.GetInt("CorridorNum");
    }

    void ResetRoomNumbering()
    {
        roomNum = 1;
        corridorNum = 1;
    }
}
