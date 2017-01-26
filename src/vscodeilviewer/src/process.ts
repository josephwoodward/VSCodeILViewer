import * as child from 'child_process';
import * as SpawnOptions from 'child_process';
import { Logger } from './logger';

export namespace DecompilerProcess {

    export function spawn(cmd: string, args: string[], logger: Logger, options?) : child.ChildProcess {
        logger.appendLine(`Starting server with command: ${cmd}`);
        return child.spawn("dotnet", [ cmd ].concat(args), options);
    }

}