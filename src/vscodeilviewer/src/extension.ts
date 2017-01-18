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
let child : child_process.ChildProcess;

let ilWindowUri = vscode.Uri.parse(`il-viewer://authority/${IntermediateLanguageContentProvider.Scheme}`);

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

const disposables: vscode.Disposable[] = [];

export function activate(context: vscode.ExtensionContext) {

    let invokationDisposable = vscode.commands.registerCommand('extension.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', ilWindowUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage(reason);
        });
    });
    disposables.push(invokationDisposable);

    let provider = new IntermediateLanguageContentProvider(ilWindowUri);
    let providerDisposable = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
    disposables.push(providerDisposable);

    const fullPath = path.join(vscode.extensions.getExtension("josephwoodward.vscodeilviewer").extensionPath) + "/server/ilViewer.WebApi.dll";
    fs.exists(fullPath, function(exists){
        if (!exists){
            vscode.window.showErrorMessage("Unable to start IL Viewer server");
            return;
        }
        
        startServer(fullPath);
    });

    context.subscriptions.push(...disposables);
}

export function deactivate() {
    child.kill('SIGTERM');
}

export function startServer(path: string){

    child = DecompilerProcess.spawn(path, [ ]);
    child.on('error', data => {
        console.log(`Child error: ${data}`);
    });

    let out = child.stdout;
    out.on("readable", function(){
        let data = child.stdout.read();
    });

    process.on('SIGTERM', () => {
        child.kill();
        process.exit(0); 
    });

    process.on('SIGHUP', () => {
        child.kill();
        process.exit(0); 
    });

    child.on('close', code => {
        console.log(code);
        if (code !== 0) {
            var data = code.toString();
        } else {
            var data = code.toString();
        }
    });

    child.stdout.on('data', data => {
        var res = data.toString();
        process.stdout.write(data);
    });

    process.stdin.on('data', data => {
        var res = data.toString();
        child.stdin.write(data); 
    });
}