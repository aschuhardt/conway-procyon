namespace Conway
{
    public enum CellState
    {
        Dead,
        Alive
    }

    public struct Cell
    {
        public CellState CurrentState { get; set; }
        public CellState NextState { get; set; }

        public void DetermineNext(int aliveCount)
        {
            NextState = CurrentState switch
            {
                CellState.Dead when aliveCount == 3 => CellState.Alive,
                CellState.Alive when aliveCount < 2 || aliveCount > 3 => CellState.Dead,
                _ => CurrentState
            };
        }

        public void Step()
        {
            CurrentState = NextState;
        }
    }
}