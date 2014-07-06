using System.ComponentModel;

using MG.Framework.Converters;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A representation of a noise setting.
	/// </summary>
	[TypeConverter(typeof(NoiseConverter))]
	public class Noise
	{
		public float Base;
		public float Scale;
		public float Amplitude;
		public float Persistence;
		public int Period;
		public int Octaves;
		
		/// <summary>
		/// Create an empty noise object.
		/// </summary>
		public Noise()
		{

		}

		/// <summary>
		/// Create a copy of another noise object.
		/// </summary>
		/// <param name="other">Noise object to copy.</param>
		public Noise(Noise other)
		{
			Base = other.Base;
			Scale = other.Scale;
			Amplitude = other.Amplitude;
			Persistence = other.Persistence;
			Period = other.Period;
			Octaves = other.Octaves;
		}

		/// <summary>
		/// Evaluate the value of this noise object at a given point.
		/// </summary>
		/// <param name="x">The X value.</param>
		/// <returns>The result value.</returns>
		public float Evaluate(float x)
		{
			if (Period > 0)
			{
				return Base + (2 * NoiseTools.ComplexNoise(x * Scale, Octaves, Persistence, Period) - 1) * Amplitude;
			}

			return Base + (2 * NoiseTools.ComplexNoise(x * Scale, Octaves, Persistence) - 1) * Amplitude;
		}
	}
}
