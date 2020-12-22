add monaco editor
`npm install monaco-editor`

install tool to generate f# fable interfaces from typescript
`yarn global add ts2fable@next`

generate interfaces
`ts2fable node_modules/monaco-editor/monaco.d.ts src/MonacoEditor.fs`

add MonacoEditor.fs to App.fsproj