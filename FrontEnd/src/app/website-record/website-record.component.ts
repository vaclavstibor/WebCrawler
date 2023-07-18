import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

declare var ForceGraph3D: any; // Assuming ForceGraph3D is a global variable

@Component({
  selector: 'app-website-record',
  templateUrl: './website-record.component.html',
  styleUrls: ['./website-record.component.css']
})
export class WebsiteRecordComponent implements OnInit {
  constructor(private route: ActivatedRoute, private http: HttpClient) { }

  ngOnInit() {
    this.buildGraph();
  }

  async buildGraph() {
    try {
      const data = await this.getData('Graph\src\data.json');
      this.createLinks(data);
      this.initializeGraph(data);
    } catch (error) {
      console.error(error);
    }
  }

  async getData(fileName: string) {
    try {
      //const response = await this.http.get(fileName).toPromise();
      const 
      return response;
    } catch (error) {
      console.error(error);
      return null; // or return a default value as per your application's requirements
    }
  }
  

  createLinks(data: any) {
    // Your logic to create links from data
    for (const parent of data.nodes) {
      if (parent.hasOwnProperty('children') && parent.children !== null) {
        for (const child of parent.children) {
          const link = { source: parent.id, target: child.id };
          data.links.push(link);
        }
      }
    }
  }

  initializeGraph(data: any) {
    // Your logic to initialize the graph using ForceGraph3D and set its properties
    let selectedNode = new Set();
    const graphElement = document.getElementById('3d-graph');

    const Graph = ForceGraph3D()(graphElement)
      .graphData(data)
      .nodeLabel('id')
      .linkOpacity(0.3)
      .nodeOpacity(0.95)
      .linkDirectionalArrowRelPos(1)
      .linkDirectionalArrowLength(3.5)
      .linkCurvature(0.15)
      .nodeAutoColorBy('domain')
      .onNodeClick((node: any, event: any) => {
        const untoggle = selectedNode.has(node);
        selectedNode.clear();
        if (!untoggle) {
          selectedNode.add(node);
        }

        const distance = 75;
        const distRatio = 1 + distance / Math.hypot(node.x, node.y, node.z);

        const newPos = node.x || node.y || node.z
          ? { x: node.x * distRatio, y: node.y * distRatio, z: node.z * distRatio }
          : { x: 0, y: 0, z: distance };

        Graph.cameraPosition(newPos, node, 1000);

        sessionStorage.setItem('first', node.id);
      });

    Graph.onNodeDragEnd((node: any) => {
      node.fx = node.x;
      node.fy = node.y;
      node.fz = node.z;
    });
  }
}
