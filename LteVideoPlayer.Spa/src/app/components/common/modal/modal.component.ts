import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
  input,
  output,
} from '@angular/core';
import {
  NgbModal,
  NgbModalOptions,
  NgbModalRef,
} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal',
  imports: [],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss',
})
export class ModalComponent {
  @Input()
  size: 'sm' | 'md' | 'lg' | 'xl' = 'md';
  @Input()
  fullscreen: boolean | null = null;
  isStatic = input(true);
  showCloseBtn = input(true);
  closeOutput = output({
    alias: 'close',
  });

  isOpen = false;
  modalRef: NgbModalRef | null = null;

  @ViewChild('modal', { static: false })
  private modal: TemplateRef<ModalComponent> | null = null;

  constructor(private modalService: NgbModal) {}

  open(): void {
    this.modalRef = this.modalService.open(this.modal, {
      animation: true,
      backdrop: this.isStatic() ? 'static' : null,
      centered: true,
      keyboard: true,
      scrollable: true,
      size: this.size,
      fullscreen: this.fullscreen,
    } as NgbModalOptions);
    this.isOpen = true;
  }

  close(): void {
    this.onCloseModal();
  }

  protected onCloseModal(): void {
    this.isOpen = false;
    this.modalRef?.dismiss();
    this.closeOutput.emit();
  }
}
