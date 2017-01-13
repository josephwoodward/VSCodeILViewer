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



export function activate(context: vscode.ExtensionContext) {

    let res = vscode.workspace.textDocuments;
    let previewUri = vscode.Uri.parse('il-viewer://authority/il-output');

    // let edit = vscode.Location;
    // vscode.workspace.findFiles("**/project.json","").then((x) => {
    //     x.forEach((y) => {
    //         y.fsPath;
    //     });
    // });

    let document = vscode.window.activeTextEditor.document;
    let fileName = path.parse(document.fileName);
    let projectPath = "/Users/josephwoodward/Dev/VsCodeIlViewerDemoProj/console/project.json";
    
    let root = vscode.workspace.rootPath;
    let projectJson;
    vscode.workspace.findFiles("**/project.json","").then((uri) => {
        uri.forEach((x) => {
            projectJson = x.fsPath;

            if (projectJson == projectPath) {
                // let provider = new IntermediateLanguageContentProvider(previewUri, fileName.name, projectPath);
                // let registration = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
                // context.subscriptions.push(registration);
            }
            
        });
    });

    let provider = new IntermediateLanguageContentProvider(previewUri, fileName.name, projectPath);
    let registration = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);

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

    context.subscriptions.push(disposable, registration);
}

export function deactivate() {}