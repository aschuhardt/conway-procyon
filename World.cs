using System;

namespace Conway
{
    public class World
    {
        public World(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[Width, Height];

            Reset();
        }

        public int Width { get; }
        public int Height { get; }

        public Cell[,] Cells { get; }

        private int GetCountOfLiveCellsAdjacentTo(int centerX, int centerY)
        {
            var count = 0;

            for (var x = centerX - 1; x < centerX + 2; x++)
            for (var y = centerY - 1; y < centerY + 2; y++)
            {
                // don't count the center
                if (x == centerX && y == centerY)
                    continue;

                // don't count out-of-bounds
                if (x < 0 || x >= Width || y < 0 || y >= Height)
                    continue;

                if (Cells[x, y].CurrentState == CellState.Alive)
                    count++;
            }

            return count;
        }

        public void ForEachCell(CellOperationWithCoordinates action)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                action(ref Cells[x, y], x, y);
        }

        public void ForEachCell(CellOperation action)
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                action(ref Cells[x, y]);
        }

        public delegate void CellOperationWithCoordinates(ref Cell cell, int x, int y);

        public delegate void CellOperation(ref Cell cell);

        public void Step()
        {
            ForEachCell((ref Cell c, int x, int y) =>
                c.DetermineNext(GetCountOfLiveCellsAdjacentTo(x, y)));
            ForEachCell((ref Cell c) => c.Step());
        }

        public void Reset()
        {
            ForEachCell((ref Cell c) =>
            {
                c.CurrentState = CellState.Dead;
                c.NextState = CellState.Dead;
            }); 
        }
    }
}