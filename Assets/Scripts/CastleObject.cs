using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleObject : MonoBehaviour
{
	[HideInInspector]
	public Collider2D coll;

	protected float holdTimer;

	public enum ObjectState
	{
		EnterHover,
		Hover,
		ExitHover,
		Tap,
		Hold,
		Release
	}
	public ObjectState objectState;
	// Use this for initialization
	protected virtual void Start ()
	{
		coll = GetComponent<Collider2D>();
	}

	public virtual void EnterHover()
	{
		print("Enter hover: " + gameObject.tag);
	}

	public virtual void Hover()
	{
		print("Hovering: " + gameObject.tag);
	}

	public virtual void ExitHover()
	{
		print("Exit hover: " + gameObject.tag);
	}

	public virtual void Tap(Vector2 pos)
	{
		print("Tapped: " + gameObject.tag);
		holdTimer = 0;
	}

	public virtual void Hold(Vector2 pos)
	{
		print("Held: " + gameObject.tag);
		holdTimer += Time.deltaTime;
	}

	public virtual void Release(Vector2 pos)
	{
		print("Released: " + gameObject.tag);
		holdTimer = 0;
	}

	//// Update is called once per frame
	//protected virtual void Update ()
	//{
		
	//}
}
