using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antifree.Blazor.GamepadAPI
{
    public class GamepadUpdateInfo { 
        public long index { get; set; }

        public double[] axes { get; set; } = new double[] { };

        public GamepadButtonDTO[] buttons { get; set; } = new GamepadButtonDTO[] { };

        public double timestamp { get; set; }

    }
    public class GamepadDTO
    {
        public GamepadButtonDTO[] buttons { get; set; } = new GamepadButtonDTO[] { };
        public double[] axes { get; set; } = new double[] { };
        public string id { get; set; }
        public string mapping { get; set; }
        public bool connected { get; set; }

        public long index { get; set; }
        public double timestamp { get; set; }

        public bool supportVibration { get; set; }
    }

    public class Gamepad
    {
        private List<GamepadButton> _buttons;
        public IReadOnlyList<GamepadButton> Buttons => _buttons;

        private List<double> _axes;

        public IReadOnlyList<double> Axes => _axes;

        public bool Connected { get; internal set; }

        public string Id { get; internal set; }

        public string Mapping { get; internal set; }

        public long Index { get; internal set; }

        public double Timestamp { get; internal set; }

        public bool SupportVibration { get; internal set; }

        internal Gamepad(GamepadDTO dto)
        {
            UpdateFromDTO(dto);
        }

        internal void Update(GamepadUpdateInfo info)
        {
            _axes = info.axes.ToList();
            _buttons = info.buttons.Select(b => new GamepadButton(b)).ToList();
            Timestamp = info.timestamp;
        }

        internal void UpdateFromDTO(GamepadDTO dto)
        {
            _axes = dto.axes.ToList();
            _buttons = dto.buttons.Select(b => new GamepadButton(b)).ToList();
            Connected = dto.connected;
            Id = dto.id;
            Mapping = dto.mapping;
            Index = dto.index;
            Timestamp = dto.timestamp;
            SupportVibration = dto.supportVibration;
        }

    }
}
