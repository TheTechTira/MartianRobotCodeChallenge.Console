using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;

namespace MartianRobotCodeChallenge.Console.Domain.Commands
{
  /// <summary>
  /// Command to rotate the robot 90 degrees to the right (clockwise).
  /// 
  /// Implements the Command pattern for robot instructions.
  /// The robot's position remains unchanged; only its facing direction is updated.
  /// 
  /// Scent and grid boundary checks are not required for this command,
  /// as turning right cannot cause the robot to be lost or leave the grid.
  /// </summary>
  public class RightCommand : ICommand
  {
    /// <summary>
    /// Rotates the robot 90 degrees to the right (clockwise), updating its facing direction.
    /// The robot remains on its current grid position.
    ///
    /// <para>
    /// <b>Modulo math explanation:</b>
    /// The robot's direction is represented as an integer (EnumDirection: N=0, E=1, S=2, W=3).
    /// To turn right, increment the direction value by 1 and use modulo 4 to wrap around:
    /// <code>
    /// (currentDirection + 1) % 4
    /// </code>
    /// This ensures:
    /// N (0) -> E (1), E (1) -> S (2), S (2) -> W (3), W (3) -> N (0).
    /// </para>
    ///
    /// <para>
    /// <b>Scent logic is not relevant,</b>
    /// since turning in place cannot cause the robot to be lost.
    /// </para>
    /// </summary>
    /// <param name="robot">The robot to rotate.</param>
    /// <param name="grid">The grid (not used for this command).</param>
    public void Execute(Robot robot, Grid grid)
    {
      // Turn right: (N -> E, E -> S, S -> W, W -> N).
      // ((int)Facing + 1) % 4 cycles forward one step in the direction enum,
      // using modulo math to wrap around after W (3).
      robot.Facing = (EnumDirection)(((int)robot.Facing + 1) % 4);
    }
  }
}
