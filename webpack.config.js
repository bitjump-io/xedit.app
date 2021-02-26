const path = require("path");
const fsp = require('fs').promises;
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

// patch monaco editor glyphMarginWidth
async function replaceRegexInFile() {
  const fileToReplaceTextIn = './node_modules/monaco-editor/esm/vs/editor/common/config/editorOptions.js';
  const data = await fsp.readFile(fileToReplaceTextIn, 'utf8');
  const result = data.replace(/glyphMarginWidth = lineHeight;/g, 'glyphMarginWidth = 5;');
  await fsp.writeFile(fileToReplaceTextIn, result, 'utf8');
}

async function getMonacoEditorVersion() {
  const fileToRead = './node_modules/monaco-editor/package.json';
  const data = await fsp.readFile(fileToRead, 'utf8');
  const matchResult = data.match(/"version": "(\d+\.\d+\.\d+)",/);
  if (matchResult === null) {
    throw `Could not find version in ${fileToRead}`;
  }
  return matchResult[1];
}

const CONFIG = {
  // The tags to include the generated JS and CSS will be automatically injected in the HTML template
  // See https://github.com/jantimon/html-webpack-plugin
  indexHtmlTemplate: "./src/index.html",
  fsharpEntry: "./build/Main.fs.js",
  tsEntry: "./src/Main.ts",
  outputDir: "./deploy",
  assetsDir: "./public",
  devServerPort: 8080,
  // When using webpack-dev-server, you may need to redirect some calls
  // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
  devServerProxy: undefined
}

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
const commonPlugins = [
  new HtmlWebpackPlugin({
    filename: 'index.html',
    template: resolve(CONFIG.indexHtmlTemplate),
    publicPath: '',
    inject: "head",
    excludeChunks: ['monaco-editor'],
    templateParameters: (compilation, assets, assetTags, options) => {
      for (let tag of assetTags.headTags) {
        if (tag.tagName === "script") {
          tag.attributes["type"] = "module";
        }
      }
      return {
        compilation,
        webpackConfig: compilation.options,
        htmlWebpackPlugin: {
          tags: assetTags,
          files: assets,
          options
        }
      };
    },
  }),
];

module.exports = async () => {
  await replaceRegexInFile();
  const monacoEditorVersion = "v" + await getMonacoEditorVersion();

  // If we're running the webpack-dev-server, assume we're in development mode
  const isProduction = process.argv.join(' ').indexOf('--env dev') === -1;
  console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

  return {
    entry: {
        app: [resolve(CONFIG.fsharpEntry), resolve(CONFIG.tsEntry)],
        "monaco-editor": [resolve("./src/editor/MonacoEditor.ts")]
    },
    experiments: {
      topLevelAwait: true,
      outputModule: true,
      layers: true
    },
    // Add a contenthash to the output file name in production
    // to prevent browser caching if code changes.
    output: {
      globalObject: 'self',
      publicPath: '',
      path: resolve(CONFIG.outputDir),
      clean: true,
      module: true,
      scriptType: 'module',
      chunkFilename: (pathData) => {
        return pathData.chunk.name != null && pathData.chunk.name.indexOf('monaco-editor') != -1 ? '[name].js' : 'chunk-[id].[contenthash].js';
      },
      filename: isProduction ? '[name].[contenthash].js' : '[name].js',
      environment: {
        // The environment supports an async import() function to import EcmaScript modules.
        dynamicImport: true,
        // The environment supports ECMAScript Module syntax to import ECMAScript modules (import ... from '...').
        module: true,
      },
    },
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? "source-map" : "cheap-source-map", //"eval-source-map",
    optimization: {
      runtimeChunk: {
        name: 'webpack-runtime' // separate file since multiple entry points.
      },
      // Named chunks in production build.
      chunkIds: "named",
      //innerGraph: true,
      //minimize: true,
      // providedExports: true,
      // usedExports: true,
      removeAvailableModules: true,
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
              if (["react", "react-dom"].includes(packageName)) {
                return `react-and-react-dom`;
              }
              if (["monaco-editor"].includes(packageName)) {
                return `${packageName}-${monacoEditorVersion}`;
              }
              if (["material-ui"].includes(packageName)) {
                return 'material-ui';
              }
              // All other node_modules in one file.
              return "vendor";
            },
          },
          // Create one js file for index.ts since it is injected in index.html.
          // Create one js file for each ts file in src/helper. This is done to easily test the ts file in the test.html file.
          other: {
            test: /([\\/]src[\\/]helper[\\/])|([\\/]src[\\/]index.ts)/,
            name(module) {
              let fileName = module.resource.match(/.*[\\/](.+\.ts)/)[1];
              return fileName;
            }
          }
        },
      },
    },
    plugins: isProduction ?
      commonPlugins.concat([
        new MiniCssExtractPlugin({ 
          chunkFilename: (pathData) => {
            return pathData.chunk.name != null && pathData.chunk.name.indexOf('monaco-editor') != -1 ? '[name].css' : 'chunk-[id].[contenthash].css';
          },
          filename: isProduction ? '[name].[contenthash].css' : '[name].css'
        }),
        new CopyWebpackPlugin(
        { 
          patterns: [
            { from: resolve(CONFIG.assetsDir) }
          ]
        }),
        new MonacoWebpackPlugin({
          filename: `monaco-editor-[name].worker-${monacoEditorVersion}.js`
        })
      ])
      : commonPlugins.concat([
        new webpack.HotModuleReplacementPlugin(),
        new MonacoWebpackPlugin({
          filename: `[name].worker-${monacoEditorVersion}.js`
        })
      ]),
    resolve: {
      // See https://github.com/fable-compiler/Fable/issues/1490
      symlinks: false,
      modules: [resolve("./node_modules")],
      alias: {
        //'monaco-editor': './monaco-editor/min/vs/editor/editor.main.js',
      },
      extensions: ['.js', '.ts']
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
          // see tsconfig.json
          test: /\.tsx?$/,
          use: 'ts-loader'
        },
        {
          test: /\.(sass|scss|css)$/,
          use: [
            isProduction
              ? { 
                  loader: MiniCssExtractPlugin.loader, 
                  options: { 
                    publicPath: '',
                  }
                }
              : 'style-loader',
            'css-loader',
            {
              loader: 'sass-loader',
              options: { 
                implementation: require("sass"),
              }
            }
          ],
        },
        {
          test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
          use: [
            {
              loader: "file-loader",
              options: {
                name(resourcePath) {
                  if (resourcePath.indexOf('node_modules/monaco-editor') !== -1 && resourcePath.endsWith('ttf')) {
                    return `monaco-editor-${monacoEditorVersion}-[name].ttf`;
                  }
                  return '[contenthash].[ext]';
                }
              }
            }
          ]
        }
      ]
    }
  }
};

function resolve(filePath) {
  return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
