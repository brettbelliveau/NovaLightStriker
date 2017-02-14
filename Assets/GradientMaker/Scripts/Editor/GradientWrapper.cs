using UnityEngine;
using UnityEditor;

using System.Reflection;
using System.Linq;

using Activator = System.Activator;
using Array = System.Array;
using Type = System.Type;

[System.Serializable]
public class GradientWrapper {
	
	/// <summary>
	/// Wrapper for <c>GradientColorKey</c>.
	/// </summary>
	public struct ColorKey {
		public Color color;
		public float time;
		
		public ColorKey(Color color, float time) {
			this.color = color;
			this.time = time;
		}
	}
	
	/// <summary>
	/// Wrapper for <c>GradientAlphaKey</c>.
	/// </summary>
	public struct AlphaKey {
		public float alpha;
		public float time;
		
		public AlphaKey(float alpha, float time) {
			this.alpha = alpha;
			this.time = time;
		}
	}	
	
	/// <summary>
	/// Type of gradient.
	/// </summary>
	public static Type s_tyGradient;
	
	/// <summary>
	/// Perform one-off setup when class is accessed for first time.
	/// </summary>
	static GradientWrapper() {
		
		s_tyGradient = typeof(Gradient);
		
	}
	
	private Gradient _gradient = new Gradient();
	
	public object GradientData {
		get { return _gradient; }
		set { _gradient = value as Gradient; }
	}
	
	public Color Evaluate(float time) {
		return _gradient.Evaluate(time);
	}
	
	public void SetKeys(ColorKey[] colorKeys, AlphaKey[] alphaKeys) {
		GradientColorKey[] actualColorKeys = null;
		GradientAlphaKey[] actualAlphaKeys = null;
		
		if (colorKeys != null)
			actualColorKeys = colorKeys.Select(key => new GradientColorKey(key.color, key.time)).ToArray();
		if (alphaKeys != null)
			actualAlphaKeys = alphaKeys.Select(key => new GradientAlphaKey(key.alpha, key.time)).ToArray();
		
		_gradient.SetKeys(actualColorKeys, actualAlphaKeys);
	}	
}