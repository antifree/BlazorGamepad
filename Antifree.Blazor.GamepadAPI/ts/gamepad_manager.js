"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Blazor = void 0;
var Blazor;
(function (Blazor) {
    class GamepadManagerType {
        constructor() { }
        register(managerDotNetRef) {
            this.managerDotNetRef = managerDotNetRef;
            this.init();
        }
        updateGamepads() {
            const infos = [];
            const gamepads = window.navigator.getGamepads();
            for (let i = 0; i < gamepads.length; i++) {
                const gamepad = gamepads[i];
                if (gamepad) {
                    infos.push(this.buildGamepadDTO(gamepad));
                }
            }
            return infos;
        }
        findGamepad(index) {
            for (const gamepad of window.navigator.getGamepads()) {
                if (gamepad && gamepad.index === index) {
                    return gamepad;
                }
            }
            return null;
        }
        playVibrationEffect(index, effectName, options) {
            const gamepad = this.findGamepad(index);
            if (gamepad && gamepad.vibrationActuator) {
                gamepad.vibrationActuator.playEffect(effectName, options);
            }
        }
        init() {
            window.addEventListener("gamepadconnected", (ev) => {
                const gamepad = ev.gamepad;
                this.managerDotNetRef.invokeMethodAsync("OnGamepadConnectedHandler", { gamepad: this.buildGamepadDTO(gamepad) });
            });
            window.addEventListener("gamepaddisconnected", (ev) => {
                const gamepad = ev.gamepad;
                this.managerDotNetRef.invokeMethodAsync("OnGamepadDisconnectedHandler", { gamepad: this.buildGamepadDTO(gamepad) });
            });
            this.managerDotNetRef.invokeMethodAsync("SetReady");
        }
        buildGamepadDTO(gamepad) {
            return {
                id: gamepad.id,
                index: gamepad.index,
                mapping: gamepad.mapping,
                axes: gamepad.axes.map(n => n),
                buttons: gamepad.buttons.map(b => { return { pressed: b.pressed, value: b.value }; }),
                connected: gamepad.connected,
                timestamp: gamepad.timestamp,
                supportVibration: gamepad.vibrationActuator != null
            };
        }
    }
    Blazor.GamepadManager = new GamepadManagerType();
})(Blazor = exports.Blazor || (exports.Blazor = {}));
