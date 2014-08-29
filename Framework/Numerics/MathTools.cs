using System;
using System.Diagnostics;

namespace MG.Framework.Numerics
{
	/// <summary>
	/// A set of helpful math tools.
	/// </summary>
	public static class MathTools
	{
		static Randomizer random = new Randomizer();

		///////////////////////////////////////////////////////////////////////////
		// Relationship between angles and coordinate system:
		//
		//                           -y
		//                         
		//                          3*PI/2
		//                            |
		//                            |
		//                            |
		//                            |
		//                            |
		//-x        PI  --------------+-------------- 0  +x
		//                            |
		//                            |
		//                            |
		//    ^ +ROT                  |
		//     \                      |
		//      \                     |
		//       \__                    
		//                          PI/2
		// 
		//                           +y
		//////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Represents the value of pi.
		/// </summary>
		public const float Pi = 3.141593f;

		/// <summary>
		/// Represents the value of pi times two.
		/// </summary>
		public const float TwoPi = 6.283185f;

		/// <summary>
		/// Represents the value of pi divided by two.
		/// </summary>
		public const float PiOver2 = 1.570796f;

		/// <summary>
		/// Represents the value of pi divided by four.
		/// </summary>
		public const float PiOver4 = 0.7853982f;

		/// <summary>
		/// Minimal floating point value.
		/// </summary>
		public static readonly float Epsilon = 0.00001f;				
		
		/// <summary>
		/// Fetch the default randomizer.
		/// </summary>
		public static Randomizer Random()
		{
			return random;
		}

		/// <summary>
		/// Rounding method that rounds any decimals to the next integer value furthest away from zero.
		/// </summary>
		/// <param name="value">Value to round.</param>
		/// <returns>Rounded value equal to ceil(abs(val)) * sign(val).</returns>
		public static float RoundBounds(float value)
		{
			return (float)Math.Ceiling(Math.Abs(value)) * Math.Sign(value);
		}
		
		/// <summary>
		/// Clamp value between min and max.
		/// </summary>
		/// <param name="value">Value to clamp.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <returns>Clamped value.</returns>
		public static T Clamp<T>(T value, T min, T max)
			where T : System.IComparable<T>
		{
			T result = value;
#if DEBUG
			if (max.CompareTo(min) < 0)
			{
				throw new Exception("max must be greater than or equal to min");
			}
#endif

			if (value.CompareTo(max) > 0)
				result = max;
			else if (value.CompareTo(min) < 0)
				result = min;
			return result;
		}

		/// <summary>
		/// Clamp between 0 and 1.
		/// </summary>
		/// <param name="value">Value to clamp.</param>
		/// <returns>Clamped value.</returns>
		public static float ClampNormal(float value)
		{
			if (value > 1) return 1;
			else if (value < 0) return 0;
			return value;
		}
				
		/// <summary>
		/// Swap the two input values.
		/// </summary>
		/// <param name="value">Value one.</param>
		/// <param name="y">Value two</param>
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		/// <summary>
		/// Test if two floats are equal.
		/// </summary>
		public static bool Equals(float v1, float v2)
		{
			return Math.Abs(v1 - v2) < Epsilon;
		}

		/// <summary>
		/// Test if two floats are equal, allowing a specified treshold.
		/// </summary>
		public static bool Equals(float v1, float v2, float treshold)
		{
			return Math.Abs(v1 - v2) < treshold;
		}

		/// <summary>
		/// Test if two doubles are equal.
		/// </summary>
		public static bool Equals(double v1, double v2)
		{
			return Math.Abs(v1 - v2) < Epsilon;
		}

		/// <summary>
		/// Test if two doubles are equal, allowing a specified treshold.
		/// </summary>
		public static bool Equals(double v1, double v2, double treshold)
		{
			return Math.Abs(v1 - v2) < treshold;
		}

		/// <summary>
		/// Test if two vectors are equal.
		/// </summary>
		public static bool Equals(Vector2 v1, Vector2 v2)
		{
			return Math.Abs(v1.X - v2.X) < Epsilon && Math.Abs(v1.Y - v2.Y) < Epsilon;
		}

		/// <summary>
		/// Test if two vectors are equal.
		/// </summary>
		public static bool Equals(Vector3 v1, Vector3 v2)
		{
			return Math.Abs(v1.X - v2.X) < Epsilon && Math.Abs(v1.Y - v2.Y) < Epsilon && Math.Abs(v1.Z - v2.Z) < Epsilon;
		}

