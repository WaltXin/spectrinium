﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIWindow : MonoBehaviour {

	public delegate void OnShowBegin();
	public delegate void OnShowComplete();
	public delegate void OnHideBegin();
	public delegate void OnHideComplete();

	private static Dictionary<int, UIWindow> mWindows = new Dictionary<int, UIWindow>();

	private static int mFucusedWindow = -1;
	public static int FocusedWindow { get { return mFucusedWindow; } private set { mFucusedWindow = value; } }

	public int WindowId = 0;
	public Transform contentHolder;
	public bool startHidden = true;
	public bool fading = true;
	public float fadeDuration = 0.5f;

	private bool isFocused = false;
	private bool mShowWindow = false;
	private UIPanel panel;
	private bool HookedEvents = false;
	private float HookEventsDelay = 1f;

	/// <summary>
	/// A delegate invoked when the window begins to fade in.
	/// </summary>
	public OnShowBegin onShowBegin;

	/// <summary>
	/// A delegate invoked when the window is completely shown.
	/// </summary>
	public OnShowComplete onShowComplete;

	/// <summary>
	/// A delegate invoked when the window begins to fade out.
	/// </summary>
	public OnHideBegin onHideBegin;

	/// <summary>
	/// A delegate invoked when the window is completely hidden.
	/// </summary>
	public OnHideComplete onHideComplete;

	/// <summary>
	/// Gets a value indicating whether this window is visible.
	/// </summary>
	/// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
	public bool IsVisible {
		get {
			return (this.panel != null && this.panel.alpha == 0f) ? false : true;
		}
	}

	void Awake()
	{
		if (mWindows.ContainsKey(this.WindowId))
		{
			Debug.LogWarning("UIWindow: A window with ID (" + this.WindowId + ") already exists, please consider changing it.");

			// Fix it, make sure we dont make a dead loop
			while (mWindows.ContainsKey(this.WindowId) && this.WindowId < 100)
				this.WindowId++;
		}

		mWindows.Add(this.WindowId, this);
	}

	void Start()
	{
		this.panel = this.GetComponent<UIPanel>();

		if (this.contentHolder == null)
			this.contentHolder = this.transform;

		this.HandleStartingState();
	}

	private void HandleStartingState()
	{
		// Start hidden
		if (this.startHidden)
		{
			if (this.panel != null)
				this.panel.alpha = 0f;
			
			this.contentHolder.gameObject.SetActive(false);
			this.mShowWindow = false;
		}
		else
		{
			if (this.panel != null)
				this.panel.alpha = 1f;
			
			this.contentHolder.gameObject.SetActive(true);
			this.mShowWindow = true;
		}
	}

	void Update()
	{
		if (!this.HookedEvents && Time.time >= this.HookEventsDelay)
		{
			this.HookFocusEvents(true);
			this.HookedEvents = true;
		}
	}

	/// <summary>
	/// Hooks the focus handling events.
	/// </summary>
	/// <param name="hook">If set to <c>true</c> hook.</param>
	public void HookFocusEvents(bool hook)
	{
		// Get the widgets of the panel
	//	BetterList<UIWidget> widgets = this.panel.widgets;
		
		// Loop the widgets and hook some press events
	/*	foreach (UIWidget widget in widgets)
		{
			if (widget.GetComponent<Collider>() != null)
			{
				if (hook)
					UIEventListener.Get(widget.gameObject).onPress += OnWidgetPress;
				else
					UIEventListener.Get(widget.gameObject).onPress -= OnWidgetPress;
			}
		}*/
	}

	/// <summary>
	/// Gets the window with the given ID.
	/// </summary>
	/// <returns>The window.</returns>
	/// <param name="id">Identifier.</param>
	public static UIWindow GetWindow(int id)
	{
		if (mWindows.ContainsKey(id))
			return mWindows[id];

		return null;
	}

	/// <summary>
	/// Focuses the window with the given ID.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public static void FocusWindow(int id)
	{
		// Focus the window
		if (GetWindow(id) != null)
			GetWindow(id).Focus();
	}

	/// <summary>
	/// Raises the focused window event.
	/// </summary>
	/// <param name="id">Identifier.</param>
	private static void OnFocusedWindow(int id)
	{
		if (mFucusedWindow > -1 && GetWindow(mFucusedWindow))
			GetWindow(mFucusedWindow).isFocused = false;

		mFucusedWindow = id;
	}

	/// <summary>
	/// Focuses this window.
	/// </summary>
	public void Focus()
	{
		if (this.isFocused)
			return;

		// Prevent focus spamm
		this.isFocused = true;

		// Call the static on focused window
		OnFocusedWindow(this.WindowId);

		// Bring the window forward
		NGUITools.BringForward(this.gameObject);
	}

	/// <summary>
	/// Raises the widget press event (Focus related).
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="isPressed">If set to <c>true</c> is pressed.</param>
	private void OnWidgetPress(GameObject go, bool isPressed)
	{
		if (isPressed)
			this.Focus();
	}

	/// <summary>
	/// Toggle the window Show/Hide.
	/// </summary>
	public void Toggle()
	{
		if (this.mShowWindow)
			this.Hide();
		else
			this.Show();
	}

	/// <summary>
	/// Show the window.
	/// </summary>
	public void Show()
	{
		this.Show(false);
	}

	/// <summary>
	/// Show the window.
	/// </summary>
	/// <param name="instant">If set to <c>true</c> instant.</param>
	public void Show(bool instant)
	{
		if (!this.enabled || !this.gameObject.activeSelf)
			return;

		// Check if the window is already shown
		if (this.mShowWindow)
		{
			this.Focus();
			return;
		}

		if (instant || !this.fading)
		{
			if (this.panel != null)
				this.panel.alpha = 1f;
			
			this.contentHolder.gameObject.SetActive(true);

			if (this.onShowComplete != null)
				this.onShowComplete();
		}
		else
		{
			this.FadeIn(this.fadeDuration);

			if (this.onShowBegin != null)
				this.onShowBegin();
		}

		this.Focus();
		this.mShowWindow = true;
	}

	/// <summary>
	/// Shows the window (Use for button event binding).
	/// </summary>
	public void ShowAlternative()
	{
		this.Show(false);
	}

	/// <summary>
	/// Hide the window.
	/// </summary>
	public void Hide()
	{
		this.Hide(false);
	}

	/// <summary>
	/// Hide the window.
	/// </summary>
	/// <param name="instant">If set to <c>true</c> instant.</param>
	public void Hide(bool instant)
	{
		if (!this.enabled || !this.gameObject.activeSelf)
			return;
		
		// Check if the window is already hidden
		if (!this.mShowWindow)
			return;

		if (instant || !this.fading)
		{
			if (this.panel != null)
				this.panel.alpha = 0f;
			
			this.contentHolder.gameObject.SetActive(false);

			if (this.onHideComplete != null)
				this.onHideComplete();
		}
		else
		{
			this.FadeOut(this.fadeDuration);

			if (this.onHideBegin != null)
				this.onHideBegin();
		}

		this.mShowWindow = false;
	}

	/// <summary>
	/// Hides the window (Use for button event binding).
	/// </summary>
	public void HideAlternative()
	{
		this.Hide(false);
	}

	/// <summary>
	/// Fades the window in.
	/// </summary>
	/// <param name="duration">Duration.</param>
	public void FadeIn(float duration)
	{
		if (!this.contentHolder.gameObject.activeSelf)
			this.contentHolder.gameObject.SetActive(true);

		if (this.animationCurrentMethod != FadeMethods.In && this.mFadeCoroutine != null)
			this.mFadeCoroutine.Stop();

		// Start the new animation
		if (this.animationCurrentMethod != FadeMethods.In)
			this.mFadeCoroutine = new UICoroutine(this, this.FadeAnimation(FadeMethods.In, duration));
	}

	/// <summary>
	/// Fades the window out.
	/// </summary>
	/// <param name="duration">Duration.</param>
	public void FadeOut(float duration)
	{
		if (!this.contentHolder.gameObject.activeSelf)
			return;

		if (this.animationCurrentMethod != FadeMethods.Out && this.mFadeCoroutine != null)
			this.mFadeCoroutine.Stop();
		
		// Start the new animation
		if (this.animationCurrentMethod != FadeMethods.Out)
			this.mFadeCoroutine = new UICoroutine(this, this.FadeAnimation(FadeMethods.Out, duration));
	}

	/*
	 * Show / Hide Fade Animation
	 */
	
	private enum FadeMethods
	{
		None,
		In,
		Out
	}

	private FadeMethods animationCurrentMethod = FadeMethods.None;
	private UICoroutine mFadeCoroutine;

	// Show / Hide fade animation coroutine
	private IEnumerator FadeAnimation(FadeMethods method, float FadeDuration)
	{
		if (this.panel == null)
			yield break;

		// Check if we are trying to fade in and the window is already shown
		if (method == FadeMethods.In && this.panel.alpha == 1f)
			yield break;
		else if (method == FadeMethods.Out && this.panel.alpha == 0f)
			yield break;
		
		// Define that animation is in progress
		this.animationCurrentMethod = method;
		
		// Get the timestamp
		float startTime = Time.time;
		
		// Determine Fade in or Fade out
		if (method == FadeMethods.In)
		{
			// Calculate the time we need to fade in from the current alpha
			float internalDuration = (FadeDuration - (FadeDuration * this.panel.alpha));
			
			// Update the start time
			startTime -= (FadeDuration - internalDuration);
			
			// Fade In
			while (Time.time < (startTime + internalDuration))
			{
				float RemainingTime = (startTime + FadeDuration) - Time.time;
				float ElapsedTime = FadeDuration - RemainingTime;
				
				// Update the alpha by the percentage of the time elapsed
				this.panel.alpha = (ElapsedTime / FadeDuration);
				
				yield return 0;
			}
			
			// Make sure it's 1
			this.panel.alpha = 1f;

			if (this.onShowComplete != null)
				this.onShowComplete();
		}
		else if (method == FadeMethods.Out)
		{
			// Calculate the time we need to fade in from the current alpha
			float internalDuration = (FadeDuration * this.panel.alpha);
			
			// Update the start time
			startTime -= (FadeDuration - internalDuration);
			
			// Fade Out
			while (Time.time < (startTime + internalDuration))
			{
				float RemainingTime = (startTime + FadeDuration) - Time.time;
				
				// Update the alpha by the percentage of the remaing time
				this.panel.alpha = (RemainingTime / FadeDuration);
				
				yield return 0;
			}
			
			// Make sure it's 0
			this.panel.alpha = 0f;

			if (this.onHideComplete != null)
				this.onHideComplete();

			this.contentHolder.gameObject.SetActive(false);
		}

		// No longer animating
		this.animationCurrentMethod = FadeMethods.None;
	}

	public class UICoroutine : IEnumerator 
	{
		private bool stop;
		
		IEnumerator enumerator;
		MonoBehaviour behaviour;
		
		public readonly Coroutine coroutine;
		
		public UICoroutine(MonoBehaviour behaviour, IEnumerator enumerator)
		{
			this.behaviour = behaviour;
			this.enumerator = enumerator;
			this.coroutine = this.behaviour.StartCoroutine(this);
		}
		
		public object Current { get { return enumerator.Current; } }
		public bool MoveNext() { return !stop && enumerator.MoveNext(); }
		public void Reset() { enumerator.Reset(); }
		public void Stop() { stop = true; }
	}
}
