import { TcpService } from './services/tcp.service';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of as observableOf } from 'rxjs';
import { MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material';
import { TransferObject } from './models/transfer-object';

const LOAD_MORE = 'LOAD_MORE';

/** Nested node */
export class LoadmoreNode {
  childrenChange = new BehaviorSubject<LoadmoreNode[]>([]);

  get children(): LoadmoreNode[] {
    return this.childrenChange.value;
  }

  constructor(public item: string,
    public hasChildren = false,
    public loadMoreParentItem: string | null = null) { }
}

/** Flat node with expandable and level information */
export class LoadmoreFlatNode {
  constructor(public item: string,
    public level = 1,
    public expandable = false,
    public loadMoreParentItem: string | null = null) { }
}

/**
 * A database that only load part of the data initially. After user clicks on the `Load more`
 * button, more data will be loaded.
 */
@Injectable()
export class LoadmoreDatabase {
  batchNumber = 5;
  dataChange = new BehaviorSubject<LoadmoreNode[]>([]);
  nodeMap = new Map<string, LoadmoreNode>();

  /** The data */
  rootLevelNodes: string[] = ['DBstud', 'DBtest'];
  dataMap = new Map<string, string[]>([
    // ['Fruits', ['Apple', 'Orange', 'Banana']],
    // ['Vegetables', ['Tomato', 'Potato', 'Onion']],
    // ['Apple', ['Fuji', 'Macintosh']],
    // ['Onion', ['Yellow', 'White', 'Purple', 'Green', 'Shallot', 'Sweet', 'Red', 'Leek']],
  ]);

  initialize() {
    const data = this.rootLevelNodes.map(name => this._generateNode(name));
    this.dataChange.next(data);
  }

  /** Expand a node whose children are not loaded */
  loadMore(item: string, onlyFirstTime = false) {
    if (!this.nodeMap.has(item) || !this.dataMap.has(item)) {
      return;
    }
    const parent = this.nodeMap.get(item)!;
    const children = this.dataMap.get(item)!;
    if (onlyFirstTime && parent.children!.length > 0) {
      return;
    }
    const newChildrenNumber = parent.children!.length + this.batchNumber;
    const nodes = children.slice(0, newChildrenNumber)
      .map(name => this._generateNode(name));
    if (newChildrenNumber < children.length) {
      // Need a new load more node
      nodes.push(new LoadmoreNode(LOAD_MORE, false, item));
    }

    parent.childrenChange.next(nodes);
    this.dataChange.next(this.dataChange.value);
  }

  private _generateNode(item: string): LoadmoreNode {
    if (this.nodeMap.has(item)) {
      return this.nodeMap.get(item)!;
    }
    const result = new LoadmoreNode(item, this.dataMap.has(item));
    this.nodeMap.set(item, result);
    return result;
  }
}



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [LoadmoreDatabase]
})

export class AppComponent {
  title = 'frontApp';
  message = '/info';

  nodeMap = new Map<string, LoadmoreFlatNode>();
  treeControl: FlatTreeControl<LoadmoreFlatNode>;
  treeFlattener: MatTreeFlattener<LoadmoreNode, LoadmoreFlatNode>;
  // Flat tree data source
  dataSource: MatTreeFlatDataSource<LoadmoreNode, LoadmoreFlatNode>;

  constructor(
    private tcpService: TcpService,
    private database: LoadmoreDatabase
  ) {
  }

  getChildren = (node: LoadmoreNode): Observable<LoadmoreNode[]> => node.childrenChange;

  transformer = (node: LoadmoreNode, level: number) => {
    const existingNode = this.nodeMap.get(node.item);

    if (existingNode) {
      return existingNode;
    }

    const newNode =
      new LoadmoreFlatNode(node.item, level, node.hasChildren, node.loadMoreParentItem);
    this.nodeMap.set(node.item, newNode);
    return newNode;
  }

  getLevel = (node: LoadmoreFlatNode) => node.level;

  isExpandable = (node: LoadmoreFlatNode) => node.expandable;

  hasChild = (_: number, _nodeData: LoadmoreFlatNode) => _nodeData.expandable;

  isLoadMore = (_: number, _nodeData: LoadmoreFlatNode) => _nodeData.item === LOAD_MORE;

  /** Load more nodes from data source */
  loadMore(item: string) {
    this.database.loadMore(item);
  }

  loadChildren(node: LoadmoreFlatNode) {
    this.database.loadMore(node.item, true);
  }


  run() {
    // var t = this.tcpService.sendMessage('/show databases');
    this.tcpService.sendMessagePromise('/show databases').then(res => {
      //  let t = res.tos;
      const dataObject: TransferObject = JSON.parse(`${res}`);
      // const dataObject: TransferObject = res;
      this.database.dataMap = new Map<string, string[]>(JSON.parse(dataObject.Data.Message));
      this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
        this.isExpandable, this.getChildren);

      this.treeControl = new FlatTreeControl<LoadmoreFlatNode>(this.getLevel, this.isExpandable);

      this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

      this.database.dataChange.subscribe(data => {
        this.dataSource.data = data;
      });

      this.database.initialize();
    });
    // this.database.dataMap = res;//this.tcpService.sendMessage('/show databases').Data.Message;
    // this.treeFlattener = new MatTreeFlattener(this.transformer, this.getLevel,
    //   this.isExpandable, this.getChildren);

    // this.treeControl = new FlatTreeControl<LoadmoreFlatNode>(this.getLevel, this.isExpandable);

    // this.dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

    // this.database.dataChange.subscribe(data => {
    //   this.dataSource.data = data;
    // });

    // this.database.initialize();
    //  this.tcpService.sendMessage(this.message);
    // })

  }

}
