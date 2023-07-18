import { Tag } from "./Tag";

export interface WebsiteRecord
{
    id: number;
    url: string;
    regExp: string;
    hours: number;
    minutes: number;
    days: number;
    label: string;
    active: boolean;
    lastExecution: Date;
    tagDTOs: Tag[];
    executionStatus: string;
    periodicity: string;
}