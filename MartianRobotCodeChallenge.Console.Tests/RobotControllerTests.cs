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
  }
}
