using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSlot : MonoBehaviour
{
	public enum GemState
	{
		INACTIVE,
		ACTIVE,
		DISABLED
	}
	public GemState gemState;
	public SpriteRenderer slot;
	public SpriteRenderer gem;
	public SpriteGlow.SpriteGlowEffect glowEffect;
	public Color activatedGem;
	public Color inactiveGem;
	public Color disabledGem;
	public float stateTimer;
	public float gemBrightness = 5;
	// Use this for initialization
	public void SetGem(GemState _state)
	{
		gemState = _state;
		stateTimer = 0;
	}
	void UpdateGem()
	{
		switch (gemState)
		{
			case GemState.ACTIVE:
				if(stateTimer < 0.5f)
				{
					glowEffect.GlowBrightness = Mathf.Lerp(1, gemBrightness, stateTimer * 2);
				}
				else
				{
					glowEffect.GlowBrightness = Mathf.Lerp(gemBrightness, 1, (stateTimer * 2) - 0.5f);
					gem.color = Color.Lerp(gem.color, activatedGem, (stateTimer * 2) - 0.5f);
				}

				break;
			case GemState.INACTIVE:
				if (stateTimer < 0.5f)
				{
					glowEffect.GlowBrightness = Mathf.Lerp(1, gemBrightness, stateTimer * 2);
				}
				else
				{
					glowEffect.GlowBrightness = Mathf.Lerp(gemBrightness, 1, (stateTimer * 2) - 0.5f);
					gem.color = Color.Lerp(gem.color, inactiveGem, (stateTimer * 2) - 0.5f);
				}
				break;
			case GemState.DISABLED:
				if (stateTimer < 0.5f)
				{
					glowEffect.GlowBrightness = Mathf.Lerp(1, gemBrightness, stateTimer * 2);
				}
				else
				{
					glowEffect.GlowBrightness = Mathf.Lerp(gemBrightness, 1, (stateTimer * 2) - 0.5f);
					gem.color = Color.Lerp(gem.color, disabledGem, (stateTimer * 2) - 0.5f);
				}
				break;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		if (stateTimer < 1)
		{
			UpdateGem();
			stateTimer += Time.deltaTime * 1.5f;
		}
		else if(stateTimer > 1)
		{
			stateTimer = 1;
			UpdateGem();
		}
		else
		{
			stateTimer = 1;
		}
	}
}
