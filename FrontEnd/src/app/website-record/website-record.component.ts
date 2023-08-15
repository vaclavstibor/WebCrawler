// https://github.com/vasturiano/3d-force-graph

// Import required modules and dependencies
import { Component, Renderer2, OnInit, HostListener, ElementRef } from '@angular/core';
import { ActivatedRoute, ChildrenOutletContexts } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { SharedService } from '../shared.service';
import { Node } from '../models/Node';
import { FormsModule } from '@angular/forms';
import { getSafePropertyAccessString } from '@angular/compiler';

// Declare an external function 'ForceGraph3D' with 'any' return type (assumed to be provided by the '3d-force-graph' library)
declare function ForceGraph3D(): any;

enum Mode {
  Website,
  Domain
}

@Component({
  selector: 'app-website-record',
  templateUrl: './website-record.component.html',
  styleUrls: ['./website-record.component.css']
})
export class WebsiteRecordComponent implements OnInit {
  mode: Mode = Mode.Website;
  
  // Data structure to store the graph nodes and links for Website mode
  Data: {
    nodes: Node[]; 
    links: any[];
  } = {
 nodes:[
   {
       'id': 0,             
       'url': 'https://www.google.com/bla', 
       'domain': 'www.google.com',
       'crawlTime': '00:30',
       'regExpMatch': null, 
       'children': [ 
           {
               'id': 1,                    
               'url':'https://www.google.com/bla/bla', 
               'domain': 'www.google.com',
               'crawlTime':'01:30', 
               'regExpMatch': null,                    
               'children': null                    
           }, 
           {
               'id': 2,                    
               'url':'https://www.google.com/bla/bla/bla', 
               'domain': 'www.google.com',
               'crawlTime':'01:40', 
               'regExpMatch': null,                    
               'children': null                    
           },                 
           {
               'id': 3,  
               'url': 'http://www.twitter.com/bla',
               'domain': 'www.twitter.com',  
               'crawlTime':'01:30', 
               'regExpMatch': null,
               'children': null
           } 
       ]
   },
   {
       'id': 1,                    
       'url':'https://www.google.com/bla/bla', 
       'domain': 'www.google.com',
       'crawlTime':'01:30', 
       'regExpMatch': null,                    
       'children': null 
   },
   {
       'id': 2,                    
       'url':'https://www.google.com/bla/bla/bla', 
       'domain': 'www.google.com',
       'crawlTime':'01:40', 
       'regExpMatch': null,                    
       'children': null 
   },
   {
       'id': 3,  
       'url': 'http://www.twitter.com/bla',
       'domain': 'www.twitter.com',  
       'crawlTime':'01:30', 
       'regExpMatch': null,
       'children': null
   }
  ],
    //nodes: [],
    links: []
  };

  // Data structure to store the graph nodes and links for Domain mode
  DomainData : {
    nodes: Node[];
    links: any[];
  } = {
    nodes: [],
    links:[]
  }
  
  // Define a set to store the currently selected nodes
  selectedNode = new Set<Node>();

  constructor(
    private route: ActivatedRoute,
    private renderer: Renderer2,
    private elementRef: ElementRef,
    private http: HttpClient,
    private sharedService: SharedService
  ) { 
  }
  
  ngOnInit() { 
    this.load3DForceGraph();
  }

  switchToWebsiteMode() {
    this.DomainData = { nodes: [], links: [] };
    this.initializeMode();
  }  

  switchToDomainMode() {
    if (this.selectedNode.size === 0) {
      return;
    }
    this.DomainFilter();
    this.initializeMode();
  }

  toggleMode() {
    this.mode = this.mode === Mode.Website ? Mode.Domain : Mode.Website;
  
    if (this.mode === Mode.Website) {
      this.switchToWebsiteMode();
    } else if (this.mode === Mode.Domain) {
      this.switchToDomainMode();
    }
  }
  

  // Listen for window resize events and reinitialize the graph on window resize
  @HostListener('window:resize', ['$event'])
  onWindowResize(event: Event) {
    //this.initializeMode();
  }

  // Function to load the '3d-force-graph' script dynamically and initialize the graph  
  load3DForceGraph() {
    const scriptElement = this.renderer.createElement('script');
    scriptElement.src = '//unpkg.com/3d-force-graph';
    scriptElement.onload = () => {
      this.initializeMode();
    };
    this.renderer.appendChild(document.body, scriptElement);
  }

  initializeMode() {
    let graphData;

    switch (this.mode) {
      case Mode.Website:
        graphData = this.Data;
        this.CreateWebsiteLinks(graphData);
        break;
      case Mode.Domain:
        graphData = this.DomainData;
        this.CreateDomainLinks(graphData)
        break;
      default:
        graphData = this.Data;
        this.CreateWebsiteLinks(graphData);        
        break;
    }

    console.log("Mode: ", this.mode);
    console.log("Data: ", graphData);
    this.initializeGraph(graphData);
  }

  // Function to initialize the 3D Force Graph with the data and settings  
  initializeGraph(data: any) {
    // Create the 3D Force Graph instance
    let graph = ForceGraph3D()
      (document.getElementById('3d-graph')) // Bind the graph to the specified DOM element
      .width(window.innerWidth)             // Set the graph width to match the window width
      .height(window.innerHeight)           // Set the graph height to match the window height
      .backgroundColor('#FFFFFF')           // Set the background color of the graph
      .graphData(data)                      // Provide the graph data (nodes and links) to the graph instance
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
        }

        // Move the camera closer to the clicked node
        const distance = 75;
        const distRatio = 1 + distance / Math.hypot(node.x, node.y, node.z);

        const newPos = node.x || node.y || node.z
          ? { x: node.x * distRatio, y: node.y * distRatio, z: node.z * distRatio }
          : { x: 0, y: 0, z: distance };

        graph.cameraPosition(newPos, node, 1000);

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
    console.log("Create Website Links");

    for (const parent of data.nodes){
      if (parent.hasOwnProperty('children') && parent.children !== null){
        for (const child of parent.children) {  
          const link = { 'source': parent.id, 'target': child.id, color: '#000000'};
          data.links.push(link);
        }
      }
    }

    console.log(data.links);
  }

  CreateDomainLinks(data: any) {
    console.log("Create Domain Links:");

    for (const parent of data.nodes){
      if (parent.hasOwnProperty('children') && parent.children !== null){
        for (const child of parent.children) {  
          if (parent.domain == child.domain) {
            const link = { 'source': parent.id, 'target': child.id, color: '#000000'};
            data.links.push(link);
          }
        }
      }
    }

    console.log(data.links);
  }
  
  DomainFilter() {
    const selectedDomain = new Set<string>();
    this.selectedNode.forEach(node => {
      selectedDomain.add(node.domain);
      console.log(node.domain);
    });

    this.DomainData.nodes = this.Data.nodes.filter(node => {
      return selectedDomain.has(node.domain);
    });
  }
}