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

```
git clone https://github.com/bitjump-io/xedit.app.git
cd xedit.app/
npm install
npm start
```

Open your browser at http://localhost:8080/

For me the debugging works best in google chrome.

The build script assumes you have the redux devtools extension installed in your browser. If not you will likely get errors in the browser console.

Open the xedit.app folder in visual studio code and modify the `App.fs` file in the `src` directory. Thanks to hot reloading the changes should be directly visible in the browser.
