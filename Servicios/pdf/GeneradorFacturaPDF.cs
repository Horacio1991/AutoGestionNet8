using AutoGestion.Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace AutoGestion.Servicios.Pdf
{
    public static class GeneradorFacturaPDF
    {
        public static void Generar(Factura factura, string rutaDestino)
        {
            // Crea el documento PDF
            Document doc = new Document();
            // Abre un flujo de archivo para escribir el PDF
            PdfWriter.GetInstance(doc, new FileStream(rutaDestino, FileMode.Create));
            // Abre el documento para agregar contenido
            doc.Open();

            var titulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var normal = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            doc.Add(new Paragraph("Factura Electrónica", titulo));
            doc.Add(new Paragraph($"Fecha: {factura.Fecha.ToShortDateString()}", normal));
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Datos del Cliente:", titulo));
            doc.Add(new Paragraph($"{factura.Cliente.Nombre} {factura.Cliente.Apellido}", normal));
            doc.Add(new Paragraph($"Contacto: {factura.Cliente.Contacto}", normal));
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Vehículo:", titulo));
            doc.Add(new Paragraph($"{factura.Vehiculo.Marca} {factura.Vehiculo.Modelo}", normal));
            doc.Add(new Paragraph($"Dominio: {factura.Vehiculo.Dominio}", normal));
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph("Detalles de Facturación:", titulo));
            doc.Add(new Paragraph($"Forma de Pago: {factura.FormaPago}", normal));
            doc.Add(new Paragraph($"Precio Total: ${factura.Precio}", normal));

            // Cierra el documento
            doc.Close();
        }
    }
}
