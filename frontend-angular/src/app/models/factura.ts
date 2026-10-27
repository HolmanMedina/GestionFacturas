export interface Factura {
  id: string;
  clienteId: string;
  numeroFactura: string;
  descripcion: string;
  valor: number;
  fechaVencimiento: string;
  estado: number;
}
