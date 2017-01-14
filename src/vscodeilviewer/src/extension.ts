'use strict';

import { IntermediateLanguageContentProvider } from './IntermediateLanguageContentProvider';
import { DecompilerProcess } from './process';
import * as path from 'path';
import * as vscode from 'vscode';
import * as child_process from 'child_process';
import * as fs from 'fs';
import * as os from 'os';

const request = require('request');
const events = require('events');


let ilWindowUri = vscode.Uri.parse(`il-viewer://authority/${IntermediateLanguageContentProvider.Scheme}`);

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

const disposables: vscode.Disposable[] = [];

export function activate(context: vscode.ExtensionContext) {

    // let res = vscode.workspace.textDocuments;
    
    let invokationDisposable = vscode.commands.registerCommand('editor.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', ilWindowUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });
    disposables.push(invokationDisposable);

    let provider = new IntermediateLanguageContentProvider(ilWindowUri);
    let providerDisposable = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
    disposables.push(providerDisposable);

    let child = DecompilerProcess.spawn("/Users/josephwoodward/Dev/VsCodeIlViewer/src/IlViewer.WebApi/bin/Debug/netcoreapp1.0/IlViewer.Core.dll", [ "--debug" ]);
    child.on('error', data => {
        console.log(`Child error: ${data}`);
    });

    //child.kill()

    

    // child.stdout.on('data', data => {
    //     var res = data.toString();
    // });

    // child.stdout.on('data', data => {
    //     var res = data.toString();
    //     process.stdout.write(data);
    // });

    // child.stdout.push("Hello!")

    // process.stdin.on('data', data => {
    //     var res = data.toString();
    //     child.stdin.write(data); 
    // });

    let platform = process.platform;
    // var res = path.join('server', 'out', 'serverMain.js');
	// let serverModule = context.asAbsolutePath(res);
	// // The debug options for the server
	// let debugOptions = { execArgv: ["--nolazy", "--debug=6004"] };

    context.subscriptions.push(...disposables);
}

export function deactivate() {}