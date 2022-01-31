
using NPOI.XWPF.UserModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

namespace JsonClipToWord
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string clipboardText = ObtenerJsonDeClipboard();

            XWPFDocument doc = new XWPFDocument();
            IEnumerable<RenglonEspecificacion> renglonesEspecificacion = ObtenerRenglonesEspecificacionFromJsonText(clipboardText);

            int rows = renglonesEspecificacion.Count();
            XWPFTable table = doc.CreateTable(rows, 3);

            int idxFila = 0;
            foreach (var fila in renglonesEspecificacion)
            {
                var row = table.GetRow(idxFila++);
                var rowTagCampos = row.GetCell(0);
                rowTagCampos.SetText(fila.TagCampos);
                row.GetCell(1).SetText(fila.TipoDato);
                row.GetCell(2).SetText(fila.Requerido);
            }

            using (FileStream fs = new FileStream("simpleTable.docx", FileMode.Create))
            {
                doc.Write(fs);
            }
            //Console.ReadLine();
        }

        private static string ObtenerJsonDeClipboard()
        {
            string clipboardText = default;
            try
            {
                if (!Clipboard.ContainsText(TextDataFormat.Text))
                    throw new Exception("Clipboard no tiene texto valido");

                clipboardText = Clipboard.GetText(TextDataFormat.Text);
                Console.WriteLine(clipboardText);
                JToken.Parse(clipboardText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSOn no valido de clipboard.Ex: {ex.Message}. Presione Cualquier Tecla para salir.");
                Console.ReadLine();
                Environment.Exit(1);
            }

            return clipboardText;
        }

        private static IEnumerable<RenglonEspecificacion> ObtenerRenglonesEspecificacionFromJsonText(string fromJsonTxt)
        {
            var modeloJson = new ModeloJson(fromJsonTxt);

            List<RenglonEspecificacion> renglonesEspecificacion = new List<RenglonEspecificacion>();

            foreach (var item in modeloJson.propiedadesJson)
                renglonesEspecificacion.AddRange(ObtenerRenglonesEspecificacionFromPropiedadJson(item));
           
            return renglonesEspecificacion;
        }
        private static IList<RenglonEspecificacion>ObtenerRenglonesEspecificacionFromPropiedadJson(PropiedadJson propiedadJson)
        {
            var listaRenglones = new List<RenglonEspecificacion>();
            listaRenglones.Add(new RenglonEspecificacion(indexRenglonGlobal++, propiedadJson.nombre, propiedadJson.GetNombreTipoDatoAbreviado, propiedadJson.NivelAnidamiento));
            
            if (propiedadJson.TieneObjeto)
            {
                foreach (var propiedadJsonNested in propiedadJson.valorObjeto)
                    listaRenglones.AddRange(ObtenerRenglonesEspecificacionFromPropiedadJson(propiedadJsonNested));
            }   

            if (propiedadJson.TieneArray)
                throw new NotImplementedException("TieneArray falta implementar en funcion recursiva.");

            return listaRenglones;
        }

        static int indexRenglonGlobal = 0;
    }


    public class ModeloJson
    {
        public IList<PropiedadJson> propiedadesJson { get; }
        public ModeloJson(string FromJsonString)
        {
            propiedadesJson = new List<PropiedadJson>();
            JObject jsonObj = JObject.Parse(FromJsonString);

            foreach (var jsonToken in jsonObj.Children())
            {
                AgregarPropiedadJsonFromJProperty(jsonToken as JProperty);
            }
        }
        void AgregarPropiedadJsonFromJProperty(JProperty jProperty)
        {
            propiedadesJson.Add(new PropiedadJson(jProperty));
        }
    }

}
