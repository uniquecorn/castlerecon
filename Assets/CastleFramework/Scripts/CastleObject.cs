namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class CastleObject : MonoBehaviour
	{
		[HideInInspector]
		public Collider2D coll;

		protected float holdTimer;
		private float holdFloored;

		protected float hoverTimer;
		private float hoverFloored;

		protected TouchManager.HoverState hoverState;
		protected TouchManager.SelectedState selectedState;
		// Use this for initialization
		protected virtual void Start()
		{
			coll = GetComponent<Collider2D>();
		}

		public virtual void EnterHover()
		{
			print("Enter hover: " + gameObject.tag);
			hoverState = TouchManager.HoverState.EnterHover;
			hoverTimer =
				hoverFloored = 0;
		}

		public virtual void Hover()
		{
			hoverState = TouchManager.HoverState.Hover;
			if (hoverFloored < Mathf.FloorToInt(hoverTimer))
			{
				hoverFloored = Mathf.FloorToInt(hoverTimer);
				print("Hovering: " + gameObject.tag + " for " + hoverFloored + " seconds");
			}
			hoverTimer += Time.deltaTime;
		}

		public virtual void ExitHover()
		{
			print("Exit hover: " + gameObject.tag);
			hoverState = TouchManager.HoverState.ExitHover;
			hoverTimer =
				hoverFloored = 0;
			StartCoroutine(ExitHoverDelay());

		}

		public virtual void Tap()
		{
			print("Tapped: " + gameObject.tag);
			selectedState = TouchManager.SelectedState.Tap;
			holdTimer =
				holdFloored = 0;
		}

		public virtual void Hold()
		{
			selectedState = TouchManager.SelectedState.Hold;
			if (holdFloored < Mathf.FloorToInt(holdTimer))
			{
				holdFloored = Mathf.FloorToInt(holdTimer);
				print("Held: " + gameObject.tag + " for " + holdFloored + " seconds");
			}
			holdTimer += Time.deltaTime;
		}

		public virtual void Release()
		{
			print("Released: " + gameObject.tag);
			selectedState = TouchManager.SelectedState.Release;
			holdTimer =
				holdFloored = 0;
			StartCoroutine(ReleaseDelay());
		}

		IEnumerator ReleaseDelay()
		{
			yield return new WaitForEndOfFrame();
			selectedState = TouchManager.SelectedState.None;
		}

		IEnumerator ExitHoverDelay()
		{
			yield return new WaitForEndOfFrame();
			hoverState = TouchManager.HoverState.None;
		}
	}
}
