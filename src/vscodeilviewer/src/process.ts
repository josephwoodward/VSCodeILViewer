import * as child from 'child_process';

export namespace DecompilerProcess {

    export function spawn(cmd: string, args: string[]) : child.ChildProcess {
        return child.spawn("dotnet", [ cmd ].concat(args));
    }

}