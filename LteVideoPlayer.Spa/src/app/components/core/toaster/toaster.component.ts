import { Component, inject } from '@angular/core';
import { NgbToast } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { ToasterService } from '../../../services/toaster/toaster.service';

@Component({
  selector: 'app-toaster',
  imports: [NgbToast, CommonModule],
  templateUrl: './toaster.component.html',
  styleUrl: './toaster.component.scss',
})
export class ToasterComponent {
  toasterService = inject(ToasterService);
}