		// <summary>
		/// Test if two rectangles are equal.
		/// </summary>
		public static bool Equals(RectangleF r1, RectangleF r2)
		{
			return Math.Abs(r1.X - r2.X) < Epsilon && Math.Abs(r1.Y - r2.Y) < Epsilon && Math.Abs(r1.Width - r2.Width) < Epsilon && Math.Abs(r1.Height - r2.Height) < Epsilon;
		}

		// <summary>
		/// Test if two matrices are equal.
		/// </summary>
		public static bool Equals(ref Matrix r1, ref Matrix r2)
		{
			return Math.Abs(r1.M11 - r2.M11) < Epsilon && Math.Abs(r1.M12 - r2.M12) < Epsilon
			       && Math.Abs(r1.M13 - r2.M13) < Epsilon && Math.Abs(r1.M14 - r2.M14) < Epsilon
			       && Math.Abs(r1.M21 - r2.M21) < Epsilon && Math.Abs(r1.M22 - r2.M22) < Epsilon
			       && Math.Abs(r1.M23 - r2.M23) < Epsilon && Math.Abs(r1.M24 - r2.M24) < Epsilon
			       && Math.Abs(r1.M31 - r2.M31) < Epsilon && Math.Abs(r1.M32 - r2.M32) < Epsilon
			       && Math.Abs(r1.M33 - r2.M33) < Epsilon && Math.Abs(r1.M34 - r2.M34) < Epsilon
			       && Math.Abs(r1.M41 - r2.M41) < Epsilon && Math.Abs(r1.M42 - r2.M42) < Epsilon
			       && Math.Abs(r1.M43 - r2.M43) < Epsilon && Math.Abs(r1.M44 - r2.M44) < Epsilon;
		}

		/// <summary>
		/// Convert degrees to radians.
		/// </summary>
		/// <param name="degrees">Degrees.</param>
		/// <returns>Angle in radians.</returns>
		public static float ToRadians(float degrees)
		{
			return (degrees / 360.0f) * TwoPi;
		}

		/// <summary>
		/// Convert radians to degrees.
		/// </summary>
		/// <param name="radians">Radians.</param>
		/// <returns>Angle in degrees.</returns>
		public static float ToDegrees(float radians)
		{
			return (radians / TwoPi) * 360.0f;
		}

		/// <summary>
		/// Returns a direction vector from a specified angle.
		/// </summary>
		/// <param name="angle">Input angle.</param>
		/// <returns>A unit vector in the direction specified by the input angle.</returns>
		public static Vector2 FromAngle(float angle)
		{
			return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		}

		/// <summary>
		/// Returns a direction vector that has been shifted by delta radians.
		/// </summary>
		/// <param name="initial">Must be a unit vector.</param>
		/// <param name="delta">Change in angle, measured in radians.</param>
		public static Vector2 DeltaDirection(Vector2 initial, float delta)
		{
			double v = Math.Atan2(initial.Y, initial.X);
			v += delta;
			return new Vector2((float)Math.Cos(v), (float)Math.Sin(v));
		}

		/// <summary>
		/// Returns closest distance, measured in radians, between thisAngle and targetAngle.
		/// Range: [-PI, PI]
		/// </summary>
		/// <param name="thisAngle">Target angle.</param>
		/// <param name="targetAngle">Angle to check distance to.</param>
		public static float GetSmallestAngleDelta(float thisAngle, float targetAngle)
		{
			bool targetNegative = targetAngle < 0;

			// Make sure that the angles are in the correct bounds
			thisAngle = WrapAngle(thisAngle);
			targetAngle = WrapAngle(targetAngle);

			// Calculate clockwise and counter-clockwise distances between the angles,
			// retaining the correct angles.
			float d1 = targetAngle - thisAngle;
			float d2 = d1 > 0 ? -(TwoPi - d1) : TwoPi + d1;
			float ad1 = Math.Abs(d1);
			float ad2 = Math.Abs(d2);

			// In the case of same distances, select the one with the same sign as the target			
			if (Math.Abs(ad1 - ad2) < Epsilon)
			{
				return targetNegative ? -ad1 : ad1;
			}

			// Else, select the lowest distance
			return ad1 <= ad2 ? d1 : d2;
		}

