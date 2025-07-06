using MartianRobotCodeChallenge.Console.Domain.Commands;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Factories
{
  public class CommandFactory : ICommandFactory
  {
    public ICommand Create(char c)
    {
      switch (char.ToUpperInvariant(c))
      {
        case 'L': return new LeftCommand();
        case 'R': return new RightCommand();
        case 'F': return new ForwardCommand();

        // Extend or add more commands as needed here:

        default: throw new ArgumentException($"Unknown command: {c}");
      }
    }
  }
}
