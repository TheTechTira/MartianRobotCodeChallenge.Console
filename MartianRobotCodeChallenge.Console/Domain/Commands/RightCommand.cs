using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Commands
{
  public class RightCommand : ICommand
  {
    public void Execute(Robot robot, Grid grid)
    {
      // Turn right: (N -> E, E -> S, S -> W, W -> N).
      // ((int)Facing + 1) % 4 cycles forward one step in the direction enum,
      // using modulo math to wrap around after W (3).
      robot.Facing = (EnumDirection)(((int)robot.Facing + 1) % 4);
    }
  }
}
