﻿using UnityEngine;
using System.Collections;

public class FrozenUI_SpellSlot_Sparkle : MonoBehaviour {

	public float tweenDuration = 0.3f;

	void Start()
	{
		TweenRotation.Begin(this.gameObject, this.tweenDuration, Quaternion.Euler(0f, 0f, 20f));
		TweenAlpha.Begin(this.gameObject, (this.tweenDuration / 2f), 1f).onFinished.Add(new EventDelegate(FadeInFinished));
	}

	public void FadeInFinished()
	{
		TweenAlpha.Begin(this.gameObject, (this.tweenDuration / 2f), 0f).onFinished.Add(new EventDelegate(OnFinished));
	}

	public void OnFinished()
	{
		DestroyImmediate(this.gameObject);
	}
}
