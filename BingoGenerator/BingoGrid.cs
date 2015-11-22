using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingoGenerator
{
    public class BingoGrid
    {
        public List<List<int>> columns = new List<List<int>>();
        Random r = new Random(DateTime.Now.Millisecond);

        public BingoGrid()
        {
            for (int i = 0; i < 5; i++)
            {
                columns.Add(new List<int>());
            }
        }

        public void PopulateColumn(int index)
        {
            columns[index].Clear();

            int start = (index * 15) + 1;
            for (int i = 0; i < 5; i++)
            {
                var nextRandomNumber = r.Next(start, start + 15);
                while (columns[index].Count(n => n == nextRandomNumber) > 0)
                {
                    nextRandomNumber = r.Next(start, start + 15);
                }

                columns[index].Add(nextRandomNumber);
            }
        }
    }
}
