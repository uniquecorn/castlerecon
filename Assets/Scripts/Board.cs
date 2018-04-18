using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Board : CastleObject
{
	public Slot[,] slots;
	public SpriteRenderer boardRenderer;
	public Transform slotTransform;

	private float spawnTimer;
	private bool spawning;

	public int defenceUsedSlots;
	public int offenceUsedSlots;

	public bool active;
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		Spawn();
		
		//CreateBoard();
	}
	public void Spawn()
	{
		//GameManager.instance.board = this;
		coll.enabled = false;
		boardRenderer.size = new Vector2(0, 0);
		spawning = true;
		spawnTimer = 0;
	}

	public void CreateBoard(int x = 5, int y = 2)
	{
		boardRenderer.size = new Vector2(5.15f, 3.2f);
		slots = new Slot[x, y];
		for(int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				Slot tempSlot = Instantiate(GameManager.instance.slotPrefab.gameObject,new Vector3(i-2,(float)j * 1.55f - 0.775f,0),Quaternion.identity).GetComponent<Slot>();
				slots[i, j] = tempSlot;
				tempSlot.x = i;
				tempSlot.y = j;
				if(j == 0)
				{
					tempSlot.affliation = GameManager.Affliation.OFFENCE;
				}
				else
				{
					tempSlot.affliation = GameManager.Affliation.DEFENCE;
				}
				tempSlot.transform.SetParent(slotTransform);
			}
		}
		SetView();
	}

	public void SetView()
	{
		switch(GameManager.instance.turn)
		{
			case GameManager.Affliation.DEFENCE:
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						if (slots[i, j].affliation == GameManager.Affliation.OFFENCE)
						{
							slots[i, j].displayMode = Slot.DisplayMode.DISABLED;
						}
					}
				}
				break;
			case GameManager.Affliation.OFFENCE:
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						slots[i, j].HideSlot();
						if (slots[i, j].affliation == GameManager.Affliation.OFFENCE)
						{
							slots[i, j].displayMode = Slot.DisplayMode.IDLE;
						}
					}
				}
				break;
			case GameManager.Affliation.BREAKDOWN:
				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						slots[i, j].ShowSlot();
					}
				}
				break;
		}
	}

	public void ApplyEffects()
	{
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				slots[i, j].ApplyEffects();
			}
		}
	}

	public void ModifyDefenceSlot(bool reduce = false)
	{
		if(reduce)
		{
			defenceUsedSlots--;
			if (defenceUsedSlots < 5)
			{
				GameManager.instance.showConfirm = false;
			}
		}
		else
		{
			defenceUsedSlots++;
			if(defenceUsedSlots == 5)
			{
				GameManager.instance.showConfirm = true;
			}
		}
	}

	public void ModifyOffenceSlot(bool reduce = false)
	{
		if (reduce)
		{
			offenceUsedSlots--;
			if (offenceUsedSlots < 5)
			{
				GameManager.instance.showConfirm = false;
			}
		}
		else
		{
			offenceUsedSlots++;
			if (offenceUsedSlots == 5)
			{
				GameManager.instance.showConfirm = true;
			}
		}
	}

	void SpawnBoard()
	{
		spawnTimer += Time.deltaTime * 5;
		if (spawnTimer < 1)
		{
			boardRenderer.size = Vector2.Lerp(new Vector2(0, 0), new Vector2(5.2f, 1), spawnTimer);
		}
		else if (spawnTimer < 2)
		{
			boardRenderer.size = Vector2.Lerp(new Vector2(5.2f, 1), new Vector2(5.15f, 3.2f), spawnTimer - 1);
		}
		else
		{
			spawnTimer = 0;
			spawning = false;
			CreateBoard();
		}
	}

	void RetireBoard()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(spawning)
		{
			SpawnBoard();
		}
		else
		{
			
		}
	}
}
