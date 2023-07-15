var givenIdOfNode = sessionStorage.getItem("first")

let data =  {
    "nodes": [],
    "links": []
}

const domainIds = []

let selectedNode = new Set();
const e = document.getElementById('3d-graph')

const Graph = ForceGraph3D()(e)
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
        if (node.hasOwnProperty("children") && node.children !== null) {
            document.getElementById('record-a').appendChild(CreateRecordList(node.children));
        }
    })
    

    .onNodeDragEnd(node => {
        node.fx = node.x;
        node.fy = node.y;
        node.fz = node.z;
    })

document.getElementById("btn-switch-to-website").addEventListener("click", () => location.href = "website-index.html");

BuildGraph()

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
        DomainFilter();
        CreateLinks();
        Graph.graphData(data);
        console.log(data);
    } catch (error) {
        console.error(error);
    }
}

/**
 * Extracts the domain name from each URL in the 'nodes' array of the 'data' object
 * and adds it as a new 'Domain' property of each node.
 */
 function DomainFilter() {
    /*
    // Loop through each node in the 'nodes' array of the 'data' object
    for (var node of data["nodes"]) {
        // Extract the domain name from the URL using the 'URL' built-in constructor
        let domain = (new URL(node.url));
        // Log the full URL and the extracted domain name for debugging purposes
        console.log(domain);
        console.log(domain.hostname);
        // Add the extracted domain name as a new 'Domain' property of the node
        node.Domain = domain.hostname;
    }
    */
    // Filters the "nodes" within the "data" object, based on the "Domain" property of each node. Only nodes with the same domain as the node identified by "givenIdOfNode" are kept in the array.
    data["nodes"] = data["nodes"].filter(obj => {
        return obj.domain === data["nodes"][givenIdOfNode].domain
    })

    for (var node of data["nodes"]) {
        domainIds.push(node.id)
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
                if (domainIds.includes(child.id)) {
                    const link = { "source": parent.id, "target": child.id };
                    data["links"].push(link);
                }
            }
        }
    }

    console.log(data)
}

/*
function addNode(node) {

    let domain = (new URL(node.Url));
    console.log(domain);
    node.Domain = domain.hostname;

    data["nodes"].push(node)
    if (node.hasOwnProperty("Nodes")) {
        for (const child of node.Nodes) {
            const link = { "source": node.id ,"target": child.id };
            console.log(link)
            data["links"].push(link)
        }
    }

    Graph.graphData(data)
    Graph.nodeAutoColorBy('Domain')
}
*/