		/// <summary>
		/// Returns closest distance, measured in radians, between thisAngle and targetAngle. Operates within the specified segment, 
		/// returning delta to closest edge if target is outside the segment.
		/// Range: [-PI, PI]
		/// </summary>
		/// <param name="thisAngle">Target angle.</param>
		/// <param name="targetAngle">Angle to check distance to.</param>
		/// <param name="segmentCenter">Center of the segment in which the returned value must lie.</param>
		/// <param name="segmentRange">Range of the segment in which the returned value must lie.</param>
		public static float GetSmallestAngleDelta(float thisAngle, float targetAngle, float segmentCenter, float segmentRange)
		{
			if (segmentRange >= Pi)
			{
				return GetSmallestAngleDelta(thisAngle, targetAngle);
			}

			// If target is out of range, reset the target to the edge closest to the original target. Then continue as normal.
			if (!AngleWithinSegment(targetAngle, segmentCenter, segmentRange))
			{
				var d1 = GetSmallestAngleDelta(targetAngle, WrapAngle(segmentCenter + segmentRange));
				var d2 = GetSmallestAngleDelta(targetAngle, WrapAngle(segmentCenter - segmentRange));

				targetAngle = Math.Abs(d1) < Math.Abs(d2) ? segmentCenter + segmentRange : segmentCenter - segmentRange;
			}

			// See if we're on the same half of the segment as the target. If so, simply return smallest angle. If not, return 
			// angle to center + angle from center to target.					
			var halfSegmentCenter = WrapAngle(segmentCenter + segmentRange / 2);
			var thisSide = AngleWithinSegment(thisAngle, halfSegmentCenter, segmentRange / 2);
			var targetSide = AngleWithinSegment(targetAngle, halfSegmentCenter, segmentRange / 2);

			if (thisSide ^ targetSide)
			{
				return GetSmallestAngleDelta(thisAngle, segmentCenter) + GetSmallestAngleDelta(segmentCenter, targetAngle);
			}

			return GetSmallestAngleDelta(thisAngle, targetAngle);
		}

		/// <summary>
		/// Returns true if the target angle is within the specified circle segment.
		/// </summary>
		/// <param name="thisAngle">Test angle.</param>
		/// <param name="segmentCenter">Center of the target segment.</param>
		/// <param name="segmentRange">Range of the segment, +- from the center. Measured in radians.</param>
		public static bool AngleWithinSegment(float thisAngle, float segmentCenter, float segmentRange)
		{
#if DEBUG
			if (segmentRange < 0)
			{
				throw new ArgumentException("invalid segment range");
			}
#endif
			// Segment is over the whole circle, cannot fail
			if (segmentRange >= Pi)
			{
				return true;
			}
						
			float val = Math.Abs(GetSmallestAngleDelta(thisAngle, segmentCenter));
			return val <= segmentRange + Epsilon;
		}

		/// <summary>
		/// Returns a direction vector that is clamped to reside in a segment 
		/// specified by a midpoint and range.
		/// </summary>
		/// <param name="initial">Must be a unit vector.</param>
		/// <param name="segmentCenter">Angle of segment segmentCenter (radians).</param>
		/// <param name="segmentRange">Accepted distance from segmentCenter on either sides (radians)</param>
		public static Vector2 ClampDirection(Vector2 initial, float segmentCenter, float segmentRange)
		{
#if DEBUG
			if (segmentRange < 0)
			{
				throw new ArgumentException("invalid segment range");
			}
#endif
			float v = (float)Math.Atan2(initial.Y, initial.X);

			// Outside of segment?
			if (!AngleWithinSegment(v, segmentCenter, segmentRange))
			{
				float max = segmentCenter + segmentRange;
				float min = segmentCenter - segmentRange;

				// Clamp to the closest side, either min or max
				v = Math.Abs(GetSmallestAngleDelta(v, min)) < Math.Abs(GetSmallestAngleDelta(v, max)) ? min : max;
			}

			return new Vector2((float)Math.Cos((double)v), (float)Math.Sin((double)v));
		}

		/// <summary>
		/// Returns a direction that is clamped to reside in a segment 
		/// specified by a midpoint and range.
		/// </summary>
		/// <param name="initial">Start angle.</param>
		/// <param name="segmentCenter">Center of segment to clamp within.</param>
		/// <param name="segmentRange">Range from center to clamp within.</param>
		public static float ClampDirection(float initial, float segmentCenter, float segmentRange)
		{
#if DEBUG
			if (segmentRange < 0)
			{
				throw new ArgumentException("invalid segment range");
			}
#endif
			
			// Outside of segment?
			if (!AngleWithinSegment(initial, segmentCenter, segmentRange))
			{
				float max = segmentCenter + segmentRange;
				float min = segmentCenter - segmentRange;

				// Clamp to the closest side, either min or max
				initial = Math.Abs(GetSmallestAngleDelta(initial, min)) < Math.Abs(GetSmallestAngleDelta(initial, max)) ? min : max;
			}

			return initial;
		}

