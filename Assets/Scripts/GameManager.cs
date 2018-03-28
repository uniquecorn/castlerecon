using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public CastleObject selectedObject;
	public CastleObject hoveredObject;

	public Slot slotPrefab;

	public Color attackColor;
	public Color shieldColor;

	public Board board;

	public static GameManager instance;
	// Use this for initialization
	void Awake()
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 worldTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 touchPos = new Vector2(worldTouchPos.x, worldTouchPos.y);
		Collider2D[] colls = Physics2D.OverlapPointAll(touchPos);

		if(hoveredObject)
		{
			if (colls.Length <= 0)
			{
				hoveredObject.ExitHover();
				hoveredObject = null;
			}
			else
			{
				if(colls.Length == 1)
				{
					if (selectedObject)
					{
						if (colls[0] != selectedObject.coll)
						{
							if (hoveredObject.coll == colls[0])
							{
								hoveredObject.Hover();
							}
							else
							{
								hoveredObject.ExitHover();
								hoveredObject = colls[0].GetComponent<CastleObject>();
								hoveredObject.EnterHover();
							}
						}
						else
						{
							hoveredObject.ExitHover();
							hoveredObject = null;
						}
					}
					else
					{
						if (hoveredObject.coll == colls[0])
						{
							hoveredObject.Hover();
						}
						else
						{
							hoveredObject.ExitHover();
							hoveredObject = colls[0].GetComponent<CastleObject>();
							hoveredObject.EnterHover();
						}
					}
				}
				else
				{
					float closestDist = 99;
					int chosenColl = 0;
					for(int i = 0; i < colls.Length; i++)
					{
						if(colls[i].transform.position.z < closestDist)
						{
							if (selectedObject)
							{
								if (colls[i] != selectedObject.coll)
								{
									closestDist = colls[i].transform.position.z;
									chosenColl = i;
								}
							}
							else
							{
								closestDist = colls[i].transform.position.z;
								chosenColl = i;
							}
						}
					}
					if (hoveredObject.coll == colls[chosenColl])
					{
						hoveredObject.Hover();
					}
					else
					{
						hoveredObject.ExitHover();
						hoveredObject = colls[chosenColl].GetComponent<CastleObject>();
						hoveredObject.EnterHover();
					}
				}
				
			}
		}
		else
		{
			if (colls.Length <= 0)
			{

			}
			else if (colls.Length == 1)
			{
				if (selectedObject)
				{
					if(colls[0] != selectedObject.coll)
					{
						hoveredObject = colls[0].GetComponent<CastleObject>();
						hoveredObject.EnterHover();
					}
				}
				else
				{
					hoveredObject = colls[0].GetComponent<CastleObject>();
					hoveredObject.EnterHover();
				}
			}
			else
			{
				float closestDist = 99;
				int chosenColl = 0;
				for (int i = 0; i < colls.Length; i++)
				{
					if (colls[i].transform.position.z < closestDist)
					{
						if (selectedObject)
						{
							if(colls[i] != selectedObject.coll)
							{
								closestDist = colls[i].transform.position.z;
								chosenColl = i;
							}
						}
						else
						{
							closestDist = colls[i].transform.position.z;
							chosenColl = i;
						}
					}
				}
				hoveredObject = colls[chosenColl].GetComponent<CastleObject>();
				hoveredObject.EnterHover();
			}
		}
		if(selectedObject)
		{
			if (Input.GetMouseButton(0))
			{
				selectedObject.Hold(touchPos);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				selectedObject.Release(touchPos);
				selectedObject = null;
			}
		}
		else
		{
			if(hoveredObject)
			{
				if (Input.GetMouseButtonDown(0))
				{
					selectedObject = hoveredObject;
					hoveredObject.ExitHover();
					hoveredObject = null;
					selectedObject.Tap(touchPos);
				}
			}
		}
	}
}
