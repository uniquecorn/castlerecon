using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButton : CastleObject
{
	public Unit unit;

	public SpriteRenderer unitRenderer;
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		unitRenderer.sprite = unit.unitRenderer.sprite;
	}

	public override void Tap(Vector2 pos)
	{
		base.Tap(pos);
		GameManager.instance.selectedObject = CreateUnit();
	}

	public Unit CreateUnit()
	{
		Unit tempUnit = Instantiate(unit.gameObject, transform.position, Quaternion.identity).GetComponent<Unit>();
		tempUnit.Spawn();
		return tempUnit;
	}
}
