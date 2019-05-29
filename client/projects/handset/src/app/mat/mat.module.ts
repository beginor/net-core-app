import { NgModule } from '@angular/core';

import {
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    MatToolbarModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSlideToggleModule,
    MatRadioModule,
    MatCardModule,
    MatSnackBarModule,
    MatGridListModule,
    MatBadgeModule
} from '@angular/material';

@NgModule({
    imports: [
        MatButtonModule,
        MatCheckboxModule,
        MatIconModule,
        MatSidenavModule,
        MatListModule,
        MatToolbarModule,
        MatMenuModule,
        MatFormFieldModule,
        MatInputModule,
        MatSlideToggleModule,
        MatRadioModule,
        MatCardModule,
        MatSnackBarModule,
        MatGridListModule,
        MatBadgeModule
    ],
    exports: [
        MatButtonModule,
        MatCheckboxModule,
        MatIconModule,
        MatSidenavModule,
        MatListModule,
        MatToolbarModule,
        MatMenuModule,
        MatFormFieldModule,
        MatInputModule,
        MatSlideToggleModule,
        MatRadioModule,
        MatCardModule,
        MatSnackBarModule,
        MatGridListModule,
        MatBadgeModule
    ]
  })
export class MatModule { }
