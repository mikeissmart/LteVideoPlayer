export class KeyValuePair {
  public id: number;
  public name: string;

  public constructor(id: number, name: string) {
    this.id = id;
    this.name = name;
  }
}
export class TableHeader {
  public name: string;
  public orderBy: string | undefined;

  public constructor(name: string, orderBy: string | undefined = undefined) {
    this.name = name;
    this.orderBy = orderBy;
  }
}
