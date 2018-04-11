using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTab : MonoBehaviour
{
	public int value;
	public int immune;

	public TextMesh valueText;
	public SpriteRenderer sr;
	public SpriteRenderer icon;
	public SpriteRenderer disabled;

	public Sprite sword;
	public Sprite shield;
	public Sprite staff;

	public bool showValue;
	public bool hideImmune;

	public enum State
	{
		NONVISIBLE,
		SPAWN,
		VISIBLE,
		DESPAWN
	}

	public State tabState;

	public Unit.ActionType actionType;

	public float stateTimer;

	public int effectPos;

	public Slot.Affliation affliation;

	public void LoadTab(Slot.Effect _effect)
	{
		//loadedAction = _action;
		if(value == 0)
		{
			ShowTab();
		}
		value = _effect.value;
		immune = _effect.immune;
		
		actionType = _effect.actionType;
		GameManager.EffectStyle style = GameManager.instance.GetStyle(actionType);
		sr.color = style.effectColor;
		icon.sprite = style.effectSprite;
		showValue = style.showValue;
		if (immune > 0)
		{
			sr.color = Color.gray;
			showValue = false;
		}
		if (value <= 0 && immune <= 0)
		{
			HideTab();
		}
		else
		{
			valueText.text = value.ToString();
		}
	}

	public void ShowTab()
	{
		stateTimer = 0;
		tabState = State.SPAWN;
		valueText.text = value.ToString();
	}

	public void HideTab()
	{
		stateTimer = 0;
		tabState = State.DESPAWN;
		valueText.text = "";
	}

	void NonVisibleState()
	{
		sr.size = Vector2.zero;
		icon.transform.localScale = Vector3.zero;
		valueText.gameObject.SetActive(false);
		icon.transform.localPosition = new Vector3(0, 0, -0.1f);
	}
	void SpawnState()
	{
		if (stateTimer < 1)
		{
			sr.size = Vector2.Lerp(Vector2.zero, Vector2.one, stateTimer * 2);
			if (stateTimer >= 0.4f && stateTimer < 0.7f)
			{
				icon.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2.1f, (stateTimer - 0.4f) / 0.3f);
			}
			else if (stateTimer >= 0.7f)
			{
				icon.transform.localScale = Vector3.Lerp(Vector3.one * 2.1f, Vector3.one * 2, (stateTimer - 0.7f) / 0.3f);
			}

		}
		else if (stateTimer < 2)
		{
			if (showValue)
			{
				if (stateTimer < 1.4f)
				{
					sr.size = Vector2.Lerp(Vector2.one, new Vector2(2.2f, 1), (stateTimer - 1) / 0.4f);
					icon.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, -0.1f), new Vector3(-0.5f, 0, -0.1f), (stateTimer - 1) / 0.4f);
				}
				else if (stateTimer < 1.8f)
				{
					icon.transform.localPosition = new Vector3(-0.5f, 0, -0.1f);
					sr.size = Vector2.Lerp(new Vector2(2.2f, 1), new Vector2(2f, 1), (stateTimer - 1.4f) / 0.4f);
					valueText.gameObject.SetActive(true);
					valueText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 4f, (stateTimer - 1.4f) / 0.4f);
				}
			}
			else
			{

			}
		}
		else if (stateTimer < 3)
		{
			if (showValue)
			{
				sr.size = new Vector2(2, 1);
				icon.transform.localPosition = new Vector3(-0.5f, 0, -0.1f);
			}
			tabState = State.VISIBLE;
			stateTimer = 0;
		}
	}
	void VisibleState()
	{

	}
	void DespawnState()
	{
		if (showValue)
		{
			if (stateTimer < 0.4f)
			{
				sr.size = Vector2.Lerp(new Vector2(2f, 1), new Vector2(2.2f, 1), stateTimer / 0.4f);

			}
			else if (stateTimer < 0.8f)
			{
				sr.size = Vector2.Lerp(new Vector2(2.2f, 1), Vector2.one, (stateTimer - 1.4f) / 0.4f);
				icon.transform.localPosition = Vector3.Lerp(new Vector3(-0.5f, 0, -0.1f), new Vector3(0, 0, -0.1f), (stateTimer - 0.4f) / 0.4f);
			}
			else if (stateTimer < 1.5f)
			{
				sr.size = Vector2.Lerp(Vector2.one, Vector2.zero, (stateTimer - 0.8f) * 2);
				icon.transform.localScale = Vector3.Lerp(Vector2.one, Vector2.zero, (stateTimer - 0.8f) * 2);
			}
			else
			{
				if(value <= 0)
				{
					Destroy(gameObject);
				}
			}
		}
		else
		{
			if (stateTimer < 0.8f)
			{
				sr.size = Vector2.Lerp(Vector2.one, Vector2.zero, stateTimer * 2);
				icon.transform.localScale = Vector3.Lerp(Vector2.one, Vector2.zero, stateTimer * 2);
			}
			else
			{
				if (value <= 0)
				{
					Destroy(gameObject);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		stateTimer += Time.deltaTime * 5;
		if (affliation == Slot.Affliation.DEFENCE)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0.5f + (effectPos * 0.3f), -0.4f), Time.deltaTime * 10);
		}
		else
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, -0.5f + (effectPos * -0.3f), -0.4f), Time.deltaTime * 10);
		}
		switch(tabState)
		{
			case State.NONVISIBLE:
				NonVisibleState();
				break;
			case State.SPAWN:
				SpawnState();
				break;
			case State.VISIBLE:
				VisibleState();
				break;
			case State.DESPAWN:
				DespawnState();
				break;
		}
	}
}
