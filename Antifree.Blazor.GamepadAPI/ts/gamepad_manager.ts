declare global {
    interface Gamepad {
        vibrationActuator: any;
    }
}

export namespace Blazor {

    class GamepadManagerType {

        private managerDotNetRef: any;

        constructor() { }

        public register(managerDotNetRef: any) {
            this.managerDotNetRef = managerDotNetRef;

            this.init();
        }

        public updateGamepads() {
            const infos: any[] = [];
            const gamepads = window.navigator.getGamepads();
            for (let i = 0; i < gamepads.length; i++) {
                const gamepad = gamepads[i];
                if (gamepad) {
                    infos.push(this.buildGamepadDTO(gamepad));
                }
            }
            return infos;
        }

        private findGamepad(index: number): Gamepad | null {
            for (const gamepad of window.navigator.getGamepads()) {
                if (gamepad && gamepad.index === index) {
                    return gamepad;
                }
            }
            return null;
        }

        public playVibrationEffect(index: number, effectName: string, options: any) {
            const gamepad = this.findGamepad(index);
            if (gamepad && gamepad.vibrationActuator) {
                gamepad.vibrationActuator.playEffect(effectName, options);
            }
        }
        
        private init() {
            window.addEventListener("gamepadconnected", (ev) => {
                const gamepad = (ev as GamepadEvent).gamepad;
                this.managerDotNetRef.invokeMethodAsync("OnGamepadConnectedHandler", { gamepad: this.buildGamepadDTO(gamepad) });
            });

            window.addEventListener("gamepaddisconnected", (ev) => {
                const gamepad = (ev as GamepadEvent).gamepad;
                this.managerDotNetRef.invokeMethodAsync("OnGamepadDisconnectedHandler", { gamepad: this.buildGamepadDTO(gamepad) });
            });

            this.managerDotNetRef.invokeMethodAsync("SetReady");
        }

        private buildGamepadDTO(gamepad: Gamepad): any {
            return {
                id: gamepad.id,
                index: gamepad.index,
                mapping: gamepad.mapping,
                axes: gamepad.axes.map(n => n),
                buttons: gamepad.buttons.map(b => { return { pressed: b.pressed, value: b.value } }),
                connected: gamepad.connected,
                timestamp: gamepad.timestamp,
                supportVibration: gamepad.vibrationActuator != null
            }
        }

    }


    export const GamepadManager = new GamepadManagerType();
}

