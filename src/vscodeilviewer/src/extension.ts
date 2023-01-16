'use strict';

import { IntermediateLanguageContentProvider } from './IntermediateLanguageContentProvider';
import { DecompilerProcess } from './process';
import { Logger } from './Logger';
import * as path from 'path';
import * as vscode from 'vscode';
import * as child_process from 'child_process';
import * as fs from 'fs';
import * as os from 'os';

let child : child_process.ChildProcess;
let logger = new Logger(message => console.log(message), "Info");

export function activate(context: vscode.ExtensionContext) {
    logger.append("Executing activate");

    vscode.commands.registerCommand('extension.showIlWindow', () => {
        const panel = vscode.window.createWebviewPanel(
			'iLViewer',
			'IL Viewer',
			vscode.ViewColumn.Two,
            {}
		);

        const fullPath = path.join(vscode.extensions.getExtension("josephwoodward.vscodeilviewer").extensionPath) + "/server/ilViewer.WebApi.dll";
        fs.exists(fullPath, function(exists){
            if (!exists){
                logger.appendLine(`Unable to start server, can't find path at ${fullPath}`);
                vscode.window.showErrorMessage("Unable to start IL Viewer server, check developer console");
                return;
            }
            
            startServer(fullPath);
        });
        
        let provider = new IntermediateLanguageContentProvider(panel);
        provider.provideTextDocumentContent();
    });


}

export function deactivate() {
    child.kill('SIGTERM');
}

export function startServer(path: string){

    child = DecompilerProcess.spawn(path, [ "StartServer" ], logger);
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