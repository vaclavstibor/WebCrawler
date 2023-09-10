import { Node } from "./Node";

export interface ExecutionState
{
    updateState: number;
    nodes: Node[];
}