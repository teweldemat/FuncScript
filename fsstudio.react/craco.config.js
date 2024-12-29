// craco.config.js
const path = require("path");

module.exports = {
  webpack: {
    configure: (config) => {
      config.module.rules.push({
        test: /\.(ts|tsx)$/,
        include: [path.resolve(__dirname, "../ts/core/src")],
        use: {
          loader: require.resolve("babel-loader"),
          options: {
            presets: [
              "@babel/preset-env",
              "@babel/preset-react",
              "@babel/preset-typescript"
            ]
          }
        }
      });
      return config;
    }
  }
};