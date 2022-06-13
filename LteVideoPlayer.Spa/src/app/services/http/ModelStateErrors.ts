export class ModelStateErrors {
  errors: ModelStateError[] = [];

  static getPropertyErrors(
    property: string,
    stateErrors: ModelStateErrors | null
  ): string[] {
    let errors: string[] = [];

    if (stateErrors != null) {
      stateErrors.errors.forEach((x) => {
        if (x.property.toLocaleLowerCase() == property.toLocaleLowerCase()) {
          errors.push(...x.descriptions);
        }
      });
    }

    return errors;
  }

  static hasError(
    property: string,
    stateErrors: ModelStateErrors | null
  ): boolean {
    if (stateErrors == null) {
      return false;
    }

    return this.getPropertyErrors(property, stateErrors).length > 0;
  }

  static convertErrors(error: any): ModelStateErrors {
    if (error.errors !== undefined) {
      return this.convertErrors(error.errors);
    }

    const modelErrors = new ModelStateErrors();
    Object.getOwnPropertyNames(error).forEach((x) => {
      if (x != '$id') {
        const modelError = new ModelStateError();
        modelError.property = x;
        error[x].forEach((y: string) => {
          modelError.descriptions.push(y);
        });
        modelErrors.errors.push(modelError);
      }
    });
    return modelErrors;
  }
}

export class ModelStateError {
  property: string = '';
  descriptions: string[] = [];
}
