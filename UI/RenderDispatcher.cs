using Rogue.Core;
using Rogue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.UI
{
    public interface IRenderEvent { }
    public static class RenderDispatcher
    {
        public static event Action<IRenderEvent>? OnRenderEvent;
        public static void Raise(IRenderEvent e) => OnRenderEvent?.Invoke(e);
    }

    // Examples:
    public sealed class RenderActionMessageEvent : IRenderEvent
    {
        public IReadOnlyList<string> Lines { get; }
        public RenderActionMessageEvent(IEnumerable<string> lines) { Lines = lines.ToList(); }
    }
    public sealed class RedrawCellEvent : IRenderEvent
    {
        public int Row { get; }
        public int Col { get; }
        public Room Room { get; }
        public RedrawCellEvent(int row, int col, Room room) { Row = row; Col = col; Room = room; }
    }
    public sealed class RenderSidePanelEvent : IRenderEvent
    {
        public Player Player { get; }
        public Room CurrentRoom { get; }
        public RenderSidePanelEvent(Player player, Room room) { Player = player; CurrentRoom = room; }
    }
    public sealed class RenderMonsterPanelEvent : IRenderEvent
    {
        public Player Player { get; }
        public Room CurrentRoom { get; }
        public RenderMonsterPanelEvent(Player player, Room room) { Player = player; CurrentRoom = room; }
    }
    public sealed class RenderInstructionsEvent : IRenderEvent
    {
        public IReadOnlyList<string> Instructions { get; }
        public RenderInstructionsEvent(IEnumerable<string> instructions) { Instructions = instructions.ToList(); }
    }

    public sealed class RequestTextInputEvent : IRenderEvent
    {
        public string Prompt { get; }
        public Action<string?> OnInputReceived { get; }

        public RequestTextInputEvent(string prompt, Action<string?> onInputReceived)
        {
            Prompt = prompt;
            OnInputReceived = onInputReceived;
        }
    }

}
