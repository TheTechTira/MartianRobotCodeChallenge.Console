using MartianRobotCodeChallenge.Console.Application;
using MartianRobotCodeChallenge.Console.Domain.Entities;
using MartianRobotCodeChallenge.Console.Domain.Enums;
using MartianRobotCodeChallenge.Console.Domain.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartianRobotCodeChallenge.Console.Tests
{
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
  }
}
