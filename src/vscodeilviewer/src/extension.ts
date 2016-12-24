'use strict';

import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';

var request = require('request');

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

export function activate(context: vscode.ExtensionContext) {

    let previewUri = vscode.Uri.parse('il-viewer://authority/il-output');

    class CustomTextDocumentContentProvider implements vscode.TextDocumentContentProvider {

        public static Scheme = 'il-viewer';
        private _onDidChange = new vscode.EventEmitter<vscode.Uri>();
        private response = "";

        public provideTextDocumentContent(uri: vscode.Uri): string {
            if (this.response === ""){
                this.requstIl();
            }

            return this.response === "" ? "Generating IL, please wait..." : this.response;
        }

        get onDidChange(): vscode.Event<vscode.Uri> {
            return this._onDidChange.event;
        }

        public update(uri: vscode.Uri) {
            this._onDidChange.fire(uri);
        }

        public requstIl() {
            request('http://google.com', (error, response, body) => {
                if (!error && response.statusCode == 200) {
                    this.response = body;
                    this.update(previewUri);
                }
            });
        }

    }

    let provider = new CustomTextDocumentContentProvider();
    let registration = vscode.workspace.registerTextDocumentContentProvider(CustomTextDocumentContentProvider.Scheme, provider);

    let disposable = vscode.commands.registerCommand('editor.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', previewUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });

    // var res = path.join('server', 'out', 'serverMain.js');
	// let serverModule = context.asAbsolutePath(res);
	// // The debug options for the server
	// let debugOptions = { execArgv: ["--nolazy", "--debug=6004"] };


    context.subscriptions.push(registration, disposable);
}

export function deactivate() {}