import { Component, input, output } from '@angular/core';
import { ScrollNearEndDirective } from '../../../directives/scroll-near-end.directive';

@Component({
  selector: 'app-infinite-scroll',
  imports: [ScrollNearEndDirective],
  templateUrl: './infinite-scroll.component.html',
  styleUrl: './infinite-scroll.component.scss',
})
export class InfiniteScrollComponent {
  /**
   * If null, nearEnd will fire after debounce time.
   * If not null, nearEnd will only fire when true.
   */
  enable = input<boolean | null>(null);
  appClass = input('');
  debounceTime = input(300);
  nearEnd = output();
}
