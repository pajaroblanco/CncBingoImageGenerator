using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoGenerator
{
    public class BingoNumberManager
    {
        public List<BingoGrid> BingoGrids { get; set; }

        public BingoNumberManager()
        {
            BingoGrids = new List<BingoGrid>();
        }

        public void GenerateBingoGrids(int numberOfGrids)
        {
            for (int i = 0; i < numberOfGrids; i++)
            {
                BingoGrids.Add(GenerateNewBingoGrid());
            }
        }

        private BingoGrid GenerateNewBingoGrid()
        {
            BingoGrid grid = new BingoGrid();

            for (int column = 0; column < 5; column++)
            {
                do
                {
                    grid.PopulateColumn(column);
                } while (SequenceAlreadyExists(grid.columns[column], column));
            }

            return grid;
        }

        private bool SequenceAlreadyExists(List<int> sequence, int column)
        {
            var existentSequences = BingoGrids.Select(n => n.columns[column].Select(m => m).ToList());
            var count = existentSequences.Count(n => n[0] == sequence[0] && n[1] == sequence[1] && n[2] == sequence[2] && n[3] == sequence[3] && n[4] == sequence[4]);
            return count > 0;
        }
    }
}
