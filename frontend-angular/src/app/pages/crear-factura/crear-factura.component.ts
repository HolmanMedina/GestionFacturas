import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import Swal from 'sweetalert2';

import { Cliente } from '../../models/cliente';
import { Factura } from '../../models/factura';

import { ClienteService } from '../../services/cliente.service';
import { FacturaService } from '../../services/factura.service';

@Component({
  selector: 'app-crear-factura',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './crear-factura.component.html',
  styleUrl: './crear-factura.component.css'
})
export class CrearFacturaComponent
  implements OnInit {


  private clienteService =
    inject(ClienteService);

  private facturaService =
    inject(FacturaService);

  private router =
    inject(Router);

  private route =
    inject(ActivatedRoute);

  clientes: Cliente[] = [];

  modoEdicion = false;

  facturaId = '';

  factura: Factura = {
    id: '',
    clienteId: '',
    numeroFactura: '',
    descripcion: '',
    valor: 0,
    fechaVencimiento: '',
    estado: 0
  };

  volver(): void {
    this.router.navigate(['/facturas']);
  }

  ngOnInit(): void {

    this.cargarClientes();

    this.facturaId =
      this.route.snapshot.paramMap.get('id') || '';

    if (this.facturaId) {

      this.modoEdicion = true;

      this.cargarFactura();
    }
  }

  cargarClientes(): void {

    this.clienteService
      .obtenerTodos()
      .subscribe({

        next: (clientes) => {

          this.clientes = clientes;
        },

        error: (error) => {

          console.error(
            'Error al cargar clientes:',
            error
          );

          Swal.fire({
            title: 'Error',
            text:
              'No se pudieron cargar los clientes.',
            icon: 'error'
          });
        }
      });
  }

  cargarFactura(): void {

    this.facturaService
      .obtenerPorId(this.facturaId)
      .subscribe({

        next: (factura) => {

          this.factura = {
            ...factura,
            fechaVencimiento:
              factura.fechaVencimiento
                .substring(0, 10)
          };
        },

        error: () => {

          Swal.fire({
            title: 'Error',
            text:
              'No se pudo cargar la factura.',
            icon: 'error'
          }).then(() => {

            this.router.navigate([
              '/facturas'
            ]);
          });
        }
      });
  }

  guardar(): void {

    if (!this.factura.clienteId) {

      Swal.fire({
        title: 'Cliente requerido',
        text:
          'Debes seleccionar un cliente.',
        icon: 'warning'
      });

      return;
    }

    if (!this.factura.numeroFactura.trim()) {

      Swal.fire({
        title: 'Número requerido',
        text:
          'Debes ingresar el número de factura.',
        icon: 'warning'
      });

      return;
    }

    if (!this.factura.descripcion.trim()) {

      Swal.fire({
        title: 'Descripción requerida',
        text:
          'Debes ingresar una descripción.',
        icon: 'warning'
      });

      return;
    }

    if (this.factura.valor <= 0) {

      Swal.fire({
        title: 'Valor inválido',
        text:
          'El valor debe ser mayor que cero.',
        icon: 'warning'
      });

      return;
    }

    if (!this.factura.fechaVencimiento) {

      Swal.fire({
        title: 'Fecha requerida',
        text:
          'Debes seleccionar la fecha de vencimiento.',
        icon: 'warning'
      });

      return;
    }

    if (this.modoEdicion) {

      this.actualizar();

    } else {

      this.crear();
    }
  }

  crear(): void {

    this.facturaService
      .crear({
        clienteId:
          this.factura.clienteId,

        numeroFactura:
          this.factura.numeroFactura,

        descripcion:
          this.factura.descripcion,

        valor:
          this.factura.valor,

        fechaVencimiento:
          this.factura.fechaVencimiento
      })
      .subscribe({

        next: () => {

          Swal.fire({
            title: 'Factura creada',
            text:
              'La factura fue creada correctamente.',
            icon: 'success'
          }).then(() => {

            this.router.navigate([
              '/facturas'
            ]);
          });
        },

        error: (error) => {

          Swal.fire({
            title: 'No se pudo crear',
            text:
              error?.error?.mensaje ||
              'Ocurrió un error.',
            icon: 'error'
          });
        }
      });
  }

  actualizar(): void {

    this.facturaService
      .actualizar(
        this.facturaId,
        this.factura
      )
      .subscribe({

        next: () => {

          Swal.fire({
            title: 'Factura actualizada',
            text:
              'Los cambios fueron guardados correctamente.',
            icon: 'success'
          }).then(() => {

            this.router.navigate([
              '/facturas'
            ]);
          });
        },

        error: (error) => {

          Swal.fire({
            title: 'No se pudo actualizar',
            text:
              error?.error?.mensaje ||
              'Ocurrió un error.',
            icon: 'error'
          });
        }
      });
  }

  cancelar(): void {

    this.router.navigate([
      '/facturas'
    ]);
  }
}
