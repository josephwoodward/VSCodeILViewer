'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as path from 'path';
import * as vscode from 'vscode';
import * as os from 'os';

// var parentfinder = require('find-parent-dir');
// var electron = require('./electron_j');

function viewIl(filename: string, lineNumber: number) {
    vscode.window.showInformationMessage('File: ' + filename + ' - Line: ' + lineNumber);
    let editor = vscode.window.activeTextEditor;
};

let getServiceUrl = () => {
    return vscode.workspace.getConfiguration("ilViewer")["serviceUrl"];
};

// function getTempWorkspace() {
// 	return path.resolve(os.tmpdir(),'vscodesws_'+makeRandomHexString(5));
// }

function makeRandomHexString(length) {
    var chars = ['0', '1', '2', '3', '4', '5', '6', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'];
    var result = '';
    for (var i = 0; i < length; i++) {
        var idx = Math.floor(chars.length * Math.random());
        result += chars[idx];
    }
    return result;
}

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {

    vscode.window.showInformationMessage('loaded');
    let storagePath = context.storagePath;
    let o = os;
    let dir = o.tmpdir();
    // if (!storagePath) {
    //     storagePath = getTempWorkspace();
    // }
    //let serverOptions = runJavaServer;

    // Use the console to output diagnostic information (console.log) and errors (console.error)
    // This line of code will only be executed once when your extension is activated
    console.log('Congratulations, your extension "vscodeilviewer" is now active!');

    // var filename = vscode.window.activeTextEditor.document.fileName;
    // var parentdir = parentfinder.sync(path.dirname(filename), 'project.json');
    // if (parentdir[parentdir.length - 1] === path.sep) {
    //     parentdir = parentdir.substr(0, parentdir.length - 1);
    // }

    // electron.fork(child, params, {}, function(err, result) {
    //     if(err) { reject(err); }
    //     if(result){ resolve(result); }
    // });

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

    let textEditor = vscode.commands.registerTextEditorCommand("editor.printReferences", editor => {
         vscode.window.showInformationMessage('called');
    });

    context.subscriptions.push(
        viewIlDisposable,
        textEditor);
}

// this method is called when your extension is deactivated
export function deactivate() {
}