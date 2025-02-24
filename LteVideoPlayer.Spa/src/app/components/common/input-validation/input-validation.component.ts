import { Component, input, Input } from '@angular/core';
import { ModelStateErrors } from '../../../models/ModelStateErrors';

@Component({
  selector: 'app-input-validation',
  imports: [],
  templateUrl: './input-validation.component.html',
  styleUrl: './input-validation.component.scss',
})
export class InputValidationComponent {
  propertyName = input.required<string>();
  @Input() set modelStateErrors(value: ModelStateErrors | null) {
    this._errors = [];
    if (value != null) {
      this._errors = value.getPropertyErrors(this.propertyName());
    }
  }

  protected _errors: string[] = [];
}
