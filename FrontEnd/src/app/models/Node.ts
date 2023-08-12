export interface Node {
    id: number;
    url: string;
    crawlTime: string;
    domain: string;
    regExpMatch: boolean | null;
    children: Node[] | null;
}