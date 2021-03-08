using System;
using System.Timers;
using ProcyonSharp;
using ProcyonSharp.Attributes;
using ProcyonSharp.Bindings;
using ProcyonSharp.Bindings.Drawing;

namespace Conway
{
    [State(GameState.Playing)]
    public class PlayingState : IGameState<GameState>
    {
        private bool _running;
        private Timer _tickTimer;
        private World _world;
        private (int X, int Y) _cursor;
        private int _generation;

        public void Load()
        {
            _world = new World(32, 32);
            _world.Cells[16, 16].CurrentState = CellState.Alive;
            _world.Cells[16, 15].CurrentState = CellState.Alive;
            _world.Cells[16, 17].CurrentState = CellState.Alive;
            _generation = 0;
            _tickTimer = new Timer(100.0) {Enabled = false, AutoReset = true};
            _tickTimer.Elapsed += TickTimerOnElapsed;
            _cursor = (_world.Width / 2, _world.Height / 2);
            _running = false;
            Global!.Window.HighFpsMode = false;
        }

        public void Unload()
        {
            _tickTimer.Stop();
            Global!.Window.HighFpsMode = false;
        }

        public void Draw(DrawContext ctx)
        {
            var (windowWidth, windowHeight) = Global!.Window.Size;
            var (glyphWidth, glyphHeight) = Global!.Window.GlyphSize;
            
            ctx.DrawString(20, 20, $"Exit: {Global!.BuildInputDescription(nameof(Exit))}");
            ctx.DrawString(windowWidth - 300, 20, $"Generation: {_generation}");

            if (_running)
            {
                ctx.DrawString(20, 40,
                    $"Stop Simulation: {Global!.BuildInputDescription(nameof(ToggleRunning))}");
            }
            else
            {
                ctx.DrawString(20, 40,
                    $"Start Simulation: {Global!.BuildInputDescription(nameof(ToggleRunning))}");
                ctx.DrawString(20, 60, $"Reset: {Global!.BuildInputDescription(nameof(Reset))}");
                ctx.DrawString(20, 80, $"Toggle cell state: {Global!.BuildInputDescription(nameof(ToggleCellState))}");
            }

            _world.ForEachCell((ref Cell cell, int x, int y) =>
            {
                var glyph = cell.CurrentState switch
                {
                    CellState.Alive => "@",
                    CellState.Dead => ".",
                    _ => throw new ArgumentOutOfRangeException()
                };

                var glyphColor = Color.White;
                var backColor = Color.Black;
                if (!_running && x == _cursor.X && y == _cursor.Y)
                {
                    glyphColor = Color.Black;
                    backColor = new Color(1.0f, 0.0f, 0.0f);
                }
                
                ctx.DrawString(20 + x * glyphWidth, 100 + y * glyphHeight, glyph, false, glyphColor, backColor);
            });
        }

        public Global<GameState>? Global { get; set; }

        private void TickTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _generation++;
            _world.Step();
        }

        [Input(Key.Escape)]
        public void Exit()
        {
            Global!.PopState();
        }

        [Input(Key.R)]
        public void Reset()
        {
            if (_running)
                return;
                
            _generation = 0;

            _world.Reset();
        }

        [Input(Key.Enter)]
        public void ToggleRunning()
        {
            if (_running)
            {
                _tickTimer.Stop();
                _running = false;
                Global!.Window.HighFpsMode = false;
            }
            else
            {
                _tickTimer.Start();
                _running = true;
                Global!.Window.HighFpsMode = true;
            }
        }

        [Input(Key.Space)]
        public void ToggleCellState()
        {
            if (_running)
                return;

            var (x, y) = _cursor;
            var cell = _world.Cells[x, y];
            cell.CurrentState = cell.CurrentState switch
            {
                CellState.Dead => CellState.Alive,
                CellState.Alive => CellState.Dead,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _world.Cells[x, y] = cell;
        }

        [Input(Key.Left)]
        [Input(Key.Kp4)]
        public void MoveCursorLeft()
        {
            if (_running)
                return;
            
            if (_cursor.X > 0)
                _cursor.X--;
        }

        [Input(Key.Right)]
        [Input(Key.Kp6)]
        public void MoveCursorRight()
        {
            if (_running)
                return;
            
            if (_cursor.X < _world.Width)
                _cursor.X++;
        }

        [Input(Key.Up)]
        [Input(Key.Kp8)]
        public void MoveCursorUp()
        {
            if (_running)
                return;
            
            if (_cursor.Y > 0)
                _cursor.Y--;
        }

        [Input(Key.Down)]
        [Input(Key.Kp2)]
        public void MoveCursorDown()
        {
            if (_running)
                return;
            
            if (_cursor.Y < _world.Height)
                _cursor.Y++;
        }
    }
}