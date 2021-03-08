using ProcyonSharp;
using ProcyonSharp.Attributes;
using ProcyonSharp.Bindings;
using ProcyonSharp.Bindings.Drawing;

namespace Conway
{
    [State(GameState.Menu)]
    public class MenuState : IGameState<GameState>
    {
        public void Load()
        {
        }

        public void Unload()
        {
        }

        public void Draw(DrawContext ctx)
        {
            ctx.DrawString(20, 20, $"Press {Global!.BuildInputDescription(nameof(Exit))} to exit");
            ctx.DrawString(20, 50, $"Press {Global!.BuildInputDescription(nameof(Start))} to begin");
        }

        public Global<GameState>? Global { get; set; }

        [Input(Key.Escape)]
        public void Exit()
        {
            Global!.PopState();
        }

        [Input(Key.Space)]
        public void Start()
        {
            Global!.PushState<PlayingState>();
        }
    }
}