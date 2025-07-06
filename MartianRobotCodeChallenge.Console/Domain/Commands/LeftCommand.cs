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
  public class LeftCommand : ICommand
  {
    public void Execute(Robot robot, Grid grid)
    {
      // Turn left: (N -> W, W -> S, S -> E, E -> N).
      // ((int)Facing + 3) % 4 cycles back one step in the direction enum,
      // using modulo math to wrap around without negative numbers.
      robot.Facing = (EnumDirection)(((int)robot.Facing + 3) % 4);
    }
  }
}
