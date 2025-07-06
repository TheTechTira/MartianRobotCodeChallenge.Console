using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;

namespace MartianRobotCodeChallenge.Console.Domain.Commands
{
  /// <summary>
  /// Command to rotate the robot 90 degrees to the left (counterclockwise).
  /// 
  /// Implements the Command pattern for robot instructions.
  /// The robot's position remains unchanged; only its facing direction is updated.
  ///
  /// <para>
  /// Scent and grid boundary checks are not required for this command,
  /// as turning left cannot cause the robot to be lost or leave the grid.
  /// </para>
  /// </summary>
  public class LeftCommand : ICommand
  {
    /// <summary>
    /// Rotates the robot 90 degrees to the left (counterclockwise), updating its facing direction.
    /// The robot remains on its current grid position.
    ///
    /// <para>
    /// <b>Note on modulo math:</b>
    /// The robot's direction is stored as an integer (EnumDirection: N=0, E=1, S=2, W=3).
    /// To turn left, we move "backwards" one direction.
    /// To avoid negative numbers, we add 3 (equivalent to -1 mod 4) and take modulo 4:
    /// <code>
    /// (currentDirection + 3) % 4
    /// </code>
    /// This ensures:
    /// N (0) -> W (3), W (3) -> S (2), S (2) -> E (1), E (1) -> N (0).
    /// </para>
    ///
    /// <para>
    /// <b>Scent logic is not relevant,</b>
    /// since no movement occurs and turning cannot cause the robot to be lost.
    /// </para>
    /// </summary>
    /// <param name="robot">The robot to rotate.</param>
    /// <param name="grid">The grid (not used for this command).</param>
    public void Execute(Robot robot, Grid grid)
    {
      // Turn left: (N -> W, W -> S, S -> E, E -> N).
      // ((int)Facing + 3) % 4 cycles back one step in the direction enum,
      // using modulo math to wrap around without negative numbers.
      robot.Facing = (EnumDirection)(((int)robot.Facing + 3) % 4);
    }
  }
}
