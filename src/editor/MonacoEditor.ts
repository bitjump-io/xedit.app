/// This module exposes functionality of the monaco editor that is used by this app.
// Only the used methods and types are exported because
// - ts2fable will not generate error-free F# declarations for the complete MonacoEditor,
//   so first manual work would be required every time the declarations are updated and
//   second the generated code could not be used without the risk that some properties or methods are missing.
// - This way the used parts of the monaco editor are explicit and can be better checked for errors.
// - It is possible to add checks or change the interface.

import { editor, Uri } from 'monaco-editor';

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
  codeEditor: editor.IStandaloneCodeEditor;
  textModels: editor.ITextModel[];
  currentTextModelIndex: number;

  constructor(elem: HTMLElement, dimension: Dimension) {
    this.textModels = [];
    this._createInitialModel();

    this.codeEditor = editor.create(elem, {
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
      lineNumbersMinChars: 1,
      lineDecorationsWidth: 0,
      suggest: {
        shareSuggestSelections: false,
        showWords: false
      },
      glyphMargin: true,
      //glyphMarginWidth: 5,
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
    const initialTextModel = editor.createModel("", null, Uri.file('/initial'));
    this.textModels[0] = initialTextModel;
    this.currentTextModelIndex = 0;
  }

  addTextModel(value: string, language: string): number {
    if (value == null) {
      throw new Error("Invalid argument: value must not be null.");
    }
    // See "class TextModel" in https://github.com/microsoft/vscode/blob/master/src/vs/editor/common/model/textModel.ts
    const newModel = editor.createModel(value, language, null);
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
    this.codeEditor.setModel(newCurrentTextModel);
  }

  changeFontSize(change: number): void {
    let fontSize = this.codeEditor.getOption(editor.EditorOption.fontSize);
    this.codeEditor.updateOptions({ fontSize: fontSize + change });
  }

  increaseFontSize(): void {
    this.changeFontSize(1);
  }

  decreaseFontSize(): void {
    this.changeFontSize(-1);
  }

  focus(): void {
    this.codeEditor.focus();
  }

  setWordWrap(value: boolean): void {
    // 'off' | 'on' | 'wordWrapColumn' | 'bounded'
    this.codeEditor.updateOptions({ wordWrap: (value === true ? 'on' : 'off') });
  }

  layout(dimension: Dimension): void  {
    this.codeEditor.layout(dimension);
  }

  setLanguage(languageId: string): void {
    languageId = languageId === "plaintext" ? null : languageId;
    let model = this.textModels[this.currentTextModelIndex];
    editor.setModelLanguage(model, languageId);
  }

  getValue(modelIndex: number): string {
    return this.textModels[modelIndex].getValue(editor.EndOfLinePreference.TextDefined, true);
  }

  getLinesContent(modelIndex: number): string[] {
    return this.textModels[modelIndex].getLinesContent();
  }
  
  onDidChangeContent(modelIndex: number, listener: (e: editor.IModelContentChangedEvent) => void): void {
    this.textModels[modelIndex].onDidChangeContent(listener)
  }
}

export function create(elem: HTMLElement, dimension: Dimension): IMonacoEditor {
  return new MonacoEditor(elem, dimension);
}



        // /**
        //  * Enable rendering of whitespace.
        //  * Defaults to none.
        //  */
        // renderWhitespace?: 'none' | 'boundary' | 'selection' | 'trailing' | 'all';
        // /**
        //  * Enable rendering of control characters.
        //  * Defaults to false.
        //  */
        // renderControlCharacters?: boolean;

        // // /**
        // //  * Syntax highlighting is copied.
        // //  */
        // // copyWithSyntaxHighlighting?: boolean;

        //         /**
        //  * Enable code folding.
        //  * Defaults to true.
        //  */
        // folding?: boolean;

        //         /**
        //  * Enable rendering of indent guides.
        //  * Defaults to true.
        //  */
        // renderIndentGuides?: boolean;

// ICodeEditor has getModel(): ITextModel
// ITextModel has getValue, getValueLength, getLineCount, getEOL methods
// ITextModel has internal method getTextBuffer(): ITextBuffer;
// TextModel has _tokens: TokensStore and _tokens2: TokensStore2
// ITextBuffer is PieceTreeTextBuffer
// PieceTreeTextBuffer has _pieceTree: PieceTreeBase
// PieceTreeBase has _buffers!: StringBuffer[]
// https://github.com/microsoft/vscode/blob/main/src/vs/editor/common/model/pieceTreeTextBuffer/pieceTreeTextBuffer.ts
// https://github.com/microsoft/vscode/blob/main/src/vs/editor/common/model/tokensStore.ts