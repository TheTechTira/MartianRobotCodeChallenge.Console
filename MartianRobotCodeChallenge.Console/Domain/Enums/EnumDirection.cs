namespace MartianRobotCodeChallenge.Console.Domain.Enums
{
  /// <summary>
  /// Cardinal directions for robot orientation on the Martian grid.
  /// Used to represent the robot's facing direction, instruction interpretation,
  /// and movement logic.
  /// 
  /// Directions are ordered as: North (N), East (E), South (S), and West (W),
  /// matching standard compass rotation and movement rules.
  /// </summary>
  public enum EnumDirection
  {
    /// <summary>
    /// North (positive Y direction).
    /// </summary>
    N,

    /// <summary>
    /// East (positive X direction).
    /// </summary>
    E,

    /// <summary>
    /// South (negative Y direction).
    /// </summary>
    S,

    /// <summary>
    /// West (negative X direction).
    /// </summary>
    W
  }
}
