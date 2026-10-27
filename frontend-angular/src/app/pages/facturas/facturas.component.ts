import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';

import { Cliente } from '../../models/cliente';
import { Factura } from '../../models/factura';

import { ClienteService } from '../../services/cliente.service';
import { FacturaService } from '../../services/factura.service';

@Component({
  selector: 'app-facturas',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './facturas.component.html',
  styleUrl: './facturas.component.css'
})
export class FacturasComponent implements OnInit {

  private facturaService = inject(FacturaService);

  private clienteService = inject(ClienteService);

  private router = inject(Router);

  constructor(
    private location: Location
  ) { }

  facturas: Factura[] = [];

  facturasFiltradas: Factura[] = [];

  clientes: Cliente[] = [];

  busqueda = '';

  filtroCliente = '';

  filtroEstado = '';

  ngOnInit(): void {

    this.cargarClientes();

    this.cargarFacturas();

  }

  cargarClientes(): void {

    this.clienteService
      .obtenerTodos()
      .subscribe({

        next: (clientes) => {

          this.clientes = clientes;

          this.aplicarFiltros();

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

  cargarFacturas(): void {

    this.facturaService
      .obtenerTodas()
      .subscribe({

        next: (facturas) => {

          this.facturas = facturas;

          this.aplicarFiltros();

        },

        error: (error) => {

          console.error(
            'Error al cargar facturas:',
            error
          );

          Swal.fire({
            title: 'Error',
            text:
              'No se pudieron cargar las facturas.',
            icon: 'error'
          });

        }

      });

  }

  aplicarFiltros(): void {

    const texto =
      this.busqueda
        .toLowerCase()
        .trim();

    this.facturasFiltradas =
      this.facturas.filter(factura => {

        const coincideBusqueda =
          !texto ||
          factura.id
            .toLowerCase()
            .includes(texto) ||

          factura.numeroFactura
            .toLowerCase()
            .includes(texto) ||

          factura.descripcion
            .toLowerCase()
            .includes(texto);


        const coincideCliente =
          !this.filtroCliente ||
          factura.clienteId ===
          this.filtroCliente;


        const coincideEstado =
          !this.filtroEstado ||
          this.obtenerEstadoTexto(
            factura.estado
          ) === this.filtroEstado;


        return (
          coincideBusqueda &&
          coincideCliente &&
          coincideEstado
        );

      });

  }

  obtenerNombreCliente(
    clienteId: string
  ): string {

    const cliente =
      this.clientes.find(
        cliente =>
          cliente.id === clienteId
      );

    return cliente
      ? cliente.nombre
      : 'Cliente no encontrado';

  }

  obtenerEstadoTexto(
    estado: number
  ): string {

    switch (estado) {

      case 0:
        return 'Pendiente';

      case 1:
        return 'Primer recordatorio';

      case 2:
        return 'Segundo recordatorio';

      case 3:
        return 'Desactivado';


      default:
        return 'Desconocido';

    }

  }

  claseEstado(
    estado: number
  ): string {

    switch (estado) {

      case 0:
        return 'pendiente';

      case 1:
        return 'primer';

      case 2:
        return 'segundo';

      case 3:
        return 'desactivado';

      default:
        return '';

    }

  }

  puedeProcesar(factura: Factura): boolean {
    if (factura.estado === 1 || factura.estado === 2) {
      return true;
    }

    if (factura.estado === 0) {
      const fechaVencimiento = new Date(factura.fechaVencimiento);
      const hoy = new Date();

      hoy.setHours(0, 0, 0, 0);
      fechaVencimiento.setHours(0, 0, 0, 0);

      return fechaVencimiento < hoy;
    }

    return false;
  }

  nuevaFactura(): void {

    this.router.navigate([
      '/facturas/nueva'
    ]);

  }

  editarFactura(
    factura: Factura
  ): void {

    this.router.navigate([
      '/facturas/editar',
      factura.id
    ]);

  }

  procesarRecordatorio(
    factura: Factura
  ): void {

    Swal.fire({

      title: '¿Procesar recordatorio?',

      text:
        `Se procesará ${factura.numeroFactura} y se enviará el correo correspondiente.`,

      icon: 'question',

      showCancelButton: true,

      confirmButtonText:
        'Sí, procesar',

      cancelButtonText:
        'Cancelar'

    }).then(resultado => {

      if (!resultado.isConfirmed) {
        return;
      }


      this.facturaService
        .procesarRecordatorioIndividual(
          factura.id
        )
        .subscribe({

          next: (respuesta) => {

            Swal.fire({

              title:
                'Recordatorio procesado',

              text:
                respuesta?.mensaje ||
                'El recordatorio fue procesado correctamente.',

              icon:
                'success'

            });

            this.cargarFacturas();

          },


          error: (error) => {

            console.error(
              'Error al procesar recordatorio:',
              error
            );

            Swal.fire({

              title:
                'No se pudo procesar',

              text:
                error?.error?.mensaje ||
                'Ocurrió un error.',

              icon:
                'error'

            });

          }

        });

    });

  }

  procesarTodos(): void {

    const cantidad =
      this.facturas.filter(
        factura =>
          factura.estado === 1 ||
          factura.estado === 2
      ).length;


    if (cantidad === 0) {

      Swal.fire({

        title:
          'Sin recordatorios',

        text:
          'No hay facturas pendientes de procesamiento.',

        icon:
          'info'

      });

      return;

    }


    Swal.fire({

      title:
        '¿Procesar recordatorios?',

      text:
        `Se procesarán ${cantidad} facturas.`,

      icon:
        'question',

      showCancelButton:
        true,

      confirmButtonText:
        'Sí, procesar todos',

      cancelButtonText:
        'Cancelar'

    }).then(resultado => {

      if (!resultado.isConfirmed) {
        return;
      }


      this.facturaService
        .procesarRecordatorios()
        .subscribe({

          next: (respuesta) => {

            Swal.fire({

              title:
                'Proceso completado',

              text:
                respuesta?.mensaje ||
                'Los recordatorios fueron procesados.',

              icon:
                'success'

            });

            this.cargarFacturas();

          },


          error: (error) => {

            console.error(
              'Error al procesar todos:',
              error
            );

            Swal.fire({

              title:
                'Error',

              text:
                error?.error?.mensaje ||
                'No se pudieron procesar los recordatorios.',

              icon:
                'error'

            });

          }

        });

    });

  }

  eliminarFactura(
    factura: Factura
  ): void {

    Swal.fire({

      title:
        '¿Eliminar factura?',

      text:
        `Se eliminará ${factura.numeroFactura}. Esta acción no se puede deshacer.`,

      icon:
        'warning',

      showCancelButton:
        true,

      confirmButtonText:
        'Sí, eliminar',

      cancelButtonText:
        'Cancelar',

      reverseButtons:
        true

    }).then(resultado => {

      if (!resultado.isConfirmed) {
        return;
      }


      this.facturaService
        .eliminar(factura.id)
        .subscribe({

          next: () => {

            Swal.fire({

              title:
                'Factura eliminada',

              text:
                'La factura fue eliminada correctamente.',

              icon:
                'success'

            });

            this.cargarFacturas();

          },


          error: (error) => {

            console.error(
              'Error al eliminar factura:',
              error
            );

            Swal.fire({

              title:
                'Error',

              text:
                error?.error?.mensaje ||
                'No se pudo eliminar la factura.',

              icon:
                'error'

            });

          }

        });

    });

  }

  limpiarFiltros(): void {

    this.busqueda = '';

    this.filtroCliente = '';

    this.filtroEstado = '';

    this.aplicarFiltros();

  }

  volver(): void {

    this.router.navigate([
      '/'
    ]);

  }

}
