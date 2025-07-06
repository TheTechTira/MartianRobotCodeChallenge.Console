using MartianRobotCodeChallenge.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Interfaces
{
  public interface ICommand
  {
    void Execute(Robot robot, Grid grid);
  }
}
