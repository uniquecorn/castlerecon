using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : CastleObject
{
	public int x, y;

	public enum Affliation
	{
		DEFENCE,
		OFFENCE
	}

	public enum DisplayMode
	{
		IDLE,
		ATTACK
	}

	public Affliation affliation;
	public DisplayMode displayMode;

	[System.Serializable]
	public class Effect
	{
		public Unit.ActionType actionType;
		public int value;
		public int immune;
		public InfoTab infoTab;
		public Affliation affliation;

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
	public SpriteMask cardMask;

	//UI
	public GameObject infoTabPrefab;

	public List<Effect> effects = new List<Effect>();

	// Use this for initialization
	public void Highlight(Unit.ActionType _actionType)
	{
		highlighted = true;
		cardOutline.color = GameManager.instance.GetStyle(_actionType).effectColor;
	}

	public void Unhighlight()
	{
		cardOutline.color = Color.clear;
	}

	void UpdateTabs()
	{
		for(int i = 0; i < effects.Count;i++)
		{
			effects[i].UpdateTab(effects.Count - i - 1);
		}
	}

	void CheckTabs(Unit.Action _action)
	{
		
	}

	public override void Tap(Vector2 pos)
	{
		base.Tap(pos);
		if (unit)
		{
			unit.Remove();
			GameManager.instance.selectedObject = unit;
			unit = null;
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
		if(affliation == Affliation.DEFENCE)
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
	}

	public void ApplyEffects()
	{
		
	}

	//public void Shield()
	//{
	//	shielded++;
		
	//	infoTab.Shield(shielded);
	//}

	public override void ExitHover()
	{
		base.ExitHover();
		//Unhighlight();
	}

	void HighlightLogic()
	{
		if (!highlighted)
		{
			cardOutline.color = Color.Lerp(cardOutline.color, Color.clear, Time.deltaTime * 5);
		}
		else
		{
			highlighted = false;
		}
	}

	private void Update()
	{
		HighlightLogic();
	}
}
