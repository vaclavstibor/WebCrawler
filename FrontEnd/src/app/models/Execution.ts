// TODO: create Enum for Executing Status 
export interface Execution
{
    websiteRecordId: number;
    websiteRecordLabel: string;
    executionStatus: string;
    numberOfSitesCrawled: number;
    startTime: Date;
    endTime: Date;
}