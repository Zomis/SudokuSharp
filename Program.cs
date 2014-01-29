using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSharp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            SolveFail();
            SolveClassic();
            SolveSmall();
            SolveExtraZones();
            SolveHyper();
            SolveSamurai();
            SolveIncompleteClassic();
        }
        private static void SolveFail()
        {
            SudokuBoard board = SudokuFactory.SizeAndBoxes(4, 4, 2, 2);
            board.AddRow("0003");
            board.AddRow("0204"); // the 2 must be a 1 on this row to be solvable
            board.AddRow("1000");
            board.AddRow("4000");
            CompleteSolve(board);
        }
        private static void SolveExtraZones()
        {
            // http://en.wikipedia.org/wiki/File:Oceans_Hypersudoku18_Puzzle.svg
            SudokuBoard board = SudokuFactory.ClassicWith3x3BoxesAndHyperRegions();
            board.AddRow(".......1.");
            board.AddRow("..2....34");
            board.AddRow("....51...");
            board.AddRow(".....65..");
            board.AddRow(".7.3...8.");
            board.AddRow("..3......");
            board.AddRow("....8....");
            board.AddRow("58....9..");
            board.AddRow("69.......");
            CompleteSolve(board);
        }
        private static void SolveSmall()
        {
            SudokuBoard board = SudokuFactory.SizeAndBoxes(4, 4, 2, 2);
            board.AddRow("0003");
            board.AddRow("0004");
            board.AddRow("1000");
            board.AddRow("4000");
            CompleteSolve(board);
        }
        private static void SolveHyper()
        {
            // http://en.wikipedia.org/wiki/File:A_nonomino_sudoku.svg
            string[] areas = new string[]{
               "111233333",
               "111222333",
               "144442223",
               "114555522",
               "444456666",
               "775555688",
               "977766668",
               "999777888",
               "999997888"
            };
            SudokuBoard board = SudokuFactory.ClassicWithSpecialBoxes(areas);
            board.AddRow("3.......4");
            board.AddRow("..2.6.1..");
            board.AddRow(".1.9.8.2.");
            board.AddRow("..5...6..");
            board.AddRow(".2.....1.");
            board.AddRow("..9...8..");
            board.AddRow(".8.3.4.6.");
            board.AddRow("..4.1.9..");
            board.AddRow("5.......7");
            CompleteSolve(board);

        }
        private static void SolveSamurai()
        {
            // http://www.freesamuraisudoku.com/1001HardSamuraiSudokus.aspx?puzzle=42
            SudokuBoard board = SudokuFactory.Samurai();
            board.AddRow("6..8..9..///.....38..");
            board.AddRow("...79....///89..2.3..");
            board.AddRow("..2..64.5///...1...7.");
            board.AddRow(".57.1.2..///..5....3.");
            board.AddRow(".....731.///.1.3..2..");
            board.AddRow("...3...9.///.7..429.5");
            board.AddRow("4..5..1...5....5.....");
            board.AddRow("8.1...7...8.2..768...");
            board.AddRow(".......8.23...4...6..");
            board.AddRow("//////.12.4..9.//////");
            board.AddRow("//////......82.//////");
            board.AddRow("//////.6.....1.//////");
            board.AddRow(".4...1....76...36..9.");
            board.AddRow("2.....9..8..5.34...81");
            board.AddRow(".5.873......9.8..23..");
            board.AddRow("...2....9///.25.4....");
            board.AddRow("..3.64...///31.8.....");
            board.AddRow("..75.8.12///...6.14..");
            board.AddRow(".......2.///.31...9..");
            board.AddRow("..17.....///..7......");
            board.AddRow(".7.6...84///8...7..5.");
            CompleteSolve(board);
        }

        private static void SolveClassic()
        {
            var board = SudokuFactory.ClassicWith3x3Boxes();
            board.AddRow("...84...9");
            board.AddRow("..1.....5");
            board.AddRow("8...2146.");
            board.AddRow("7.8....9.");
            board.AddRow(".........");
            board.AddRow(".5....3.1");
            board.AddRow(".2491...7");
            board.AddRow("9.....5..");
            board.AddRow("3...84...");
            CompleteSolve(board);
        }

        private static void SolveIncompleteClassic()
        {
            var board = SudokuFactory.ClassicWith3x3Boxes();
            board.AddRow("...84...9");
            board.AddRow("..1.....5");
            board.AddRow("8...2.46."); // Removed a "1" on this line
            board.AddRow("7.8....9.");
            board.AddRow(".........");
            board.AddRow(".5....3.1");
            board.AddRow(".2491...7");
            board.AddRow("9.....5..");
            board.AddRow("3...84...");
            CompleteSolve(board);
        }

        private static void CompleteSolve(SudokuBoard board)
        {
            Console.WriteLine("Rules:");
            board.OutputRules();
            Console.WriteLine("Board:");
            board.Output();
            var solutions = board.Solve().ToList();
            Console.WriteLine("Base Board Progress:");
            board.Output();
            Console.WriteLine("--");
            Console.WriteLine("--");
            Console.WriteLine("All " + solutions.Count + " solutions:");
            var i = 1;
            foreach (var solution in solutions)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Solution " + i++.ToString() + " / " + solutions.Count + ":");
                solution.Output();
            }
        }
    }
}
