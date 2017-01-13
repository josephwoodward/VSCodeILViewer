'use strict';

import { IntermediateLanguageContentProvider } from './IntermediateLanguageContentProvider';
import * as path from 'path';
import * as vscode from 'vscode';
import * as fs from 'fs';
import * as os from 'os';

var request = require('request');

let ilWindowUri = vscode.Uri.parse('il-viewer://authority/' + IntermediateLanguageContentProvider.Scheme);

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

export function activate(context: vscode.ExtensionContext) {

    // let res = vscode.workspace.textDocuments;

    let invokationDisposable = vscode.commands.registerCommand('editor.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', ilWindowUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });
    registerDisposable(invokationDisposable);    

    let provider = new IntermediateLanguageContentProvider(ilWindowUri);
    let providerDisposable = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
    registerDisposable(providerDisposable);

    // var res = path.join('server', 'out', 'serverMain.js');
	// let serverModule = context.asAbsolutePath(res);
	// // The debug options for the server
	// let debugOptions = { execArgv: ["--nolazy", "--debug=6004"] };

    function registerDisposable(d : vscode.Disposable){
        context.subscriptions.push(d);
    }
}

export function deactivate() {}