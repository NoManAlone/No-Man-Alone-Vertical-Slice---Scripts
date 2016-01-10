using UnityEngine;
using System.Collections;

public class MouseBehaviours : MonoBehaviour
{	
	RaycastHit2D hit;
	LayerMask switchableLayerMask;

	Texture2D cursor, cursorHighlighted;

	void Awake()
	{
		switchableLayerMask = LayerMask.GetMask("ConsoleInteraction");

		cursor = (Texture2D)Resources.Load("Cursor");
		cursorHighlighted = (Texture2D)Resources.Load("Cursor_Highlighted");
	}

	void Update()
	{
		CursorState();
	}

	void CursorState()
	{
        if (Camera.main)
        {
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)), Vector2.zero, 0, switchableLayerMask);

            if (hit)
                Cursor.SetCursor(cursorHighlighted, new Vector2(3, 14), CursorMode.Auto);
            else
                Cursor.SetCursor(cursor, new Vector2(14, 16), CursorMode.Auto);
        }
	}
}

