using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrapherApp.UI.Model
{
	public struct Vector3
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
		/// <summary>
		///   <para>Z component of the vector.</para>
		/// </summary>
		public float z;
		public float this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
		}
		/// <summary>
		///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
		/// </summary>
		public Vector3 normalized
		{
			get
			{
				return Vector3.Normalize(this);
			}
		}
		/// <summary>
		///   <para>Returns the length of this vector (Read Only).</para>
		/// </summary>
		public float magnitude
		{
			get
			{
				return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
			}
		}
		/// <summary>
		///   <para>Returns the squared length of this vector (Read Only).</para>
		/// </summary>
		public float sqrMagnitude
		{
			get
			{
				return this.x * this.x + this.y * this.y + this.z * this.z;
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(0, 0, 0).</para>
		/// </summary>
		public static Vector3 zero
		{
			get
			{
				return new Vector3(0f, 0f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(1, 1, 1).</para>
		/// </summary>
		public static Vector3 one
		{
			get
			{
				return new Vector3(1f, 1f, 1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(0, 0, 1).</para>
		/// </summary>
		public static Vector3 forward
		{
			get
			{
				return new Vector3(0f, 0f, 1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(0, 0, -1).</para>
		/// </summary>
		public static Vector3 back
		{
			get
			{
				return new Vector3(0f, 0f, -1f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(0, 1, 0).</para>
		/// </summary>
		public static Vector3 up
		{
			get
			{
				return new Vector3(0f, 1f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(0, -1, 0).</para>
		/// </summary>
		public static Vector3 down
		{
			get
			{
				return new Vector3(0f, -1f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(-1, 0, 0).</para>
		/// </summary>
		public static Vector3 left
		{
			get
			{
				return new Vector3(-1f, 0f, 0f);
			}
		}
		/// <summary>
		///   <para>Shorthand for writing Vector3(1, 0, 0).</para>
		/// </summary>
		public static Vector3 right
		{
			get
			{
				return new Vector3(1f, 0f, 0f);
			}
		}
		[Obsolete("Use Vector3.forward instead.")]
		public static Vector3 fwd
		{
			get
			{
				return new Vector3(0f, 0f, 1f);
			}
		}
		/// <summary>
		///   <para>Creates a new vector with given x, y, z components.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		/// <summary>
		///   <para>Creates a new vector with given x, y components and sets z to zero.</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Vector3(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.z = 0f;
		}
		/// <summary>
		///   <para>Linearly interpolates between two vectors.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			t = t < 0 ? 0 : t > 1 ? 1 : t;
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}
		/// <summary>
		///   <para>Linearly interpolates between two vectors.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="t"></param>
		public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		///   <para>Moves a point current in a straight line towards a target point.</para>
		/// </summary>
		/// <param name="current"></param>
		/// <param name="target"></param>
		/// <param name="maxDistanceDelta"></param>
		public static Vector3 MoveTowards(Vector3 current, Vector3 target, float maxDistanceDelta)
		{
			Vector3 a = target - current;
			float magnitude = a.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0f)
			{
				return target;
			}
			return current + a / magnitude * maxDistanceDelta;
		}


		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
		{
			smoothTime = Math.Max(0.0001f, smoothTime);
			float num = 2f / smoothTime;
			float num2 = num * deltaTime;
			float d = 1f / (1f + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
			Vector3 vector = current - target;
			Vector3 vector2 = target;
			float maxLength = maxSpeed * smoothTime;
			vector = Vector3.ClampMagnitude(vector, maxLength);
			target = current - vector;
			Vector3 vector3 = (currentVelocity + num * vector) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3) * d;
			Vector3 vector4 = target + (vector + vector3) * d;
			if (Vector3.Dot(vector2 - current, vector4 - vector2) > 0f)
			{
				vector4 = vector2;
				currentVelocity = (vector4 - vector2) / deltaTime;
			}
			return vector4;
		}
		/// <summary>
		///   <para>Set x, y and z components of an existing Vector3.</para>
		/// </summary>
		/// <param name="new_x"></param>
		/// <param name="new_y"></param>
		/// <param name="new_z"></param>
		public void Set(float new_x, float new_y, float new_z)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
		}
		/// <summary>
		///   <para>Multiplies two vectors component-wise.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static Vector3 Scale(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}
		/// <summary>
		///   <para>Multiplies every component of this vector by the same component of scale.</para>
		/// </summary>
		/// <param name="scale"></param>
		public void Scale(Vector3 scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}
		/// <summary>
		///   <para>Cross Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}
		public override int GetHashCode()
		{
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2 ^ this.z.GetHashCode() >> 2;
		}
		public override bool Equals(object other)
		{
			if (!(other is Vector3))
			{
				return false;
			}
			Vector3 vector = (Vector3)other;
			return this.x.Equals(vector.x) && this.y.Equals(vector.y) && this.z.Equals(vector.z);
		}
		/// <summary>
		///   <para>Reflects a vector off the plane defined by a normal.</para>
		/// </summary>
		/// <param name="inDirection"></param>
		/// <param name="inNormal"></param>
		public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
		{
			return -2f * Vector3.Dot(inNormal, inDirection) * inNormal + inDirection;
		}
		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="value"></param>
		public static Vector3 Normalize(Vector3 value)
		{
			float num = Vector3.Magnitude(value);
			if (num > 1E-05f)
			{
				return value / num;
			}
			return Vector3.zero;
		}
		/// <summary>
		///   <para>Makes this vector have a magnitude of 1.</para>
		/// </summary>
		public void Normalize()
		{
			float num = Vector3.Magnitude(this);
			if (num > 1E-05f)
			{
				this /= num;
			}
			else
			{
				this = Vector3.zero;
			}
		}
		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public override string ToString()
		{
			return String.Format("({0:F1}, {1:F1}, {2:F1})", new object[]
			{
				this.x,
				this.y,
				this.z
			});
		}
		/// <summary>
		///   <para>Returns a nicely formatted string for this vector.</para>
		/// </summary>
		/// <param name="format"></param>
		public string ToString(string format)
		{
			return String.Format("({0}, {1}, {2})", new object[]
			{
				this.x.ToString(format),
				this.y.ToString(format),
				this.z.ToString(format)
			});
		}
		/// <summary>
		///   <para>Dot Product of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static float Dot(Vector3 lhs, Vector3 rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}
		/// <summary>
		///   <para>Projects a vector onto another vector.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="onNormal"></param>
		public static Vector3 Project(Vector3 vector, Vector3 onNormal)
		{
			float num = Vector3.Dot(onNormal, onNormal);
			if (num < float.Epsilon)
			{
				return Vector3.zero;
			}
			return onNormal * Vector3.Dot(vector, onNormal) / num;
		}
		/// <summary>
		///   <para>Projects a vector onto a plane defined by a normal orthogonal to the plane.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="planeNormal"></param>
		public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
		{
			return vector - Vector3.Project(vector, planeNormal);
		}
		[Obsolete("Use Vector3.ProjectOnPlane instead.")]
		public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
		{
			return fromThat - Vector3.Project(fromThat, excludeThis);
		}

		/// <summary>
		///   <para>Returns the distance between a and b.</para>
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static float Distance(Vector3 a, Vector3 b)
		{
			Vector3 vector = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
			return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
		}
		/// <summary>
		///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="maxLength"></param>
		public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}
		public static float Magnitude(Vector3 a)
		{
			return (float)Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
		}
		public static float SqrMagnitude(Vector3 a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}
		/// <summary>
		///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3 Min(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
		}
		/// <summary>
		///   <para>Returns a vector that is made from the largest components of two vectors.</para>
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		public static Vector3 Max(Vector3 lhs, Vector3 rhs)
		{
			return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
		}
		public static Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		public static Vector3 operator -(Vector3 a)
		{
			return new Vector3(-a.x, -a.y, -a.z);
		}
		public static Vector3 operator *(Vector3 a, float d)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}
		public static Vector3 operator *(float d, Vector3 a)
		{
			return new Vector3(a.x * d, a.y * d, a.z * d);
		}
		public static Vector3 operator /(Vector3 a, float d)
		{
			return new Vector3(a.x / d, a.y / d, a.z / d);
		}
		public static bool operator ==(Vector3 lhs, Vector3 rhs)
		{
			return Vector3.SqrMagnitude(lhs - rhs) < 9.99999944E-11f;
		}
		public static bool operator !=(Vector3 lhs, Vector3 rhs)
		{
			return Vector3.SqrMagnitude(lhs - rhs) >= 9.99999944E-11f;
		}
	}
}