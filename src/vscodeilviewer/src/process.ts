import * as child from 'child_process';
import * as SpawnOptions from 'child_process';
import { Logger } from './Logger';

export namespace DecompilerProcess {

    export function spawn(cmd: string, args: string[], logger: Logger, options?) : child.ChildProcess {
        logger.appendLine(`Executing command: ${cmd}`);
        return child.spawn("dotnet", [ cmd ].concat(args), options);
    }
}