using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
	/// <summary>
	/// The input action that triggers the interaction.
	/// Used for displaying which button to press in input prompts.
	/// </summary>
	InputActionReference Input { get; }

	/// <summary>
	/// Interactables with a higher priority will be chosen over others.
	/// If their priorities are the same, it'll be chosen based on distance.
	/// </summary>
	int Priority => 0;

	/// <summary>
	/// Whether the interactable is active or not.
	/// Can be useful for switches that are in their cooldown period.
	/// </summary>
	bool IsActive => true;

	/// <summary>
	/// Performs the interaction.
	/// </summary>
	void Interact();
}
