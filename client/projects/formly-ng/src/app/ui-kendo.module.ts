import { NgModule } from '@angular/core';

import { FormlyAutocompleteModule } from '../autocomplete/autocomplete.module';
import { FormlyButtonModule } from '../button/button.module';
import { FormlyCheckboxModule } from '../checkbox/checkbox.module';
import { FormlyDateModule } from '../date/date.module';
import { FormlyFormFieldModule } from '../form-field/form-field.module';
import { FormlyGridSelectModule } from '../grid-select/grid-select.module';
import { FormlyInputModule } from '../input/input.module';
import { FormlyMultiColumnComboBoxModule } from '../multicolumncombobox/multicolumncombobox.module';
import { FormlyMultiSelectModule } from '../multiselect/multiselect.module';
import { FormlyMultiSelectAllModule } from '../multiSelectAll/multiSelectAll.module';
import { FormlyMultiSelectTreeModule } from '../multiselecttree/multiselecttree.module';
import { FormlyNumericModule } from '../numeric/numeric.module';
import { FormlyProgressBarModule } from '../progressBar/progressBar.module';
import { FormlyRadioModule } from '../radio/radio.module';
import { FormlySelectModule } from '../select/select.module';
import { FormlyTextModule } from '../text/text.module';
import { FormlyTextButtonModule } from '../text-button/text-button.module';
import { FormlyTextAreaModule } from '../textarea/textarea.module';
import { FormlyTextareaButtonModule } from '../textarea-button/textarea-button.module';
import { FormlyTimeModule } from '../time/time.module';
import { FormlySwitchModule } from '../switch/switch.module';
import { FormlyEditorModule } from '../editor/editor.module';
import { FormlyLocaleTabsModule } from '../locale-tabs/locale-tabs.module';
import { FormlyHtmlEditorModule } from '../htmleditor/htmleditor.module';
import { FormlyGridGroupSelectModule } from '../grid-group-select/grid-group-select.module';
import { FormlyRepeatSectionModule } from '../repeat-section/repeat-section.module';

@NgModule({
    imports: [
        FormlyAutocompleteModule,
        FormlyFormFieldModule,
        FormlyInputModule,
        FormlyTextAreaModule,
        FormlyTextareaButtonModule,
        FormlyRadioModule,
        FormlyCheckboxModule,
        FormlySelectModule,
        FormlyDateModule,
        FormlyTimeModule,
        FormlyButtonModule,
        FormlyMultiColumnComboBoxModule,
        FormlyMultiSelectModule,
        FormlyMultiSelectAllModule,
        FormlyMultiSelectTreeModule,
        FormlyNumericModule,
        FormlyProgressBarModule,
        FormlyTextModule,
        FormlyTextButtonModule,
        FormlyGridGroupSelectModule,
        FormlyGridSelectModule,
        FormlySwitchModule,
        FormlyEditorModule,
        FormlyLocaleTabsModule,
        FormlyHtmlEditorModule,
        FormlyRepeatSectionModule
    ],
})
export class FormlyKendoModule { }
