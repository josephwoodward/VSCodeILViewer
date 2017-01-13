'use strict';

import { IntermediateLanguageContentProvider } from './IntermediateLanguageContentProvider';
import * as path from 'path';
import * as vscode from 'vscode';
import * as fs from 'fs';
import * as os from 'os';

var request = require('request');
var parentfinder = require('find-parent-dir');

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

let previewUri = vscode.Uri.parse('il-viewer://authority/il-output');

export function activate(context: vscode.ExtensionContext) {

    let disposable = vscode.commands.registerCommand('editor.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', previewUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });
    registerDisposable(disposable);
    

    let res = vscode.workspace.textDocuments;

    let document = vscode.window.activeTextEditor.document;
    let parsedPath = path.parse(document.fileName);
    let projectPath = "/Users/josephwoodward/Dev/VsCodeIlViewerDemoProj/console/project.json";
    
    let root = vscode.workspace.rootPath;
    let projectJson;
    
    let provider = new IntermediateLanguageContentProvider(previewUri, parsedPath);
    let disposable2 = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
    registerDisposable(disposable2);

    // var res = path.join('server', 'out', 'serverMain.js');
	// let serverModule = context.asAbsolutePath(res);
	// // The debug options for the server
	// let debugOptions = { execArgv: ["--nolazy", "--debug=6004"] };

    function registerDisposable(d : vscode.Disposable){
        // disposables.forEach((d) => {
        //     context.subscriptions.push(d);
        // })
        context.subscriptions.push(d);
    }
}

export function deactivate() {}