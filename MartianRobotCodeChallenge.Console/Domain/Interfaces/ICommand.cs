using MartianRobotCodeChallenge.Console.Domain.Entities;

namespace MartianRobotCodeChallenge.Console.Domain.Interfaces
{
  /// <summary>
  /// Defines the contract for a robot movement or control command,
  /// as used in the Command pattern for Martian Robots.
  ///
  /// Implementations of <see cref="ICommand"/> encapsulate a single instruction
  /// (such as 'L' for turn left, 'R' for turn right, or 'F' for move forward)
  /// that can be executed against a robot and the current grid state.
  ///
  /// To add new robot behaviors or instructions, implement this interface
  /// and register the new command in the <c>CommandFactory</c>.
  /// </summary>
  public interface ICommand
  {
    /// <summary>
    /// Executes the command logic for the given robot and grid.
    /// </summary>
    /// <param name="robot">The robot instance to operate on.</param>
    /// <param name="grid">The grid representing the Martian surface.</param>
    void Execute(Robot robot, Grid grid);
  }
}
