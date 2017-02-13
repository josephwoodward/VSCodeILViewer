'use strict';

import { IntermediateLanguageContentProvider } from './IntermediateLanguageContentProvider';
import { DecompilerProcess } from './process';
import { Logger } from './logger';
import * as path from 'path';
import * as vscode from 'vscode';
import * as child_process from 'child_process';
import * as fs from 'fs';
import * as os from 'os';

let child : child_process.ChildProcess;
let ilWindowUri = vscode.Uri.parse(`il-viewer://authority/${IntermediateLanguageContentProvider.Scheme}`);
let logger = new Logger(message => console.log(message), "Info");

const disposables: vscode.Disposable[] = [];

export function activate(context: vscode.ExtensionContext) {
    logger.append("Executing activate");

    let invokationDisposable = vscode.commands.registerCommand('extension.showIlWindow', () => {
        return vscode.commands.executeCommand('vscode.previewHtml', ilWindowUri, vscode.ViewColumn.Two, 'IL Viewer').then((success) => {
        }, (reason) => {
            vscode.window.showErrorMessage("There's been an error: " + reason);
        });
    });
    disposables.push(invokationDisposable);

    let provider = new IntermediateLanguageContentProvider(ilWindowUri);
    let providerDisposable = vscode.workspace.registerTextDocumentContentProvider(IntermediateLanguageContentProvider.Scheme, provider);
    disposables.push(providerDisposable);

    const fullPath = path.join(vscode.extensions.getExtension("josephwoodward.vscodeilviewer").extensionPath) + "/server/ilViewer.WebApi.dll";
    fs.exists(fullPath, function(exists){
        if (!exists){
            logger.appendLine(`Unable to start server, can't find path at ${fullPath}`);
            vscode.window.showErrorMessage("Unable to start IL Viewer server, check developer console");
            return;
        }
        
        // startServer(fullPath);
    });

    context.subscriptions.push(...disposables);
}

export function deactivate() {
    child.kill('SIGTERM');
}

export function startServer(path: string){

    child = DecompilerProcess.spawn(path, [ ], logger);
    child.on('error', data => {
        logger.appendLine("Error starting server");
        console.log(`Child error`);
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
        // if (code !== 0) {
        //     var data = code.toString();
        // } else {
        //     var data = code.toString();
        // }
        // let data = (code != null) ? code.toString() : "unknown";
        logger.appendLine("Closing server");
    });

    child.stdout.on('data', data => {
        let response = data != null ? data.toString() : "";
        logger.appendLine(`Server output: ${response}`)
        process.stdout.write(data);
    });

    process.stdin.on('data', data => {
        var res = data.toString();
        child.stdin.write(data); 
    });
}