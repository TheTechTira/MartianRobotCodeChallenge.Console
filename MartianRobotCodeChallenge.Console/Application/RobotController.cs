using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using MartianRobotCodeChallenge.Console.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Application
{
  public class RobotController
  {
    private readonly Grid _grid;
    private readonly ICommandFactory _factory;

    public RobotController(Grid grid, ICommandFactory factory)
    {
      _grid = grid;
      _factory = factory;
    }

    public RobotRunResult RunRobot(Robot robot, string instructions)
    {
      // Validate starting position is inside the current grid.
      if (!_grid.IsInBounds(robot.Position.X, robot.Position.Y))
        throw new ArgumentException(
            $"Robot starting position ({robot.Position.X}, {robot.Position.Y}) must be inside the grid boundaries: (0,0) to ({_grid.MaxX},{_grid.MaxY}).");

      // Validate instruction length size
      if (instructions.Length >= 100)
        throw new ArgumentOutOfRangeException(
            $"Instructions length limit is less than 100 characters, received instruction length is: {instructions.Length}.");

      instructions = instructions.Trim().ToUpperInvariant();// Trim whitespaces and ensure command characters are uppercase for consistency.

      foreach (char c in instructions)
      {
        ICommand cmd = _factory.Create(c);
        cmd.Execute(robot, _grid);
        if (robot.IsLost) break;
      }

      return new RobotRunResult(robot.Position, robot.Facing, robot.IsLost);
    }
  }
}
