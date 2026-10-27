import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Factura } from '../models/factura';

@Injectable({
  providedIn: 'root'
})
export class FacturaService {

  private http = inject(HttpClient);

  private apiUrl =
    'http://localhost:5167/api/Facturas';

  obtenerTodas(): Observable<Factura[]> {
    return this.http.get<Factura[]>(
      this.apiUrl
    );
  }

  obtenerPorId(
    id: string
  ): Observable<Factura> {

    return this.http.get<Factura>(
      `${this.apiUrl}/${id}`
    );
  }

  obtenerPorCliente(
    clienteId: string
  ): Observable<Factura[]> {

    return this.http.get<Factura[]>(
      `${this.apiUrl}/cliente/${clienteId}`
    );
  }

  crear(factura: {
    clienteId: string;
    numeroFactura: string;
    descripcion: string;
    valor: number;
    fechaVencimiento: string;
  }): Observable<any> {

    return this.http.post(
      this.apiUrl,
      factura
    );
  }

  actualizar(
    id: string,
    factura: Factura
  ): Observable<any> {

    return this.http.put(
      `${this.apiUrl}/${id}`,
      factura
    );
  }

  eliminar(
    id: string
  ): Observable<any> {

    return this.http.delete(
      `${this.apiUrl}/${id}`
    );
  }

  procesarRecordatorios(): Observable<any> {

    return this.http.post(
      `${this.apiUrl}/procesar-recordatorios`,
      {}
    );
  }

  procesarRecordatorioIndividual(
    id: string
  ): Observable<any> {

    return this.http.post(
      `${this.apiUrl}/${id}/procesar-recordatorio`,
      {}
    );
  }
}
