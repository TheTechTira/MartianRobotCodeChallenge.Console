namespace MartianRobotCodeChallenge.Console.Domain.Interfaces
{
  /// <summary>
  /// Factory interface for producing robot command instances based on input instruction characters.
  /// Implements the Factory pattern to decouple the creation of <see cref="ICommand"/> implementations
  /// from their usage in the robot control logic.
  ///
  /// The <see cref="Create"/> method should map each valid instruction character
  /// (such as 'L', 'R', or 'F') to a corresponding <see cref="ICommand"/> implementation.
  ///
  /// To extend the system with new robot commands (e.g., 'B' for backward),
  /// implement a new <see cref="ICommand"/> and register it in the concrete factory.
  /// </summary>
  public interface ICommandFactory
  {
    /// <summary>
    /// Returns a new command instance for the given instruction character.
    /// Throws <see cref="ArgumentException"/> if the character is not recognized.
    /// </summary>
    /// <param name="c">The instruction character (e.g., 'L', 'R', 'F').</param>
    /// <returns>An <see cref="ICommand"/> that executes the associated behavior.</returns>
    ICommand Create(char c);
  }
}