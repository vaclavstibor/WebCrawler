export interface IRecord
{
    Id: number;
    URL: string;
    RegExp?: string;
    Hours?: number;
    Minutes?: number;
    Days?: number;
    Label?: string;
    Active: boolean;
    LastExecution: string;
    Tags: [ITag]; 
    ExecutionStatus?: boolean;
    Periodicity: string;
}

export interface ITag
{
    Content: string;
    Id: number;
    WebsiteRecordId: number;
}