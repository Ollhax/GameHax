using System;
using System.Collections.Generic;

namespace MG.Framework.Particle
{
	/// <summary>
	/// A holder of particle data. This class is set up to keep arrays of data required to represent a single particle system.
	/// </summary>
	public class ParticleData
	{
		private Dictionary<string, GenericDataArray> attributes = new Dictionary<string, GenericDataArray>();
		private int maxParticles;

		/// <summary>
		/// Create this data structure with room for a specified initial number of particles.
		/// </summary>
		/// <param name="maxParticles">Initial number of particles.</param>
		public ParticleData(int maxParticles)
		{
			this.maxParticles = maxParticles;
		}

		/// <summary>
		/// Return the number of currently active particles.
		/// </summary>
		public int ActiveParticles;

		/// <summary>
		/// Get or set the maximum number of particles this data structure can hold.
		/// </summary>
		/// <remarks>You may not reduce the maximum particle count.</remarks>
		public int MaxParticles
		{
			get { return maxParticles; }

			set
			{
				maxParticles = value;
				foreach (var kvp in attributes)
				{
					var array = kvp.Value;
					array.Resize(maxParticles);
				}
			}
		}

		/// <summary>
		/// Increase the maximum number of particles.
		/// </summary>
		public void Resize()
		{
			MaxParticles = (int)(MaxParticles * 1.5f);
		}

		/// <summary>
		/// Register a list of attributes.
		/// </summary>
		/// <typeparam name="T">Type of attributes.</typeparam>
		/// <param name="attribute">Name of the attribute list.</param>
		/// <returns>The newly created list of attributes.</returns>
		public List<T> Register<T>(string attribute)
		{
			if (attributes.ContainsKey(attribute)) throw new ArgumentException("Attribute already registered: " + attribute);

			var data = new GenericDataArray(typeof(T), MaxParticles);
			attributes[attribute] = data;
			return data.Get<T>();
		}

		/// <summary>
		/// Get a specified list of attributes.
		/// </summary>
		/// <typeparam name="T">The expected type of attributes.</typeparam>
		/// <param name="attribute">Name of the attribute list.</param>
		/// <returns>A list of attributes.</returns>
		public List<T> Get<T>(string attribute)
		{
			return attributes[attribute].Get<T>();
		}
		
		/// <summary>
		/// Move all attributes from one index to another, effectively overwriting the particle at the destination index.
		/// </summary>
		/// <param name="source">Source index.</param>
		/// <param name="destination">Destination index.</param>
		public void Move(int source, int destination)
		{
			foreach (var kvp in attributes)
			{
				var array = kvp.Value;
				array.Move(source, destination);
			}
		}

		/// <summary>
		/// Shuffle all attributes one index forward or backward.
		/// </summary>
		/// <param name="start">Start index of attributes to move.</param>
		/// <param name="end">End index of attributes to move.</param>
		public void Shuffle(int start, int end)
		{
			foreach (var kvp in attributes)
			{
				var array = kvp.Value;
				array.Shuffle(start, end);
			}
		}
	}
}
