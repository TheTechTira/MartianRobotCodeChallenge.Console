using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Commands
{
  public class ForwardCommand : ICommand
  {
    // Mapping of facing direction (EnumDirection) to movement deltas (dx, dy).
    // The order must match EnumDirection: N=0, E=1, S=2, W=3.
    private static readonly (int dx, int dy)[] Moves = { (0, 1), (1, 0), (0, -1), (-1, 0) };

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
