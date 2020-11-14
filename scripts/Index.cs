
using System;

class Index : IEquatable<Index>
{
	// Readonly auto-implemented properties.
	public int X { get; private set; }
	public int Y { get; private set; }

	// Set the properties in the constructor.
	public Index(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}

	public override bool Equals(object obj)
	{
		return this.Equals(obj as Index);
	}

	public bool Equals(Index p)
	{
		// If parameter is null, return false.
		if (Object.ReferenceEquals(p, null))
		{
			return false;
		}

		// Optimization for a common success case.
		if (Object.ReferenceEquals(this, p))
		{
			return true;
		}

		// If run-time types are not exactly the same, return false.
		if (this.GetType() != p.GetType())
		{
			return false;
		}

		// Return true if the fields match.
		// Note that the base class is not invoked because it is
		// System.Object, which defines Equals as reference equality.
		return (X == p.X) && (Y == p.Y);
	}

	public override int GetHashCode()
	{
		return X * 0x00010000 + Y;
	}

	public static bool operator ==(Index lhs, Index rhs)
	{
		// Check for null on left side.
		if (Object.ReferenceEquals(lhs, null))
		{
			if (Object.ReferenceEquals(rhs, null))
			{
				// null == null = true.
				return true;
			}

			// Only the left side is null.
			return false;
		}
		// Equals handles case of null on right side.
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Index lhs, Index rhs)
	{
		return !(lhs == rhs);
	}
}
