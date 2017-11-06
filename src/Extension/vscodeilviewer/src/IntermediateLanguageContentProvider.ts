import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';
import * as request from 'request';
import * as findParentDir from 'find-parent-dir';
import * as findUpGlob from 'find-up-glob';

let parsePath = () => {        
    let document = vscode.window.activeTextEditor.document;
    let parsedPath = path.parse(document.fileName);
    return parsedPath;
}

export class IntermediateLanguageContentProvider implements vscode.TextDocumentContentProvider {

        public static Scheme = 'il-viewer';

        private _response;
        private _previewUri;
        private _onDidChange = new vscode.EventEmitter<vscode.Uri>();

        constructor(previewUri : vscode.Uri,) {
            this._previewUri = previewUri;
        }

        // Implementation
        public provideTextDocumentContent(uri: vscode.Uri) : string {

            if (!this._response){
                this.locateCsProjFile((csproj, filename) => {
                    return this.requestIl(csproj, filename);
                });

                return '<p>Inspecting IL, please wait a moment...<p>';
            }

            let output = this.renderPage(this._response);
            this._response = null;

            return output;
        }

        get onDidChange(): vscode.Event<vscode.Uri> {
            return this._onDidChange.event;
        }

        private locateCsProjFile(sendToServerFunc){
            const parsedPath = parsePath();
            var projectFile = findUpGlob.sync('*.csproj', { cwd: parsedPath.dir });
            let filename = parsedPath.name;
            sendToServerFunc(filename, projectFile[0]);
            return;
        }

        private renderPage(body: IInspectionResult) : string {
            let output = "";
            if (body.hasErrors && body.compilationErrors.length > 0) {
                output += "<p>Unable to extract IL for the following reason(s):</p>";
                output += "<ol>";
                body.compilationErrors.forEach(function(value : ICompilationError, index: number){
                    output += "<li style=\"margin-bottom: 10px;\">" + value.message + "</li>";
                });
                output += "</ol>";
            } else if (body.ilResults.length > 0) {
                body.ilResults.forEach(function(value: IInstructionResult, index: number){
                    output += "<div style=\"font-size: 14px\"><pre>" + value.value + "</pre></div>";
                });
            }

            return `
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

        public requestIl(filename: string, csProjFile : string) {

            let postData = {
                ProjectFilePath : csProjFile,
                Filename : filename + ".cs"
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
                    this._onDidChange.fire(this._previewUri);
                } else if (!error && response.statusCode == 500) {
                    // Something went wrong!
                    this._response = `
                    <p>Uh oh, something went wrong.</p>
                    <p>${body}</p>`;
                    this._onDidChange.fire(this._previewUri);
                }
            });
        }
    }