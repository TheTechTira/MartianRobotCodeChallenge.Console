using MartianRobotCodeChallenge.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.ValueObjects
{
  public class RobotRunResult
  {
    // The robot's final position (X, Y) after executing all instructions.
    public Position Position { get; }

    // The robot's final facing direction (N, E, S, W).
    public EnumDirection Facing { get; }

    // True if the robot was lost (i.e., moved off the grid and left a scent) during execution; otherwise, false.
    public bool IsLost { get; }

    // Optional message describing the result for UI display or debugging purposes.
    public string? Message { get; }

    public RobotRunResult(Position position, EnumDirection facing, bool isLost, string? message = null)
    {
      Position = position;
      Facing = facing;
      IsLost = isLost;
      Message = message;
    }
  }
}
