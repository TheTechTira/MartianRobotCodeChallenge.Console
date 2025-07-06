using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Interfaces
{
  public interface ICommandFactory
  {
    ICommand Create(char c);
  }
}
