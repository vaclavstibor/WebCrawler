Main parts Components + Templates

Template is basically html stuff with some typescript stuff such as: 
    *ngIf
    *ngFor
    {{}} interpolation
    event binding (click)="..."
    prop biding <img [src]="...">
    two-way binding <input [(ngModel)]="...">
    functions

The component fills this into the template 

There can be urlTemplate or just template in the decorator(the thing comming before component)

import component
export component

Module = Here we specify all components - custom or extern

Component Lifecycle Hooks

OnInit: Initialization - retrieve data

OnChanges: Action after change to input properties

OnDestroy: Cleanup



Encapsulate custom css with styleUrls in the decorator



Custom Pipe - add to same Module as The Component where we need it



Getter/Setter - use when we want to execute code after get/set

JavaSciprt `` backdashes can use interpolatedString

If I want to pass data to a component, I need to do it with binding inside a template where the component is used. This data needs to have @Input() before its name

On the other hand if I want to put the data outside I need to emit an event. This event needs to first be defined using Output() and its type needs to be EventEmitter<Type>

However if I want to use this emitted data in the template and pass it to the component than I need to pass it via an event of the component

Service - 

Inject with Injector - available throughout the application

Inject with Component Injector - available only to the component and its children(nested) components, Isolation, Multiple instances of the service

Use Services for dependancy Injection - assign in the constructor

assign data in onInit
