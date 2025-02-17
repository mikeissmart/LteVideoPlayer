import { Pipe, PipeTransform } from '@angular/core';
import { KeyValuePair } from '../models/view-model';

@Pipe({
  name: 'enumToArray',
})
export class EnumToArrayPipe implements PipeTransform {
  transform(data: object): KeyValuePair[] {
    const values = Object.keys(data).slice(Object.keys(data).length / 2);
    const keys = Object.keys(data).slice(0, Object.keys(data).length / 2);
    const pairs = [] as KeyValuePair[];

    for (let i = 0; i < keys.length; i++) {
      pairs[i] = new KeyValuePair(Number.parseInt(keys[i]), values[i]);
    }

    return pairs;
  }
}
