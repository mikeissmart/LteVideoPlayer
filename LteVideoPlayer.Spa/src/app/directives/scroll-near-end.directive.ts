import {
  Directive,
  ElementRef,
  HostListener,
  inject,
  Input,
  input,
  OnDestroy,
  OnInit,
  output,
} from '@angular/core';

@Directive({
  selector: '[appScrollNearEnd]',
})
export class ScrollNearEndDirective implements OnInit {
  el = inject(ElementRef);
  /**
   * threshold in PX when to emit before page end scroll
   */
  thresholdPx = input(500);
  debounceTime = input(300);
  manualEnable = input<boolean | null>();
  /**
   * This might emit multiple times. Disabled after each emit and must be re-enable emit.
   */
  nearEnd = output<void>();

  private _enabled = false;
  private window!: Window;

  constructor() {}

  ngOnInit(): void {
    // save window object for type safety
    this.window = window;
  }

  @HostListener('window:scroll', ['$event.target'])
  windowScrollEvent(event: KeyboardEvent) {
    // height of whole window page
    const heightOfWholePage = this.window.document.documentElement.scrollHeight;

    // how big in pixels the element is
    const heightOfElement = this.el.nativeElement.scrollHeight;

    // currently scrolled Y position
    const currentScrolledY = this.window.scrollY;

    // height of opened window - shrinks if console is opened
    const innerHeight = this.window.innerHeight;

    /**
     * the area between the start of the page and when this element is visible
     * in the parent component
     */
    const spaceOfElementAndPage = heightOfWholePage - heightOfElement;

    // calculated whether we are near the end
    const scrollToBottom =
      heightOfElement - innerHeight - currentScrolledY + spaceOfElementAndPage;

    // if the user is near end
    if (scrollToBottom < this.thresholdPx() && heightOfElement > 0) {
      if (this.manualEnable() == null) {
        this.nearEnd.emit();
        this._enabled = false;
        setTimeout(() => {
          this._enabled = true;
        }, this.debounceTime());
      } else if (this.manualEnable()) {
        this.nearEnd.emit();
      }
    }
  }
}
