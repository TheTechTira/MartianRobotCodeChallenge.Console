using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using MartianRobotCodeChallenge.Console.Domain.ValueObjects;

namespace MartianRobotCodeChallenge.Console.Application
{
  /// <summary>
  /// Responsible for orchestrating robot movement on the Martian grid.
  /// Encapsulates the business logic for executing robot instructions,
  /// enforcing grid boundaries, and utilizing the command pattern for extensibility.
  /// 
  /// The controller delegates instruction interpretation and execution
  /// to an <see cref="ICommandFactory"/> and concrete <see cref="ICommand"/>s,
  /// allowing new robot behaviors to be added without modifying core logic.
  /// </summary>
  public class RobotController
  {
    private readonly Grid _grid;
    private readonly ICommandFactory _factory;

    /// <summary>
    /// Initializes a new <see cref="RobotController"/> with the provided grid and command factory.
    /// </summary>
    /// <param name="grid">The grid representing the Martian surface and boundary constraints.</param>
    /// <param name="factory">The factory used to instantiate commands for robot instructions.</param>
    public RobotController(Grid grid, ICommandFactory factory)
    {
      _grid = grid;
      _factory = factory;
    }

    /// <summary>
    /// Executes the given instruction sequence for the specified robot, updating its state.
    /// 
    /// - Validates that the robot's starting position is within the grid.
    /// - Processes each instruction character using the factory and command pattern.
    /// - Terminates execution early if the robot is lost (moves off the grid).
    /// - Returns a <see cref="RobotRunResult"/> describing the robot's final position, orientation, and lost status.
    /// </summary>
    /// <param name="robot">The robot to execute instructions for.</param>
    /// <param name="instructions">The sequence of instruction characters (e.g., "LFRF").</param>
    /// <returns>
    /// A <see cref="RobotRunResult"/> containing the robot's final position, facing direction, and lost status.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the robot's starting position is outside the grid boundaries.
    /// </exception>
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

      instructions = instructions.Trim().ToUpperInvariant();// Trim whitespaces

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
