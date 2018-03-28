using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTab : MonoBehaviour
{
	public int value;

	public TextMesh valueText;
	public SpriteRenderer sr;
	public SpriteRenderer icon;

	public Sprite sword;
	public Sprite shield;

	public enum State
	{
		NONVISIBLE,
		SPAWN,
		VISIBLE,
		DESPAWN
	}

	public State tabState;

	public enum InfoType
	{
		SWORD,
		SHIELD
	}

	public InfoType infoType;

	public float stateTimer;

	public void Damage(int damage)
	{
		value = damage;
		sr.color = GameManager.instance.attackColor;
		
		if(tabState == State.NONVISIBLE)
		{
			stateTimer = 0;
			tabState = State.SPAWN;
		}
		else
		{
			//BOUNCE
		}
		infoType = InfoType.SWORD;
		icon.sprite = sword;
		valueText.text = value.ToString();
	}
	public void LoseDamage(int damage)
	{
		value = damage;
		sr.color = GameManager.instance.attackColor;
		if(value <= 0)
		{
			if (tabState == State.VISIBLE)
			{
				stateTimer = 0;
				tabState = State.DESPAWN;
			}
			else
			{
				//BOUNCE
			}
			infoType = InfoType.SWORD;
			icon.sprite = sword;
			valueText.gameObject.SetActive(false);
		}
	}
	public void Shield(int shielded)
	{
		value = shielded;
		sr.color = GameManager.instance.shieldColor;
		if (tabState == State.NONVISIBLE)
		{
			stateTimer = 0;
			tabState = State.SPAWN;
		}
		else
		{
			//BOUNCE
		}
		infoType = InfoType.SHIELD;
		icon.sprite = shield;
	}
	public void LoseShield(int shielded)
	{
		value = shielded;
		sr.color = GameManager.instance.shieldColor;
		if (value <= 0)
		{
			if (tabState == State.VISIBLE)
			{
				stateTimer = 0;
				tabState = State.DESPAWN;
			}
			infoType = InfoType.SHIELD;
			icon.sprite = shield;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		stateTimer += Time.deltaTime * 5;
		switch(tabState)
		{
			case State.NONVISIBLE:
				sr.size = Vector2.zero;
				icon.transform.localScale = Vector3.zero;
				valueText.gameObject.SetActive(false);
				icon.transform.localPosition = new Vector3(0, 0, -0.1f);
				break;
			case State.SPAWN:
				if(stateTimer < 1)
				{
					sr.size = Vector2.Lerp(Vector2.zero, Vector2.one, stateTimer * 2);
					if(stateTimer >= 0.4f && stateTimer < 0.7f)
					{
						icon.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2.1f, (stateTimer - 0.4f)/0.3f);
					}
					else if(stateTimer >= 0.7f)
					{
						icon.transform.localScale = Vector3.Lerp(Vector3.one * 2.1f, Vector3.one * 2, (stateTimer - 0.7f) / 0.3f);
					}

				}
				else if(stateTimer < 2)
				{
					if(infoType == InfoType.SWORD)
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
				else if(stateTimer < 3)
				{
					if (infoType == InfoType.SWORD)
					{
						sr.size = new Vector2(2, 1);
						icon.transform.localPosition = new Vector3(-0.5f, 0, -0.1f);
					}
					tabState = State.VISIBLE;
					stateTimer = 0;
				}
				
				break;
			case State.VISIBLE:

				break;
			case State.DESPAWN:
					if (infoType == InfoType.SWORD)
					{
						if (stateTimer < 0.4f)
						{
							sr.size = Vector2.Lerp(new Vector2(2f, 1), new Vector2(2.2f, 1), stateTimer/ 0.4f);
							
						}
						else if (stateTimer < 0.8f)
						{
							sr.size = Vector2.Lerp(new Vector2(2.2f, 1), Vector2.one, (stateTimer - 1.4f) / 0.4f);
							icon.transform.localPosition = Vector3.Lerp(new Vector3(-0.5f, 0, -0.1f), new Vector3(0, 0, -0.1f), (stateTimer - 0.4f) / 0.4f);
						}
						else if(stateTimer < 1.5f)
						{
							sr.size = Vector2.Lerp(Vector2.one, Vector2.zero, (stateTimer - 0.8f) * 2);
							icon.transform.localScale = Vector3.Lerp(Vector2.one, Vector2.zero, (stateTimer - 0.8f) * 2);
						}
						else
						{
							tabState = State.NONVISIBLE;
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
							tabState = State.NONVISIBLE;
						}
				}
				break;

		}
	}
}
