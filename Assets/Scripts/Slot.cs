using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Castle;

public class Slot : CastleObject
{
	public int x, y;

	public enum DisplayMode
	{
		IDLE,
		DISABLED,
		HIDE,
		ATTACK
	}

	public GameManager.Affliation affliation;
	public DisplayMode displayMode;

	[System.Serializable]
	public class Effect
	{
		public Unit.ActionType actionType;
		public int value;
		public int immune;
		public bool apply;
		public InfoTab infoTab;
		public GameManager.Affliation affliation;

		public void Set(Unit.Action _action)
		{
			value = _action.value;
			immune = _action.immune;
		}
		public void Add(Unit.Action _action)
		{
			value += _action.value;
			immune += _action.immune;
		}
		public void Remove(Unit.Action _action)
		{
			value -= _action.value;
			immune -= _action.immune;
		}
		public void SetTab(InfoTab _infoTab)
		{
			infoTab = _infoTab;
		}
		public void UpdateTab(int position)
		{
			infoTab.effectPos = position;
			infoTab.LoadTab(this);
		}
	}

	private bool highlighted = false;
	public Unit unit;

	public SpriteRenderer cardInner;
	public SpriteRenderer cardOutline;
	public SpriteRenderer cardStripe;
	public SpriteMask cardMask;

	//UI
	public GameObject infoTabPrefab;

	public List<Effect> effects = new List<Effect>();

	// Use this for initialization
	public void Highlight(Unit.ActionType _actionType)
	{
		highlighted = true;
		cardOutline.color = cardStripe.color = GameManager.instance.GetStyle(_actionType).effectColor;
	}

	public void Unhighlight()
	{
		cardOutline.color = Color.white;
	}

	void UpdateTabs()
	{
		for(int i = 0; i < effects.Count; i++)
		{
			effects[i].UpdateTab(effects.Count - i - 1);
		}
	}

	public void DoAction(Unit.Action _action)
	{
		AddEffect(_action);
		UpdateTabs();
	}

	public void AddEffect(Unit.Action _action)
	{
		bool effectExists = false;
		for (int j = 0; j < effects.Count; j++)
		{
			if(effects[j].actionType == _action.actionType && effects[j].affliation == _action.affliation)
			{
				effectExists = true;
				effects[j].Add(_action);
				break;
			}
		}
		if(!effectExists)
		{
			CreateEffect(_action);
		}
	}

	public void RemoveEffect(Unit.Action _action)
	{
		for (int j = 0; j < effects.Count; j++)
		{
			if (effects[j].actionType == _action.actionType && effects[j].affliation == _action.affliation)
			{
				effects[j].Remove(_action);
				if(effects[j].value <= 0 && effects[j].immune <= 0)
				{
					DestroyEffect(effects[j]);
				}
				break;
			}
		}
	}

	public void DestroyEffect(Effect _effect)
	{
		_effect.UpdateTab(_effect.infoTab.effectPos);
		effects.Remove(_effect);
	}

	public void CreateEffect(Unit.Action _action)
	{
		InfoTab tempTab = Instantiate(infoTabPrefab, transform).GetComponent<InfoTab>();
		tempTab.affliation = affliation;
		if(affliation == GameManager.Affliation.DEFENCE)
		{
			tempTab.transform.localPosition = new Vector3(0, 0.5f, -0.4f);
		}
		else
		{
			tempTab.transform.localPosition = new Vector3(0, -0.5f, -0.4f);
		}
		Effect tempEffect = new Effect
		{
			actionType = _action.actionType,
			value = _action.value,
			immune = _action.immune,
			infoTab = tempTab,
			affliation = _action.affliation
		};
		tempTab.LoadTab(tempEffect);
		effects.Add(tempEffect);
	}

	public void UndoAction(Unit.Action _action)
	{
		RemoveEffect(_action);
		UpdateTabs();
	}

	public void HideSlot()
	{
		for (int i = 0; i < effects.Count; i++)
		{
			effects[i].infoTab.HideTab();
		}
		if(unit)
		{
			unit.Hide();
		}
	}

	public void ShowSlot()
	{
		for (int i = 0; i < effects.Count; i++)
		{
			effects[i].infoTab.hidden = false;
			effects[i].infoTab.ShowTab();
		}
		if (unit)
		{
			unit.Show();
		}
	}

	public bool ApplyEffects()
	{
		//APPLY
		for (int i = 0; i < effects.Count; i++)
		{
			if(!CheckImmune(effects[i].actionType) && !CheckShield() && effects[i].actionType != Unit.ActionType.SHIELD && !effects[i].apply)
			{
				switch (effects[i].actionType)
				{
					case Unit.ActionType.ATTACK:
							unit.health -= effects[i].value;
						break;
					case Unit.ActionType.MAGICDAMAGE:
							unit.health -= effects[i].value;
						break;
				}
				effects[i].apply = true;
			}
		}
		if (unit.health <= 0)
		{
			unit.Die();
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool CheckImmune(Unit.ActionType actionType)
	{
		for (int i = 0; i < effects.Count; i++)
		{
			if (effects[i].actionType == actionType)
			{
				if (effects[i].immune > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckShield()
	{
		bool shielded = false;
		for (int i = 0; i < effects.Count; i++)
		{
			if(effects[i].actionType == Unit.ActionType.SHIELD)
			{
				if(effects[i].immune > 0)
				{
					return false;
				}
				else if(effects[i].value > 0)
				{
					return true;
				}
			}
		}
		return shielded;
	}

	void HighlightLogic()
	{
		if (!highlighted)
		{
			cardOutline.color = cardStripe.color = Color.Lerp(cardOutline.color, Color.white, Time.deltaTime * 5);
		}
		else
		{
			highlighted = false;
		}
	}

	void DisplayLogic()
	{
		switch(displayMode)
		{
			case DisplayMode.IDLE:
				cardStripe.gameObject.SetActive(false);
				break;
			case DisplayMode.DISABLED:
				//cardMask.gameObject.SetActive(true);
				cardStripe.gameObject.SetActive(true);
				break;
		}
	}

	public override void Tap()
	{
		base.Tap();
		if (unit && displayMode == DisplayMode.IDLE && affliation == GameManager.instance.turn)
		{
			unit.Remove();
			TouchManager.Select(unit, true);
			unit = null;
		}
	}

	public override void Hover()
	{
		base.Hover();
		if(displayMode == DisplayMode.IDLE && !unit)
		{
			if (TouchManager.selectedObject && TouchManager.selectedObject is Unit)
			{
				((Unit)TouchManager.selectedObject).HeldOverSlot(this);
			}
		}
	}

	private void Update()
	{
		HighlightLogic();
		DisplayLogic();
	}
}
