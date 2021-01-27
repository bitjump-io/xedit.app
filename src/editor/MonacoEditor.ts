/// This module exposes functionality of the monaco editor that is used by this app.
// Only the used methods and types are exported because
// - ts2fable will not generate error-free F# declarations for the complete MonacoEditor,
//   so first manual work would be required every time the declarations are updated and
//   second the generated code could not be used without the risk that some properties or methods are missing.
// - This way the used parts of the monaco editor are explicit and can be better checked for errors.
// - It is possible to add checks or change the interface.

import * as monaco from 'monaco-editor';

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
  currentTextModelIndex: number;
}

// The class wraps the editor instance and may be used for contains custom state.
// There shall be only one editor for the website which is used by all tabs.
// Include only instance methods that are needed to verify that the state is consistent.
class MonacoEditor implements IMonacoEditor {
  editor: monaco.editor.IStandaloneCodeEditor;
  textModels: monaco.editor.ITextModel[];
  currentTextModelIndex: number;

  constructor(elem: HTMLElement, dimension: Dimension) {
    this.textModels = [];
    this._createInitialModel();

    this.editor = monaco.editor.create(elem, {
      theme: "vs-dark",
      scrollbar: {
        verticalScrollbarSize: 25,
        horizontalScrollbarSize: 25
      },
      minimap: {
        enabled: false
      },
      //renderWhitespace?: 'none' | 'boundary' | 'selection' | 'trailing' | 'all';
      //renderControlCharacters?: boolean;
      // allow move selections via drag and drop.
      dragAndDrop: true,
      dimension,
      padding: {
        top: 5,
        bottom: 5
      },
      model: this.textModels[0]
    });
  }

  _createInitialModel() {
    const initialTextModel = monaco.editor.createModel("", null, monaco.Uri.file('/initial'));
    this.textModels[0] = initialTextModel;
    this.currentTextModelIndex = 0;
  }

  addTextModel(value: string, language: string): number {
    if (value == null) {
      throw new Error("Invalid argument: value must not be null.");
    }
    const newModel = monaco.editor.createModel(value, language, null);
    this.textModels.push(newModel);
    return this.textModels.length - 1; // return modelIndex
  }

  removeTextModel(modelIndex: number): void {
    if (modelIndex == null || modelIndex < 0 || modelIndex >= this.textModels.length) {
      throw new Error(`Invalid argument to removeTextModel: modelIndex (${modelIndex}) must be >= 0 and < the number of textModels (${this.textModels.length}).`);
    }
    const toRemoveTextModel = this.textModels[modelIndex];
    this.textModels.splice(modelIndex, 1);
    
    if (this.textModels.length > 0 && modelIndex === this.currentTextModelIndex) {
      const newCurrentTextModelIndex = modelIndex > 0 ? modelIndex - 1 : 0;
      this.setTextModelIndex(newCurrentTextModelIndex);
    }
    else if (this.textModels.length == 0) {
      this._createInitialModel();
    }
    toRemoveTextModel.dispose();
  }

  setTextModelIndex(modelIndex: number): void {
    if (modelIndex == null || modelIndex < 0 || modelIndex >= this.textModels.length) {
      throw new Error(`Invalid argument to setTextModelIndex: modelIndex (${modelIndex}) must be >= 0 and < the number of textModels (${this.textModels.length}).`);
    }
    this.currentTextModelIndex = modelIndex;
    const newCurrentTextModel = this.textModels[modelIndex];
    this.editor.setModel(newCurrentTextModel);
  }
}

export function create(elem: HTMLElement, dimension: Dimension): IMonacoEditor {
  return new MonacoEditor(elem, dimension);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function addTextModel(value: string, language: string, editor: IMonacoEditor): number {
  return (editor as MonacoEditor).addTextModel(value, language);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function removeTextModel(modelIndex: number, editor: IMonacoEditor): void {
  return (editor as MonacoEditor).removeTextModel(modelIndex);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function setTextModelIndex(modelIndex: number, editor: IMonacoEditor): void {
  (editor as MonacoEditor).setTextModelIndex(modelIndex);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function layout(dimension: Dimension, editor: IMonacoEditor): void  {
  (editor as MonacoEditor).editor.layout(dimension);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function setWordWrap(value: boolean, editor: IMonacoEditor): void {
  // 'off' | 'on' | 'wordWrapColumn' | 'bounded'
  (editor as MonacoEditor).editor.updateOptions({ wordWrap: (value === true ? 'on' : 'off') });
}

function changeFontSize(editor: IMonacoEditor, change: number): void {
  let monacoEditor = (editor as MonacoEditor).editor;
  let fontSize = monacoEditor.getOption(monaco.editor.EditorOption.fontSize);
  monacoEditor.updateOptions({ fontSize: fontSize + change });
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function increaseFontSize(editor: IMonacoEditor): void {
  changeFontSize(editor, 1);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function decreaseFontSize(editor: IMonacoEditor): void {
  changeFontSize(editor, -1);
}

// The F# signature is a curried function. It gets compiled to a function call with all arguments passed at once.
export function setLanguage(languageId: string, editor: IMonacoEditor): void {
  const monacoEditor = (editor as MonacoEditor);
  languageId = languageId === "plaintext" ? null : languageId;
  let model = monacoEditor.textModels[monacoEditor.currentTextModelIndex];
  monaco.editor.setModelLanguage(model, languageId);
}
