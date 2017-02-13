import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';

const request = require('request');
const findParentDir = require('find-parent-dir');
const findUpGlob = require('find-up-glob');


function parsePath() {
    let document = vscode.window.activeTextEditor.document;
    let parsedPath = path.parse(document.fileName);
    return parsedPath;
}

function getProjectRootDirOfFilePath(filepath) {
    var projectrootdir = findParentDir.sync(path.dirname(filepath), 'project.json');
    if (projectrootdir == null) {
        var csprojfiles = findUpGlob.sync('*.csproj', { cwd: path.dirname(filepath) });
        if (csprojfiles == null) {
            return null;
        }

        //projectrootdir = path.dirname(csprojfiles[0]);
        projectrootdir = csprojfiles[0];
    }

    return projectrootdir;
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
                this.findProjectJson((projectJson, filename) => {
                    return this.requstIl(projectJson, filename);
                })
                
                return `
                <p>Inspecting IL, hold onto your seat belts!<p>
                <p>If this is your first inspection then it may take a moment longer.</p>`;
            }

            let output = this.renderPage(this._response);
            this._response = null;

            return output;
        }

        get onDidChange(): vscode.Event<vscode.Uri> {
            return this._onDidChange.event;
        }

        private findProjectJson(requestIntermediateLanguage){
            const parsedPath = parsePath();
            const filename = parsedPath.name;

            let document = vscode.window.activeTextEditor.document;
            //let parsedPath = path.parse(document.fileName);
            var projectFilePath = getProjectRootDirOfFilePath(document.fileName);

            if (projectFilePath !== null) {
                requestIntermediateLanguage(parsedPath.name, projectFilePath);
                return;
            }
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

        public requstIl(filename: string, projectJsonPath : string) {

            let postData = {
                ProjectFilePath : projectJsonPath,
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