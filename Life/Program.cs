using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
 
namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        public bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class CliLifeSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
        public static CliLifeSettings LoadSettings(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<CliLifeSettings>(json);
        }
    }
 
    public class Board
    {
        public bool IsSymmetric()
        {
            int halfWidth = Columns / 2;
            for (int x = 0; x < halfWidth; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[x, y].IsAlive != Cells[Columns - x - 1, y].IsAlive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
 
        public double GetSymmetricCount()
        {
            double count = 0;
            int halfWidth = Columns / 2;
            for (int x = 0; x < halfWidth; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[x, y].IsAlive == Cells[Columns - x - 1, y].IsAlive)
                    {
                        count++;
                    }
                }
            }
            if (Columns % 2 != 0)
            {
                int centerX = Columns / 2;
                for (int y = 0; y < Rows; y++)
                {
                    if (Cells[centerX, y].IsAlive)
                    {
                        count++;
                    }
                }
            }
            count = count/(Columns * Rows) * 100; 
            return count*2;
        }
        public int StableGenerations { get; set; }
        private int stableCount;
        public void CheckStability()
        {
            if (GetAliveCellsCount() == TotalCells || GetAliveCellsCount() == 0)
                stableCount++;
            else
                stableCount = 0;
 
            if (stableCount >= 15)
            {
                Console.WriteLine("Stable phase reached");
                StableGenerations = stableCount;
            }
        }
        public int TotalCells
        {
            get { return Columns * Rows; }
        }
 
        public int GetAliveCellsCount()
        {
            int count = 0;
            foreach (var cell in Cells)
            {
                if (cell.IsAlive)
                    count++;
            }
            return count;
        }
        public int FindFigure(Cell[,] gameBoard, Cell[,] figure)
        {
            int boardWidth = gameBoard.GetLength(0);
            int boardHeight = gameBoard.GetLength(1);
            int figureWidth = figure.GetLength(0);
            int figureHeight = figure.GetLength(1);
            int matchCount = 0;
 
            for (int x = 0; x < boardWidth - figureWidth; x++)
            {
                for (int y = 0; y < boardHeight - figureHeight; y++)
                {
                    bool isMatch = true;
 
                    for (int fx = 0; fx < figureWidth; fx++)
                    {
                        for (int fy = 0; fy < figureHeight; fy++)
                        {
                            if (figure[fx, fy].IsAlive != gameBoard[x + fx, y + fy].IsAlive)
                            {
                                isMatch = false;
                                break;
                            }
                        }
                        if (!isMatch)
                        {
                            break;
                        }
                    }
 
                    if (isMatch)
                    {
                        matchCount++;
                    }
                }
            }
 
            return matchCount;
        }
 
        public readonly Cell[,] Cells;
        public readonly int CellSize;
 
        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public int generetion = 0;
        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;
 
            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();
 
            ConnectNeighbors();
            Randomize(liveDensity);
        }
 
        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }
 
        public void Advance()
        {
            generetion++;
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
 
            var originalAliveCells = GetAliveCellsCount();
            foreach (var cell in Cells)
                cell.Advance();
 
            var newAliveCells = GetAliveCellsCount();
            if (newAliveCells == originalAliveCells)
                stableCount++;
            else
                stableCount = 0;
 
            Console.WriteLine($"Generetion: {generetion}");
            if (stableCount >= 15)
            {
                Console.WriteLine("Stable phase reached");
                StableGenerations = stableCount;
            }
            if (IsSymmetric())
            {
                Console.WriteLine($"Symmetric cells: {GetSymmetricCount()} / {generetion}");
            }
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;
 
                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;
 
                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
    }
    class Program
    {
        public static void LoadPatternFromFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var pattern = new bool[lines[0].Length, lines.Length];
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    pattern[x, y] = (lines[y][x] == '1');
                }
            }
            int centerX = pattern.GetLength(0) / 2;
            int centerY = pattern.GetLength(1) / 2;
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                for (int x = 0; x < pattern.GetLength(0); x++)
                {
                    board.Cells[board.Columns / 2 - centerX + x, board.Rows / 2 - centerY + y].IsAlive = pattern[x, y];
                }
            }
        }
        public static void SaveToFile(string filePath)
        {
            var cellsArray = new bool[board.Columns, board.Rows];
            for (int col = 0; col < board.Columns; col++)
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    cellsArray[col, row] = board.Cells[col, row].IsAlive;
                }
            }
            var json = JsonConvert.SerializeObject(cellsArray);
            File.WriteAllText(filePath, json);
        }
 
        public static void LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var cellsArray = JsonConvert.DeserializeObject<bool[,]>(json);
            for (int col = 0; col < board.Columns; col++)
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    board.Cells[col, row].IsAlive = cellsArray[col, row];
                }
            }
        }
 
        static Board board;
        static private void Reset(string settingsFilePath)
        {
            var settings = CliLifeSettings.LoadSettings(settingsFilePath);
            board = new Board(
                width: settings.Width,
                height: settings.Height,
                cellSize: settings.CellSize,
                liveDensity: settings.LiveDensity);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
            Reset("settings.json");
            Console.WriteLine("Choose a figure:");
            Console.WriteLine("1");
            Console.WriteLine("2");
            Console.WriteLine("3");
            int choice = int.Parse(Console.ReadLine());
            string patternFilePath = Path.Combine("Patterns", choice + ".txt");
            LoadPatternFromFile(patternFilePath);
            while (true)
            {
                Render();
                board.Advance();
 
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.S)
                    {
                        SaveToFile("state.txt");
                        Thread.Sleep(100);
                    }
                    else if (key == ConsoleKey.L)
                    {
                        LoadFromFile("state.txt");
                        Thread.Sleep(100);
                    }
                }
                Cell[,] figure = new Cell[,]
                {
                    { new Cell() { IsAlive = false }, new Cell() { IsAlive = true } },
                    { new Cell() { IsAlive = true }, new Cell() { IsAlive = true } }
                };
                int matchCount = board.FindFigure(board.Cells, figure);
                Console.WriteLine("S - Save, L - Load");
                Console.WriteLine($"Total cells: {board.TotalCells}");
                Console.WriteLine($"Alive cells: {board.GetAliveCellsCount()}");
                Console.WriteLine($"Number of matches with the figure: {matchCount}");
                Console.WriteLine($"Symmetry of the field vertically: {board.GetSymmetricCount()}%");
                Console.WriteLine($"Symmetry of the entire system: {board.IsSymmetric()}");
                Thread.Sleep(1);
                Console.Clear();
            }
        }
    }
}
