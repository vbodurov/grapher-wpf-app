using System;
using System.ComponentModel;

namespace GrapherApp.UI.Model
{
    public struct Vector2
	{
		public const float kEpsilon = 1E-05f;
		/// <summary>
		///   <para>X component of the vector.</para>
		/// </summary>
		public float x;
		/// <summary>
		///   <para>Y component of the vector.</para>
		/// </summary>
		public float y;
		public float this[int index]
		{
			get
			{
				if (index == 0)
				{
					return this.x;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2 index!");
				}
				return this.y;
			}
			set
			{
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2 index!");
					}
					this.y = value;
				}
				else
				{
					this.x = value;
				}
			}
		}
		/// <summary>
		///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
		/// </summary>
		public Vector2 normalized
		{
			get
			{
				Vector2 result = new Vector2(this.x, this.y);
				result.Normalize();
				return result;
			}
		}
		/// <summary>
		///   <para>Returns the length of this vector (Read Only).</para>
		/// </summary>
		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(this.x * this.x + this.y * this.y);
			}
		}
		/// <summary>
		///   <para>Returns the squared length of this vector (Read Only).</para>
		/// </summary>
		public float sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y;
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(0, 0).</para>
		/// </summary>
		public static Vector2 zero
		{
			get
			{
				return new Vector2(0f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(1, 1).</para>
		/// </summary>
		public static Vector2 one
		{
			get
			{
				return new Vector2(1f, 1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(0, 1).</para>
		/// </summary>
		public static Vector2 up
		{
			get
			{
				return new Vector2(0f, 1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(0, -1).</para>
		/// </summary>
		public static Vector2 down
		{
			get
			{
				return new Vector2(0f, -1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(-1, 0).</para>
		/// </summary>
		public static Vector2 left
		{
			get
			{
				return new Vector2(-1f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector2(1, 0).</para>
		/// </summary>
		public static Vector2 right
		{
			get
			{
				return new Vector2(1f, 0f);
			}
		}
		/// <summary>
		///   <para>Constructs a new vector with given x, y components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		/// <summary>
		///   <para>Set x and y components of an existing Vector2.</para>
		/// </summary>
		/// <param name="new_x"></param>
		/// <param name="new_y"></param>
		public void Set(float new_x, float new_y)
		{
			this.x = new_x;
			this.y = new_y;
		}
		/// <summary>
		///   <para>Linearly interpolates between vectors a and b by t.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
		{
		    t = t < 0 ? 0 : t > 1 ? 1 : t;
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}
		/// <summary>
		///   <para>Linearly interpolates between vectors a and b by t.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
		{
			return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
		}
		/// <summary>
		///   <para>Moves a point current towards target.</para>
		/// </summary>
		/// <param name="current"></param>
		/// <param name="target"></param>
		/// <param name="maxDistanceDelta"></param>
		public static Vector2 MoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
		{
			Vector2 a = target - current;
			float magnitude = a.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0f)
			{
				return target;
			}
			return current + a / magnitude * maxDistanceDelta;
		}
		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector2 Scale(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x * b.x, a.y * b.y);
		}
		/// <summary>
		///   <para>Multiplies every component of this vector by the same component of scale.</para>
		/// </summary>
		/// <param name="scale"></param>
		public void Scale(Vector2 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}
		/// <summary>
		///   <para>Makes this vector have a magnitude of 1.</para>
		/// </summary>
		public void Normalize()
		{
			float magnitude = this.magnitude;
			if (magnitude > 1E-05f)
			{
				this /= magnitude;
			}
			else
			{
				this = Vector2.zero;
			}
		}
		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public override string ToString()
		{
			return String.Format("({0:F1}, {1:F1})", new object[]
			{
				this.x,
				this.y
			});
		}
		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString(string format)
		{
			return String.Format("({0}, {1})", new object[]
			{
				this.x.ToString(format),
				this.y.ToString(format)
			});
		}
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
		}
		public override bool Equals(object other)
		{
			if (!(other is Vector2))
			{
				return false;
			}
			Vector2 vector = (Vector2)other;
			return this.x.Equals(vector.x) && this.y.Equals(vector.y);
		}
		/// <summary>
		///   <para>Reflects a vector off the vector defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
		{
			return -2f * Vector2.Dot(inNormal, inDirection) * inNormal + inDirection;
		}
		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static float Dot(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}
		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector2 a, Vector2 b)
		{
			return (a - b).magnitude;
		}
		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="maxLength"></param>
		public static Vector2 ClampMagnitude(Vector2 vector, float maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}
		public static float SqrMagnitude(Vector2 a)
		{
			return a.x * a.x + a.y * a.y;
		}
		public float SqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}
		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector2 Min(Vector2 lhs, Vector2 rhs)
		{
			return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
		}

		public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Math.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			Vector2 vector = current - target;
			Vector2 vector2 = target;
			float maxLength = maxSpeed * smoothTime;
			vector = Vector2.ClampMagnitude(vector, maxLength);
			target = current - vector;
			Vector2 vector3 = (currentVelocity + num * vector) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3) * d;
			Vector2 vector4 = target + (vector + vector3) * d;
			if (Vector2.Dot(vector2 - current, vector4 - vector2) > 0f)
			{
				vector4 = vector2;
				currentVelocity = (vector4 - vector2) / deltaTime;
			}
			return vector4;
		}
		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x + b.x, a.y + b.y);
		}
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.x - b.x, a.y - b.y);
		}
		public static Vector2 operator -(Vector2 a)
		{
			return new Vector2(-a.x, -a.y);
		}
		public static Vector2 operator *(Vector2 a, float d)
		{
			return new Vector2(a.x * d, a.y * d);
		}
		public static Vector2 operator *(float d, Vector2 a)
		{
			return new Vector2(a.x * d, a.y * d);
		}
		public static Vector2 operator /(Vector2 a, float d)
		{
			return new Vector2(a.x / d, a.y / d);
		}
		public static bool operator ==(Vector2 lhs, Vector2 rhs)
		{
			return Vector2.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
		}
		public static bool operator !=(Vector2 lhs, Vector2 rhs)
		{
			return Vector2.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
		}
		public static implicit operator Vector2(Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}
		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}
	}
}