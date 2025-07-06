using MartianRobotCodeChallenge.Console.Domain.Enums;

namespace MartianRobotCodeChallenge.Console.Domain.Entities
{
  /// <summary>
  /// Models the rectangular Martian grid and manages lost robot "scents."
  /// 
  /// The grid is bounded from (0,0) to (MaxX, MaxY), where the maximum value for either coordinate is 50.
  /// When a robot is lost moving off the grid, a "scent" is left at the last valid position and facing direction.
  /// Future robots encountering a scent at the same position/direction will ignore any instruction that would cause loss there.
  /// </summary>
  public class Grid
  {
    /// <summary>
    /// The maximum valid X coordinate (east boundary).
    /// </summary>
    public int MaxX { get; }

    /// <summary>
    /// The maximum valid Y coordinate (north boundary).
    /// </summary>
    public int MaxY { get; }

    /// <summary>
    /// Stores all grid points and directions where robots have previously been lost.
    /// </summary>
    private readonly HashSet<(int, int, EnumDirection)> scents = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid"/> class with specified boundaries.
    /// Throws if the grid size is out of allowed range (0–50).
    /// </summary>
    /// <param name="maxX">Maximum X coordinate (must be 0–50).</param>
    /// <param name="maxY">Maximum Y coordinate (must be 0–50).</param>
    public Grid(int maxX, int maxY)
    {
      if (maxX < 0 || maxX > 50)
        throw new ArgumentOutOfRangeException(nameof(maxX), "MaxX must be between 0 and 50.");
      if (maxY < 0 || maxY > 50)
        throw new ArgumentOutOfRangeException(nameof(maxY), "MaxY must be between 0 and 50.");

      MaxX = maxX;
      MaxY = maxY;
    }

    /// <summary>
    /// Checks if the given coordinates are inside the grid boundaries.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>True if (x,y) is within (0,0) to (MaxX,MaxY), otherwise false.</returns>
    public bool IsInBounds(int x, int y) => x >= 0 && x <= MaxX && y >= 0 && y <= MaxY;

    /// <summary>
    /// Records a "scent" at the specified location and direction.
    /// Throws if the coordinates are out of bounds.
    /// </summary>
    /// <param name="x">X coordinate where the robot was lost.</param>
    /// <param name="y">Y coordinate where the robot was lost.</param>
    /// <param name="dir">Direction the robot was facing when lost.</param>
    public void LeaveScent(int x, int y, EnumDirection dir)
    {
      if (!IsInBounds(x, y))
        throw new ArgumentException("Cannot leave scent outside the grid.");
      scents.Add((x, y, dir));
    }

    /// <summary>
    /// Checks if a scent exists at the specified location and direction.
    /// Returns false if the coordinates are out of bounds.
    /// </summary>
    /// <param name="x">X coordinate to check.</param>
    /// <param name="y">Y coordinate to check.</param>
    /// <param name="dir">Direction to check for a scent.</param>
    /// <returns>True if a scent exists at (x, y, dir), otherwise false.</returns>
    public bool HasScent(int x, int y, EnumDirection dir)
    {
      if (!IsInBounds(x, y))
        return false;
      return scents.Contains((x, y, dir));
    }
  }
}
