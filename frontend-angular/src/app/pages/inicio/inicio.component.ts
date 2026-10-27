import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { Cliente } from '../../models/cliente';
import { Factura } from '../../models/factura';

import { ClienteService } from '../../services/cliente.service';
import { FacturaService } from '../../services/factura.service';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './inicio.component.html',
  styleUrls: ['./inicio.component.css']
})
export class InicioComponent implements OnInit {

  private clienteService = inject(ClienteService);
  private facturaService = inject(FacturaService);
  private router = inject(Router);

  clientes: Cliente[] = [];
  facturas: Factura[] = [];

  cargando = true;

  totalClientes = 0;
  totalFacturas = 0;
  facturasPendientes = 0;
  facturasRecordatorio = 0;
  facturasDesactivadas = 0;

  ngOnInit(): void {
    this.cargarInformacion();
  }

  cargarInformacion(): void {

    this.cargando = true;

    this.clienteService.obtenerTodos().subscribe({
      next: (clientes) => {
        this.clientes = clientes;
        this.totalClientes = clientes.length;

        this.cargarFacturas();
      },
      error: (error) => {
        console.error('Error al cargar clientes:', error);
        this.cargando = false;
      }
    });
  }

  cargarFacturas(): void {

    this.facturaService.obtenerTodas().subscribe({
      next: (facturas) => {

        this.facturas = facturas;

        this.totalFacturas = facturas.length;

        this.facturasPendientes = facturas.filter(
          factura => factura.estado === 0
        ).length;

        this.facturasRecordatorio = facturas.filter(
          factura =>
            factura.estado === 1 ||
            factura.estado === 2
        ).length;

        this.facturasDesactivadas = facturas.filter(
          factura => factura.estado === 3
        ).length;

        this.cargando = false;
      },

      error: (error) => {
        console.error('Error al cargar facturas:', error);
        this.cargando = false;
      }
    });
  }

  irAClientes(): void {
    this.router.navigate(['/clientes']);
  }

  irAFacturas(): void {
    this.router.navigate(['/facturas']);
  }

  irACrearCliente(): void {
    this.router.navigate(['/clientes']);
  }

  irACrearFactura(): void {
    this.router.navigate(['/facturas/nueva']);
  }
}
