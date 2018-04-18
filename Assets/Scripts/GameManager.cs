using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Castle;

public class GameManager : MonoBehaviour
{
	public Slot slotPrefab;
	public GameObject gemSlotPrefab;

	[System.Serializable]
	public struct EffectStyle
	{
		public Unit.ActionType actionType;
		public Color effectColor;
		public Sprite effectSprite;
		public bool showValue;
	}
	public enum Affliation
	{
		DEFENCE,
		OFFENCE,
		BREAKDOWN
	}
	public EffectStyle[] effectStyles;

	public Sprite hiddenSprite;
	public Sprite cardSprite;
	public Sprite cardOSprite;
	public Sprite circleSprite;
	public Sprite circleOSprite;

	public Board board;
	public GemSlot[] gemSlots;
	public int gemsAvailable;

	public GameObject boardPrefab;

	public Affliation turn;
	public Text turnDisplay;
	public int offenceTurn;

	public bool showConfirm;
	public CanvasGroup confirmButton;
	public Text confirmButtonText;
	private RectTransform confirmButtonTrans;

	public static GameManager instance;
	// Use this for initialization
	void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		TouchManager.TouchInit(TouchManager.TouchInputMode.SIMPLE);
		confirmButtonTrans = confirmButton.GetComponent<RectTransform>();
		CreateBoard();
	}
	
	void CreateBoard()
	{
		board = Instantiate(boardPrefab, Vector3.zero, Quaternion.identity).GetComponent<Board>();
	}

	public EffectStyle GetStyle(Unit.ActionType _actionType)
	{
		for (int i = 0; i < effectStyles.Length; i++)
		{
			if (effectStyles[i].actionType == _actionType)
			{
				return effectStyles[i];
			}
		}
		print("No style found!");
		return effectStyles[0];
	}

	public void ConfirmFormation()
	{
		showConfirm = false;
		switch(turn)
		{
			case Affliation.DEFENCE:
				turn = Affliation.OFFENCE;
				board.SetView();
				ResetGems();
				offenceTurn = 0;
				break;
			case Affliation.OFFENCE:
				turn = Affliation.BREAKDOWN;
				board.SetView();
				break;
		}
	}

	public bool UseGems(int gems = 1)
	{
		if(gemsAvailable >= gems)
		{
			for(int i = (3 - gemsAvailable); i < (3 - gemsAvailable + gems); i++)
			{
				gemSlots[i].SetGem(GemSlot.GemState.INACTIVE);
			}
			gemsAvailable -= gems;
			return true;
		}
		else
		{
			return false;
		}
	}
	public void AddGems(int gems = 1)
	{
		for (int i = 0; i < gems; i++)
		{
			gemsAvailable++;
			print((3 - gemsAvailable));
			gemSlots[(3 - gemsAvailable)].SetGem(GemSlot.GemState.ACTIVE);
		}
	}

	public void ResetGems()
	{
		gemsAvailable = 3;
		for(int i = 0; i < 3; i++)
		{
			gemSlots[i].SetGem(GemSlot.GemState.ACTIVE);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		TouchManager.TouchUpdate();
		if(showConfirm)
		{
			confirmButtonTrans.anchoredPosition = Vector2.Lerp(confirmButtonTrans.anchoredPosition, new Vector2(0, 210), Time.deltaTime * 5);
			confirmButton.alpha = Mathf.Lerp(confirmButton.alpha, 1, Time.deltaTime * 5);
		}
		else
		{
			confirmButtonTrans.anchoredPosition = Vector2.Lerp(confirmButtonTrans.anchoredPosition, new Vector2(0, 200), Time.deltaTime * 5);
			confirmButton.alpha = Mathf.Lerp(confirmButton.alpha, 0, Time.deltaTime * 5);
		}
	}
}