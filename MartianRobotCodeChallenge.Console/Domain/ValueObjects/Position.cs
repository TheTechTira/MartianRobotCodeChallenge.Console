namespace MartianRobotCodeChallenge.Console.Domain.ValueObjects
{
  /// <summary>
  /// Represents a point (X, Y) on the Martian grid.
  /// Designed as a value object for coordinate comparison and grid logic.
  /// </summary>
  public class Position : IEquatable<Position>
  {
    /// <summary>The X coordinate (column).</summary>
    public int X { get; set; }

    /// <summary>The Y coordinate (row).</summary>
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
