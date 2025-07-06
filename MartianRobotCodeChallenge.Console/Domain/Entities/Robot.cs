using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.ValueObjects;

namespace MartianRobotCodeChallenge.Console.Domain.Entities
{
  /// <summary>
  /// Represents a robot navigating the Martian grid.
  /// 
  /// Each robot instance has its own <see cref="Position"/> (X, Y coordinates) and
  /// <see cref="Facing"/> direction (N, E, S, W). The robot can be in a "lost" state (<see cref="IsLost"/>),
  /// indicating it has moved off the grid. Robot movement and rotation are handled by
  /// <see cref="ICommand"/> implementations, with external state management via the controller and grid.
  /// </summary>
  public class Robot
  {
    /// <summary>
    /// The robot's current position on the grid (X/Y coordinates).
    /// </summary>
    public Position Position { get; set; }

    /// <summary>
    /// The direction the robot is currently facing (N, E, S, W).
    /// </summary>
    public EnumDirection Facing { get; set; }

    /// <summary>
    /// True if the robot is lost (i.e., has moved off the grid); otherwise, false.
    /// </summary>
    public bool IsLost { get; set; }

    /// <summary>
    /// Initializes a new robot with the given starting coordinates and facing direction.
    /// </summary>
    /// <param name="x">Initial X coordinate on the grid.</param>
    /// <param name="y">Initial Y coordinate on the grid.</param>
    /// <param name="facing">Initial direction the robot is facing.</param>
    public Robot(int x, int y, EnumDirection facing)
    {
      Position = new Position(x, y);
      Facing = facing;
      IsLost = false;
    }
  }
}
