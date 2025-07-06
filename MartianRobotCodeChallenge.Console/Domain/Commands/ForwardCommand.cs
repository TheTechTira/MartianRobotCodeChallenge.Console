using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;

namespace MartianRobotCodeChallenge.Console.Domain.Commands
{
  /// <summary>
  /// Command to move the robot forward one grid point in its current facing direction.
  /// Implements the Command pattern for robot instructions.
  /// 
  /// Movement is based on the robot's orientation:
  /// - N (North): Y+1
  /// - E (East):  X+1
  /// - S (South): Y-1
  /// - W (West):  X-1
  /// 
  /// If moving forward would take the robot off the grid:
  /// - If there is no "scent" (left by a previously lost robot), the robot is marked as lost and a scent is left.
  /// - If there is a scent at the current position and direction, the instruction is ignored (robot remains in place).
  /// </summary>
  public class ForwardCommand : ICommand
  {
    /// <summary>
    /// Mapping of facing direction (EnumDirection) to movement deltas (dx, dy).
    /// The order must match EnumDirection: N=0, E=1, S=2, W=3.
    /// </summary>
    private static readonly (int dx, int dy)[] Moves = { (0, 1), (1, 0), (0, -1), (-1, 0) };

    /// <summary>
    /// Executes the forward movement:
    /// - Advances the robot one grid cell in the direction it is facing, if within bounds.
    /// - If the move would go off the grid and no scent exists, the robot is marked as lost and leaves a scent.
    /// - If a scent exists, the move is ignored and the robot continues with the next instruction.
    /// </summary>
    /// <param name="robot">The robot to move.</param>
    /// <param name="grid">The grid defining boundaries and storing scents.</param>
    public void Execute(Robot robot, Grid grid)
    {
      // Do nothing if the robot is already lost.
      if (robot.IsLost) return;

      // Determine the movement delta for the current facing direction.
      var move = Moves[(int)robot.Facing];
      int nx = robot.Position.X + move.dx;
      int ny = robot.Position.Y + move.dy;

      // Check if the next position is out of grid bounds.
      if (!grid.IsInBounds(nx, ny))
      {
        // If no scent exists for this position/direction, robot is lost and leaves a scent.
        if (!grid.HasScent(robot.Position.X, robot.Position.Y, robot.Facing))
        {
          robot.IsLost = true;
          grid.LeaveScent(robot.Position.X, robot.Position.Y, robot.Facing);
        }
        // If a scent exists, ignore this instruction (robot remains in place and continues with next instruction).
      }
      else
      {
        // Valid move; advance the robot to the new position.
        robot.Position.X = nx;
        robot.Position.Y = ny;
      }
    }
  }
}
