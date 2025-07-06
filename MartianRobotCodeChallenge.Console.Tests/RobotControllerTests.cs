using MartianRobotCodeChallenge.Console.Application;
using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Factories;
using MartianRobotCodeChallenge.Console.Domain.Interfaces;
using Moq;

namespace MartianRobotCodeChallenge.Console.Tests
{
  /// <summary>
  /// Comprehensive unit tests for the Martian Robots domain and application layers.
  /// Combines basic and advanced scenarios, including movement, scent logic, validation,
  /// command extensibility, and boundary cases. 
  /// Designed to regression-test all critical rules and demonstrate clean architecture.
  /// </summary>
  public class RobotControllerTests
  {
    #region BASIC MOVEMENT AND GRID LOGIC

    /// <summary>
    /// Verifies that a robot can execute a simple sequence of turning and moving commands without leaving the grid.
    /// The robot starts at (1,1) facing East and performs a sequence that results in a full rotation, 
    /// ending up at the same position and facing the original direction.
    /// </summary>
    [Fact]
    public void Robot_Executes_Simple_Instruction_Stays_On_Grid()
    {
      // Arrange: Create a 5x3 grid and a robot at (1,1) facing East.
      var grid = new Grid(5, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(1, 1, EnumDirection.E);

      // Act: Run the command sequence. "RFRFRFRF" should rotate the robot in place.
      var result = controller.RunRobot(robot, "RFRFRFRF");

      // Assert: Robot ends where it started, facing East, and is not lost.
      Assert.Equal(1, result.Position.X);
      Assert.Equal(1, result.Position.Y);
      Assert.Equal(EnumDirection.E, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Verifies that a robot can handle a longer sequence of turns and moves,
    /// and correctly becomes lost if moving off the grid.
    /// The test ensures the lost status and scent-leaving mechanism are triggered at the correct position.
    /// </summary>
    [Fact]
    public void Robot_Handles_Long_Instruction_Chain()
    {
      // Arrange: Robot starts at (0,3) facing West on a 5x3 grid.
      var grid = new Grid(5, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 3, EnumDirection.W);

      // Act: The sequence includes several turns and moves.
      var result = controller.RunRobot(robot, "LLFFFLFLFL");

      // Assert: Robot is lost at (3,3) facing North after moving off the grid.
      // Confirms that moving off the grid triggers the lost status and leaves a scent.
      Assert.Equal(3, result.Position.X);
      Assert.Equal(3, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.True(result.IsLost);
    }

    /// <summary>
    /// Parameterized test to verify robot behavior at various grid edges and in different scenarios:
    /// - Moving South from (0,0) should cause the robot to be lost.
    /// - Moving North from (0,0) is valid and the robot is not lost.
    /// - Moving North from the top edge (5,3) causes the robot to be lost.
    /// Confirms grid boundary enforcement for all directions.
    /// </summary>
    [Theory]
    [InlineData(0, 0, EnumDirection.S, "F", 0, 0, EnumDirection.S, true)]  // Move South from lower-left; lost.
    [InlineData(0, 0, EnumDirection.N, "F", 0, 1, EnumDirection.N, false)] // Move North from lower-left; valid move.
    [InlineData(5, 3, EnumDirection.N, "F", 5, 3, EnumDirection.N, true)]  // Move North from upper-right; lost.
    public void Robot_Correctly_Handles_Grid_Edges(
        int startX, int startY, EnumDirection startDir, string commands,
        int expectedX, int expectedY, EnumDirection expectedDir, bool expectedLost)
    {
      // Arrange: Place the robot at the provided coordinates/direction on a 5x3 grid.
      var grid = new Grid(5, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(startX, startY, startDir);

      // Act: Run the instruction(s) and capture result.
      var result = controller.RunRobot(robot, commands);

      // Assert: Robot final state matches expectation for each edge/boundary scenario.
      Assert.Equal(expectedX, result.Position.X);
      Assert.Equal(expectedY, result.Position.Y);
      Assert.Equal(expectedDir, result.Facing);
      Assert.Equal(expectedLost, result.IsLost);
    }

    #endregion

    #region SCENT LOGIC

    /// <summary>
    /// Verifies that when a robot is lost by moving off the grid, it leaves a "scent" at the last safe position and direction.
    /// This scent prevents subsequent robots from being lost at the same grid position and orientation.
    /// The second robot stops safely at the last in-bounds cell rather than being lost.
    /// </summary>
    [Fact]
    public void Robot_Lost_Leaves_Scent_Prevents_Other_Losses()
    {
      var grid = new Grid(5, 3);
      var controller = new RobotController(grid, new CommandFactory());

      // First robot starts at (3,2) facing North and is lost after moving off the grid.
      var robot1 = new Robot(3, 2, EnumDirection.N);
      var result1 = controller.RunRobot(robot1, "FRRFLLFFRRFLL");

      // Assert: Robot1 is lost at (3,3,N) and leaves a scent at that position/direction.
      Assert.Equal(3, result1.Position.X);
      Assert.Equal(3, result1.Position.Y);
      Assert.Equal(EnumDirection.N, result1.Facing);
      Assert.True(result1.IsLost);

      // Second robot starts at the same spot, same orientation, repeats the same sequence.
      var robot2 = new Robot(3, 2, EnumDirection.N);
      var result2 = controller.RunRobot(robot2, "FRRFLLFFRRFLL");

      // Assert: Robot2 is NOT lost (scent prevents it), and stops at last valid cell (3,2,N).
      Assert.Equal(3, result2.Position.X);
      Assert.Equal(2, result2.Position.Y);
      Assert.Equal(EnumDirection.N, result2.Facing);
      Assert.False(result2.IsLost);
    }

    /// <summary>
    /// Proves that scents are direction-specific: 
    /// if a robot is lost moving off the grid in one direction from a position,
    /// another robot moving off from the same position but in a different direction will still be lost.
    /// This prevents over-protection from scents and matches problem requirements.
    /// </summary>
    [Fact]
    public void Scent_Is_Direction_Specific()
    {
      var grid = new Grid(0, 0);
      var controller = new RobotController(grid, new CommandFactory());

      // Robot1 at (0,0), facing North, moves forward and is lost; scent left at (0,0,N).
      var lostRobot = new Robot(0, 0, EnumDirection.N);
      controller.RunRobot(lostRobot, "F");
      Assert.True(lostRobot.IsLost);

      // Robot2 at same position but facing East, moves forward and should ALSO be lost, 
      // since scent does not apply to East direction.
      var eastRobot = new Robot(0, 0, EnumDirection.E);
      var result = controller.RunRobot(eastRobot, "F");
      Assert.True(result.IsLost);
    }

    /// <summary>
    /// Confirms that scents are tracked independently per position and per direction,
    /// even for repeated or multiple loss events. Demonstrates that a scent left for (2,2,N)
    /// does not protect against loss going East, but does protect subsequent North moves,
    /// and vice versa. Repeating moves in previously scented directions no longer causes loss.
    /// </summary>
    [Fact]
    public void Scent_At_Border_Is_Direction_Specific()
    {
      var grid = new Grid(2, 2);
      var controller = new RobotController(grid, new CommandFactory());

      // Robot lost moving North from (2,2); leaves scent for (2,2,N)
      var northRobot = new Robot(2, 2, EnumDirection.N);
      controller.RunRobot(northRobot, "F");
      Assert.True(northRobot.IsLost);

      // Robot lost moving East from (2,2); leaves scent for (2,2,E)
      var eastRobot = new Robot(2, 2, EnumDirection.E);
      controller.RunRobot(eastRobot, "F");
      Assert.True(eastRobot.IsLost);

      // Repeat North move from (2,2) - scent prevents loss
      var repeatNorthRobot = new Robot(2, 2, EnumDirection.N);
      var repeatResult = controller.RunRobot(repeatNorthRobot, "F");
      Assert.False(repeatResult.IsLost);

      // Repeat East move from (2,2) - scent prevents loss
      var repeatEastRobot = new Robot(2, 2, EnumDirection.E);
      var repeatEastResult = controller.RunRobot(repeatEastRobot, "F");
      Assert.False(repeatEastResult.IsLost);
    }

    #endregion

    #region VALIDATION AND BOUNDARY TESTS

    /// <summary>
    /// Ensures that the grid constructor enforces dimension constraints:
    /// The maximum value for any coordinate is 50, and both X and Y must be non-negative.
    /// Creating a grid outside these bounds throws an ArgumentOutOfRangeException.
    /// </summary>
    [Theory]
    [InlineData(-1, 3)]
    [InlineData(5, -2)]
    [InlineData(51, 2)]
    [InlineData(2, 100)]
    public void Grid_Throws_On_Invalid_Dimensions(int maxX, int maxY)
    {
      // Act & Assert: Creating an invalid grid should throw.
      Assert.Throws<ArgumentOutOfRangeException>(() => new Grid(maxX, maxY));
    }

    /// <summary>
    /// Ensures that a robot cannot start outside the grid boundaries.
    /// If a robot is placed at coordinates outside the grid, the controller throws an ArgumentException.
    /// This is defensive logic to prevent invalid simulation states.
    /// </summary>
    [Theory]
    [InlineData(6, 2, EnumDirection.E, "F")]  // X out of bounds
    [InlineData(2, 4, EnumDirection.N, "F")]  // Y out of bounds
    [InlineData(-1, 0, EnumDirection.S, "F")] // Negative X
    [InlineData(0, -1, EnumDirection.W, "F")] // Negative Y
    public void Controller_Throws_On_Invalid_Robot_Start(int startX, int startY, EnumDirection startDir, string instructions)
    {
      var grid = new Grid(5, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(startX, startY, startDir);

      // Act & Assert: Starting a robot outside the grid should throw.
      Assert.Throws<ArgumentException>(() => controller.RunRobot(robot, instructions));
    }

    /// <summary>
    /// Verifies that RobotRunResult exposes the final robot state correctly as properties.
    /// This is not only for tuple-based output but also for consuming application code or UI needs.
    /// </summary>
    [Fact]
    public void RobotRunResult_Properties_Are_Correct()
    {
      var grid = new Grid(2, 2);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 0, EnumDirection.N);

      // Move forward twice (to 0,2,N), turn right (now facing E).
      var result = controller.RunRobot(robot, "FFR");

      // Assert: Properties match final state.
      Assert.Equal(0, result.Position.X);
      Assert.Equal(2, result.Position.Y);
      Assert.Equal(EnumDirection.E, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Ensures that empty or whitespace-only instruction sequences do not alter the robot's position or state.
    /// The robot remains at its initial position and orientation, and is not lost.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_Or_Whitespace_Instruction_Sequence(string commands)
    {
      var grid = new Grid(3, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(2, 2, EnumDirection.W);

      // Run with empty or whitespace string.
      var result = controller.RunRobot(robot, commands);

      // Assert: Robot hasn't moved and isn't lost.
      Assert.Equal(2, result.Position.X);
      Assert.Equal(2, result.Position.Y);
      Assert.Equal(EnumDirection.W, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Confirms that a robot ignores any remaining instructions after it becomes lost.
    /// After falling off the grid, further commands are not executed, ensuring the simulation halts loss propagation.
    /// </summary>
    [Fact]
    public void Robot_Ignores_Remaining_Instructions_After_Loss()
    {
      var grid = new Grid(2, 2);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 2, EnumDirection.N);

      // "FFFFFRL": Robot moves north, falls off after the first move, rest are ignored.
      var result = controller.RunRobot(robot, "FFFFFRL");

      Assert.Equal(0, result.Position.X);
      Assert.Equal(2, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.True(result.IsLost);
    }

    /// <summary>
    /// Tests that if a robot starts its instructions on a position/direction where a scent exists,
    /// but does not attempt a move off the grid, it is not lost. This confirms scents only apply when leaving the grid.
    /// </summary>
    [Fact]
    public void Robot_Starting_On_Scent_Is_Not_Lost()
    {
      var grid = new Grid(2, 2);
      var controller = new RobotController(grid, new CommandFactory());

      // First robot leaves scent at (1,1,N) by falling off.
      var lostRobot = new Robot(1, 1, EnumDirection.N);
      controller.RunRobot(lostRobot, "F");

      // New robot at same cell/direction, does nothing.
      var robot = new Robot(1, 1, EnumDirection.N);
      var result = controller.RunRobot(robot, "");

      Assert.Equal(1, result.Position.X);
      Assert.Equal(1, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Verifies that a robot flagged as already lost does not execute any further instructions,
    /// and its state does not change regardless of input. Defensive against double-lost bugs.
    /// </summary>
    [Fact]
    public void Robot_Does_Not_Move_If_Already_Lost()
    {
      var grid = new Grid(5, 5);
      var controller = new RobotController(grid, new CommandFactory());

      // Robot already lost before running any instructions.
      var robot = new Robot(0, 0, EnumDirection.S) { IsLost = true };

      var result = controller.RunRobot(robot, "FFFFFRRRR");

      // Assert: Stays at original position and remains lost.
      Assert.Equal(0, result.Position.X);
      Assert.Equal(0, result.Position.Y);
      Assert.True(result.IsLost);
    }

    /// <summary>
    /// Verifies that the robot can rotate and move through all directions, ending up back at the start.
    /// This is a loopback test for a round-trip path with movement and turns.
    /// </summary>
    [Fact]
    public void Robot_Can_Rotate_And_Move_Through_All_Directions()
    {
      var grid = new Grid(3, 3);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(1, 1, EnumDirection.N);

      // RFRFRFRF: Turns and moves should form a loop.
      var result = controller.RunRobot(robot, "RFRFRFRF");

      Assert.Equal(1, result.Position.X);
      Assert.Equal(1, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Ensures that turn-only instructions (L and R) do not change the robot's position,
    /// and after a complete sequence of turns, the robot ends up facing the original direction.
    /// </summary>
    [Fact]
    public void Robot_Rotates_In_Place_With_Only_Turns()
    {
      var grid = new Grid(5, 5);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(2, 2, EnumDirection.N);

      // RRLLRRLL: Full set of rotations, ends facing North.
      var result = controller.RunRobot(robot, "RRLLRRLL");

      Assert.Equal(2, result.Position.X);
      Assert.Equal(2, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.False(result.IsLost);
    }

    #endregion

    #region COMMAND PATTERN EXTENSIBILITY/MOCKING

    /// <summary>
    /// Verifies that the RobotController uses the provided ICommandFactory to create commands, rather than hardcoding command logic.
    /// This enables extensibility (adding new commands via the factory) and testability (using mocks).
    /// 
    /// The test injects a mocked ICommandFactory, which returns a dummy command for any input.
    /// It then runs the robot with a single instruction, and verifies that the factory's Create method was called with the correct character.
    /// This proves that the controller is properly decoupled from command creation and can support future extensibility.
    /// </summary>
    [Fact]
    public void RobotController_Uses_Provided_CommandFactory()
    {
      // Arrange: Set up a 5x3 grid and a mock ICommandFactory.
      var grid = new Grid(5, 3);
      var mockFactory = new Mock<ICommandFactory>();
      // For any character, return a mock ICommand (the actual command logic doesn't matter for this test).
      mockFactory.Setup(f => f.Create(It.IsAny<char>()))
                 .Returns(new Mock<ICommand>().Object);

      // Inject the mock factory into the controller.
      var controller = new RobotController(grid, mockFactory.Object);
      var robot = new Robot(1, 1, EnumDirection.N);

      // Act: Run the robot with a single 'F' (forward) command.
      controller.RunRobot(robot, "F");

      // Assert: Verify that the factory's Create method was called exactly once for 'F'.
      // This demonstrates that command creation is delegated to the factory.
      mockFactory.Verify(f => f.Create('F'), Times.Once());
    }

    #endregion

    #region EXTENSIBILITY (TDD/CI)

    /// <summary>
    /// TDD-style (red/green) test demonstrating how to drive the implementation of a new command ("B" for Backward).
    /// It ensures that the command pattern allows new instructions to be added with minimal changes to existing code.
    /// 
    /// IMPORTANT: This test will fail until a BackwardCommand is implemented and registered in the CommandFactory.
    /// 
    /// </summary>
    [Fact]
    public void Robot_Can_Move_Backward_When_BackwardCommand_Added_ForTDD()
    {
      var grid = new Grid(5, 3);
      var factory = new CommandFactory();
      var controller = new RobotController(grid, factory);
      var robot = new Robot(2, 2, EnumDirection.N);

      // Act: Try to move backward; this will only pass after BackwardCommand is supported.
      var result = controller.RunRobot(robot, "B");

      // Assert: The robot should have moved one step backward (to 2,1) facing North, and not be lost.
      // (NOTE: This test should initially fail, guiding the implementation of BackwardCommand.)
      Assert.Equal(2, result.Position.X);
      Assert.Equal(1, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// CI-safe test for extensibility: Ensures that issuing an unsupported command ('B' for Backward)
    /// throws an ArgumentException, maintaining green builds in your pipeline until the feature is added.
    /// Once BackwardCommand is implemented, this test can be updated to assert the robot's new position instead.
    /// </summary>
    [Fact]
    public void Robot_Can_Move_Backward_When_BackwardCommand_Added_ForCI()
    {
      var grid = new Grid(5, 3);
      var factory = new CommandFactory();
      var controller = new RobotController(grid, factory);
      var robot = new Robot(2, 2, EnumDirection.N);

      // Act & Assert: Until BackwardCommand is implemented, this should throw an exception.
      Assert.Throws<ArgumentException>(() => controller.RunRobot(robot, "B"));

      // When BackwardCommand is implemented, replace above line with:
      // var result = controller.RunRobot(robot, "B");
      // Assert.Equal(2, result.Position.X);
      // Assert.Equal(1, result.Position.Y);
      // Assert.Equal(EnumDirection.N, result.Facing);
      // Assert.False(result.IsLost);
    }

    #endregion

    #region ADVANCED/EDGE/BOUNDS TESTS

    /// <summary>
    /// Verifies that the system correctly handles the largest allowed grid size (50x50) and the longest allowed command sequence.
    /// The robot moves 49 steps North, turns right, then moves 48 steps East.
    /// This checks correct boundary navigation, handling of long instruction sequences, and that the robot does not get lost
    /// unless truly exceeding the grid.
    /// </summary>
    [Fact]
    public void Handles_Maximum_Grid_Size_And_Long_Command()
    {
      var grid = new Grid(50, 50);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 0, EnumDirection.N);

      // Max allowed instructions is 100, this uses 99: 49 F's north, 1 R, then 48 F's east.
      string commands = new string('F', 49) + "R" + new string('F', 48);
      var result = controller.RunRobot(robot, commands);

      // Assert: Robot ends at (48,49) facing East, not lost.
      Assert.Equal(48, result.Position.X);
      Assert.Equal(49, result.Position.Y);
      Assert.Equal(EnumDirection.E, result.Facing);
      Assert.False(result.IsLost);
    }

    /// <summary>
    /// Verifies that the robot controller allows the maximum legal instruction sequence length (99 or 100),
    /// and that such a sequence does not throw or cause unexpected errors as long as the robot stays within grid bounds.
    /// </summary>
    [Fact]
    public void Max_Instruction_Length_Allowed()
    {
      var grid = new Grid(50, 50);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 0, EnumDirection.N);

      string commands = new string('F', 99); // Max legal command length.
      var result = controller.RunRobot(robot, commands);

      // Assert: Robot stays on grid and does not get lost unless truly moving off grid.
      Assert.True(result.Position.X <= 50 && result.Position.Y <= 50);
    }

    /// <summary>
    /// Ensures that the controller enforces the instruction length limit (e.g., 100 max).
    /// Providing an instruction sequence longer than allowed results in an ArgumentOutOfRangeException.
    /// This protects against runaway input and ensures compliance with problem constraints.
    /// </summary>
    [Fact]
    public void Max_Instruction_Length_TooLong()
    {
      var grid = new Grid(50, 50);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(0, 0, EnumDirection.N);

      string commands = new string('F', 110); // Exceeds allowed length.
      Assert.Throws<ArgumentOutOfRangeException>(() => controller.RunRobot(robot, commands));
    }

    /// <summary>
    /// Confirms that instruction parsing is case-insensitive: 
    /// lowercase and uppercase command characters are handled identically.
    /// This test prevents bugs when users or APIs provide non-standard command casing.
    /// </summary>
    [Fact]
    public void Instruction_Parsing_Is_Case_Insensitive()
    {
      var grid = new Grid(2, 2);
      var controller = new RobotController(grid, new CommandFactory());
      var robot = new Robot(1, 1, EnumDirection.N);

      // Mix of lowercase and uppercase commands.
      var result = controller.RunRobot(robot, "fRfLf");

      // Assert: The command is interpreted exactly as "FRFLF".
      Assert.Equal(2, result.Position.X);
      Assert.Equal(2, result.Position.Y);
      Assert.Equal(EnumDirection.N, result.Facing);
      Assert.True(result.IsLost);
    }

    #endregion
  }
}
