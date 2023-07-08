import { CrawlingState } from "./CrawlingState";

export interface Execution
{
    websiteRecordId: number;
    websiteRecordLabel: string;
    executionStatus: CrawlingState;
    numberOfSitesCrawled: number;
    startTime: Date;
    endTime: Date|null;
}