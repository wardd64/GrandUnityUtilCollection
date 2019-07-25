using System;
using UnityEngine;

/// <summary>
/// Base class for creating Data classes that can save and load 
/// parameters of a given component.
/// Make sure implementations of data classes have appropriate constructors
/// </summary>
[Serializable]
public abstract class ComponentData{

	/// <summary>
	/// Apply values saved by this Data to the given target component.
	/// </summary>
	abstract public void Apply(Component targetComponent);

	/// <summary>
	/// Apply interpolated values of this Data and the rightData
	/// to the given target component, using the given interpolation factor.
	/// </summary>
	abstract public void ApplyLerp(
		Component targetComponent, ComponentData rightData, float factor);
}