		/// <summary>
		/// Wrap angle between [0, 2 * PI].
		/// </summary>
		/// <param name="angle">Input angle.</param>
		/// <returns>Angle between 0 and 2 * PI.</returns>
		public static float WrapAngle(float angle)
		{
			var val = angle % TwoPi;
			if (val < 0)
			{
				return TwoPi + val;
			}
			return val;
		}

		/// <summary>
		/// Interpolate angles. Like normal lerp, but wraps around 2 * PI.
		/// </summary>
		/// <param name="from">Interpolate from this value.</param>
		/// <param name="to">Interpolate to this value.</param>
		/// <param name="method">Interpolate using this method.</param>
		/// <param name="fraction">Interpolator, usually between 0-1.</param>
		/// <returns>The output angle.</returns>
		public static float InterpolateAngle(float from, float to, Tween method, float fraction)
		{
			var delta = GetSmallestAngleDelta(from, to);
			return from + Interpolate(fraction, method) * delta;
		}
		
		/// <summary>
		/// Interpolate vectors.
		/// </summary>
		/// <param name="from">Interpolate from this value.</param>
		/// <param name="to">Interpolate to this value.</param>
		/// <param name="method">Interpolate using this method.</param>
		/// <param name="fraction">Interpolator, usually between 0-1.</param>
		/// <returns>The output angle.</returns>
		public static Vector2 Interpolate(Vector2 from, Vector2 to, Tween method, float fraction)
		{
			var diff = to - from;
			var len = diff.Length();

			if (len == 0)
				return from;

			return from + diff.Normalized(fraction) * Interpolate(fraction, method);
		}
		
		/// <summary>
		/// Linearly interpolate between the source and target value.
		/// </summary>
		/// <param name="from">Interpolate from this value.</param>
		/// <param name="to">Interpolate to this value.</param>
		/// <param name="fraction">Interpolator, usually between 0-1.</param>
		/// <returns>The interpolated value.</returns>
		public static float Lerp(float from, float to, float fraction)
		{
			return from + fraction * to - from;
		}

		/// <summary>
		/// Linearly interpolate vectors.
		/// </summary>
		/// <param name="from">Interpolate from this value.</param>
		/// <param name="to">Interpolate to this value.</param>
		/// <param name="fraction">Interpolator, usually between 0-1.</param>
		/// <returns>The output angle.</returns>
		public static Vector2 Lerp(Vector2 from, Vector2 to, float fraction)
		{
			var diff = to - from;
			var len = diff.Length();

			if (len == 0)
				return from;

			return from + diff.Normalized(len) * fraction * len;
		}

		/// <summary>
		/// Linearly interpolate vectors.
		/// </summary>
		/// <param name="from">Interpolate from this value.</param>
		/// <param name="to">Interpolate to this value.</param>
		/// <param name="fraction">Interpolator, usually between 0-1.</param>
		/// <returns>The output angle.</returns>
		public static Vector3 Lerp(Vector3 from, Vector3 to, float fraction)
		{
			var diff = to - from;
			var len = diff.Length();

			if (len == 0)
				return from;

			return from + diff.Normalized(len) * fraction * len;
		}

		/// <summary>
		/// Find the smallest positive integer congruent to value modulo mod
		/// </summary>
		/// <param name="value">Any value.</param>
		/// <param name="mod">A positive value.</param>
		public static int PositiveModulo(int value, int mod)
		{
#if DEBUG
			if (mod < 0)
			{
				throw new ArgumentException("mod value must be positive");
			}
#endif
			return ((value % mod) + mod) % mod;
		}

		/// <summary>
		/// Interpolate a value using the specified tweening method.
		/// </summary>
		public static float Interpolate(float value, Tween method)
		{
			switch (method)
			{
				case Tween.One:
					return 1;
				case Tween.Zero:
					return 0;
				case Tween.SmoothStep:
					return SmoothStep(0, 1, value);
				case Tween.Linear:
					return value;
				case Tween.Quadratic:
					return value * value;
				case Tween.SmoothQuadratic:
					return 1 - (value - 1) * (value - 1);
				case Tween.InvSmoothStep:
					return 1 - SmoothStep(0, 1, value);
				case Tween.InvLinear:
					return 1 - value;
				case Tween.InvQuadratic:
					return 1 - value * value;
				case Tween.InvSmoothQuadratic:
					return (value - 1) * (value - 1);
			}

			throw new ArgumentException("Unknown tweening method.");
		}

