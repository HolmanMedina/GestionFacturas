import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common'; 

import Swal from 'sweetalert2';

import { Cliente } from '../../models/cliente';
import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './clientes.component.html',
  styleUrl: './clientes.component.css'
})
export class ClientesComponent implements OnInit {

  private clienteService = inject(ClienteService);
  constructor(private location: Location) { }

  clientes: Cliente[] = [];

  clientesFiltrados: Cliente[] = [];

  busqueda = '';


  filtroEstado = 'todos';

  mostrarFormulario = false;

  modoEdicion = false;

  clienteEditandoId = '';

  nuevoCliente: Cliente = {
    id: '',
    nombre: '',
    email: '',
    activo: true
  };

  ngOnInit(): void {
    this.cargarClientes();
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
              error?.error?.mensaje ||
              'No se pudieron cargar los clientes.',
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

    this.clientesFiltrados =
      this.clientes.filter(cliente => {

        const coincideTexto =
          !texto ||
          cliente.id
            .toLowerCase()
            .includes(texto) ||
          cliente.nombre
            .toLowerCase()
            .includes(texto) ||
          cliente.email
            .toLowerCase()
            .includes(texto);

        const coincideEstado =
          this.filtroEstado === 'todos' ||
          (
            this.filtroEstado === 'activos' &&
            cliente.activo
          ) ||
          (
            this.filtroEstado === 'inactivos' &&
            !cliente.activo
          );

        return coincideTexto &&
          coincideEstado;
      });
  }

  abrirCrear(): void {

    this.modoEdicion = false;

    this.clienteEditandoId = '';

    this.nuevoCliente = {
      id: '',
      nombre: '',
      email: '',
      activo: true
    };

    this.mostrarFormulario = true;
  }

  abrirEditar(cliente: Cliente): void {

    this.modoEdicion = true;

    this.clienteEditandoId = cliente.id;

    this.nuevoCliente = {
      ...cliente
    };

    this.mostrarFormulario = true;
  }

  cerrarFormulario(): void {

    this.mostrarFormulario = false;

    this.modoEdicion = false;

    this.clienteEditandoId = '';
  }

  guardarCliente(): void {

    if (!this.nuevoCliente.nombre.trim()) {

      Swal.fire({
        title: 'Dato requerido',
        text: 'El nombre del cliente es obligatorio.',
        icon: 'warning'
      });

      return;
    }

    if (!this.nuevoCliente.email.trim()) {

      Swal.fire({
        title: 'Dato requerido',
        text: 'El email del cliente es obligatorio.',
        icon: 'warning'
      });

      return;
    }

    if (this.modoEdicion) {

      this.actualizarCliente();

    } else {

      this.crearCliente();
    }
  }

  crearCliente(): void {

    this.clienteService
      .crear({
        nombre: this.nuevoCliente.nombre,
        email: this.nuevoCliente.email
      })
      .subscribe({

        next: () => {

          Swal.fire({
            title: 'Cliente creado',
            text: 'El cliente fue creado correctamente.',
            icon: 'success',
            confirmButtonText: 'Aceptar'
          });

          this.cerrarFormulario();

          this.cargarClientes();
        },

        error: (error) => {

          console.error(
            'Error al crear cliente:',
            error
          );

          Swal.fire({
            title: 'No se pudo crear',
            text:
              error?.error?.mensaje ||
              'Ocurrió un error al crear el cliente.',
            icon: 'error'
          });
        }
      });
  }

  actualizarCliente(): void {

    this.clienteService
      .actualizar(
        this.clienteEditandoId,
        this.nuevoCliente
      )
      .subscribe({

        next: () => {

          Swal.fire({
            title: 'Cliente actualizado',
            text: 'Los cambios fueron guardados correctamente.',
            icon: 'success'
          });

          this.cerrarFormulario();

          this.cargarClientes();
        },

        error: (error) => {

          console.error(
            'Error al actualizar cliente:',
            error
          );

          Swal.fire({
            title: 'No se pudo actualizar',
            text:
              error?.error?.mensaje ||
              'Ocurrió un error al actualizar el cliente.',
            icon: 'error'
          });
        }
      });
  }

  eliminarCliente(cliente: Cliente): void {

    Swal.fire({
      title: '¿Eliminar cliente?',
      text:
        `Se eliminará a ${cliente.nombre}. Esta acción no se puede deshacer.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
      reverseButtons: true
    }).then(resultado => {

      if (!resultado.isConfirmed) {
        return;
      }

      this.clienteService
        .eliminar(cliente.id)
        .subscribe({

          next: () => {

            Swal.fire({
              title: 'Cliente eliminado',
              text: 'El cliente fue eliminado correctamente.',
              icon: 'success'
            });

            this.cargarClientes();
          },

          error: (error) => {

            Swal.fire({
              title: 'No se pudo eliminar',
              text:
                error?.error?.mensaje ||
                'Ocurrió un error al eliminar el cliente.',
              icon: 'error'
            });
          }
        });
    });
  }

  cambiarEstado(cliente: Cliente): void {

    const clienteActualizado = {
      ...cliente,
      activo: !cliente.activo
    };

    const accion =
      clienteActualizado.activo
        ? 'activar'
        : 'inactivar';

    Swal.fire({
      title: `¿Deseas ${accion} este cliente?`,
      text: `${cliente.nombre}`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Sí, continuar',
      cancelButtonText: 'Cancelar'
    }).then(resultado => {

      if (!resultado.isConfirmed) {
        return;
      }

      this.clienteService
        .actualizar(
          cliente.id,
          clienteActualizado
        )
        .subscribe({

          next: () => {

            Swal.fire({
              title: 'Estado actualizado',
              text: `El cliente fue ${accion}do correctamente.`,
              icon: 'success'
            });

            this.cargarClientes();
          },

          error: (error) => {

            Swal.fire({
              title: 'Error',
              text:
                error?.error?.mensaje ||
                'No se pudo actualizar el estado.',
              icon: 'error'
            });
          }
        });
    });

  }
  volver(): void {
    this.location.back();
  }
}
