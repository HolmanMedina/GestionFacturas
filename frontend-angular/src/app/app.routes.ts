import { Routes } from '@angular/router';

import { InicioComponent } from './pages/inicio/inicio.component';
import { ClientesComponent } from './pages/clientes/clientes.component';
import { FacturasComponent } from './pages/facturas/facturas.component';
import { CrearFacturaComponent } from './pages/crear-factura/crear-factura.component';

export const routes: Routes = [

  {
    path: '',
    component: InicioComponent
  },

  {
    path: 'clientes',
    component: ClientesComponent
  },

  {
    path: 'facturas',
    component: FacturasComponent
  },

  {
    path: 'facturas/nueva',
    component: CrearFacturaComponent
  },

  {
    path: 'facturas/editar/:id',
    component: CrearFacturaComponent
  },


  {
    path: '**',
    redirectTo: ''
  }

];
