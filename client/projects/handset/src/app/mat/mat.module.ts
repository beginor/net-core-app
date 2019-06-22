import { NgModule } from '@angular/core';

import { A11yModule } from '@angular/cdk/a11y';

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
        A11yModule,
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
        A11yModule,
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
