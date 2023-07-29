export interface Node {
    id: number;
    url: string;
    crawlTime: Date;
    domain: string;
    regExpMatch: boolean | null;
    children: Node[] | null;
}