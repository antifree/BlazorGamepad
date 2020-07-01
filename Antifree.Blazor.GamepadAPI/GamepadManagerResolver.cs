using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Antifree.Blazor.GamepadAPI
{
    public class GamepadManagerResolver
    {
        private readonly IJSRuntime _jsRuntime;

        private GamepadManager _manager;

        private GamepadManagerOptions _options;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private bool _scriptLoaded = false;

        public GamepadManagerResolver(IJSRuntime jsRuntime, IOptions<GamepadManagerOptions> options)
        {
            _jsRuntime = jsRuntime;
            _options = options?.Value ?? new GamepadManagerOptions();
        }

        public async Task<GamepadManager> ResolveAsync()
        {
            if (_manager == null) {
                _manager = new GamepadManager(_jsRuntime, _options);

                await RegisterManagerOnClientSideAsync();
            }

            return _manager;
        }

        private async Task RegisterManagerOnClientSideAsync()
        {
            await EnsureScriptLoadedAsync();

            var managerObjRef = DotNetObjectReference.Create(_manager);
            await _jsRuntime.InvokeVoidAsync("Antifree.Blazor.GamepadManager.register", new object[] { managerObjRef });
        }

        private async ValueTask EnsureScriptLoadedAsync()
        {
            if (_scriptLoaded) return;

            await _semaphore.WaitAsync();
            try {
                if (_scriptLoaded) return;
                const string scriptPath = "/Antifree.Blazor.GamepadAPI/gamepad_manager.min.js";
                await _jsRuntime.InvokeVoidAsync("eval", 
                    "new Promise(r=>((d,t,s)=>(h=>h.querySelector(t+`[src=\"${s}\"]`)?r():(e=>(e.src=s,e.onload=r,h.appendChild(e)))(d.createElement(t)))(d.head))(document,'script','" + scriptPath + "'))");
                _scriptLoaded = true;
            }
            catch (Exception) { }
            finally { _semaphore.Release(); }
        }
    }
}
