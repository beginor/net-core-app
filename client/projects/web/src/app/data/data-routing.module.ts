import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DashbordComponent } from './dashbord/dashbord.component';


// tslint:disable:max-line-length
const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: DashbordComponent },
    {
        path: 'slpks',
        loadChildren: () => import('./slpks/slpks.module').then(m => m.SlpkModule)
    },
    {
        path: 'tile-maps',
        loadChildren: () => import('./tile-maps/tile-maps.module').then(m => m.TileMapModule)
    }
];
// tslint:enable:max-line-length
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DataRoutingModule { }
