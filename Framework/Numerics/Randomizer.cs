using System;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A simple random number generator based on George Marsaglia's MWC (multiply with carry) generator.
	/// </summary>
	public class Randomizer
	{
		// These values are not magical, just the default values Marsaglia used.
		// Any pair of unsigned integers should be fine.
		private uint m_w = 521288629;
		private uint m_z = 362436069;
		
		/// <summary>
		/// Create a randomizer with seed from system time.
		/// </summary>
		public Randomizer()
		{
			SetSeedFromSystemTime();
		}

		/// <summary>
		/// Create a randomizer with the specified seed.
		/// </summary>
		public Randomizer(uint seed1, uint seed2)
		{
			SetSeed(seed1, seed2);
		}

		/// <summary>
		/// Set the seed.
		/// </summary>
		public void SetSeed(uint seed1, uint seed2)
		{
			m_w = seed1;
			m_z = seed2;
		}

		/// <summary>
		/// Produce a uniform random sample between [0, System.Int32.MaxValue-1]. Note that it will not return negative numbers.
		/// </summary>
		/// <returns>A random value between [0, System.Int32.MaxValue-1].</returns>
		public int Next()
		{
			int v = (int)(GetUint() & System.Int32.MaxValue);

			// Special case: in order to keep the interface the same as System.Random,
			// disallow System.Int32.MaxValue. Here we simply try again if we by chance
			// get this value.
			if (v == System.Int32.MaxValue)
				return Next();

			return v;
		}

		/// <summary>
		/// Produce a uniform random sample between [0, max-1].
		/// </summary>
		/// <param name="max">Maximum value. Note that the upper value is not included in the output range.</param>
		/// <returns>A random value between [0, max-1].</returns>
		public int Next(int max)
		{
			if (max < 0)
			{
				throw new ArgumentOutOfRangeException("value must be >= 0");
			}

			return (int)(NextDouble() * (double)max);
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [min, max-1].
		/// </summary>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value. Note that the upper value is not included in the output range.</param>
		/// <returns>A random value between [min, max-1].</returns>
		public int Next(int min, int max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException("min must be less than max");

			return min + (int)(NextDouble() * (double)(max - min));
		}

		/// <summary>
		/// Produce a uniform random sample that is either true or false.
		/// </summary>
		/// <returns>A random boolean value.</returns>
		public bool NextBool()
		{
			return Next(2) == 0;
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [0, 1).
		/// </summary>
		/// <returns>A value in the range [0, 1). Note that it will not return exactly 1.0.</returns>
		public double NextDouble()
		{
			// 0 <= u <= 2^32
			uint u = GetUint();
			// The magic number below is 1/(2^32 + 2).
			// The result is strictly between 0 and 1. // Olle: Actually, 0 happens.
			return (u + 1) * 2.328306435454494e-10;
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [0, max).
		/// </summary>
		/// <param name="max">Maximum value. Note that the upper value is not included in the output range.</param>
		/// <returns>A value in the range [0, max).</returns>
		public double NextDouble(float max)
		{
			return NextDouble() * max;
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [min, max).
		/// </summary>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value. Note that the upper value is not included in the output range.</param>
		/// <returns>A value in the range [min, max).</returns>
		public double NextDouble(float min, float max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException("min must be less than max");

			return min + NextDouble() * (max - min);
		}
		
		/// <summary>
		/// Get normal (Gaussian) random sample with mean 0 and standard deviation 1.
		/// </summary>
		/// <returns>A normal sample.</returns>
		public double NextNormalDouble()
		{
			// Use Box-Muller algorithm
			double u1 = NextDouble();
			double u2 = NextDouble();
			double r = Math.Sqrt(-2.0 * Math.Log(u1));
			double theta = 2.0 * Math.PI * u2;
			return r * Math.Sin(theta);
		}

		/// <summary>
		/// Get normal (Gaussian) random sample with specified mean and standard deviation.
		/// </summary>
		/// <param name="mean">Specified mean value.</param>
		/// <param name="standardDeviation">Specified standard deviation.</param>
		/// <returns>A normal sample.</returns>
		public double NextNormalDouble(double mean, double standardDeviation)
		{
			return mean + standardDeviation * NextNormalDouble();
		}

		/// <summary>
		/// Get exponential random sample with mean 1.
		/// </summary>
		/// <returns>An exponential random sample with mean 1.</returns>
		public double NextExponentialDouble()
		{
			return -Math.Log(NextDouble());
		}

		/// <summary>
		/// Get exponential random sample with specified mean.
		/// </summary>
		/// <param name="mean">Specified mean value.</param>
		/// <returns>A exponential random sample.</returns>
		public double NextExponentialDouble(double mean)
		{
			return mean * NextExponentialDouble();
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [0, 1].
		/// </summary>
		/// <remarks>Includes the max bound, unlike the double interface!</remarks>
		/// <returns>A floating point value between [0, 1].</returns>
		public float NextFloat()
		{
			uint u = GetUint();
			return (u + 1) * 2.32830644e-10f;
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [0, max].
		/// </summary>
		/// <param name="max">Maximum value.</param>
		/// <remarks>Includes the max bound, unlike the double interface!</remarks>
		/// <returns>A floating point value between [0, max].</returns>
		public float NextFloat(float max)
		{
			return NextFloat() * max;
		}

		/// <summary>
		/// Produce a uniform random sample from the interval [min, max].
		/// </summary>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <remarks>Includes the max bound, unlike the double interface!</remarks>
		/// <returns>A floating point value between [min, max].</returns>
		public float NextFloat(float min, float max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException("min must be less than max");

			return min + NextFloat() * (max - min);
		}

		/// <summary>
		/// Get normal (Gaussian) random sample with mean 0 and standard deviation 1.
		/// </summary>
		/// <returns>A normal sample.</returns>
		public float NextNormalFloat()
		{
			return (float)NextNormalDouble();
		}

		/// <summary>
		/// Get normal (Gaussian) random sample with specified mean and standard deviation.
		/// </summary>
		/// <param name="mean">Specified mean value.</param>
		/// <param name="standardDeviation">Specified standard deviation.</param>
		/// <returns>A normal sample.</returns>
		public float NextNormalFloat(float mean, float standardDeviation)
		{
			return (float)NextNormalDouble(mean, standardDeviation);
		}

		/// <summary>
		/// Get exponential random sample with mean 1.
		/// </summary>
		/// <returns>An exponential random sample with mean 1.</returns>
		public float NextExponentialFloat()
		{
			return (float)NextExponentialDouble();
		}

		/// <summary>
		/// Get exponential random sample with specified mean.
		/// </summary>
		/// <param name="mean">Specified mean value.</param>
		/// <returns>A exponential random sample.</returns>
		public float NextExponentialFloat(float mean)
		{
			return (float)NextExponentialDouble(mean);
		}

		/// <summary>
		/// Returns a random RGB color.
		/// </summary>
		///	<remarks>
		///	The alpha channel of the returned color is set to 255 (or 1.0f).
		///	</remarks>
		/// <returns>A new Color with random values for the R, G, and B components.</returns>
		public Color NextColor()
		{
			return new Color(NextFloat(), NextFloat(), NextFloat());
		}

		/// <summary>
		/// Returns a random RGBA color.
		/// </summary>
		/// <returns>A new Color with random values for the R, G, B, and A components.</returns>
		public Color NextRGBAColor()
		{
			return new Color(NextFloat(), NextFloat(), NextFloat(), NextFloat());
		}

		/// <summary>
		/// Return a unit vector with a uniform random direction.
		/// </summary>
		/// <returns>A unit vector of random direction.</returns>
		public Vector2 RandomDirection()
		{
			var v = NextDouble() * Math.PI * 2;
			return new Vector2((float)Math.Cos(v), (float)Math.Sin(v));
		}

		/// <summary>
		/// Returns a direction vector that has been randomly shifted 
		/// by +-[spread] radians, using an even distribution.
		/// </summary>
		/// <param name="initial">Must be a unit vector.</param>
		/// <param name="spread">Spread, [0-PI].</param>
		/// <returns>A unit vector.</returns>
		public Vector2 Spread(Vector2 initial, float spread)
		{
			var v = Math.Atan2(initial.Y, initial.X);
			v += (NextDouble() * spread * 2) - spread;
			return new Vector2((float)Math.Cos(v), (float)Math.Sin(v));
		}

		/// <summary>
		/// Returns a direction vector that has been randomly shifted 
		/// by +-[spread] radians, using normal (gaussian) distribution.
		/// </summary>
		/// <param name="initial">Must be a unit vector.</param>
		/// <param name="spread">Spread, [0-PI].</param>
		/// <param name="spread">Sigma, [> 0]. See http://stat.wvu.edu/SRS/Modules/Normal/normal.html.</param>
		/// <returns>A unit vector.</returns>
		public Vector2 NormalSpread(Vector2 initial, float sigma)
		{
			var v = Math.Atan2(initial.Y, initial.X);
			v += NextNormalDouble(0, (double)sigma);
			return new Vector2((float)Math.Cos(v), (float)Math.Sin(v));
		}
		
		/// <summary>
		/// This is the heart of the generator.
		/// It uses George Marsaglia's MWC algorithm to produce an unsigned integer.
		/// See http://www.bobwheeler.com/statistics/Password/MarsagliaPost.txt
		/// </summary>
		/// <returns>A random number within the full range of the uint.</returns>
		public uint GetUint()
		{
			m_z = 36969 * (m_z & 65535) + (m_z >> 16);
			m_w = 18000 * (m_w & 65535) + (m_w >> 16);
			return (m_z << 16) + m_w;
		}
		
		/// <summary>
		/// Set the seed from the current system time.
		/// </summary>
		private void SetSeedFromSystemTime()
		{
			System.DateTime dt = System.DateTime.Now;
			long x = dt.ToFileTime();

			m_w = (uint)(x >> 16);
			m_z = (uint)(x % 4294967296);
		}

		//public static void Test()
		//{
		//    // http://www.codeproject.com/KB/cs/fastrandom.aspx?display=Print
		//    Randomizer rand = new Randomizer(123, 12516);
		//    Random sysRand = new Random();

		//    int[] buckets = new int[10];
		//    int count = 10000000;

		//    for (int i = 0; i < count; i++)
		//    {
		//        buckets[rand.Next(0, buckets.Length)]++;
		//    }

		//    for (int i = 0; i < buckets.Length; i++)
		//    {
		//        Debug.WriteLine(i + ": " + (float)buckets[i] / (float)count);
		//        buckets[i] = 0;
		//    }

		//    for (int i = 0; i < count; i++)
		//    {
		//        buckets[sysRand.Next(0, buckets.Length)]++;
		//    }

		//    for (int i = 0; i < buckets.Length; i++)
		//    {
		//        Debug.WriteLine(i + ": " + (float)buckets[i] / (float)count);
		//        buckets[i] = 0;
		//    }
		//}

		// Original comments:
		// SimpleRNG is a simple random number generator based on 
		// George Marsaglia's MWC (multiply with carry) generator.
		// Although it is very simple, it passes Marsaglia's DIEHARD
		// series of random number generator tests.
		// 
		// Written by John D. Cook 
		// http://www.johndcook.com
	}
}
