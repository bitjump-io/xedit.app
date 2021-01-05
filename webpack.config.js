// Template for webpack.config.js in Fable projects
// Find latest version in https://github.com/fable-compiler/webpack-config-template

// In most cases, you'll only need to edit the CONFIG object (after dependencies)
// See below if you need better fine-tuning of Webpack options

// Dependencies. Also required: core-js, fable-loader, fable-compiler, @babel/core,
// @babel/preset-env, babel-loader, sass, sass-loader, css-loader, style-loader, file-loader
const path = require("path");
const fs = require('fs');
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

const CONFIG = {
  // The tags to include the generated JS and CSS will be automatically injected in the HTML template
  // See https://github.com/jantimon/html-webpack-plugin
  indexHtmlTemplate: "./src/index.html",
  fsharpEntry: "./src/Main.fs.js",
  tsEntry: "./src/main.ts",
  cssEntry: "./styles/main.scss",
  outputDir: "./deploy",
  assetsDir: "./public",
  devServerPort: 8080,
  // When using webpack-dev-server, you may need to redirect some calls
  // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
  devServerProxy: undefined,
  // Use babel-preset-env to generate JS compatible with most-used browsers.
  // More info at https://babeljs.io/docs/en/next/babel-preset-env.html
  babel: {
    presets: [
      // ["@babel/preset-env", {
      //   "targets": "> 0.25%, not dead",
      //   // This adds polyfills when needed. Requires core-js dependency.
      //   // See https://babeljs.io/docs/en/babel-preset-env#usebuiltins
      //   "useBuiltIns": "usage",
      //   "corejs": 3
      // }],
      ["@babel/preset-react"],
      ["@babel/preset-typescript"]
    ],
  }
}

// If we're running the webpack-dev-server, assume we're in development mode
const isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

if (isProduction) {
  fs.rmdirSync(resolve(CONFIG.outputDir), { recursive: true });
}

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
const commonPlugins = [
  new HtmlWebpackPlugin({
    filename: 'index.html',
    template: resolve(CONFIG.indexHtmlTemplate)
  })
];

module.exports = {
  // In development, bundle styles together with the code so they can also
  // trigger hot reloads. In production, put them in a separate CSS file.
  entry: isProduction ? {
      app: [resolve(CONFIG.fsharpEntry), resolve(CONFIG.tsEntry), resolve(CONFIG.cssEntry)],
      //...monacoEditorFiles
    } : {
      app: [resolve(CONFIG.fsharpEntry), resolve(CONFIG.tsEntry)],
      style: [resolve(CONFIG.cssEntry)],
      //...monacoEditorFiles
    },
  // Add a hash to the output file name in production
  // to prevent browser caching if code changes
  output: {
    globalObject: 'self',
    path: resolve(CONFIG.outputDir),
    filename: isProduction ? '[name].[hash].js' : '[name].js'
  },
  mode: isProduction ? "production" : "development",
  devtool: isProduction ? "source-map" : "cheap-source-map", //"eval-source-map",
  optimization: {
    runtimeChunk: 'single',
    splitChunks: {
      chunks: 'all',
      maxInitialRequests: Infinity,
      minSize: 0,
      cacheGroups: {
        vendor: {
          test: /[\\/]node_modules[\\/]/,
          name(module) {
            let packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1];
            packageName = packageName.replace('@', '');

            // Output large libraries in separate files.
            if (["monaco-editor", "material-ui", "react-do"].includes(packageName)) {
              return `vendor.${packageName}`;
            }
            // All other node_modules in one file.
            return "vendor";
          },
        },
      },
    },
  },

  // Besides the HtmlPlugin, we use the following plugins:
  // PRODUCTION
  //    - MiniCssExtractPlugin: Extracts CSS from bundle to a different file
  //      To minify CSS, see https://github.com/webpack-contrib/mini-css-extract-plugin#minimizing-for-production
  //    - CopyWebpackPlugin: Copies static assets to output directory
  // DEVELOPMENT
  //    - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
  plugins: isProduction ?
    commonPlugins.concat([
      new MiniCssExtractPlugin({ filename: 'style.css' }),
      new CopyWebpackPlugin([{ from: resolve(CONFIG.assetsDir) }]),
      new MonacoWebpackPlugin({
        filename: isProduction ? '[name].worker.[hash].js' : '[name].js',
      })
    ])
    : commonPlugins.concat([
      new webpack.HotModuleReplacementPlugin(),
    ]),
  resolve: {
    // See https://github.com/fable-compiler/Fable/issues/1490
    symlinks: false,
    modules: [resolve("./node_modules")],
    alias: {
      // Some old libraries still use an old specific version of core-js
      // Redirect the imports of these libraries to the newer core-js
      'core-js/es6': 'core-js/es'
    },
    extensions: ['*', '.js', '.jsx', '.tsx', '.ts']
  },
  // Configuration for webpack-dev-server
  devServer: {
    publicPath: "/",
    contentBase: resolve(CONFIG.assetsDir),
    port: CONFIG.devServerPort,
    proxy: CONFIG.devServerProxy,
    hot: true,
    inline: true
  },
  // - fable-loader: transforms F# into JS
  // - babel-loader: transforms JS and typescript to old syntax (compatible with old browsers)
  // - sass-loaders: transforms SASS/SCSS into JS
  // - file-loader: Moves files referenced in the code (fonts, images) into output folder
  module: {
    rules: [
      {
        test: /\.(js|jsx|tsx|ts)$/,
        exclude: /node_modules/,
        use: [{
            loader: require.resolve('babel-loader'),
            options: CONFIG.babel
            }]
      },
      {
        test: /\.(sass|scss|css)$/,
        use: [
          isProduction
            ? MiniCssExtractPlugin.loader
            : 'style-loader',
          'css-loader',
          {
            loader: 'sass-loader',
            options: { implementation: require("sass") }
          }
        ],
      },
      {
        test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
        use: ["file-loader"]
      }
    ]
  }
};

function resolve(filePath) {
  return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
