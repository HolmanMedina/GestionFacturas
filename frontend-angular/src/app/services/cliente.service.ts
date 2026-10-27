import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Cliente } from '../models/cliente';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {

  private http = inject(HttpClient);

  private apiUrl =
    'http://localhost:5167/api/Clientes';

  obtenerTodos(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(
      this.apiUrl
    );
  }

  obtenerPorId(
    id: string
  ): Observable<Cliente> {

    return this.http.get<Cliente>(
      `${this.apiUrl}/${id}`
    );
  }

  crear(cliente: {
    nombre: string;
    email: string;
  }): Observable<any> {

    return this.http.post(
      this.apiUrl,
      cliente
    );
  }

  actualizar(
    id: string,
    cliente: Cliente
  ): Observable<any> {

    return this.http.put(
      `${this.apiUrl}/${id}`,
      cliente
    );
  }

  eliminar(
    id: string
  ): Observable<any> {

    return this.http.delete(
      `${this.apiUrl}/${id}`
    );
  }
}
