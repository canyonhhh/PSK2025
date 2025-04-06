const HTMLWebpackPlugin = require("html-webpack-plugin");
const path = require("path");

module.exports = (env) => {
  const apiTarget =
    process.env.services__api__https__0 ||
    process.env.services__api__http__0 ||
    "https://localhost:7548";

  return {
    entry: "./src/index.tsx", // ✅ Entry now uses TypeScript
    devServer: {
      port: env.PORT || 5468,
      allowedHosts: "all",
      historyApiFallback: true,
      proxy: [
        {
          context: ["/api"],
          target: apiTarget,
          pathRewrite: { "^/api": "" },
          secure: false,
          changeOrigin: true,
        },
      ],
    },
    output: {
      path: path.resolve(__dirname, "dist"),
      filename: "bundle.js",
    },
    resolve: {
      extensions: [".tsx", ".ts", ".js"], // ✅ Add TS/TSX support
    },
    plugins: [
      new HTMLWebpackPlugin({
        template: "./src/index.html",
        favicon: "./src/favicon.ico",
      }),
    ],
    module: {
      rules: [
        {
          test: /\.(ts|tsx)$/, // ✅ TS loader
          exclude: /node_modules/,
          use: "ts-loader",
        },
        {
          test: /\.js$/,
          exclude: /node_modules/,
          use: {
            loader: "babel-loader",
            options: {
              presets: [
                "@babel/preset-env",
                ["@babel/preset-react", { runtime: "automatic" }],
              ],
            },
          },
        },
        {
          test: /\.css$/,
          exclude: /node_modules/,
          use: ["style-loader", "css-loader"],
        },
      ],
    },
  };
};
