using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[HideInInspector]
	public CastleObject selectedObject;
	[HideInInspector]
	public CastleObject hoveredObject;

	public Slot slotPrefab;
	public GameObject gemSlotPrefab;

	[System.Serializable]
	public struct EffectStyle
	{
		public Unit.ActionType actionType;
		public Color effectColor;
		public Sprite effectSprite;
		public bool showValue;
	}

	public EffectStyle[] effectStyles;

	public Sprite hiddenSprite;

	public Board board;
	public GemSlot[] gemSlots;
	public int gemsAvailable;

	public GameObject boardPrefab;

	public enum GameState
	{
		DEFENCE,
		OFFENCE,
		DONE
	}
	public GameState gameState;

	public static GameManager instance;
	// Use this for initialization
	void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CreateBoard();
	}
	
	void CreateBoard()
	{
		board = Instantiate(boardPrefab, Vector3.zero, Quaternion.identity).GetComponent<Board>();
	}

	public EffectStyle GetStyle(Unit.ActionType _actionType)
	{
		for (int i = 0; i < effectStyles.Length; i++)
		{
			if (effectStyles[i].actionType == _actionType)
			{
				return effectStyles[i];
			}
		}
		print("No style found!");
		return effectStyles[0];
	}

	public void HideDefence()
	{
		board.HideDefence();
	}

	public bool UseGems(int gems = 1)
	{
		if(gemsAvailable >= gems)
		{
			for(int i = (3 - gemsAvailable); i < (3 - gemsAvailable + gems); i++)
			{
				gemSlots[i].SetGem(GemSlot.GemState.INACTIVE);
			}
			gemsAvailable -= gems;
			return true;
		}
		else
		{
			return false;
		}
	}
	public void AddGems(int gems = 1)
	{
		for (int i = 0; i < gems; i++)
		{
			gemsAvailable++;
			print((3 - gemsAvailable));
			gemSlots[(3 - gemsAvailable)].SetGem(GemSlot.GemState.ACTIVE);
		}
	}

	void StartHover(Collider2D coll)
	{
		if(hoveredObject)
		{
			hoveredObject.ExitHover();
		}
		hoveredObject = coll.GetComponent<CastleObject>();
		hoveredObject.EnterHover();
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
								StartHover(colls[0]);
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
							StartHover(colls[0]);
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
						StartHover(colls[chosenColl]);
					}
				}
				
			}
		}
		else
		{
			if (colls.Length <= 0)
			{
				//uh oh
			}
			else if (colls.Length == 1)
			{
				if (selectedObject)
				{
					if(colls[0] != selectedObject.coll)
					{
						StartHover(colls[0]);
					}
				}
				else
				{
					StartHover(colls[0]);
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
				StartHover(colls[chosenColl]);
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
		if(Input.GetKeyDown("x"))
		{
			board.HideDefence();
		}
	}
}