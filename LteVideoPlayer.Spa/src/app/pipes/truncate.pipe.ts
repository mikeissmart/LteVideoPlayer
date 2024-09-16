import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate',
})
export class TruncatePipe implements PipeTransform {
  /**
   * {{ str | truncate:[20] }}
   * {{ str | truncate:[20, '...'] }}
   * @param value
   * @param args
   * @returns
   */
  transform(
    value: string | undefined,
    limit = 25,
    completeWords = false,
    ellipsis = '...'
  ) {
    if (value == undefined) {
      return '';
    }

    if (value.length <= limit) {
      return value;
    }

    if (completeWords) {
      limit = value.substring(0, limit).lastIndexOf(' ');
    }
    return value.length > limit ? value.substring(0, limit) + ellipsis : value;
  }
}