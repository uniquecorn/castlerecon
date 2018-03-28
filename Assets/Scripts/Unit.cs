using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : CastleObject
{
	public SpriteRenderer backingCircle;
	public SpriteRenderer unitRenderer;

	public enum ActionType
	{
		ATTACK,
		SHIELD
	}

	[System.Serializable]
	public struct Action
	{
		public int xOffset;
		public int yOffset;
		public int damage;
		public ActionType actionType;
	}

	public int health;
	public Action[] actions;

	public Slot slot;

	public enum State
	{
		DRAGGED,
		PLACED
	}
	public State state;

	private float spawnTimer;

	public void Place(Slot _slot)
	{
		coll.enabled = false;
		backingCircle.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		state = State.PLACED;
		slot = _slot;
		//slot.affliation = Slot.Affliation.OFFENCE;
		slot.unit = this;
		CommitActions();
		spawnTimer = 0;
	}
	public void Remove()
	{
		coll.enabled = true;
		RemoveActions();
		backingCircle.transform.localScale = Vector3.one;
		backingCircle.maskInteraction = SpriteMaskInteraction.None;
		spawnTimer = 0;
		state = State.DRAGGED;
		slot = null;
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
		Slot tempSlot = GetSlot(_action.xOffset, _action.yOffset);
		if(_action.actionType == ActionType.ATTACK)
		{
			tempSlot.TakeDamage(_action.damage);
		}
		else if(_action.actionType == ActionType.SHIELD)
		{
			tempSlot.Shield();
		}
	}
	public void UndoAction(Action _action)
	{
		Slot tempSlot = GetSlot(_action.xOffset, _action.yOffset);
		if (_action.actionType == ActionType.ATTACK)
		{
			tempSlot.RemoveDamage(_action.damage);
		}
		else if (_action.actionType == ActionType.SHIELD)
		{
			//tempSlot.Shield();
		}
	}

	public Slot GetSlot(int xOffset, int yOffset)
	{
		int x = slot.x + xOffset;
		int y = slot.y + yOffset;

		if(x >= 5)
		{
			x -= 5;
		}
		else if(x < 0)
		{
			x += 5;
		}

		return GameManager.instance.board.slots[x, y];
	}

	public void Spawn()
	{
		transform.localScale = Vector3.zero;
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
			transform.position = new Vector3(GameManager.instance.hoveredObject.transform.position.x, GameManager.instance.hoveredObject.transform.position.y, transform.position.z);
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
			Place((Slot)GameManager.instance.hoveredObject);
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
				if (spawnTimer < 1)
				{
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.75f, spawnTimer);
				}
				else if (spawnTimer < 2)
				{
					backingCircle.transform.localScale = Vector3.Lerp(Vector3.one * 0.75f, new Vector3(1.5f,1.8f,0.75f), spawnTimer - 1);
				}
				else
				{
					backingCircle.transform.localScale = Vector3.Lerp(backingCircle.transform.localScale, new Vector3(1.5f, 1.8f, 0.75f), Time.deltaTime);
				}
				break;
		}
	}
}
