﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : CastleObject
{
	public Slot[,] slots;
	public SpriteRenderer boardRenderer;
	public Transform slotTransform;

	private float spawnTimer;
	private bool spawning;

	public bool active;
	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		GameManager.instance.board = this;
		coll.enabled = false;
		boardRenderer.size = new Vector2(0, 0);
		spawning = true;
		spawnTimer = 0;
		
		//CreateBoard();
	}

	public void CreateBoard(int x = 5, int y = 2)
	{
		slots = new Slot[x, y];
		for(int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				Slot tempSlot = Instantiate(GameManager.instance.slotPrefab.gameObject,new Vector3(i-2,(float)j * 1.2f - 0.6f,0),Quaternion.identity).GetComponent<Slot>();
				slots[i, j] = tempSlot;
				tempSlot.x = i;
				tempSlot.y = j;
				tempSlot.transform.SetParent(slotTransform);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(spawning)
		{
			spawnTimer += Time.deltaTime * 5;
			if(spawnTimer < 1)
			{
				boardRenderer.size = Vector2.Lerp(new Vector2(0, 0), new Vector2(5.2f, 1), spawnTimer);
			}
			else if(spawnTimer < 2)
			{
				boardRenderer.size = Vector2.Lerp(new Vector2(5.2f, 1), new Vector2(5f, 2.4f), spawnTimer - 1);
			}
			else
			{
				spawnTimer = 0;
				spawning = false;
				CreateBoard();
			}
		}
	}
}