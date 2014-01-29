using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuSharp
{
    public class SudokuBoard
    {
        public SudokuBoard(SudokuBoard copy)
        {
            _maxValue = copy._maxValue;
            tiles = new SudokuTile[copy.Width, copy.Height];
            CreateTiles();
            // Copy the tile values
            foreach (var pos in SudokuFactory.box(Width, Height))
            {
                tiles[pos.Item1, pos.Item2] = new SudokuTile(pos.Item1, pos.Item2, _maxValue);
                tiles[pos.Item1, pos.Item2].Value = copy.tiles[pos.Item1, pos.Item2].Value;
            }

            // Copy the rules
            foreach (SudokuRule rule in copy.rules) 
            {
                var ruleTiles = new HashSet<SudokuTile>();
                foreach (SudokuTile tile in rule) 
                {
                    ruleTiles.Add(tiles[tile.X, tile.Y]);
                }
                rules.Add(new SudokuRule(ruleTiles, rule.Description));
            }
        }

        public SudokuBoard(int width, int height, int maxValue)
        {
            _maxValue = maxValue;
            tiles = new SudokuTile[width, height];
            CreateTiles();
            if (_maxValue == width || _maxValue == height) // If maxValue is not width or height, then adding line rules would be stupid
                SetupLineRules();
        }

        public SudokuBoard(int width, int height) : this(width, height, Math.Max(width, height)) {}

        private int _maxValue;

        private void CreateTiles()
        {
            foreach (var pos in SudokuFactory.box(tiles.GetLength(0), tiles.GetLength(1)))
            {
                tiles[pos.Item1, pos.Item2] = new SudokuTile(pos.Item1, pos.Item2, _maxValue);
            }
        }

        private void SetupLineRules()
        {
            // Create rules for rows and columns
            for (int x = 0; x < Width; x++)
            {
                IEnumerable<SudokuTile> row = GetCol(x);
                rules.Add(new SudokuRule(row, "Row " + x.ToString()));
            }
            for (int y = 0; y < Height; y++)
            {
                IEnumerable<SudokuTile> col = GetRow(y);
                rules.Add(new SudokuRule(col, "Col " + y.ToString()));
            }
        }

        internal IEnumerable<SudokuTile> TileBox(int startX, int startY, int sizeX, int sizeY)
        {
            return from pos in SudokuFactory.box(sizeX, sizeY) select tiles[startX + pos.Item1, startY + pos.Item2];
        }

        private IEnumerable<SudokuTile> GetRow(int row)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                yield return tiles[i, row];
            }
        }
        private IEnumerable<SudokuTile> GetCol(int col)
        {
            for (int i = 0; i < tiles.GetLength(1); i++)
            {
                yield return tiles[col, i];
            }
        }

        private ISet<SudokuRule> rules = new HashSet<SudokuRule>();
        private SudokuTile[,] tiles;

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height {
            get { return tiles.GetLength(1); }
        }

        public void CreateRule(string description, params SudokuTile[] tiles)
        {
            rules.Add(new SudokuRule(tiles, description));
        }
        public void CreateRule(string description, IEnumerable<SudokuTile> tiles)
        {
            rules.Add(new SudokuRule(tiles, description));
        }

        public bool CheckValid()
        {
            return rules.All(rule => rule.CheckValid());
        }

        public IEnumerable<SudokuBoard> Solve()
        {
            ResetSolutions();
            SudokuProgress simplify = SudokuProgress.PROGRESS;
            while (simplify == SudokuProgress.PROGRESS) simplify = Simplify();

            if (simplify == SudokuProgress.FAILED)
                yield break;

            // Find one of the values with the least number of alternatives, but that still has at least 2 alternatives
            var query = from rule in rules
                        from tile in rule
                        where tile.PossibleCount > 1
                        orderby tile.PossibleCount ascending
                        select tile;

            SudokuTile chosen = query.FirstOrDefault();
            if (chosen == null)
            {
                // The board has been completed, we're done!
                yield return this;
                yield break;
            }

            Console.WriteLine("SudokuTile: " + chosen.ToString());

            foreach (var value in Enumerable.Range(1, _maxValue))
            {
                // Iterate through all the valid possibles on the chosen square and pick a number for it
                if (!chosen.IsValuePossible(value))
                    continue;
                var copy = new SudokuBoard(this);
                copy.Tile(chosen.X, chosen.Y).Fix(value, "Trial and error");
                foreach (var innerSolution in copy.Solve()) 
                    yield return innerSolution;
            }
            yield break;
        }

        public void Output()
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    Console.Write(tiles[x, y].ToStringSimple());
                }
                Console.WriteLine();
            }
        }

        public SudokuTile Tile(int x, int y)
        {
            return tiles[x, y];
        }

        private int _rowAddIndex;

        public void AddRow(string s)
        {
            // Method for initializing a board from string
            for (int i = 0; i < s.Length; i++)
            {
                var tile = tiles[i, _rowAddIndex];
                if (s[i] == '/')
                {
                    tile.Block();
                    continue;
                }
                int value = s[i] == '.' ? 0 : (int)Char.GetNumericValue(s[i]);
                tile.Value = value;
            }
            _rowAddIndex++;
        }

        internal void ResetSolutions()
        {
            foreach (SudokuTile tile in tiles)
                tile.ResetPossibles();
        }
        internal SudokuProgress Simplify()
        {
            SudokuProgress result = SudokuProgress.NO_PROGRESS;
            bool valid = CheckValid();
            if (!valid)
                return SudokuProgress.FAILED;

            foreach (SudokuRule rule in rules)
                result = SudokuTile.CombineSolvedState(result, rule.Solve());

            return result;
        }

        internal void AddBoxesCount(int boxesX, int boxesY)
        {
            int sizeX = Width / boxesX;
            int sizeY = Height / boxesY;

            var boxes = SudokuFactory.box(sizeX, sizeY);
            foreach (var pos in boxes)
            {
                IEnumerable<SudokuTile> boxTiles = TileBox(pos.Item1 * sizeX, pos.Item2 * sizeY, sizeX, sizeY);
                CreateRule("Box at (" + pos.Item1.ToString() + ", " + pos.Item2.ToString() + ")", boxTiles);
            }
        }

        internal void OutputRules()
        {
            foreach (var rule in rules)
            {
                Console.WriteLine(String.Join(",", rule) + " - " + rule.ToString());
            }
        }
    }
}
