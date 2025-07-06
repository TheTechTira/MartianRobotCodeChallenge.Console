using MartianRobotCodeChallenge.Console.Application;
using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Factories;
using MartianRobotCodeChallenge.Console.Domain.ValueObjects;

Console.WriteLine("== Martian Robots ==");

// Prompt for grid size
Console.WriteLine("Enter the upper-right grid coordinates (e.g., 5 3):");
var gridDims = Console.ReadLine()?.Split();
if (gridDims == null || gridDims.Length != 2
    || !int.TryParse(gridDims[0], out int maxX)
    || !int.TryParse(gridDims[1], out int maxY))
{
  Console.WriteLine("Invalid grid size. Please enter two integers separated by a space.");
  return;
}

if (maxX < 0 || maxX > 50 || maxY < 0 || maxY > 50)
{
  Console.WriteLine("Grid size coordinates must be between 0 and 50.");
  return;
}

var grid = new Grid(maxX, maxY);
var factory = new CommandFactory();
var controller = new RobotController(grid, factory);

Console.WriteLine(
@"Enter each robot's starting position and direction (e.g., '1 1 E'),
then its instruction sequence (e.g., 'RFRFRFRF').
Just press Enter on a blank line at any time to stop entering robots.

Available directions: N, S, E, W.
Available commands: L (left), R (right), F (forward).
Example:
1 1 E
RFRFRFRF

Ready for robots!

Enter your first robot's starting position (or press Enter to finish):
");

int robotNum = 0, lostCount = 0;
string? line;
while (true)
{
  Console.ForegroundColor = ConsoleColor.Cyan;
  if (robotNum > 0)
    Console.WriteLine("Enter the next robot's starting position (or press Enter to finish):");
  Console.ResetColor();

  line = Console.ReadLine();
  if (string.IsNullOrWhiteSpace(line))
    break;

  var parts = line.Trim().Split();
  if (parts.Length != 3
      || !int.TryParse(parts[0], out int x)
      || !int.TryParse(parts[1], out int y)
      || !Enum.TryParse<EnumDirection>(parts[2], true, out var dir))
  {
    Console.WriteLine("Invalid robot start. Example: 3 2 N");
    continue;
  }

  if (!grid.IsInBounds(x, y))
  {
    Console.WriteLine($"Robot starting position ({x}, {y}) must be inside the grid boundaries: (0,0) to ({maxX},{maxY}).");
    continue;
  }

  Console.WriteLine("Enter instruction sequence for this robot:");
  var instructions = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

  if (!IsValidInstructions(instructions, out var error))
  {
    Console.WriteLine(error);
    continue;
  }

  var robot = new Robot(x, y, dir);
  var runRobotResult = controller.RunRobot(robot, instructions);

  robotNum++;

  if (runRobotResult.IsLost)
  {
    lostCount++;

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(
        $"Result: {runRobotResult.Position.X} {runRobotResult.Position.Y} {runRobotResult.Facing} LOST\n" +
        $"  -> Robot was LOST: It attempted to move off the grid from ({runRobotResult.Position.X}, {runRobotResult.Position.Y}) facing {runRobotResult.Facing}.\n");
    Console.ResetColor();
  }
  else
  {
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(
        $"Result: {runRobotResult.Position.X} {runRobotResult.Position.Y} {runRobotResult.Facing}\n" +
        $"  -> Robot completed its instructions and is safe at ({runRobotResult.Position.X}, {runRobotResult.Position.Y}) facing {runRobotResult.Facing}.\n");
    Console.ResetColor();
  }

  PrintGrid(grid, runRobotResult.Position, runRobotResult.Facing, runRobotResult.IsLost);
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"\nSummary: {robotNum} robot(s) processed, {lostCount} robot(s) were lost.");
Console.WriteLine("All robots processed! Press ENTER to exit.");
Console.ResetColor();
Console.ReadLine();



// --- CLI methods

bool IsValidInstructions(string instructions, out string error)
{
  if (string.IsNullOrEmpty(instructions))
  {
    error = "Instruction sequence cannot be empty.";
    return false;
  }

  if (instructions.Length >= 100)
  {
    error = "Instruction sequence must be less than 100 characters.";
    return false;
  }

  // Update or extend this for new commands
  if (instructions.Any(c => !"LRF".Contains(c)))
  {
    error = "Invalid instruction sequence. Use only L, R, and F (currently supported).";
    return false;
  }

  error = "";
  return true;
}

void PrintGrid(Grid grid, Position robotPosition, EnumDirection robotDir, bool robotLost)
{
  // Map directions to grid symbols
  var dirSymbol = new Dictionary<EnumDirection, char>
  {
    [EnumDirection.N] = '^',
    [EnumDirection.E] = '>',
    [EnumDirection.S] = 'v',
    [EnumDirection.W] = '<'
  };

  Console.WriteLine("Grid state (top is North):");
  for (int y = grid.MaxY; y >= 0; y--)
  {
    for (int x = 0; x <= grid.MaxX; x++)
    {
      // Draw the robot
      if (robotPosition.X == x && robotPosition.Y == y)
      {
        if (robotLost)
        {
          Console.ForegroundColor = ConsoleColor.Yellow;
          Console.Write("X "); // Lost robot
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.Write("R "); // Active robot
        }
        Console.ResetColor();
        continue;
      }

      // Draw a scent (if any) at this position/direction
      bool isScent = false;
      foreach (var dir in Enum.GetValues<EnumDirection>())
      {
        if (grid.HasScent(x, y, dir))
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Write(dirSymbol[dir] + " ");
          Console.ResetColor();
          isScent = true;
          break; // Only show one
        }
      }
      if (isScent) continue;

      // Empty cell
      Console.Write(". ");
    }
    Console.WriteLine();
  }
  Console.WriteLine();
}