		/// <summary>
		/// Interpolate between two specified segments. You specify a separation line and interpolation
		/// methods for each segment.
		/// </summary>
		public static float Interpolate2(float value, float a, Tween first, Tween second)
		{
			Debug.Assert(a > 0 && a < 1);

			if (value < a)
			{
				return Interpolate(value / a, first);
			}
			else
			{
				return Interpolate((value - a) / (1 - a), second);
			}
		}
		
		/// <summary>
		/// Interpolate between three specified segments. You specify two separation lines, a and b, and interpolation
		/// methods for each segment. For example, a = 0.25, b = 0.75, first = Linear, second = One, third = InvLinear.
		/// </summary>
		public static float Interpolate3(float value, float a, float b, Tween first, Tween second, Tween third)
		{
			Debug.Assert(a >= 0 && a < 1);
			Debug.Assert(b > 0 && b < 1);
			Debug.Assert(a < b);
									
			if (value < a)
			{
				return Interpolate(value / a, first);
			}
			else if (value < b)
			{
				return Interpolate((value - a) / (b - a), second);
			}
			else
			{
				return Interpolate((value - b) / (1 - b), third);
			}
		}

		/// <summary>
		/// Interpolate between four specified segments. You specify trhee separation lines, a, b and c, and interpolation
		/// methods for each segment.
		/// </summary>
		public static float Interpolate4(float value, float a, float b, float c, Tween first, Tween second, Tween third, Tween fourth)
		{
			Debug.Assert(a > 0 && a < 1);
			Debug.Assert(b > 0 && b < 1);
			Debug.Assert(c > 0 && c < 1);
			Debug.Assert(a < b);
			Debug.Assert(b < c);

			if (value < a)
			{
				return Interpolate(value / a, first);
			}
			else if (value < b)
			{
				return Interpolate((value - a) / (b - a), second);
			}
			else if (value < c)
			{
				return Interpolate((value - b) / (c - b), third);
			}
			else
			{
				return Interpolate((value - c) / (1 - c), fourth);
			}
		}

		/// <summary>
		/// Interpolate using the smoothstep function.
		/// </summary>
		/// <param name="value1">Start value.</param>
		/// <param name="value2">End value.</param>
		/// <param name="amount">Interpolation factor.</param>
		/// <returns>Interpolated value.</returns>
		public static float SmoothStep(float value1, float value2, float amount)
		{
			var result = Clamp(amount, 0.0f, 1.0f);
			return Hermite(value1, 0f, value2, 0f, result);
		}

		/// <summary>
		/// Interpolate using a Hermite spline.
		/// </summary>
		/// <param name="value1">Start position.</param>
		/// <param name="tangent1">Start tangent.</param>
		/// <param name="value2">End position.</param>
		/// <param name="tangent2">End tangent.</param>
		/// <param name="amount">Weight factor.</param>
		/// <returns></returns>
		public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
		{
			float a2 = amount * amount;
			float asqr3 = amount * a2;
			float a3 = a2 + a2 + a2;

			return (value1 * (((asqr3 + asqr3) - a3) + 1f)) +
				   (value2 * ((-2f * asqr3) + a3)) +
				   (tangent1 * ((asqr3 - (a2 + a2)) + amount)) +
				   (tangent2 * (asqr3 - a2));
		}
		
		/// <summary>
		/// A simple square wave function.
		/// </summary>
		/// <param name="input">Input value with the same range as Math.Sin</param>
		/// <returns>1 or -1.</returns>
		public static float SquareWave(float input)
		{
			return Math.Sign(Math.Sin(input));
		}

		/// <summary>
		/// Similar to a square wave function, but the range is normalized between 0-1 and the output is a boolean rather than -1 or 1. Also,
		/// this function has a customizable frequency setting.
		/// </summary>
		/// <param name="input">Input value, usually ranging between [0-1].</param>
		/// <param name="frequency">Number of blinks (on period + off period) per input unit. I.e, Input 1 returns true between [0-0.5], false between [0.5-1.0]. Input 0.5 returns true between [0-1], then false [1-2], etc.</param>
		/// <returns>True or false.</returns>
		public static bool Blink(double input, double frequency)
		{
			return Math.Sin(input * TwoPi * frequency) > 0;
		}

