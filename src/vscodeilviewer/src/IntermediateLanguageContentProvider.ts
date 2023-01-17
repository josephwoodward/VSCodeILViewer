import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';
import * as request from 'request';
import * as findParentDir from 'find-parent-dir';

let parsePath = () => {
    let document = vscode.window.activeTextEditor.document;
    let parsedPath = path.parse(document.fileName);
    return parsedPath;
}

export class IntermediateLanguageContentProvider {

        public static Scheme = 'il-viewer';

        private _response;
        private _panel;

        constructor(panel: vscode.WebviewPanel) {
            this._panel = panel;
        }

        // Implementation
        public provideTextDocumentContent() : void {

            this._panel.webview.html = `
            <p>Inspecting IL, hold onto your seat belts!<p>
            <p>If this is your first inspection then it may take a moment longer.</p>`;
            this.requstIl();
        }

        // get onDidChange(): vscode.Event<vscode.Uri> {
        //     return this._onDidChange.event;
        // }


        private renderIlPage() : void {
            let output = "";
            if (this._response.hasErrors && this._response.compilationErrors.length > 0) {
                output += "<p>Unable to extract IL for the following reason(s):</p>";
                output += "<ol>";
                this._response.compilationErrors.forEach(function(value : ICompilationError, index: number){
                    output += "<li style=\"margin-bottom: 10px;\">" + value.message + "</li>";
                });
                output += "</ol>";
            } else if (this._response.ilResults?.length > 0) {
                this._response.ilResults.forEach(function(value: IInstructionResult, index: number){
                    output += "<div style=\"font-size: 14px\"><pre>" + value.value + "</pre></div>";
                });
            }

            this._panel.webview.html = `
            <style type="text/css">
                .outOfDateBanner {
                    display: table;
                    background-color: red;
                }

                .outOfDateBanner span {
                    display: table-cell;
                }
            </style>
            <body>
            ${output}
            </body>`;
        }

        public requstIl() {
            const parsedPath = parsePath();
            const filename = parsedPath.name;
            const projectPath = parsedPath.dir;

            let postData = {
                ProjectFilePath : projectPath,
                Filename : filename
            }

            let options = {
                method: 'post',
                body: postData,
                json: true,
                url: 'http://localhost:65530/api/il/'
            }

            request(options, (error, response, body) => {
                if (!error && response.statusCode == 200) {
                    this._response = body;
                    this.renderIlPage();

                } else if (!error && response.statusCode == 500) {
                    this._panel.webview.html = `
                    <p>Uh oh, something went wrong.</p>
                    <p>${body}</p>`;
                }
            });
        }
    }
