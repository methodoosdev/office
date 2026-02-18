import { FormlyFieldProps as CoreFormlyFieldProps } from '@ngx-formly/core';
import { Orientation } from '@progress/kendo-angular-inputs';

export interface BaseFormlyFieldProps extends CoreFormlyFieldProps {
    hideRequiredMarker?: boolean;
    hideLabel?: boolean;
    orientation?: Orientation;
    markAsRequired?: boolean;
}