		/// <summary>
		/// Like Blink, but with a linearly interpolated frequency, set at [0] (start) and [1] (end). If the input exceeds the normal range, the frequecy is extrapolated.
		/// </summary>
		/// <param name="input">Input value, usually ranging between [0-1].</param>
		/// <param name="frequencyStart">Number of blinks (on period + off period) per input unit at the start (input = 0)</param>
		/// <param name="frequencyEnd">Number of blinks (on period + off period) per input unit at the end (input = 1)</param>
		/// <returns>True or false.</returns>
		public static bool Blink(float input, float frequencyStart, float frequencyEnd)
		{
			var k = frequencyEnd - frequencyStart;
			var c = frequencyStart;
			var freq = k * input + c;
			return Math.Sin(input * TwoPi * freq) > 0;
		}
		
		/// <summary>
		/// Simple, minimal affine transformation of a vector, given translation and rotations.
		/// </summary>
		/// <param name="vector">Vector to transform.</param>
		/// <param name="translationX">Translation in the X axis.</param>
		/// <param name="translationY">Translation in the Y axis.</param>
		/// <param name="rotation">Rotational value.</param>
		/// <returns>A transformed vector.</returns>
		public static Vector2 AffineTransform(Vector2 vector, float translationX, float translationY, float rotation)
		{
			var cos = (float)Math.Cos(rotation);
			var sin = (float)Math.Sin(rotation);
			return new Vector2(cos * vector.X - sin * vector.Y + translationX, +sin * vector.X + cos * vector.Y + translationY);
		}

		/// <summary>
		/// Simple, minimal inverse affine transformation of a vector, given translation and rotations.
		/// </summary>
		/// <param name="vector">Vector to transform.</param>
		/// <param name="translationX">Translation in the X axis.</param>
		/// <param name="translationY">Translation in the Y axis.</param>
		/// <param name="rotation">Rotational value.</param>
		/// <returns>A transformed vector.</returns>
		public static Vector2 InverseAffineTransform(Vector2 vector, float translationX, float translationY, float rotation)
		{
			var cos = (float)Math.Cos(rotation);
			var sin = (float)Math.Sin(rotation);
			var x = vector.X - translationX;
			var y = vector.Y - translationY;
			return new Vector2(cos * x + sin * y, cos * y - sin*x);
		}

		/// <summary>
		/// Create an affine matrix with the specified scale, X rotation and translation.
		/// </summary>
		public static Matrix Create2DAffineMatrix(float translationX, float translationY, float scaleX, float scaleY, float rotation)
		{
			float sinX = 0;
			float cosX = 1;
			if (rotation != 0)
			{
				sinX = (float)Math.Sin(rotation);
				cosX = (float)Math.Cos(rotation);
			}

			return new Matrix(
				scaleX * cosX, scaleX * sinX, 0, 0,
				scaleY * -sinX, scaleY * cosX, 0, 0,
				0, 0, 1, 0,
				translationX, translationY, 0, 1
				);
		}

		/// <summary>
		/// Create an affine matrix with the specified scale, X rotation and translation.
		/// </summary>
		public static Matrix Create3DAffineMatrix(float translationX, float translationY, float translationZ, float scaleX, float scaleY, float scaleZ, float rotation)
		{
			float sinX = 0;
			float cosX = 1;
			if (rotation != 0)
			{
				sinX = (float)Math.Sin(rotation);
				cosX = (float)Math.Cos(rotation);
			}

			return new Matrix(
				scaleX * cosX, scaleX * sinX, 0, 0,
				scaleY * -sinX, scaleY * cosX, 0, 0,
				0, 0, scaleZ, 0,
				translationX, translationY, translationZ, 1
				);
		}

		/// <summary>
		/// Do the 2D cross product 
		/// </summary>
		/// <param name="v1">First vector.</param>
		/// <param name="v2">Second vector.</param>
		/// <returns>Cross product value between the two vectors.</returns>
		public static float Cross(Vector2 v1, Vector2 v2)
		{
			return v1.X * v2.Y - v1.Y * v2.X;
		}
		
		/// <summary>
		/// Vector2 dummy.
		/// </summary>
		public static Vector2 Vector2Dummy;

		/// <summary>
		/// Vector3 dummy.
		/// </summary>
		public static Vector3 Vector3Dummy;
		
		/// <summary>
		/// Float dummy.
		/// </summary>
		public static float FloatDummy;

		/// <summary>
		/// Line dummy.
		/// </summary>
		public static Line LineDummy;
	}
}
