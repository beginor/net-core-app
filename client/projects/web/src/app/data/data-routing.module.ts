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
        path: 'datasources',
        loadChildren: () => import('./datasources/datasources.module').then(m => m.DataSourceModule)
    },
    {
        path: 'dataservices',
        loadChildren: () => import('./dataservices/dataservices.module').then(m => m.DataServiceModule)
    },
    {
        path: 'vectortiles',
        loadChildren: () => import('./vectortiles/vectortiles.module').then(m => m.VectortileModule)
    }
];
// tslint:enable:max-line-length
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class DataRoutingModule { }
