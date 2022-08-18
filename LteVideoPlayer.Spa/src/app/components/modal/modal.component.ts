import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import {
  NgbModal,
  NgbModalOptions,
  NgbModalRef,
} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.scss'],
})
export class ModalComponent implements OnInit {
  modalRef: NgbModalRef | null = null;

  @ViewChild('modal', { static: false })
  private modal: TemplateRef<ModalComponent> | null = null;

  @Input()
  showCloseBtn = true;
  @Input()
  size: 'sm' | 'md' | 'lg' | 'xl' = 'md';
  @Input()
  fullscreen: 'sm' | 'md' | 'lg' | 'xl' | 'xxl' | boolean | null = null;
  @Output()
  onClose = new EventEmitter();

  constructor(private modalService: NgbModal) {}

  ngOnInit(): void {}

  public openModal(): void {
    this.modalService.dismissAll();
    this.modalRef = this.modalService.open(this.modal, {
      animation: true,
      backdrop: 'static',
      centered: true,
      keyboard: true,
      scrollable: true,
      size: this.size,
      fullscreen: this.fullscreen,
    } as NgbModalOptions);
  }

  closeModal(): void {
    this.modalService.dismissAll();
    this.onClose.emit();
  }
}
