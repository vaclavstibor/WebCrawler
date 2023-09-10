// https://github.com/vasturiano/3d-force-graph

import { Component, Input, OnInit, OnDestroy, OnChanges, SimpleChange, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SharedService } from '../../../shared.service';
import { Node } from '../../../models/Node';

import ForceGraph3D, { 
  ConfigOptions, 
  ForceGraph3DInstance 
} from "3d-force-graph";
import { State, Mode } from '../../website-record.component';

@Component({
  selector: 'app-website-mode',
  templateUrl: './website-mode.component.html',
  styleUrls: ['./website-mode.component.css']
})
export class WebsiteModeComponent implements OnInit, OnDestroy {
  @Input() state!: State;
  mode: Mode = Mode.Website;
  private graph!: ForceGraph3DInstance;
  public selectedNode = new Set<Node>();
  private interval: any;

  id: number = 0;

  lastUpdateState: number = 0;
  
  data: {
    nodes: Node[];
    links: any[];
  } = {
    nodes: [],
    links: []
  };

  constructor(private sharedService:SharedService, private route: ActivatedRoute) {}

  ngOnChanges(changes: SimpleChanges) {
    let state = changes['state'];
    if (!state.firstChange) {
      switch(state.currentValue) {
        case 1: // Current state is Static
          clearInterval(this.interval);
          break;
        case 0: // Current state is Live
          this.getLiveData();
          this.interval = setInterval(() => this.getLiveData(), 5000);
          break;
      }
    }
  }

  ngOnInit(): void {
    this.route.params.subscribe(x => this.id = x['id']);
    this.replaceGraphElement();
  }

  getInitialStaticData(): void {
    console.log(`${Mode[this.mode]} Mode: Trying to get initial ${State[this.state]} data.`);  

    this.sharedService.getGraph(this.id).subscribe(result => {
      this.data.nodes = result;

      if (this.data.nodes.length > 0) {
        this.initializeGraph();
      } 
    });    
  }

  getInitialLiveData(): void {
    console.log(`${Mode[this.mode]} Mode: Trying to get initial ${State[this.state]} data.`);    

    this.sharedService.getGraphLive(this.id, this.lastUpdateState).subscribe(result => {
      this.data.nodes = result.nodes;
      this.lastUpdateState = result.updateState;

      if (this.data.nodes.length > 0) {
        this.initializeGraph();
        this.interval = setInterval(() => this.getLiveData(), 2000);
      } 
    });
  }  

  getLiveData(): void {
    console.log(`${Mode[this.mode]} Mode: Trying to get periodically new data.`);

    this.sharedService.getGraphLive(this.id, this.lastUpdateState).subscribe(result => {
      this.lastUpdateState = result.updateState;
      this.CreateWebsiteLinks(result.nodes);
    });
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
      
      this.sharedService.getExecutionStatus(this.id).subscribe(executionStatus => {
        switch (executionStatus) {
          case "executed":
            //TODO: better remove
            if ((document.getElementById('switch-state') as HTMLInputElement))
            {(document.getElementById('switch-state') as HTMLInputElement).remove();}
            this.getInitialStaticData();
            break;
          case "executing":
            this.getInitialLiveData();
            break;
          default:
            console.log("Unknownd executing status: ", executionStatus)
            break;
        }
      });
    }
  }

  // Function to initialize the 3D Force Graph with the data and settings  
  initializeGraph() {
    this.CreateWebsiteLinks(this.data.nodes);
    this.graph = ForceGraph3D({ controlType: 'orbit' })
      (document.getElementById('3d-graph-website')!) // Bind the graph to the specified DOM element
      .width(window.innerWidth)                      // Set the graph width to match the window width
      .height(window.innerHeight)                    // Set the graph height to match the window height
      .backgroundColor('#FFFFFF')                    // Set the background color of the graph
      .graphData(this.data)                          // Provide the graph data (nodes and links) to the graph instance
      .nodeLabel('title')                            // Display the 'id' property as the node label
      .linkDirectionalArrowRelPos(1)                 // Set the relative position of the directional arrow on the links
      .linkDirectionalArrowLength(3.5)               // Set the length of the directional arrow on the links
      .nodeAutoColorBy('domain')                     // Automatically color the nodes based on the 'domain' property
      .onNodeClick((node: any, event: Event) => { 
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
        (document.getElementById('crawl-time-a') as HTMLInputElement).textContent=node.crawlTime;

        // Delete previous record list and create a new one for the selected node's children        
        this.deleteRecordsList();
        if (node.hasOwnProperty('children') && node.children != null) {
          (document.getElementById('record-a') as HTMLInputElement).appendChild(this.createRecordList(node.children));
        }
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

  deleteTextContentOfElements(): void {
    (document.getElementById('title-a') as HTMLInputElement).textContent='Title';
    (document.getElementById('url-a') as HTMLInputElement).textContent='Url';
    (document.getElementById('crawl-time-a') as HTMLInputElement).textContent='Crawl time';
    this.deleteRecordsList();
  }

  CreateWebsiteLinks(data: any) { 
    for (const parent of data) {
      for (const child of parent.children) {
        if (!this.data.nodes.some(node => node.id === child.id)) {
          /*
          if (child.regExpMatch === false) {
            child.color = '#000000';
          }
          */

          this.data.nodes.push(child);
        }
        if (!this.data.links.some(link => link.source.id === parent.id && link.target.id === child.id))
        {
          const link = { 'source': parent.id, 'target': child.id, color: '#000000' };
          this.data.links.push(link);
        }
      } 
  
      if (!this.data.nodes.some(node => node.id === parent.id)) {
        /*
        if (parent.regExpMatch === false) {
          parent.color = '#000000';
        }
        */

        this.data.nodes.push(parent);
      }
    }
    if (this.graph !== undefined)
    {
      this.graph.graphData(this.data);
      console.log("Count of nodes: ", this.data.nodes.length)
      console.log("Count of links: ", this.data.links.length)
    }
  }

  ngOnDestroy() {
    console.log("Destorying website");

    if (this.interval) {
      clearInterval(this.interval);
    }

    this.selectedNode.clear();
    this.deleteTextContentOfElements();
  }
}

// Animatiopn of graph is more optiaplzation then last time. But Domai  mode is not enough
// It hase smoother load without line courcave
// Need TODO optialization of searching new nodes.
