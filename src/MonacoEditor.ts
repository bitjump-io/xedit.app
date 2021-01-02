/// This module exposes functionality of the monaco editor that is used by this app.
// Only the used methods and types are exported because
// - ts2fable will not generate error-free F# declarations for the complete MonacoEditor, 
//   so first manual work would be required every time the declarations are updated and
//   second the generated code could not be used without the risk that some properties or methods are missing.
// - This way the used parts of the monaco editor are explicit and can be better checked for errors.
// - It is possible to add checks or change the interface.

import * as monaco from 'monaco-editor';

// @ts-ignore
self.MonacoEnvironment = {
  getWorkerUrl: function (moduleId, label) {
    if (label === 'json') {
      return './json.worker.js';
    }
    if (label === 'css') {
      return './css.worker.js';
    }
    if (label === 'html') {
      return './html.worker.js';
    }
    if (label === 'typescript' || label === 'javascript') {
      return './ts.worker.js';
    }
    return './editor.worker.js';
  }
};

export class Dimension {
  width: number;
  height: number;
}

// export interface IEditorOptions {
//   wordWrap?: 'off' | 'on' | 'wordWrapColumn' | 'bounded';
//   wordWrapColumn?: number;
//   renderControlCharacters?: boolean;
//   fontSize?: number;
//   tabSize?: number; // 4
//   insertSpaces?: boolean; // true
// }

// Monaco editor instance methods exposed to F#.
export interface IMonacoEditor {
}

class MonacoEditor implements IMonacoEditor {
  editor: monaco.editor.IStandaloneCodeEditor;

  constructor(elem: HTMLElement, dimension: Dimension) {
    this.editor = monaco.editor.create(elem, {
      theme: "vs-dark",
      scrollbar: {
        verticalScrollbarSize: 25,
        horizontalScrollbarSize: 25
      },
      minimap: {
        enabled: false
      },
      dimension,
      padding: {
        top: 5,
        bottom: 5
      },
      value: ['function x() {', '\tconsole.log("Hello world!");', '}'].join('\n'),
      language: "javascript"
    });
  }

  dispose(): void {
    this.editor.dispose();
  }

  layout(dimension: Dimension): void {
    this.editor.layout(dimension)
  }

  // 'off' | 'on' | 'wordWrapColumn' | 'bounded'
  setWordWrap(value: boolean): void {
    this.editor.updateOptions({ wordWrap: (value === true ? 'on' : 'off') });
  }

  setValue(newValue: string): void {
    let model = this.editor.getModel();
    if (model) {
      model.setValue(newValue);
    }
    else {
      console.error("model not set in MonacoEditor.setValue method.");
    }
  }

  setLanguage(languageId: string): void {
    let model = this.editor.getModel();
    if (model) {
      monaco.editor.setModelLanguage(model, languageId);
    }
    else {
      console.error("model not set in MonacoEditor.setLanguage method.");
    }
  }
}

export function create(elem: HTMLElement, dimension: Dimension): IMonacoEditor {
  return new MonacoEditor(elem, dimension);
}

export function dispose(editor: IMonacoEditor): void {
  (editor as MonacoEditor).dispose();
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function layout(dimension: Dimension, editor: IMonacoEditor): void  {
  (editor as MonacoEditor).layout(dimension);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function setWordWrap(value: boolean, editor: IMonacoEditor): void {
  (editor as MonacoEditor).setWordWrap(value);
}

export function setValue(newValue: string, editor: IMonacoEditor): void {
  (editor as MonacoEditor).setValue(newValue);
}

export function setLanguage(languageId: string, editor: IMonacoEditor): void {
  (editor as MonacoEditor).setLanguage(languageId);
}