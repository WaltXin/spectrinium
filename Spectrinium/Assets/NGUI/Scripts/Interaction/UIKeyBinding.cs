//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// This class makes it possible to activate or select something by pressing a key (such as space bar for example).
/// </summary>

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	public enum Action
	{
		PressAndClick,
		Select,
		All,
	}

	public enum Modifier
	{
		None,
		Shift,
		Control,
		Alt,
	}

	/// <summary>
	/// Key that will trigger the binding.
	/// </summary>

	public KeyCode keyCode = KeyCode.None;

	/// <summary>
	/// Modifier key that must be active in order for the binding to trigger.
	/// </summary>

	public Modifier modifier = Modifier.None;

	/// <summary>
	/// Action to take with the specified key.
	/// </summary>

	public Action action = Action.PressAndClick;

	bool mIgnoreUp = false;
	bool mIsInput = false;
	bool mPress = false;

	/// <summary>
	/// If we're bound to an input field, subscribe to its Submit notification.
	/// </summary>

	protected virtual void Start ()
	{
		UIInput input = GetComponent<UIInput>();
		mIsInput = (input != null);
		if (input != null) EventDelegate.Add(input.onSubmit, OnSubmit);
	}

	/// <summary>
	/// Ignore the KeyUp message if the input field "ate" it.
	/// </summary>

	protected virtual void OnSubmit () { if (UICamera.currentKey == keyCode && IsModifierActive()) mIgnoreUp = true; }

	/// <summary>
	/// Convenience function that checks whether the required modifier key is active.
	/// </summary>

	protected virtual bool IsModifierActive ()
	{
		if (modifier == Modifier.None) return true;

		if (modifier == Modifier.Alt)
		{
			if (Input.GetKey(KeyCode.LeftAlt) ||
				Input.GetKey(KeyCode.RightAlt)) return true;
		}
		else if (modifier == Modifier.Control)
		{
			if (Input.GetKey(KeyCode.LeftControl) ||
				Input.GetKey(KeyCode.RightControl)) return true;
		}
		else if (modifier == Modifier.Shift)
		{
			if (Input.GetKey(KeyCode.LeftShift) ||
				Input.GetKey(KeyCode.RightShift)) return true;
		}
		return false;
	}

	/// <summary>
	/// Process the key binding.
	/// </summary>

	protected virtual void Update ()
	{
		if (keyCode == KeyCode.None || !IsModifierActive()) return;

		if (action == Action.PressAndClick || action == Action.All)
		{
			if (UICamera.inputHasFocus) return;

			UICamera.currentTouch = UICamera.controller;
			UICamera.currentScheme = UICamera.ControlScheme.Mouse;
			UICamera.currentTouch.current = gameObject;

			if (Input.GetKeyDown(keyCode))
			{
				mPress = true;
				OnBindingPress(true);
			}

			if (Input.GetKeyUp(keyCode))
			{
				OnBindingPress(false);

				if (mPress)
				{
					OnBindingClick();
					mPress = false;
				}
			}
			UICamera.currentTouch.current = null;
		}

		if (action == Action.Select || action == Action.All)
		{
			if (Input.GetKeyUp(keyCode))
			{
				if (mIsInput)
				{
					if (!mIgnoreUp && !UICamera.inputHasFocus)
					{
						UICamera.selectedObject = gameObject;
					}
					mIgnoreUp = false;
				}
				else
				{
					UICamera.selectedObject = gameObject;
				}
			}
		}
	}

	protected virtual void OnBindingPress (bool pressed) { UICamera.Notify(gameObject, "OnPress", pressed); }
	protected virtual void OnBindingClick () { UICamera.Notify(gameObject, "OnClick", null); }
}