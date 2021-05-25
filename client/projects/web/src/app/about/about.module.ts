import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AppSharedModule } from 'app-shared';

import { AboutRoutingModule } from './about-routing.module';
import { AboutComponent } from './about/about.component';

@NgModule({
  declarations: [
    AboutComponent
  ],
  imports: [
    CommonModule,
    AppSharedModule,
    AboutRoutingModule
  ]
})
export class AboutModule { }
