using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Entities
{
  public class Robot
  {
    public Position Position { get; set; }
    public EnumDirection Facing { get; set; }
    public bool IsLost { get; set; }

    public Robot(int x, int y, EnumDirection facing)
    {
      Position = new Position(x, y);
      Facing = facing;
      IsLost = false;
    }
  }
}
