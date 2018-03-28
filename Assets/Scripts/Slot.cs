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
	public int damageTaken;
	public int shielded;
	public Unit unit;



	public SpriteRenderer cardInner;
	public SpriteRenderer cardOutline;

	//UI
	public InfoTab infoTab;

	// Use this for initialization
	public void Highlight()
	{

	}

	public void ShowTab()
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

	public void TakeDamage(int damage)
	{
		damageTaken += damage;
		cardOutline.color = GameManager.instance.attackColor;
		infoTab.Damage(damageTaken);
	}

	public void RemoveDamage(int damage)
	{
		damageTaken -= damage;
		infoTab.LoseDamage(damageTaken);
		if (damageTaken <= 0)
		{
			cardOutline.color = Color.clear;
		}
	}

	public void Shield()
	{
		shielded++;
		cardOutline.color = GameManager.instance.shieldColor;
		infoTab.Shield(shielded);
	}
}
