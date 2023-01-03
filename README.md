# xedit.app
The open source text editor in the browser. The short-term goals are to edit, search and diff files of any size.

See live on https://xedit.app  


## Used technologies are

- [F#](https://fsharp.org/)
- [Feliz](https://github.com/Zaid-Ajaj/Feliz) which is based on [Fable](https://fable.io/) and [Fable.React](https://github.com/fable-compiler/fable-react)
- [Feliz.MaterialUI](https://github.com/cmeeren/Feliz.MaterialUI) which are Feliz-style Fable bindings for [Material-UI](https://material-ui.com/)
- [Elmish](https://elmish.github.io/elmish/) for state management. It is similar to redux and supports time travel debugging.
- [Monaco Editor](https://microsoft.github.io/monaco-editor/) the code editor that powers VS Code.

## Getting started

Install the latest version of Node.js (the LTS version is not enough) and .NET Core SDK. Also git is used to clone this repository.

```
git clone https://github.com/bitjump-io/xedit.app.git
cd xedit.app/
npm install
npm start
```

Open your browser at http://localhost:8080/

For me the debugging works best in google chrome.

The build script assumes you have the redux devtools extension installed in your browser. If not you will likely get errors in your browser's console.

Open the xedit.app folder in visual studio code and modify the `App.fs` file in the `src` directory. Thanks to hot reloading the changes should be directly visible in the browser.

## Why F#

- F# is super concise. Where you need 100 classes spread over 100 files in most languages you just need 100 lines of F# in one file.  
- Every value is immutable by default. This prevents many errors.  
- There is no null and thus no null reference exception in idiomatic F# code.  
- Everything is an expression in F# even `if`.  
- There are many higher-level functions and list comprehensions available.  
- Method overloading, operator overloading and generics are supported.

## Notes about Compilation and Webpack

Changing a FSharp file will trigger rebuilding the .js file in the `/build` directory.

TypeScript files are also compiled to .js but webpack-dev-server will not write the output to disk.

Further webpack bundles all files. Watch the output of `npm start` to see the files that you can request in the browser, for example `http://localhost:8080/app.js`. Note again that the file `app.js` is not saved to disk in development mode. You can control in `webpack.config.js` which files are bundled together into `.js` files under the key `cacheGroups`.
