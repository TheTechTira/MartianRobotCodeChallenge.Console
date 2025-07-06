using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.ValueObjects
{
  public class Position : IEquatable<Position>
  {
    /// The X coordinate (column).
    public int X { get; set; }

    /// The Y coordinate (row).
    public int Y { get; set; }

    public Position(int x, int y)
    {
      X = x;
      Y = y;
    }

    // Enables use in sets/dictionaries and robust equality checks
    public override bool Equals(object? obj) => Equals(obj as Position);
    public bool Equals(Position? other) => other != null && X == other.X && Y == other.Y;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
  }
}
