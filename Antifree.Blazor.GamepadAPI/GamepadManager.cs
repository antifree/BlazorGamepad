using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.JSInterop;

namespace Antifree.Blazor.GamepadAPI
{
    public class VibrationOptions 
    {
        public long StartDelay { get; set; } = 0;

        public long Duration { get; set; } = 1000;

        public double WeakMagnitude { get; set; } = 1.0;

        public double StrongMagnitude { get; set; } = 1.0;
    }

    public class GamepadManagerOptions
    {
        public TimeSpan Latency { get; set; } = TimeSpan.FromMilliseconds(25);
    }

    public class GamepadManager: IDisposable
    {

        public event Action<GamepadEvent> OnGamepadConnected;

        public event Action<GamepadEvent> OnGamepadDisconnected;

        public event Action OnUpdate;

        private List<Gamepad> _gamepads = new List<Gamepad>();

        private readonly IJSRuntime _jsRuntime;

        public bool IsReady { get; private set; } = false;

        private readonly System.Timers.Timer _updateTimer;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public GamepadManager(IJSRuntime jsRuntime, GamepadManagerOptions options)
        {
            _jsRuntime = jsRuntime;

            _updateTimer = new System.Timers.Timer(options.Latency.TotalMilliseconds);
            _updateTimer.Elapsed += Update;
            _updateTimer.Enabled = false;
        }

        public Gamepad[] GetGamepads()
        {
            return _gamepads.ToArray();
        }

        public void SetLatency(TimeSpan time)
        {
            _updateTimer.Interval = time.TotalMilliseconds;
        }

        [JSInvokable(nameof(SetReady)), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetReady()
        {
            IsReady = true;
            _updateTimer.Enabled = true;
        }

        [JSInvokable(nameof(OnGamepadConnectedHandler)), EditorBrowsable(EditorBrowsableState.Never)]
        public void OnGamepadConnectedHandler(GamepadEventDTO eventDTO)
        {
            var gamepad = _gamepads.Find(g => g.Index == eventDTO.gamepad.index);
            if (gamepad == null)
            {
                var ev = new GamepadEvent(eventDTO);

                _gamepads.Add(ev.Gamepad);

                OnGamepadConnected?.Invoke(ev);
                OnUpdate?.Invoke();
            }
        }

        [JSInvokable(nameof(OnGamepadDisconnectedHandler)), EditorBrowsable(EditorBrowsableState.Never)]
        public void OnGamepadDisconnectedHandler(GamepadEventDTO eventDTO)
        {
            var gamepad = _gamepads.Find(g => g.Index == eventDTO.gamepad.index);
            if (gamepad != null) 
            {
                var ev = new GamepadEvent(gamepad);

                gamepad.Connected = false;
                _gamepads.Remove(gamepad);

                OnGamepadDisconnected?.Invoke(ev);
                OnUpdate?.Invoke();
            }
        }

        public Task PlayVibrationEffectAsync(long gamepadIndex, string effectName, VibrationOptions options = null)
        {
            var gamepad = _gamepads.Find(g => g.Index == gamepadIndex);
            return PlayVibrationEffectAsync(gamepad, effectName, options = null);
        }

        public async Task PlayVibrationEffectAsync(Gamepad gamepad, string effectName, VibrationOptions options = null) 
        {
            if (options == null) 
            {
                options = new VibrationOptions();
            }

            if (gamepad != null && gamepad.SupportVibration) 
            { 
                await _jsRuntime.InvokeVoidAsync("Antifree.Blazor.GamepadManager.playVibrationEffect", gamepad.Index, effectName, options);
            }
        }

        private async void Update(object sender, EventArgs args)
        {
            if (_gamepads.Count > 0)
                await UpdateGamepadsAsync();
        }

        private async Task UpdateGamepadsAsync()
        {
            var infos = await _jsRuntime.InvokeAsync<List<GamepadUpdateInfo>>("Antifree.Blazor.GamepadManager.updateGamepads");
            if (infos.Count > 0)
            {
                foreach (var info in infos)
                { 
                    if (info != null)
                    {
                        var gamepad = _gamepads.Find(g => g.Index == info.index);
                        if (gamepad != null) {
                            gamepad.Update(info);
                        }
                    }
                }
                OnUpdate?.Invoke();
            }
        }

        public void Dispose()
        {
            _updateTimer.Stop();
        }
    }
}
