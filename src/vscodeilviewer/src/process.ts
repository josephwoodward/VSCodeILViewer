import * as child from 'child_process';
import * as SpawnOptions from 'child_process';

export namespace DecompilerProcess {

    export function spawn(cmd: string, args: string[], options?) : child.ChildProcess {
        return child.spawn("dotnet", [ cmd ].concat(args), options);
    }

}