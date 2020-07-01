using System;
using System.Collections.Generic;
using System.Text;

namespace Antifree.Blazor.GamepadAPI
{
    public class GamepadEventDTO
    {
        public GamepadDTO gamepad { get; set; }
    }

    public class GamepadEvent
    {
        public Gamepad Gamepad { get; internal set; }

        internal GamepadEvent(Gamepad gamepad)
        {
            Gamepad = gamepad;
        }

        internal GamepadEvent(GamepadEventDTO dto)
        {
            Gamepad = new Gamepad(dto.gamepad);
        }
    }
}
