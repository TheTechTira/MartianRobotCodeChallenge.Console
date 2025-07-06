using MartianRobotCodeChallenge.Console.Domain.Enums;

namespace MartianRobotCodeChallenge.Console.Domain.ValueObjects
{
  /// <summary>
  /// Encapsulates the result of executing a robot's instruction sequence on the Martian grid.
  /// 
  /// Contains the robot's final <see cref="Position"/>, <see cref="Facing"/> direction,
  /// and lost status (<see cref="IsLost"/>) after processing all instructions.
  /// Optionally includes a <see cref="Message"/> for display, debugging, or user feedback.
  /// </summary>
  public class RobotRunResult
  {
    /// <summary>
    /// The robot's final position (X, Y) after executing all instructions.
    /// </summary>
    public Position Position { get; }

    /// <summary>
    /// The robot's final facing direction (N, E, S, W).
    /// </summary>
    public EnumDirection Facing { get; }

    /// <summary>
    /// True if the robot was lost (i.e., moved off the grid and left a scent) during execution; otherwise, false.
    /// </summary>
    public bool IsLost { get; }

    /// <summary>
    /// Optional message describing the result for UI display or debugging purposes.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Initializes a new instance representing the outcome of a robot's instruction run.
    /// </summary>
    /// <param name="position">The robot's final position on the grid.</param>
    /// <param name="facing">The robot's final facing direction.</param>
    /// <param name="isLost">Indicates whether the robot was lost (went off-grid).</param>
    /// <param name="message">Optional result message for display or logging.</param>
    public RobotRunResult(Position position, EnumDirection facing, bool isLost, string? message = null)
    {
      Position = position;
      Facing = facing;
      IsLost = isLost;
      Message = message;
    }
  }
}
