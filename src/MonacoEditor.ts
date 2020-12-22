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

// Monaco editor methods expoed to F#.
export interface IMonacoEditor {
    dispose(): void;
}

export function create(elem: HTMLElement): IMonacoEditor {
    let editor = monaco.editor.create(elem, {
        theme: "vs-dark",
        scrollbar: {
            verticalScrollbarSize: 30,
            horizontalScrollbarSize: 30
        },
        minimap: {
            enabled: false
        },
        value: ['function x() {', '\tconsole.log("Hello world!");', '}'].join('\n'),
        language: 'javascript'
    });
    return editor;
}


let elem = document.getElementById('container');