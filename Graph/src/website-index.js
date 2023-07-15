// '../example/datasets/miserables.json'

/*

For selected website records (active selection) user can view a map of crawled pages as a graph. Nodes are websites/domains. DONE
There is an oriented edge (connection) from one node to another if there is a hyperlink connecting them in a given direction. DONE
The graph should also contain nodes for websites/domains that were not crawled due to a Boundary RegExp restriction. Those nodes will have different visuals so they can be easily identified. PRBLY DONE
A user can switch between website view and domain view. In the website view, every website is represented by a node. In the domain view, all nodes from a given domain (use a full domain name) are replaced by a single node.  DONE
By double-clicking (!NOT DOUBLE!), the node the user can open node detail. For crawled nodes, the details contain URL, Crawl time, and list of website record that crawled given node. DONE
The user can start new executions for one of the listed website records. For other nodes, the detail contains only URL and the user can create and execute a new website record. MISSIN` 
The newly created website record is automatically added to the active selection and mode is changed to live. The visualisation can be in live or static mode. In static data are not refreshed. DONE/MISSIN`
In the live mode data are periodically updated based on the new executions for active selection. MISSINN` 
If a single node is crawled by multiple executions from active selection data from lates execution are used for detail. !BORIS! It depends on updating database - backend. 
Use page title or URL, in given order of preference, as a node label. In domain node employ the URL.

*/


/*
 *TODO*

 - popis, jestli to je staticky/live a aktualni mode
 - DONE vrcholy se stejnou domenou obarvit na stejnou barvu
 - DONE zmena na graf POOUZE s DOMENOU
 - DONE obarvovani po pridani nodes 
 - list of list namísto array of list v node.Nodes

*/

let data =  {
    "nodes": [],
    "links": []
}

let selectedNode = new Set();
const graphElement = document.getElementById('3d-graph')

const Graph = ForceGraph3D()(graphElement)
    .graphData(data)
    .nodeLabel('id')
    .linkOpacity(0.3)
    .nodeOpacity(0.95)
    .linkDirectionalArrowRelPos(1)
    .linkDirectionalArrowLength(3.5)
    .linkCurvature(0.15)
    .nodeAutoColorBy('domain')
    .onNodeClick((node, event) => {
        const untoggle = selectedNode.has(node);
        selectedNode.clear();
        !untoggle && selectedNode.add(node);

        const distance = 75
        const distRatio = 1 + distance/Math.hypot(node.x, node.y, node.z);

        const newPos = node.x || node.y || node.z
            ? { x: node.x * distRatio, y: node.y * distRatio, z: node.z * distRatio }
            : { x: 0, y: 0, z: distance };

        Graph.cameraPosition(newPos, node, 1000);
        document.getElementById("url-a").textContent=node.url;
        document.getElementById("crawl-time-a").textContent=node.crawlTime;
        DeleteRecordsList();
        if (node.hasOwnProperty("children") && node.children != null) {
            document.getElementById('record-a').appendChild(CreateRecordList(node.children));
        }
        sessionStorage.setItem("first", node.id)
    })
    

    .onNodeDragEnd(node => {
        node.fx = node.x;
        node.fy = node.y;
        node.fz = node.z;
    })

document.getElementById("btn-switch-to-domain").addEventListener("click", () => location.href = "domain-index.html");

BuildGraph()

// Get data from JSON file
async function GetData(fileName) {
    try {
        const response = await fetch(fileName);
        const dataJSON = await response.json();
        return dataJSON;
    } catch (error) {
        console.error(error);
    }
}

async function BuildGraph() {
    try {
        data = await GetData('./data.json');
        CreateLinks();
        Graph.graphData(data);
        console.log(data);
    } catch (error) {
        console.error(error);
    }
}

function CreateRecordList(children){
    var list = document.createElement('ul');
    list.setAttribute('id', 'record-list');

    for (var i = 0; i < children.length; i++){
        var item = document.createElement('li');
        item.appendChild(document.createTextNode(children[i].url));
        console.log(children[i].url)
        list.appendChild(item);
    }

    return list;
}

function DeleteRecordsList() {
    let element = document.getElementById('record-list')

    if (element !== null) {
        element.remove()
    }
}

function CreateLinks() {
    for (const parent of data["nodes"]) {
        if (parent.hasOwnProperty("children") && parent.children !== null) {
            for (const child of parent.children) {
                const link = { "source": parent.id, "target": child.id };
                data["links"].push(link);
            }
        }
    }

    console.log(data)
}

/*
function DomainFilter() {
    for (var node of data["nodes"]) {
        let domain = (new URL(node.url));
        //console.log(domain);
        node.Domain = domain.hostname;
        //console.log(domain.hostname);
    }
}
*/

/*
const externalNode = {
    "id":5, 
    "Group":1, 
    "Url":"http://www.facbook.com/bla/bla", 
    "Time":"03:30", 
    "Nodes": 
        [{
            "id":2, 
            "Group":0, 
            "Url":"http://www.twitter.com/bla", 
            "Time":"02:30", 
            "Nodes": 
                [{
                    "id":3, 
                    "Group":1, 
                    "Url":"http://www.facbook.com/bla", 
                    "Time":"01:30"  
                }]
        }, 
        {
            "id":3, 
            "Group":1, 
            "Url":"http://www.facbook.com/bla", 
            "Time":"03:30"
        }]
}
*/

/*
function addNode(node) {

    let domain = (new URL(node.Url));
    //console.log(domain);
    node.Domain = domain.hostname;

    data["nodes"].push(node)
    if (node.hasOwnProperty("nodes")) {
        for (const child of node.nodes) {
            const link = { "source": node.id ,"target": child.id };
            //console.log(link)
            data["links"].push(link)
        }
    }

    Graph.graphData(data)
    Graph.nodeAutoColorBy('Domain')
}
*/