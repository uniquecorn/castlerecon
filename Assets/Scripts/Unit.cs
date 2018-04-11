using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : CastleObject
{
	public SpriteRenderer backingCircle;
	public SpriteMask backingMask;
	public SpriteRenderer unitRenderer;

	public Sprite unitSprite;
	public Sprite circle;
	public Sprite rectangle;

	public enum ActionType
	{
		ATTACK,
		SHIELD,
		MAGICDAMAGE
	}

	[System.Serializable]
	public class Action
	{
		public int xOffset;
		public int yOffset;
		public int value;
		public int immune;
		public ActionType actionType;
		[HideInInspector]
		public Slot.Affliation affliation;
	}

	public int health;
	public int gemCost;
	public Action[] actions;

	public Slot slot;

	public enum State
	{
		DRAGGED,
		PLACED
	}
	public State state;
	public GemSlot[] gemSlots;

	private float spawnTimer;

	public void Place(Slot _slot)
	{
		coll.enabled = false;
		backingCircle.sprite = rectangle;
		backingMask.sprite = rectangle;
		//backingMask.gameObject.SetActive(false);
		state = State.PLACED;
		slot = _slot;
		//slot.cardMask.gameObject.SetActive(true);
		//SetGems(GemSlot.GemState.ACTIVE);
		//slot.affliation = Slot.Affliation.OFFENCE;
		slot.unit = this;
		CommitActions();
		spawnTimer = 0;
	}
	public void Remove()
	{
		coll.enabled = true;
		RemoveActions();
		backingCircle.sprite = circle;
		backingCircle.transform.localScale = Vector3.one;
		backingMask.sprite = circle;
		//backingCircle.maskInteraction = SpriteMaskInteraction.None;
		spawnTimer = 0;
		//SetGems(GemSlot.GemState.INACTIVE);
		state = State.DRAGGED;
		//slot.cardMask.gameObject.SetActive(false);
		slot = null;
	}

	public void TestActions(Slot _slot)
	{
		for (int i = 0; i < actions.Length; i++)
		{
			TestAction(_slot,actions[i]);
		}
	}

	public void RemoveActions()
	{
		for (int i = 0; i < actions.Length; i++)
		{
			UndoAction(actions[i]);
		}
	}

	public void CommitActions()
	{
		for(int i = 0; i < actions.Length; i++)
		{
			DoAction(actions[i]);
		}
	}

	public void DoAction(Action _action)
	{
		Slot tempSlot = GetSlot(slot, _action.xOffset, _action.yOffset);
		_action.affliation = slot.affliation;
		tempSlot.DoAction(_action);
	}
	
	public void UndoAction(Action _action)
	{
		Slot tempSlot = GetSlot(slot,_action.xOffset, _action.yOffset);
		tempSlot.UndoAction(_action);
	}

	public void TestAction(Slot _slot, Action _action)
	{
		Slot tempSlot = GetSlot(_slot, _action.xOffset, _action.yOffset);
		tempSlot.Highlight(_action.actionType);
	}

	public Slot GetSlot(Slot _slot, int xOffset, int yOffset)
	{
		int x = _slot.x + xOffset;
		int y = _slot.y + yOffset;

		if (_slot.y == 1)
		{
			x = _slot.x - xOffset;
			y = _slot.y - yOffset;
		}

		if (x >= 5)
		{
			x -= 5;
		}
		else if(x < 0)
		{
			x += 5;
		}

		return GameManager.instance.board.slots[x, y];
	}
	public void Hide()
	{
		unitRenderer.sprite = GameManager.instance.hiddenSprite;
	}
	public void Show()
	{
		unitRenderer.sprite = unitSprite;
	}

	public void Spawn()
	{
		transform.localScale = Vector3.zero;
		CreateGemSlots();
		spawnTimer = 0;
	}

	public override void Tap(Vector2 pos)
	{
		base.Tap(pos);
	}

	public override void Hold(Vector2 pos)
	{
		base.Hold(pos);
		state = State.DRAGGED;
		if(GameManager.instance.hoveredObject is Slot && !((Slot)GameManager.instance.hoveredObject).unit)
		{
			switch (GameManager.instance.gameState)
			{
				case GameManager.GameState.DEFENCE:
					if (((Slot)GameManager.instance.hoveredObject).affliation == Slot.Affliation.DEFENCE)
					{
						TestActions((Slot)GameManager.instance.hoveredObject);
						transform.position = new Vector3(GameManager.instance.hoveredObject.transform.position.x, GameManager.instance.hoveredObject.transform.position.y, transform.position.z);
					}
					else
					{
						transform.position = new Vector3(pos.x, pos.y, transform.position.z);
					}
					break;
				case GameManager.GameState.OFFENCE:
					if (((Slot)GameManager.instance.hoveredObject).affliation == Slot.Affliation.OFFENCE)
					{
						TestActions((Slot)GameManager.instance.hoveredObject);
						transform.position = new Vector3(GameManager.instance.hoveredObject.transform.position.x, GameManager.instance.hoveredObject.transform.position.y, transform.position.z);
					}
					else
					{
						transform.position = new Vector3(pos.x, pos.y, transform.position.z);
					}
					break;
			}
		}
		else
		{
			transform.position = new Vector3(pos.x, pos.y, transform.position.z);
		}
	}

	public override void Release(Vector2 pos)
	{
		base.Release(pos);
		
		if (GameManager.instance.hoveredObject is Slot && !((Slot)GameManager.instance.hoveredObject).unit)
		{
			switch(GameManager.instance.gameState)
			{
				case GameManager.GameState.DEFENCE:
					if(((Slot)GameManager.instance.hoveredObject).affliation == Slot.Affliation.DEFENCE)
					{
						Place((Slot)GameManager.instance.hoveredObject);
					}
					else
					{
						Clear();
					}
					break;
				case GameManager.GameState.OFFENCE:
					if (((Slot)GameManager.instance.hoveredObject).affliation == Slot.Affliation.OFFENCE)
					{
						Place((Slot)GameManager.instance.hoveredObject);
					}
					else
					{
						Clear();
					}
					break;
			}
		}
		else
		{
			Clear();
		}
	}
	public void Clear()
	{
		Destroy(gameObject);
		Effects.instance.UseEffect("Poof", transform.position, null);
		RemoveGems();
	}
	public void RemoveGems()
	{
		GameManager.instance.AddGems(gemCost);
	}
	public void SetGems(GemSlot.GemState _state)
	{
		for (int i = 0; i < gemCost; i++)
		{
			gemSlots[i].SetGem(_state);
		}
	}
	public void CreateGemSlots()
	{
		gemSlots = new GemSlot[gemCost];
		float arc = 60;
		float angle = 270 - (arc / 2);
		if (gemCost == 1)
		{
			angle = 270;
		}
		float step = arc / (gemCost - 1);
		for (int i = 0; i < gemCost; i++)
		{
			GameObject gemSlot = Instantiate(GameManager.instance.gemSlotPrefab, transform);
			gemSlots[i] = gemSlot.GetComponent<GemSlot>();
			gemSlot.transform.localScale = Vector3.one * 0.5f;
			Vector2 pos = Tools.Vector2FromAngle(angle);
			gemSlot.transform.localPosition = pos / 2;
			angle += step;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		spawnTimer += Time.deltaTime * 8;
		
		switch(state)
		{
			case State.DRAGGED:
				if (spawnTimer < 1)
				{
					transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.5f, 1, 0.75f), spawnTimer);
				}
				else if (spawnTimer < 2)
				{
					transform.localScale = Vector3.Lerp(new Vector3(0.5f, 1, 0.75f), new Vector3(0.8f, 0.6f, 0.75f), spawnTimer - 1);
				}
				else if (spawnTimer < 3)
				{
					transform.localScale = Vector3.Lerp(new Vector3(0.8f, 0.6f, 0.75f), Vector3.one * 0.75f, spawnTimer - 2);
				}
				else
				{
					transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.75f, Time.deltaTime);
				}
				break;
			case State.PLACED:
				transform.localScale = Vector3.one * 0.75f;
				if (spawnTimer < 1)
				{
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, spawnTimer);
				}
				else if (spawnTimer < 2)
				{
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, new Vector3(1.3f,1.3f,0.75f), spawnTimer - 1);
				}
				else
				{
					backingCircle.transform.localScale = Vector3.Lerp(backingCircle.transform.localScale, new Vector3(1.3f, 1.3f, 0.75f), Time.deltaTime);
				}
				break;
		}
	}
}
