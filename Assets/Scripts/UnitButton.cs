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
		CreateGemSlots();
	}

	public override void Tap(Vector2 pos)
	{
		base.Tap(pos);
		if(GameManager.instance.UseGems(unit.gemCost))
		{
			GameManager.instance.selectedObject = CreateUnit();
		}
	}

	public Unit CreateUnit()
	{
		Unit tempUnit = Instantiate(unit.gameObject, transform.position, Quaternion.identity).GetComponent<Unit>();
		tempUnit.Spawn();
		return tempUnit;
	}

	public void CreateGemSlots()
	{
		float arc = 60;
		float angle = 270 - (arc/2);
		if(unit.gemCost == 1)
		{
			angle = 270;
		}
		float step = arc / (unit.gemCost - 1);
		for (int i = 0; i < unit.gemCost; i++)
		{
			GameObject gemSlot = Instantiate(GameManager.instance.gemSlotPrefab, transform);
			gemSlot.transform.localScale = Vector3.one * 0.5f;
			Vector2 pos = Tools.Vector2FromAngle(angle);
			gemSlot.transform.localPosition = pos/2;
			gemSlot.GetComponent<GemSlot>().SetGem(GemSlot.GemState.INACTIVE);
			angle += step;
		}
	}
	
}