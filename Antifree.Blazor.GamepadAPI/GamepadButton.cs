using System;
using System.Collections.Generic;
using System.Text;

namespace Antifree.Blazor.GamepadAPI
{
    public class GamepadButtonDTO
    { 
        public bool pressed { get; set; }

        public double value { get; set; }
    }

    public class GamepadButton
    {

        public bool Pressed { get; internal set; }

        public double Value { get; internal set; }

        internal GamepadButton(GamepadButtonDTO dto)
        {
            UpdateFromDTO(dto);
        }

        internal void UpdateFromDTO(GamepadButtonDTO dto)
        {
            Pressed = dto.pressed;
            Value = dto.value;
        }

    }
}
