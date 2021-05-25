import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { IframeComponent } from '../common';

import { AboutComponent } from './about/about.component';

const routes: Routes = [
    {
        path: '',
        component: AboutComponent
    },
    {
        path: ':src',
        component: IframeComponent,
        canLoad: []
    }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AboutRoutingModule { }
