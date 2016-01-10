using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class PlatformBuilder : EditorWindow
{
    GameObject selectedPlatform, selectedRoom;

    float platformWidth, minPlatformSize = 1;
    int selectedPlatformArt;
    int orientation = 0;

    Sprite platformSprite, platformEndSprite;

    bool platformEndLeft = true, platformEndRight = true;
    bool doorEndLeft, doorEndRight;

    int doorCount;

    //GUI
    GUIStyle boldFont = new GUIStyle();
    GUIStyle boldItalicFont = new GUIStyle();

    float maxFieldWidth = 180;

    // Adds menu item named "Platform Builder" to the NMA Tool menu
    [MenuItem("NMA Tools/Platform Builder")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.        
        EditorWindow platformBuilder = EditorWindow.GetWindow(typeof(PlatformBuilder), false, "Platform Builder", false);
		platformBuilder.minSize = new Vector2(275, 400); 
    }

    void OnEnable()
    {
        boldFont.fontStyle = FontStyle.Bold;
        boldItalicFont.fontStyle = FontStyle.BoldAndItalic;
    }

    void OnGUI()
    {
        //Start Main Layout
        GUILayout.BeginVertical();

        //Platform Orientation Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Orientation", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Platform Orientation Layout
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Orientation:");
        string[] orientationOptions = new string[] { "Horizontal", "Vertical" };
        orientation = EditorGUILayout.Popup(orientation, orientationOptions, GUILayout.MaxWidth(maxFieldWidth));
        GUILayout.EndHorizontal();

        //Platform Size Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        
        if(orientation == 0)
            GUILayout.Label("Width", boldFont);

        else
            GUILayout.Label("Height", boldFont);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Platform Size Layout
        GUILayout.BeginVertical("box");
        GUILayout.BeginHorizontal();

        if (orientation == 0)
            GUILayout.Label("Width:");

        else
            GUILayout.Label("Height:");

        platformWidth = EditorGUILayout.FloatField(platformWidth, GUILayout.MaxWidth(maxFieldWidth));
        GUILayout.EndHorizontal();

        if (orientation == 0)
        {
            if (platformEndLeft && platformEndRight)
                minPlatformSize = (8.6f * doorCount) + 2;

            else if (platformEndLeft && !platformEndRight)
                minPlatformSize = (8.6f * doorCount) + 1;

            else if (!platformEndLeft && platformEndRight)
                minPlatformSize = (8.6f * doorCount) + 1;

            else
            {
                if (doorCount > 0)
                    minPlatformSize = (8.6f * doorCount);

                else
                    minPlatformSize = 1;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Min Platform Width = " + minPlatformSize);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
        }

        else
        {
            if (doorCount > 0)
                minPlatformSize = (8.3f * doorCount);

            else minPlatformSize = 1;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Min Platform Height = " + minPlatformSize);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        //Platform Art Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Art", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Platform Art layout
        GUILayout.BeginHorizontal("box");
        string[] platformArtList = new string[] { "Blue Metal" };
        GUILayout.Label("Art:");
        selectedPlatformArt = EditorGUILayout.Popup(selectedPlatformArt, platformArtList, GUILayout.MaxWidth(maxFieldWidth));
        GUILayout.EndHorizontal();

        if (orientation == 0)
        {
            //Platform End Label
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Platform Ends", boldFont);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Platform End Layout
            GUILayout.BeginHorizontal("box");
            platformEndLeft = GUILayout.Toggle(platformEndLeft, "Left");
            platformEndRight = GUILayout.Toggle(platformEndRight, "Right");
            GUILayout.EndHorizontal();
        }

        //Doors Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Doors", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //Doors Layout
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("No. of Doors");
        string[] doorCountList = new string[] { "0", "1", "2", "3", "4"};
        doorCount = EditorGUILayout.Popup(doorCount, doorCountList, GUILayout.MaxWidth(maxFieldWidth));

        GUILayout.EndHorizontal();

        //Build Platform Label
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Build Platform", boldFont, GUILayout.Width(80));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Parent to:", boldFont);

        if (selectedRoom == null)
            GUILayout.Label("No room selected", GUILayout.MaxWidth(maxFieldWidth));

        else
            GUILayout.Label(selectedRoom.name, GUILayout.MaxWidth(maxFieldWidth));

        GUILayout.EndHorizontal();

        if (selectedRoom != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Room Dimensions:", boldFont);
            GUILayout.Label(selectedRoom.GetComponent<Collider2D>().bounds.size.x.ToString() + " x "
                + selectedRoom.GetComponent<Collider2D>().bounds.size.y.ToString(), GUILayout.MaxWidth(maxFieldWidth));

            GUILayout.EndHorizontal();
        }
        
        if (platformWidth < minPlatformSize - 0.1f)//!!! Why???
            GUILayout.Label("Platform width must be greater than min platform width");

        else
        {
            //Build Room Button
            GUILayout.BeginHorizontal();
            //Build Room Button
            if (GUILayout.Button("Build"))
                BuildPlatform(true);

            //Build Room W/O Walls Button
            if (GUILayout.Button("Build w/o mid-sections"))
                BuildPlatform(false);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();

        //Selected Room
        if (Selection.activeGameObject)
        {
            if (selectedPlatform)
                DispaySelectedPlatform(selectedPlatform);
        }

        //End Main Layout
        GUILayout.EndVertical();
    }

    void BuildPlatform(bool buildMidSections)
    {
        //Set Art
        switch (selectedPlatformArt)
        {
            case 0:
            platformEndSprite = Resources.Load<Sprite>("Blue Metal Platform_H_End");
            break;
            default:
            break;
        }

        //Create Parent Object
        GameObject platformParent = new GameObject();
        platformParent.name = "Platform";

        //Build Platform Ends
        GameObject endLeft = null, endRight = null, endTop = null, endBottom = null;

		//For horizontal platforms.
        if (orientation == 0)
        {
            Object platformHPrefab = Resources.Load("PlatformH");

            if (platformEndLeft)
            {
                endLeft = (GameObject)PrefabUtility.InstantiatePrefab(platformHPrefab);
                endLeft.name = "Platform End Left";
                endLeft.transform.SetParent(platformParent.transform);
                endLeft.transform.localPosition = new Vector2((-platformWidth / 2) + 0.5f, 0);
                endLeft.transform.FindChild("Platform").GetComponent<SpriteRenderer>().sprite = platformEndSprite;
                endLeft.transform.FindChild("Platform Lit").GetComponent<SpriteRenderer>().sprite = platformEndSprite;
                endLeft.transform.FindChild("Platform Lit").GetComponent<SpriteRenderer>().sprite = platformEndSprite;
            }

            else
            {
                endLeft = new GameObject();
                endLeft.name = "Platform End Left";
                endLeft.tag = "Platform";
                endLeft.transform.SetParent(platformParent.transform);
                endLeft.transform.localPosition = new Vector2((-platformWidth / 2), 0);
            }

            if (platformEndRight)
            {
                endRight = (GameObject)PrefabUtility.InstantiatePrefab(platformHPrefab);
                endRight.name = "Platform End Right";
                endRight.transform.SetParent(platformParent.transform);
                endRight.transform.localPosition = new Vector2((platformWidth / 2) - 0.5f, 0);
                endRight.transform.localScale = new Vector3(-1, 1, 1);
                endRight.transform.FindChild("Platform").GetComponent<SpriteRenderer>().sprite = platformEndSprite;
                endRight.transform.FindChild("Platform Lit").GetComponent<SpriteRenderer>().sprite = platformEndSprite;
            }

            else
            {
                endRight = new GameObject();
                endRight.name = "Platform End Right";
                endRight.tag = "Platform";
                endRight.transform.SetParent(platformParent.transform);
                endRight.transform.localPosition = new Vector2((platformWidth / 2), 0);
            }
        }

        // For vertical platforms.
        else
        {
            endTop = new GameObject();
            endTop.name = "Platform End Top";
            endTop.tag = "Platform";
            endTop.transform.SetParent(platformParent.transform);
            endTop.transform.localPosition = new Vector2((-platformWidth / 2), 0);

            endBottom = new GameObject();
            endBottom.name = "Platform End Bottom";
            endBottom.tag = "Platform";
            endBottom.transform.SetParent(platformParent.transform);
            endBottom.transform.localPosition = new Vector2((platformWidth / 2), 0);
        }

        //Build Doors
        if (doorCount > 0)
        {
            // For horizontal doors.
            if (orientation == 0)
            {
                Object interiorDoorHPrefab = Resources.Load("InteriorDoorH");

                //Create Parent Object
                GameObject doorsParent = new GameObject("Doors");
                doorsParent.transform.SetParent(platformParent.transform);

                for (int i = 0; i < doorCount; i++)
                {
                    GameObject d = (GameObject)PrefabUtility.InstantiatePrefab(interiorDoorHPrefab);
                    d.name = "InteriorDoorH" + (i + 1);
                    d.transform.SetParent(doorsParent.transform);
                    d.transform.localPosition = new Vector3(0, 0, -1);
                }
            }

			// For vertical doors.
            else
            {
                Object interiorDoorVPrefab = Resources.Load("InteriorDoorV");

                //Create Parent Object
                GameObject doorsParent = new GameObject("Doors");
                doorsParent.transform.SetParent(platformParent.transform);

                for (int i = 0; i < doorCount; i++)
                {
                    GameObject d = (GameObject)PrefabUtility.InstantiatePrefab(interiorDoorVPrefab);
                    d.name = "InteriorDoorV" + (i + 1);
                    d.transform.SetParent(doorsParent.transform);
                    d.transform.localPosition = new Vector3(0, 0, -1);
                    d.transform.Rotate(0, 0, 90);
                }
            }
        }

        List<GameObject> doors = new List<GameObject>();

        if (platformParent.transform.FindChild("Doors"))
        {
            foreach (Transform child in platformParent.transform.FindChild("Doors"))
                doors.Add(child.gameObject);
        }

        // Order doors and platform ends
        GameObject[] orderedObjects = new GameObject[0];
        int objArrayPos = 0;

        //Add Platform Ends to ordered array

        if (orientation == 0)
        {
            System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
            orderedObjects[objArrayPos] = endLeft;
            objArrayPos++;

            System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
            orderedObjects[objArrayPos] = endRight;
            objArrayPos++;
        }

        else
        {
            System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
            orderedObjects[objArrayPos] = endTop;
            objArrayPos++;

            System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
            orderedObjects[objArrayPos] = endBottom;
            objArrayPos++;
        }

        //Add doors to ordered array
        if (doors.Count > 0)
        {
            foreach (GameObject door in doors)
            {
                System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
                orderedObjects[objArrayPos] = door;
                objArrayPos++;
            }
        }

        //Order array from left to right
        orderedObjects = orderedObjects.OrderBy(go => go.transform.position.x).ToArray();

        //Get leftmost and rightmost objects
        GameObject objectLeft = orderedObjects[0];
        GameObject objectRight = orderedObjects[orderedObjects.Length - 1];

        float start, end;

        if (objectLeft.GetComponent<Collider2D>())
            start = objectLeft.GetComponent<Collider2D>().bounds.max.x;

        else
            start = objectLeft.transform.position.x;

        if (objectRight.GetComponent<Collider2D>())
            end = objectRight.GetComponent<Collider2D>().bounds.min.x;

        else
            end = objectRight.transform.position.x;

        float difference = end - start;

        //Remove End objects from array
        List<GameObject> o = orderedObjects.ToList();
        o.RemoveAt(0);
        o.RemoveAt(o.Count - 1);

        orderedObjects = o.ToArray();

        ////Position Doors and Ends
        foreach (GameObject orderedObject in orderedObjects)
        {
            float objectSizeX = orderedObject.GetComponent<Collider2D>().bounds.size.x;
            float objectExtentsX = orderedObject.GetComponent<Collider2D>().bounds.extents.x;
            float totalObjectLength = objectSizeX * orderedObjects.Length;
            float leftOverSpace = difference - totalObjectLength;

            if (orientation == 0)
            {
                orderedObject.transform.position = new Vector3(start + objectExtentsX + (leftOverSpace / (orderedObjects.Length + 1)), orderedObject.transform.position.y, orderedObject.transform.position.z);
                start = orderedObject.GetComponent<Collider2D>().bounds.max.x;
            }

            else
            {
                orderedObject.transform.position = new Vector3(start + objectExtentsX + 0.15f + (leftOverSpace / (orderedObjects.Length + 1)), orderedObject.transform.position.y, orderedObject.transform.position.z);
                start = orderedObject.GetComponent<Collider2D>().bounds.max.x;
            }
        }

        if (orientation == 1)
            platformParent.transform.Rotate(0, 0, -90);

        if (buildMidSections)
            BuildPlatformMidSections(platformParent);

        //Parent to room
        if (selectedRoom)
        {
            platformParent.transform.SetParent(selectedRoom.transform);
            platformParent.transform.localPosition = new Vector3(0, 0, 0);
        }


        Selection.activeGameObject = platformParent;
    }

    //Build Mid Sections
    void BuildPlatformMidSections(GameObject platformParent)
    {
        if (orientation == 1)
            platformParent.transform.Rotate(0, 0, 90);

        GameObject platformMidSections = new GameObject("Platform Mid Sections");
        platformMidSections.transform.SetParent(platformParent.transform);
        platformMidSections.transform.localPosition = new Vector3(0, 0, 0);

        // Create Ordered Array
        GameObject[] orderedObjects = new GameObject[0];
        int objArrayPos = 0;

        foreach(Transform child in platformParent.transform)
        {
            if (child.gameObject.tag == "Platform")
            {
                System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
                orderedObjects[objArrayPos] = child.gameObject;
                objArrayPos++;
            }
        }

        if (platformParent.transform.FindChild("Doors"))
        {
            foreach (Transform child in platformParent.transform.FindChild("Doors"))
            {
                if (child.gameObject.tag == "Door")
                {
                    System.Array.Resize(ref orderedObjects, orderedObjects.Length + 1);
                    orderedObjects[objArrayPos] = child.gameObject;
                    objArrayPos++;
                }
            }
        }

        //Sort Array
        orderedObjects = orderedObjects.OrderBy(go => go.transform.position.x).ToArray();

        //Create platforms in between doors and ends
        for (int i = 0; i < orderedObjects.Length - 1; i++)
        {
            float objAPosX, objBPosX;

            if (orderedObjects[i].GetComponent<Collider2D>())
                objAPosX = orderedObjects[i].GetComponent<Collider2D>().bounds.max.x;

            else
                objAPosX = orderedObjects[i].transform.position.x;

            if (orderedObjects[i + 1].GetComponent<Collider2D>())
                objBPosX = orderedObjects[i + 1].GetComponent<Collider2D>().bounds.min.x;

            else
                objBPosX = orderedObjects[i + 1].transform.position.x;

            float distance = objBPosX - objAPosX;
            float midPoint = objAPosX + distance * 0.5f;

            //If there is space between doors or corners
            Object platformPrefab;

            if (distance > 0)
            {
                if(orientation == 0)
                    platformPrefab = Resources.Load("PlatformH");

                else
                    platformPrefab = Resources.Load("PlatformV");


                GameObject platform = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
                platform.name = "PlatformMid " + (i + 1);
                platform.transform.SetParent(platformMidSections.transform);
                platform.transform.position = new Vector2(midPoint, orderedObjects[i].transform.position.y);
                platform.transform.localScale = new Vector3(distance, 1, 1);
            }
        }

        if (orientation == 1)
            platformParent.transform.Rotate(0, 0, -90);
    }

    void OnSelectionChange()
    {
        // Platform Selection
        if (Selection.activeGameObject)
        {
            // If selection is a platform
            if (Selection.activeGameObject.name == "Platform")
                selectedPlatform = Selection.activeGameObject;

            else
            {
                // Check if object has parent
                if (Selection.activeGameObject.transform.parent != null)
                {
                    // Check if root parent is Platform or Level
                    if (Selection.activeGameObject.transform.parent.root.name == "Platform" || Selection.activeGameObject.transform.parent.root.name == "Level")
                    {
                        selectedPlatform = Selection.activeGameObject;

                        if (Selection.activeGameObject.transform.parent.root.name == "Platform")
                        {
                            // Jump up through parents until Platform is found
                            while (selectedPlatform.name != "Platform")
                                selectedPlatform = selectedPlatform.transform.parent.gameObject;
                        }

                        else
                        {
                            bool platformFound = false;

                            while (selectedPlatform.name != "Level" && !platformFound)
                            {
                                selectedPlatform = selectedPlatform.transform.parent.gameObject;

                                if (selectedPlatform.name == "Platform" || selectedPlatform.name == "Level")
                                        platformFound = true;
                            }

                            if (selectedPlatform.name == "Level")
                                selectedPlatform = null;
                        }
                    }
                }
            }

            //Room Selection
            if (Selection.activeGameObject.transform.parent != null)
            {
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
        {
            selectedPlatform = null;
            selectedRoom = null;
        }

        Repaint();
    }

    void DispaySelectedPlatform(GameObject p)
    {
        GUILayout.Space(15);

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected Platform", boldFont);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box");

        GUILayout.Label("Platform Name: " + p.name);

        //ReBuild Walls Button
        if (p.transform.FindChild("Platform Mid Sections"))
        {
            if (GUILayout.Button("ReBuild Mid Sections"))
            {
                ////Destroy old Mid Sections
                GameObject.DestroyImmediate(p.transform.FindChild("Platform Mid Sections").gameObject);

                BuildPlatformMidSections(p);
            }
        }

        //Build Walls Button
        else
        {
            if (GUILayout.Button("Build Mid Sections"))
                BuildPlatformMidSections(p); 
        }

        //Delete Platform Button
        if (GUILayout.Button("Delete Selected Platform"))
        {
            GameObject.DestroyImmediate(selectedPlatform);
        }

        GUILayout.EndVertical();
    }
}
