import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { _MatMenuDirectivesModule, MatMenuModule } from '@angular/material/menu';
import { MatSliderModule } from '@angular/material/slider';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule } from '@angular/material/tabs';
import { NgxPermissionsModule } from 'ngx-permissions';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ErrorDialogComponent } from './errors/error-dialog/error-dialog.component';
// import {LoadingDialogComponent} from './loading/loading-dialog/loading-dialog.component';
// import {LoadingDialogService} from './loading/loading-dialog.service';
import { MatDialogModule } from '@angular/material/dialog';
// import {ErrorHandlerModule} from '../../core/errors/error-handler.module';
import { DialogInjectableComponent } from '../../components/dialog-injectable/dialog-injectable.component';
import { EventService } from '../../services/event.service';
import { FormDialogComponent } from './forms/form-dialog/form-dialog.component';
import { FormDialogService } from './forms/form-dialog.service';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSortModule } from '@angular/material/sort';

@NgModule({
  declarations: [
    ErrorDialogComponent,
    DialogInjectableComponent,
    FormDialogComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    NgxPermissionsModule,
    _MatMenuDirectivesModule,
    MatSliderModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatRadioModule,
    MatCardModule,
    MatCheckboxModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    MatMenuModule,
    MatAutocompleteModule,
    MatIconModule,
    MatTooltipModule,
    MatTabsModule,
    ReactiveFormsModule,
    MatTableModule,
    MatGridListModule,
    FormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatButtonToggleModule,
    MatSortModule
  ],
  exports: [
    NgxPermissionsModule,
    _MatMenuDirectivesModule,
    MatSliderModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatRadioModule,
    MatCardModule,
    MatCheckboxModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    MatMenuModule,
    MatAutocompleteModule,
    MatIconModule,
    MatTooltipModule,
    MatTabsModule,
    FormsModule,
    MatTableModule,
    MatGridListModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule,
    MatPaginatorModule,
    MatButtonToggleModule,
    MatSortModule
  ],
  providers: [EventService, FormDialogService]
})
export class SharedModule {
}
