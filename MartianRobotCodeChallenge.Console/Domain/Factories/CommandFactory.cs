using MartianRobotCodeChallenge.Console.Domain.Interfaces;

namespace MartianRobotCodeChallenge.Console.Domain.Factories
{
  /// <summary>
  /// Concrete implementation of <see cref="ICommandFactory"/> that produces
  /// robot command instances based on input instruction characters.
  /// 
  /// This factory maps instruction characters (such as 'L', 'R', 'F')
  /// to their corresponding <see cref="ICommand"/> implementations.
  /// 
  /// To extend robot behavior, add new cases to the <c>Create</c> method,
  /// associating new instruction characters with custom <see cref="ICommand"/> types.
  /// 
  /// Throws <see cref="ArgumentException"/> for unknown instruction characters.
  /// </summary>
  public class CommandFactory : ICommandFactory
  {
    /// <summary>
    /// Returns a new <see cref="ICommand"/> instance for the given instruction character.
    /// Characters are matched case-insensitively.
    /// Extend this method to support additional robot commands.
    /// </summary>
    /// <param name="c">The instruction character (e.g., 'L', 'R', 'F').</param>
    /// <returns>An <see cref="ICommand"/> implementing the requested behavior.</returns>
    /// <exception cref="ArgumentException">Thrown if the instruction character is not recognized.</exception>
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
