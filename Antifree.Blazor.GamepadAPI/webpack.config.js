
const path = require("path");

module.exports = {
    entry: ["./ts/gamepad_manager.ts"],
    stats: { warnings: false },
    devtool: "source-map",
    output: {
        library: 'Antifree',
        path: path.resolve(__dirname, 'wwwroot/'),
        filename: "gamepad_manager.min.js"
    },
    resolve: {
        extensions: [".ts"]
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: "ts-loader"
            }
        ]
    },
    watchOptions: {
        aggregateTimeout: 2000
    }
};