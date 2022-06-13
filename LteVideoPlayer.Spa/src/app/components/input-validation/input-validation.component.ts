import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ModelStateErrors } from 'src/app/services/http/ModelStateErrors';

@Component({
  selector: 'app-input-validation',
  templateUrl: './input-validation.component.html',
  styleUrls: ['./input-validation.component.scss'],
})
/**
 * @description
 * Adds ModelStateErrors boilerplate code around inside content.
 *
 * @usageNotes
 * Property and id of input element must match for proper errors to show.
 *
 */
export class InputValidationComponent implements OnInit {
  errors: string[] = [];

  @ViewChild('content', { static: false })
  content: ElementRef | null = null;

  @Input()
  property: string = '';
  @Input()
  set validations(stateErrors: ModelStateErrors | null) {
    this.errors = ModelStateErrors.getPropertyErrors(
      this.property,
      stateErrors
    );
    if (this.content != null) {
      const propertyInput = this.content.nativeElement.querySelector(
        `#${this.property}`
      );

      if (propertyInput != null) {
        propertyInput.classList.remove('is-invalid');
        propertyInput.classList.remove('is-valid');
        if (this.errors.length > 0) {
          propertyInput.classList.add('is-invalid');
        } else {
          propertyInput.classList.add('is-valid');
        }
      }
    }
  }

  constructor() {}

  ngOnInit(): void {}
}
