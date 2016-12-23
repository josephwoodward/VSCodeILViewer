'use strict';

import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

export function activate(context: vscode.ExtensionContext) {

    class CustomTextDocumentContentProvider implements vscode.TextDocumentContentProvider {

        public static Scheme :string = "il-viewer";

        private _onDidChange = new vscode.EventEmitter<vscode.Uri>();

        public provideTextDocumentContent(uri: vscode.Uri): string {
            return "Hello world";
        }

    }

    let previewUri = vscode.Uri.parse(CustomTextDocumentContentProvider.Scheme + '://authority/css-preview');

    let provider = new CustomTextDocumentContentProvider();
    let registration = vscode.workspace.registerTextDocumentContentProvider(CustomTextDocumentContentProvider.Scheme, provider);

    let disposable = vscode.commands.registerCommand('editor.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', previewUri, vscode.ViewColumn.Two, 'CSS Property Preview').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });

    context.subscriptions.push(registration, disposable);
}

export function deactivate() {}