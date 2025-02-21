export class ModelStateErrors {
  errors: ModelStateError[] = [];

  getPropertyErrors(property: string): string[] {
    let errors: string[] = [];

    this.errors.forEach((x) => {
      if (x.property.toLocaleLowerCase() == property.toLocaleLowerCase()) {
        errors.push(...x.descriptions);
      }
    });

    return errors;
  }

  hasError(property: string): boolean {
    return this.getPropertyErrors(property).length > 0;
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
