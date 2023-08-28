// https://github.com/vasturiano/3d-force-graph

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SharedService } from '../../../shared.service';
import { Node } from '../../../models/Node';

import ForceGraph3D, { 
  ConfigOptions, 
  ForceGraph3DInstance 
} from "3d-force-graph";

@Component({
  selector: 'app-domain-mode',
  templateUrl: './domain-mode.component.html',
  styleUrls: ['./domain-mode.component.css']
})
export class DomainModeComponent implements OnInit, OnDestroy {
  private graph!: ForceGraph3DInstance;
  public selectedNode = new Set<Node>();
  private domainToNodeMap: { [key: string]: Node } = {};
  private intervalId: any;
  
  id: number = 0;

  data: {
    nodes: Node[]; 
    links: any[];
  } = {
    nodes: [],
    links: []
  };

  constructor(private sharedService: SharedService, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.route.params.subscribe(x => this.id = x['id']);
    this.replaceGraphElement();
  }

  getInitialData(): void {
    console.log("Domain Mode: Trying to get initial data.");

    this.sharedService.getGraphLive(this.id).subscribe(result => {
      this.data.nodes = result;

      if (this.data.nodes.length > 0) {
        this.initializeGraph();
        this.intervalId = setInterval(() => this.getData(), 5000);
      } 
    });
  }

  getData(): void {
    console.log("Trying to get new data after initial.");

    this.sharedService.getGraphLive(this.id).subscribe(result => {
      this.CreateWebsiteLinks(result);
    });
  }

  // Function to replace the graph element with a new one
  replaceGraphElement() {
    const currentGraphElement = document.getElementById('3d-graph-domain');
    
    if (currentGraphElement) {
      // Create a new div element with the same ID
      const newGraphElement = document.createElement('div');
      newGraphElement.id = '3d-graph-domain';

      // Replace the existing element with the new one
      currentGraphElement.replaceWith(newGraphElement);

      // Call the initializeGraph function again with the new element
      this.getInitialData();
    }
  }

  // Function to initialize the 3D Force Graph with the data and settings  
  initializeGraph() {
    // Create the 3D Force Graph instance
    this.CreateWebsiteLinks(this.data.nodes);
    this.graph = ForceGraph3D()
      (document.getElementById('3d-graph-domain')!) // Bind the graph to the specified DOM element
      .width(window.innerWidth)             // Set the graph width to match the window width
      .height(window.innerHeight)           // Set the graph height to match the window height
      .backgroundColor('#FFFFFF')           // Set the background color of the graph
      .graphData(this.data)                      // Provide the graph data (nodes and links) to the graph instance
      .nodeLabel('title')                      // Display the 'id' property as the node label
      .linkOpacity(0.3)                     // Set the opacity of the links
      .nodeOpacity(0.95)                    // Set the opacity of the nodes
      .linkDirectionalArrowRelPos(1)        // Set the relative position of the directional arrow on the links
      .linkDirectionalArrowLength(3.5)      // Set the length of the directional arrow on the links
      .linkCurvature(0.15)                  // Set the curvature of the links
      .nodeAutoColorBy('domain')            // Automatically color the nodes based on the 'domain' property
      .onNodeClick((node: any, event: Event) => {
        // Event handler for node click     
        const untoggle = this.selectedNode.has(node);
        this.selectedNode.clear();
        if (!untoggle) {
          this.selectedNode.add(node);
        }

        // Move the camera closer to the clicked node
        const distance = 75;
        const distRatio = 1 + distance / Math.hypot(node.x, node.y, node.z);

        const newPos = node.x || node.y || node.z
          ? { x: node.x * distRatio, y: node.y * distRatio, z: node.z * distRatio }
          : { x: 0, y: 0, z: distance };

        this.graph.cameraPosition(newPos, node, 1000);

        // Display node details in the UI
        (document.getElementById('title-a') as HTMLInputElement).textContent=node.title;
        (document.getElementById('url-a') as HTMLInputElement).textContent=node.url;
        (document.getElementById('crawl-time-a') as HTMLInputElement).textContent=node.crawlTime.toFixed(2);
        
        // Delete previous record list and create a new one for the selected node's children        
        this.deleteRecordsList();
        if (node.hasOwnProperty('children') && node.children != null) {
          (document.getElementById('record-a') as HTMLInputElement).appendChild(this.createRecordList(node.children));
        }

        // Store the selected node's 'id' in session storage        
        sessionStorage.setItem('first', node.id);
      })
      .onNodeDragEnd((node: any) => {
        // Event handler for node drag end - set the node's fixed position after dragging     
        node.fx = node.x;
        node.fy = node.y;
        node.fz = node.z;
      });
  }

  // Function to create a nested list of child nodes
  createRecordList(children: Node[]): HTMLUListElement {
    const list = document.createElement('ul');
    list.setAttribute('id', 'record-list');

    for (const child of children) {
      const item = document.createElement('li');
      item.appendChild(document.createTextNode(child.url));
      list.appendChild(item);
    }

    return list;
  }

  // Function to delete the previous record list
  deleteRecordsList() {
    const element = document.getElementById('record-list');
    if (element !== null) {
      element.remove();
    }
  }

  CreateWebsiteLinks(data: any) {     
    let nodes: Node[] = [];
    let links: any = [];

    for (const parent of data) {
      this.DomainToNodeMapping(parent);
      if (parent.hasOwnProperty('children') && parent.children.length > 0) {
        for (const child of parent.children) {
          this.DomainToNodeMapping(child);
          //@ts-ignore
          if (!links.some(_link => _link.sourceDomain === parent.domain && _link.targetDomain === child.domain)) {
            const link = { 'source': this.domainToNodeMap[parent.domain].id, sourceDomain: parent.domain, 'target': this.domainToNodeMap[child.domain].id, targetDomain: child.domain, color: '#000000', };
            links.push(link);
          }
        }
      }   
    } 
    
    nodes = Object.values(this.domainToNodeMap);
    
    if (this.graph !== undefined) {
      this.graph.graphData({nodes, links});
      console.log("domain: ", nodes.length);
      console.log("links: ", links.length);
    } else {
      this.data.nodes = nodes;
      this.data.links = links;
    }
  }

  DomainToNodeMapping(node: Node) {
    const domain = node.domain;

    if (!(domain in this.domainToNodeMap)) {
      this.domainToNodeMap[domain] = node;
      console.log("Mapping domain: ", domain);
    }
  }
  

  ngOnDestroy() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
    }
    console.log("Destroying domain");
  }
}
