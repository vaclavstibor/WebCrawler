// https://github.com/vasturiano/3d-force-graph

import { Component, Renderer2, OnInit, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SharedService } from '../../../shared.service';
import { Node } from '../../../models/Node';

import ForceGraph3D, { 
  ConfigOptions, 
  ForceGraph3DInstance 
} from "3d-force-graph";

@Component({
  selector: 'app-website-mode',
  templateUrl: './website-mode.component.html',
  styleUrls: ['./website-mode.component.css']
})
export class WebsiteModeComponent implements OnInit, OnDestroy {
  private graph!: ForceGraph3DInstance;
  public selectedNode = new Set<Node>();
  
  id: number = 0;
  // Data structure to store the graph nodes and links for Website mode
  
  data: {
    nodes: Node[]; 
    links: any[];
  } = {
    nodes: [],
    links: []
  };

  constructor(private sharedService:SharedService, private route: ActivatedRoute, private route2: ActivatedRoute,private sharedService2:SharedService, private elementRef: ElementRef) {

  }
  
  ngOnInit(): void {
    this.replaceGraphElement();
  }

  getLiveInitialData(): void {
    console.log("Trying to get initial data.");
    this.route.params.subscribe(x => this.id = x['id']);

    this.sharedService.getGraphLiveInitial(this.id).subscribe(x => 
    {
      this.data.nodes = x;
      console.log(x);
      this.initializeGraph();
      setInterval(() => this.getLiveData(), 10000);
    });
  }

  getLiveData(): void {
    console.log("Trying to get new data after initial.");

    this.sharedService.getGraphLive(this.id)
      .subscribe(
        result => this.AddNodes(result))
  }

  // Function to replace the graph element with a new one
  replaceGraphElement() {
    const currentGraphElement = document.getElementById('3d-graph-website');
    
    if (currentGraphElement) {
      // Create a new div element with the same ID
      const newGraphElement = document.createElement('div');
      newGraphElement.id = '3d-graph-website';

      // Replace the existing element with the new one
      currentGraphElement.replaceWith(newGraphElement);

      // Call the initializeGraph function again with the new element
      //this.initializeGraph();
      this.getLiveInitialData();
    }
  }

  // Function to initialize the 3D Force Graph with the data and settings  
  initializeGraph() {
    // Create the 3D Force Graph instance
    //console.log(this.data.nodes);
    this.CreateWebsiteLinks(this.data);
    this.graph = ForceGraph3D()
      (document.getElementById('3d-graph-website')!) // Bind the graph to the specified DOM element
      .width(window.innerWidth)             // Set the graph width to match the window width
      .height(window.innerHeight)           // Set the graph height to match the window height
      .backgroundColor('#FFFFFF')           // Set the background color of the graph
      .graphData(this.data)                 // Provide the graph data (nodes and links) to the graph instance
      .nodeLabel('id')                      // Display the 'id' property as the node label
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
          this.sharedService.setSelectedNode(this.selectedNode);
        }

        // Move the camera closer to the clicked node
        const distance = 75;
        const distRatio = 1 + distance / Math.hypot(node.x, node.y, node.z);

        const newPos = node.x || node.y || node.z
          ? { x: node.x * distRatio, y: node.y * distRatio, z: node.z * distRatio }
          : { x: 0, y: 0, z: distance };

        this.graph.cameraPosition(newPos, node, 1000);

        // Display node details in the UI
        (document.getElementById('url-a') as HTMLInputElement).textContent=node.url;
        (document.getElementById('crawl-time-a') as HTMLInputElement).textContent=node.crawlTime;
        
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

  // Function to create links between nodes based on the 'children' property of nodes
  CreateWebsiteLinks(data: any) {
    for (const parent of data.nodes){
      if (parent.hasOwnProperty('children') && parent.children !== null){
        for (const child of parent.children) {  
          const link = { 'source': parent.id, 'target': child.id, color: '#000000'};
          data.links.push(link);
        }
      }
    }
  }

  AddNodes(data: any) {
    for (const parent of data.nodes) {
      if (parent.hasOwnProperty('children') && parent.children !== null){
        for (const child of parent.children) {  
          if (this.data.nodes.find(node => node.id !== child.id)) {
            this.data.nodes.push(child);
          }
          else {
            //data.nodes.
            return;
          }
        }
      }
      if (this.data.nodes.find(node => node.id === parent.id)) {
        this.data.nodes.push(parent);
      } 
    }
  }

  ngOnDestroy()
  {
    console.log("Destorying website")
  }
}

// 

// If crawling -> active/static || static
// API - isCrawling, newNodes

 /*
  @HostListener('window:resize', ['$event'])
  public windowResize(event: Event): void {
    const element = this.elementRef.nativeElement as HTMLElement;
    const box = element.getBoundingClientRect();
    this.graph.width(box.width);
    this.graph.height(box.height);
    // @ts-ignore
    this.graph?.controls().handleResize();
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