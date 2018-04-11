using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
	public static Effects instance;
	[System.Serializable]
	public class EffectBase
	{
		public string name;
		public GameObject effect;
		private List<GameObject> effectPool;
		private bool initialized;
		
		//EFFECT MODIFIERS
		[System.Serializable]
		public struct Modifiers
		{
			public bool poolAfter;
			public float poolDelay;
		}
		public Modifiers modifiers;

		public void Init()
		{
			initialized = true;
			effectPool = new List<GameObject>();
			AddEffect();
		}
		public void AddEffect()
		{
			GameObject tempEffect = Instantiate(effect, Vector3.zero, Quaternion.identity);
			tempEffect.SetActive(false);
			effectPool.Add(tempEffect);
		}
		public GameObject Use(Vector3 _pos, Transform _par = null)
		{
			if(!initialized)
			{
				Init();
			}
			for (int i = 0; i < effectPool.Count; i++)
			{
				if (!effectPool[i].activeSelf)
				{
					effectPool[i].transform.position = _pos;
					effectPool[i].SetActive(true);
					effectPool[i].transform.SetParent(_par);
					RunModifiers(effectPool[i]);
					return effectPool[i];
				}
			}
			AddEffect();
			return Use(_pos, _par);
		}
		private void RunModifiers(GameObject _effect)
		{
			if (modifiers.poolAfter)
			{
				instance.PoolEffect(_effect, modifiers.poolDelay);
			}
		}
	}

	public EffectBase[] effects;
	// Use this for initialization
	void Awake ()
	{
		instance = this;
	}
	public GameObject UseEffect(string _name, Vector3 _pos, Transform _par = null)
	{
		for(int i = 0; i < effects.Length; i++)
		{
			if(effects[i].name == _name)
			{
				return UseEffect(effects[i], _pos, _par);
			}
		}
		Debug.LogWarning("Effect " + _name + " does not exist!");
		return null;
	}
	public GameObject UseEffect(EffectBase effectBase, Vector3 _pos, Transform _par = null)
	{
		return effectBase.Use(_pos,_par);
	}
	public void PoolEffect(GameObject target, float seconds = 1, bool stopParticles = true)
	{
		StartCoroutine(PoolDelay(target, seconds, stopParticles));
	}
	IEnumerator PoolDelay(GameObject target, float seconds, bool stopParticles = true)
	{
		if (stopParticles)
		{
			if (target.GetComponent<ParticleSystem>() != null)
			{
				target.GetComponent<ParticleSystem>().Stop();
			}
		}
		yield return new WaitForSeconds(seconds);
		target.SetActive(false);
		target.transform.position = Vector3.zero;
		target.transform.SetParent(transform);
	}
}
