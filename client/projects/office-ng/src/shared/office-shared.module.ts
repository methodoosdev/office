import { CommonModule } from "@angular/common";
import { ModuleWithProviders, NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { FormlyModule } from "@ngx-formly/core";

import { KendoSharedModule } from "../api/kendo-shared";
import { EqualValidatorDirective } from "./directives/equal-validator.directive";
import { HasAuthUserViewPermissionDirective } from "./directives/has-auth-user-view-permission.directive";
import { IsVisibleForAuthUserDirective } from "./directives/is-visible-for-auth-user.directive";
import { IgnoreDirtyDirective } from "./directives/ignore-dirty";
import { AfterValueChangedDirective } from "./directives/after-value-changed.directive";

import { SafePipe } from "./pipes/safe.pipe";
import { PermissionPipe } from "./pipes/permission.pipe";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        FormlyModule,
        KendoSharedModule
    ],
    declarations: [
        IsVisibleForAuthUserDirective,
        HasAuthUserViewPermissionDirective,
        EqualValidatorDirective,
        IgnoreDirtyDirective,
        AfterValueChangedDirective,
        // Pipes
        PermissionPipe,
        SafePipe
    ],
    exports: [
        CommonModule,
        FormsModule,
        FormlyModule,
        KendoSharedModule,

        IsVisibleForAuthUserDirective,
        HasAuthUserViewPermissionDirective,
        EqualValidatorDirective,
        IgnoreDirtyDirective,
        AfterValueChangedDirective,
        // Pipes
        PermissionPipe,
        SafePipe
    ]
    /* No providers here! Since they’ll be already provided in AppModule. */
})
export class OfficeSharedModule {
    static forRoot(): ModuleWithProviders<OfficeSharedModule> {
        // Forcing the whole app to use the returned providers from the AppModule only.
        return {
            ngModule: OfficeSharedModule,
            providers: [ /* All of your services here. It will hold the services needed by `itself`. */]
        };
    }
}
