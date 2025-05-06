const HTMLWebpackPlugin = require("html-webpack-plugin");
const ESLintPlugin = require("eslint-webpack-plugin");
const path = require("path");

module.exports = (env) => {
  const apiTarget =
    process.env.services__api__https__0 ||
    process.env.services__api__http__0 ||
    "https://localhost:7548";

  return {
    entry: "./src/index.tsx", // ✅ Entry now uses TypeScript
    devServer: {
      port: Number(env.PORT) || 5468,
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
      new ESLintPlugin({
        extensions: ["js", "jsx", "ts", "tsx"], // Lint JS, JSX, TS, and TSX files
        emitWarning: true, // Emit warnings for ESLint errors
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
          test: /\.css$/,
          exclude: /node_modules/,
          use: ["style-loader", "css-loader"],
        },
        {
          test: /\.(png|jpe?g|gif|svg)$/i,
          type: 'asset/resource',
        },
      ],
    },
  };
};
