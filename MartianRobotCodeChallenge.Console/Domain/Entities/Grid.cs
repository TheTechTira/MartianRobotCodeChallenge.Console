using MartianRobotCodeChallenge.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Domain.Entities
{
  public class Grid
  {
    // The maximum valid X coordinate (east boundary).
    public int MaxX { get; }

    // The maximum valid Y coordinate (north boundary).
    public int MaxY { get; }

    // Stores all grid points and directions where robots have previously been lost.
    private readonly HashSet<(int, int, EnumDirection)> scents = new();

    public Grid(int maxX, int maxY)
    {
      if (maxX < 0 || maxX > 50)
        throw new ArgumentOutOfRangeException(nameof(maxX), "MaxX must be between 0 and 50.");
      if (maxY < 0 || maxY > 50)
        throw new ArgumentOutOfRangeException(nameof(maxY), "MaxY must be between 0 and 50.");

      MaxX = maxX;
      MaxY = maxY;
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x <= MaxX && y >= 0 && y <= MaxY;

    public void LeaveScent(int x, int y, EnumDirection dir)
    {
      if (!IsInBounds(x, y))
        throw new ArgumentException("Cannot leave scent outside the grid.");
      scents.Add((x, y, dir));
    }

    public bool HasScent(int x, int y, EnumDirection dir)
    {
      if (!IsInBounds(x, y))
        return false;
      return scents.Contains((x, y, dir));
    }
  }
}
