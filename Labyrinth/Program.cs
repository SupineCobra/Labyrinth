using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;






namespace Labyrinth_of_madness
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string scoreFilePath = "scores.txt"; // Path to the file containing scores

            // Initialize score variables
            int currentScore = 0;
            int lastScore = 0;
            int highScore = ReadHighScore(scoreFilePath, out lastScore);

            string Mazestring =
                "#################\n" +
                "# ###### #####  #\n" +
                "#  ###   ##     #\n" +
                "##     #####  ##\n" +
                "### ##  #   # ###\n" +
                "#    ###### #  ##\n" +
                "####   ####   ###\n" +
                "## #            #\n" +
                "## #  ######  ###\n" +
                "##  ### ###    ##\n" + // Updated goal representation with three hashtags around it
                "###         #####\n" +
                "#################";
            char[,] Maze = new char[12, 17]; // Adjusted dimensions
            int playerX = 1; // Initial player position
            int playerY = 1;

            // Parsing the maze string into the char array
            string[] rows = Mazestring.Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                string row = rows[i].Trim();
                for (int j = 0; j < row.Length; j++)
                {
                    Maze[i, j] = row[j];
                }
            }

            // Add points and enemies to the maze
            AddRandomPoints(Maze);
            AddRandomEnemies(Maze);

            DrawMazeWithPlayer(Maze, playerX, playerY);

            // Player movement loop
            bool b = true;
            while (b)
            {
                // Read player input
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                ConsoleKey key = keyInfo.Key;

                // Update player position based on input
                int newPlayerX = playerX;
                int newPlayerY = playerY;
                Console.WriteLine($"Current player position: ({playerX}, {playerY})");
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        newPlayerY = Math.Max(0, playerY - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        newPlayerY = Math.Min(11, playerY + 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        newPlayerX = Math.Max(0, playerX - 1);
                        break;
                    case ConsoleKey.RightArrow:
                        newPlayerX = Math.Min(16, playerX + 1);
                        break;
                    case ConsoleKey.Q: // Quit the game if 'Q' is pressed
                                       // Update high score if the current score is higher
                        highScore = Math.Max(highScore, currentScore);
                        // Write high score to file
                        WriteHighScore(scoreFilePath, highScore, lastScore);
                        return;
                    default:
                        continue; // Ignore other keys
                }
                Console.WriteLine($"New player position: ({newPlayerX}, {newPlayerY})");

                // Check if the new position is a valid move
                if (newPlayerX >= 0 && newPlayerX < 17 && newPlayerY >= 0 && newPlayerY < 12)
                {
                    Console.WriteLine($"Maze array dimensions: {Maze.GetLength(0)} x {Maze.GetLength(1)}");

                    // Check if the new position is a valid move
                    if (Maze[newPlayerY, newPlayerX] != '#')
                    {
                        // Clear the previous player position
                        Maze[playerY, playerX] = ' ';

                        // Update player position
                        playerX = newPlayerX;
                        playerY = newPlayerY;

                        // Check if the player reached the goal
                        if (playerX == 9 && playerY == 4)
                        {
                            Console.WriteLine("Congratulations! You reached the goal!");
                            // Update high score if the current score is higher
                            highScore = Math.Max(highScore, currentScore);
                            // Update last score
                            lastScore = currentScore;
                            // Write high score to file
                            WriteHighScore(scoreFilePath, highScore, lastScore);
                            currentScore = 0; // Reset current score
                            return; // End the game
                        }

                        // Check for points and enemies
                        if (Maze[playerY, playerX] == '*')
                        {
                            Random rnd = new Random();
                            int gain = rnd.Next(1, 6);
                            currentScore += gain; // Increment score for collecting a point
                            Maze[playerY, playerX] = ' '; // Remove the point from the maze
                        }
                        else if (Maze[playerY, playerX] == '+')
                        {
                            Random rnd = new Random();
                            int deduction = rnd.Next(2, 5);
                            currentScore -= deduction; // Deduct score for encountering an enemy
                            if (currentScore < 0)
                            {
                                Console.WriteLine("Oh no! Your score dropped below zero. Game over!");
                                Thread.Sleep(500);
                                // Update last score
                                lastScore = currentScore;
                                // Write high score to file
                                WriteHighScore(scoreFilePath, highScore, lastScore);
                                return; // End the game
                            }
                        }

                        // Draw the maze with updated player position
                        DrawMazeWithPlayer(Maze, playerX, playerY);

                        // Display current score, last score, and high score
                        Console.WriteLine($"Current Score: {currentScore}");
                        Console.WriteLine($"Last Score: {lastScore}");
                        Console.WriteLine($"High Score: {highScore}");
                        if (lastScore == 0)
                        {
                            Console.WriteLine("-");
                        }

                        Console.WriteLine($"\n\nenemies are marked with '+' and they deduce points from your score \n points are marked with '*' and they add points to your score\n If your score drops below 0, you die.\n press 'Q' to exit game");
                     

                    }
                }
                else
                {
                    Console.WriteLine("Invalid player position!");
                }
            }
        }

        static void DrawMazeWithPlayer(char[,] maze, int playerX, int playerY)
        {
            Console.Clear(); // Clear the console before drawing

            // Draw the maze
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    if (i == playerY && j == playerX)
                    {
                         Console.ForegroundColor = ConsoleColor.Blue;    
                         Console.Write('P'); // Draw the player
                         Console.ResetColor();
                    }
                    else if (maze[i, j] == 'G') // Draw the goal
                    {
                        Console.Write("###"); // Draw three hashtags around the goal
                        j += 2; // Skip two extra columns
                    }
                    else if (maze[i, j] == '*') // Draw points in green color
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('*');
                        Console.ResetColor(); // Reset console color
                    }
                    else if (maze[i, j] == '+') // Draw enemies in red color
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('+');
                        Console.ResetColor(); // Reset console color
                    }
                    else
                    {
                        Console.Write(maze[i, j]); // Draw the maze cell
                    }
                }
                Console.WriteLine();
            }
        }

        static void AddRandomPoints(char[,] maze)
        {
            Random random = new Random();
            int pointsCount = random.Next(5, 10); // Random number of points between 5 and 10

            for (int i = 0; i < pointsCount; i++)
            {
                int x = random.Next(0, 17);
                int y = random.Next(0, 12);

                // Check if the random position is a valid place for a point and not the goal
                if (maze[y, x] != '#' && maze[y, x] != 'G' && maze[y, x] != '*' && maze[y, x] != '+')
                {

                    maze[y, x] = '*'; // Assign '*' to represent the point

                }
            }
        }

        static void AddRandomEnemies(char[,] maze)
        {
            Random random = new Random();
            int enemiesCount = random.Next(1, 5); // Random number of enemies between 1 and 4

            for (int i = 0; i < enemiesCount; i++)
            {
                int x = random.Next(0, 17);
                int y = random.Next(0, 12);

                // Check if the random position is a valid place for an enemy and not the goal, a point, or another enemy
                if (maze[y, x] != '#' && maze[y, x] != 'G' && maze[y, x] != '*' && maze[y, x] != '+')
                {
                    maze[y, x] = '+'; // Assign '+' to represent the enemy
                }
                else
                {
                    // Try again if the position is not valid
                    i--;
                }
            }
        }

        static int ReadHighScore(string scoreFilePath, out int lastScore)
        {
            int highScore = 0;
            lastScore = 0;

            // Check if the score file exists
            if (File.Exists(scoreFilePath))
            {
                // Read previous high score from the file
                using (StreamReader sr = new StreamReader(scoreFilePath))
                {
                    string line;
                    if ((line = sr.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out lastScore))
                        {
                            Console.WriteLine($"Last Score: {lastScore}");
                        }
                    }
                    if ((line = sr.ReadLine()) != null)
                    {
                        if (int.TryParse(line, out highScore))
                        {
                            Console.WriteLine($"Previous High Score: {highScore}");
                        }
                    }
                }
            }

            return highScore;
        }

        static void WriteHighScore(string scoreFilePath, int highScore, int lastScore)
        {
            // Write high score to file
            using (StreamWriter sw = new StreamWriter(scoreFilePath))
            {
                sw.WriteLine(lastScore); // Write last score to the file
                sw.WriteLine(highScore); // Write high score to the file
            }
        }
    }
}
