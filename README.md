# Martian Robots Code Challenge

A C#/.NET 8 solution for the classic Martian Robots problem, demonstrating clean architecture, extensibility, OOP, and comprehensive unit testing.

---

## 🛰️ Overview

This project models a swarm of robots exploring a rectangular grid on Mars. Robots execute command sequences (`L`, `R`, `F`) to navigate the grid, avoid getting lost, and leave “scents” to protect future robots from falling off at the same location and orientation. The code is designed for clarity, robustness, and easy extension (Command and Factory Pattern).

---

## 🗂️ Architecture & Design

- **Clean architecture:** Separation of domain, infrastructure, and presentation layers.
- **OOP & Value Objects:** Core concepts modeled as entities (`Robot`, `Grid`), enums (`EnumDirection`), and value objects (`Position`).
- **Command Pattern:** Supports current and future robot commands with minimal code change.
- **Defensive Validation:** All input and boundary conditions are validated.
- **Comprehensive Documentation:** XML summary comments and inline explanations throughout.
- **Unit Testing:** Thorough coverage using xUnit and Moq.

---


## 🗂️ Solution Structure

- **MartianRobotCodeChallenge.Console:** Contains all domain, infrastructure, and CLI logic (Main console application (CLI)).
- **MartianRobotCodeChallenge.Console.Tests:** Contains all automated tests for logic and edge cases (Unit test project (xUnit + Moq)).
  
---

## 🚀 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [xUnit](https://xunit.net/)
- [Moq](https://github.com/moq/moq4) (for mocking dependencies in tests)

---

## 🛠️ Getting Started

1. **Clone and build:**
    ```sh
    git clone https://github.com/TheTechTira/MartianRobotCodeChallenge.Console.git
    cd MartianRobotCodeChallenge.Console
    dotnet build
    ```

2. **Run the CLI app:**
    ```sh
    dotnet run --project MartianRobotCodeChallenge.Console
    ```

3. **Input Example:**  
    - You’ll be prompted for grid size, then each robot’s starting position and commands.
    - Submit an empty line to finish entering robots.
    ```
    5 3
    1 1 E
    RFRFRFRF
    3 2 N
    FRRFLLFFRRFLL
    0 3 W
    LLFFFLFLFL
    ```

4. **Output Example:**
    ```
    1 1 E
    3 3 N LOST
    2 3 S
    ```

---

## ✅ Acceptance Criteria

This solution is considered complete when **all** of the following requirements are met:

### 1. Grid Modeling
- The Martian surface is a bounded, rectangular grid.
- The upper-right grid coordinates are supplied as input (lower-left is always (0, 0)).
- **Maximum value** for any coordinate is **50**.

### 2. Robot Movement
- Each robot is initialized with a position (`X Y`) and an orientation (`N`, `S`, `E`, or `W`).
- Each robot receives a string of instructions (`L`, `R`, `F`):
  - `L`: Turn left 90°, no movement.
  - `R`: Turn right 90°, no movement.
  - `F`: Move forward one grid position in the current orientation.
- Directions: North increases Y, East increases X, South decreases Y, West decreases X.

### 3. Extensibility
- Code is designed to support future command types with minimal change (Command Pattern).

### 4. Grid Boundaries & Scent Logic
- Robots moving “off” the grid are lost forever and leave a “scent” at the last valid position/direction.
- Any future robot attempting to move off the grid from the same cell and orientation will ignore the instruction instead of getting lost.

### 5. Input & Output
- **Input:**  
  - First line: Grid upper-right coordinates (e.g., `5 3`)
  - For each robot: one line for starting position/direction, one line for instructions
- **Output:**  
  - Final position and orientation for each robot, appending `LOST` if the robot falls off the grid.

### 6. Constraints
- All coordinates ≤ 50
- All instruction strings are **less than 100** characters

### 7. Sample Input/Output

**Input:**
5 3
1 1 E
RFRFRFRF
3 2 N
FRRFLLFFRRFLL
0 3 W
LLFFFLFLFL

**Output:**
1 1 E
3 3 N LOST
2 3 S

---

## 🧪 Running the Tests

Unit tests are implemented using **xUnit** and **Moq** for mocking and injection.  
Core business rules, grid logic, scent logic, and extensibility are all covered by automated tests.

To run the full test suite:

```sh
dotnet test MartianRobotCodeChallenge.Console.Tests
```
**Important** - 1 test is designed to fail:
- 31 tests in total.
- 30 should pass.
- 1 should fail due to show TDD unit test.

---

## 📝 Technical Notes and Design Decisions
- **Clean architecture**: Domain, infrastructure, and CLI clearly separated.
- **OOP**: Entities (`Robot`, `Grid`), value objects (`Position`), and enums (`EnumDirection`).
- **Command Pattern**: Each command (`L`, `R`, `F`) is an injectable strategy. New commands are easily added.
- **Input validation**: Defensive checks for coordinates, instructions, and command lengths.
- **Extensive XML docs**: Every class and method is documented for maintainability.
- **Comprehensive unit tests**: All requirements and edge cases are validated by tests.

---

## 📚 Documentation
All public classes and methods are documented using XML comments.

--- 

## 🧭 How I Approached This Challenge

**1.Requirements-Driven Design:**
- I began by carefully analyzing the problem statement and acceptance criteria, identifying all edge cases, and mapping out the domain entities (robots, grid, commands, scents).

**2. Clean Architecture:**
- To maximize clarity, testability, and extensibility, I separated core domain logic (entities, value objects, command pattern) from presentation (CLI) and infrastructure. This allows future features or interfaces (e.g., web API) with minimal refactoring.

**3. OOP and Command Pattern:**
- I applied the Factory and Command Pattern for robot instructions (L, R, F) — enabling easy addition of new commands (like B for backward) in the future, which is explicitly requested in the requirements.
- Each robot action is a command object, and a factory is used to instantiate the appropriate command based on instruction input, supporting extensibility and testability.
- This demonstrates both forward-thinking and adherence to SOLID principles.

**4. Defensive Programming & Validation:**
- All input (grid size, robot positions, commands) is robustly validated, with clear user feedback for invalid input.
- Boundary cases (edge of grid, robot loss, scent logic) are handled defensively to ensure system resilience.

**5. Documentation & Readability:**
- Every class and method includes XML summary documentation, with explanatory comments in complex sections.
- The CLI offers user-friendly instructions and contextual feedback after each robot.
- With a visual grid as a **fun little bonus**! :D

**7. Comprehensive Unit Testing:**
- I used xUnit and Moq to thoroughly test business rules, edge cases, error handling, and command extensibility.
- Tests are structured for both basic and advanced scenarios, with clear separation and full coverage.

**8. Developer Experience**
- The codebase is easy to navigate and maintain, with clean naming, clear folder structure, and summary output on program completion.

---

## ⏱️ From Start to Finish

- Core coding and design: **~1 hour** (if you subtract coffee breaks)
- Total elapsed time with caffeine and interruptions: **1 hour 26 minutes**

Great problems—and good coffee—make for productive coding!

## 🎉 Closing Thoughts
This code challenge was a fun opportunity to demonstrate robust C# design, architecture, and testing approaches.

I enjoyed bringing Martian Robots to life and ensuring the solution is clear, extensible, and easy to maintain.

If you have feedback or would like to see more advanced features — let me know


### Happy coding! 🚀

---

## Branch History and Merges:

![image](https://github.com/user-attachments/assets/c9099d2f-488f-4f12-905d-264827835607)

Can also be viewed here: https://github.com/TheTechTira/MartianRobotCodeChallenge.Console/network
