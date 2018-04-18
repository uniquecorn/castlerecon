using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class UnitButton : CastleObject
{
	public Unit unit;

	public SpriteRenderer unitRenderer;
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		unitRenderer.transform.localPosition = unit.spriteOffset;
		unitRenderer.sprite = unit.unitRenderer.sprite;
		CreateGemSlots();
	}

	public override void Tap()
	{
		base.Tap();
		if(GameManager.instance.UseGems(unit.gemCost))
		{
			TouchManager.Select(CreateUnit(),true);
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
			Vector3 gemPos = new Vector3(pos.x / 2, pos.y / 2, -0.2f);
			gemSlot.transform.localPosition = gemPos;
			gemSlot.GetComponent<GemSlot>().SetGem(GemSlot.GemState.INACTIVE);
			angle += step;
		}
	}
	
}