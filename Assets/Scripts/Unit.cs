using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Unit : CastleObject
{
	public SpriteRenderer backingCircle;
	public SpriteRenderer backingOutline;
	public SpriteMask backingMask;
	public SpriteRenderer unitRenderer;

	public Sprite unitSprite;
	public Sprite unitAttackSprite;
	public Sprite circle;
	public Sprite rectangle;

	public Vector2 spriteOffset;

	public enum ActionType
	{
		ATTACK,
		SHIELD,
		MAGICDAMAGE,
		NONE
	}

	[System.Serializable]
	public class Action
	{
		public int xOffset;
		public int yOffset;
		public int value;
		public int immune;
		public bool removeOnDeath;
		public ActionType actionType;
		[HideInInspector]
		public GameManager.Affliation affliation;
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

	public enum Flip
	{
		NORMAL,
		FLIPPED
	}
	public Flip flipState;
	public bool flippable;
	public GemSlot[] gemSlots;

	private float spawnTimer;
	public bool heldOverSlot;

	public void Place(Slot _slot)
	{
		coll.enabled = false;

		
		state = State.PLACED;
		slot = _slot;
		transform.position = Tools.Vec3RepZ(_slot.transform.position,transform.position.z);
		if(slot.affliation == GameManager.Affliation.DEFENCE)
		{
			GameManager.instance.board.ModifyDefenceSlot();
		}
		else
		{
			GameManager.instance.board.ModifyOffenceSlot();
		}
		slot.unit = this;
		CommitActions();
		spawnTimer = 0;
	}
	public void Remove()
	{
		unitRenderer.sprite = unitSprite;
		coll.enabled = true;
		RemoveActions();
		backingCircle.transform.localScale = Vector3.one;
		backingCircle.sprite = backingMask.sprite = GameManager.instance.circleSprite;
		backingOutline.gameObject.SetActive(true);
		backingOutline.sprite = GameManager.instance.circleOSprite;
		if (slot.affliation == GameManager.Affliation.DEFENCE)
		{
			GameManager.instance.board.ModifyDefenceSlot(true);
		}
		else
		{
			GameManager.instance.board.ModifyOffenceSlot(true);
		}
		//backingCircle.maskInteraction = SpriteMaskInteraction.None;
		spawnTimer = 0;
		//SetGems(GemSlot.GemState.INACTIVE);
		state = State.DRAGGED;
		//slot.cardMask.gameObject.SetActive(false);
		slot = null;
	}

	public void ApplyAction(Action _action)
	{

	}

	public void Die()
	{
		for (int i = 0; i < actions.Length; i++)
		{
			if(actions[i].removeOnDeath)
			{
				UndoAction(actions[i]);
			}
		}
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
		for(int i = 0; i < gemSlots.Length; i++)
		{
			gemSlots[i].gameObject.SetActive(false);
		}
	}
	public void Show()
	{
		unitRenderer.sprite = unitSprite;
		for (int i = 0; i < gemSlots.Length; i++)
		{
			gemSlots[i].gameObject.SetActive(true);
		}
	}

	public void Spawn()
	{
		transform.localScale = Vector3.zero;
		CreateGemSlots();
		spawnTimer = 0;
	}

	public void HeldOverSlot(Slot _slot)
	{
		TestActions(_slot);
		transform.position = Vector3.Lerp(transform.position, Tools.Vec3RepZ(_slot.transform.position, transform.position.z), Time.deltaTime * 15);
		_slot.Highlight(ActionType.NONE);
		heldOverSlot = true;
	}

	public override void Hold()
	{
		base.Hold();
		state = State.DRAGGED;
		if(heldOverSlot)
		{
			heldOverSlot = false;
		}
		else
		{
			this.Drag();
		}
	}

	public override void Release()
	{
		base.Release();
		if(heldOverSlot)
		{
			Place((Slot)TouchManager.hoveredObject);
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
			Vector3 pos = Tools.Vector2FromAngle(angle);
			pos = new Vector3(pos.x, pos.y, -0.5f);
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
				unitRenderer.transform.localPosition = Vector3.Lerp(unitRenderer.transform.localPosition, spriteOffset, Time.deltaTime * 8);
				if (spawnTimer < 1)
				{
					transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.5f, 1.25f, 1f), spawnTimer);
				}
				else if (spawnTimer < 2)
				{
					transform.localScale = Vector3.Lerp(new Vector3(0.5f, 1.25f, 1f), new Vector3(1.2f, 0.6f, 1), spawnTimer - 1);
				}
				else if (spawnTimer < 3)
				{
					transform.localScale = Vector3.Lerp(new Vector3(1.2f, 0.6f, 1), Vector3.one, spawnTimer - 2);
				}
				else
				{
					transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime);
				}
				break;
			case State.PLACED:
				unitRenderer.transform.localPosition = Vector3.Lerp(unitRenderer.transform.localPosition, Vector3.zero, Time.deltaTime * 8);
				backingCircle.color = Color.Lerp(backingCircle.color, new Color(0.8f, 0.8f, 0.8f), Time.deltaTime * 5);
				transform.localScale = Vector3.one;
				if (spawnTimer < 1)
				{
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, spawnTimer);
				}
				else if (spawnTimer < 2)
				{
					unitRenderer.sprite = unitAttackSprite;
					backingCircle.sprite = backingMask.sprite = GameManager.instance.cardSprite;
					backingOutline.sprite = GameManager.instance.cardOSprite;
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, Vector3.one, spawnTimer - 1);
				}
				else
				{
					backingOutline.gameObject.SetActive(false);
					backingCircle.transform.localScale = Vector3.Lerp(backingCircle.transform.localScale, Vector3.one, Time.deltaTime);
				}
				break;
		}
	}
}
