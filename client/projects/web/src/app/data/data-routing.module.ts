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
        path: 'tilemaps',
        loadChildren: () => import('./tilemaps/tilemaps.module').then(m => m.TileMapModule)
    },
    {
        path: 'connections',
        loadChildren: () => import('./connections/connections.module').then(m => m.ConnectionModule)
    },
    {
        path: 'datasources',
        loadChildren: () => import('./datasources/datasources.module').then(m => m.DataSourceModule)
    }
];
// tslint:enable:max-line-length
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DataRoutingModule { }
