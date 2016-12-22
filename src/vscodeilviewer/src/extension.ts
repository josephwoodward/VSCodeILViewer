'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';

function viewIl(filename: string, lineNumber: number) {
    vscode.window.showInformationMessage('File: ' + filename + ' - Line: ' + lineNumber);
    let editor = vscode.window.activeTextEditor;
};

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "vscodeilviewer" is now active!');

    // The command has been defined in the package.json file
    // Now provide the implementation of the command with  registerCommand
    // The commandId parameter must match the command field in package.json
    let viewIlDisposable = vscode.commands.registerCommand('extension.viewIl', () => {

        let workspace = vscode.workspace;
        let env = vscode.env;

        let editor = vscode.window.activeTextEditor;
        let range = editor.revealRange;
        let doc = editor.document;

        viewIl(editor.document.fileName, 0);

    });

    context.subscriptions.push(viewIlDisposable);
}

// this method is called when your extension is deactivated
export function deactivate() {
}