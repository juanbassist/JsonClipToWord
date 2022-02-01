using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace JsonClipToWord
{
    [DebuggerDisplay("{nombre}|{valorString}|S:{EsString}|O:{TieneObjeto}|N:{NivelAnidamiento}")]
    public class PropiedadJson
    {
        JProperty _jProperty;
        JValue _jValue;
        public int NivelAnidamiento { get; set; }
        public string nombre { get; }
        public bool TieneValor { get; }
        public bool TieneArray { get; }
        public IEnumerable<PropiedadJson> valorArray { get; }

        public bool EsString { get; }
        public string valorString { get; }

        public bool EsDateTime { get; }
        public DateTimeOffset valorDateTime { get; }

        public bool EsBooleano { get; }
        public bool valorBooleano { get; }

        public bool EsInteger { get; }
        public int valorInteger { get; }

        public bool EsFloat { get; }
        public float valorFloat { get; }

        public bool TieneObjeto { get; }
        public IList<PropiedadJson> valorObjeto { get; }

        public string GetNombreTipoDatoAbreviado
        {
            get
            {
                if (EsDateTime) return "date";
                if (EsBooleano) return "bol";
                if (EsInteger) return "int";
                if (EsFloat) return "dec";
                if (TieneObjeto) return "T";
                if (TieneArray) return "[T]";
                return "str";
            }
        }

        public PropiedadJson(JProperty jProperty, int nivelAnidamiento = 0)
        {
            _jProperty = jProperty;
            NivelAnidamiento = nivelAnidamiento;
            nombre = jProperty.Name;

            TieneValor = jProperty.Value is JValue;
            _jValue = TieneValor ? jProperty.Value as JValue : null;

            TieneObjeto = jProperty.Value is JObject; ;
            TieneArray = jProperty.Value is JArray;

            if (TieneValor)
            {
                EsString = _jValue.Type == JTokenType.String || _jValue.Type == JTokenType.Guid || _jValue.Type == JTokenType.Uri;
                valorString = jProperty.Value.ToString();

                DateTimeOffset.TryParse(valorString, out var _valorDateTime);
                EsDateTime = (EsString || _jValue.Type == JTokenType.Date) && _valorDateTime != DateTimeOffset.MinValue;
                valorDateTime = EsDateTime ? _valorDateTime : DateTimeOffset.MinValue;

                EsBooleano = _jValue.Type == JTokenType.Boolean;
                valorBooleano = EsBooleano ? bool.Parse(valorString) : false;

                EsInteger = _jValue.Type == JTokenType.Integer;
                valorInteger = EsInteger ? int.Parse(valorString) : 0;

                EsFloat = _jValue.Type == JTokenType.Float;
                valorFloat = EsFloat ? float.Parse(valorString) : 0;

                // Si ya se determino un tipo especifico difente a string desmarcar booleano de esEstring
                EsString = !new[] { EsDateTime, EsBooleano, EsInteger, EsFloat, TieneArray, TieneObjeto }.Any(b => b == true);

                // De acuerdo al enumerador JTokenType, que no se hayan mapeado en las lineas de arriba
                bool propiedadValorNoMapeada = new[] { 0, 1, 2, 3, 4, 5, 10, 11, 13, 14, 17 }.Any(a => a == (int)_jValue.Type);
                if (propiedadValorNoMapeada)
                    throw new ApplicationException($"Se detecto propiedad con Valor No Mapeada con {nombre}:{valorString} como type '{_jValue.Type}'. Actualizar codigo...");

            }

            if (TieneObjeto)
                valorObjeto = TieneObjeto ? MapListPropiedadJsonFromJObject(jProperty.Value as JObject).ToList() : new List<PropiedadJson>();

            if (TieneArray)
                valorObjeto = TieneArray ? MapListPropiedadJsonFromJArray(jProperty.Value as JArray).ToList() : new List<PropiedadJson>();
        }

        IEnumerable<PropiedadJson> MapListPropiedadJsonFromJObject(JObject jobject)
        {
            var propiedadesJson = jobject.Children().Select(c => new PropiedadJson(c as JProperty, NivelAnidamiento+1));
            return propiedadesJson;
        }

        /// <summary>
        /// Solo se queda con la primer tupla del array como ejemplo.
        /// </summary>
        IEnumerable<PropiedadJson> MapListPropiedadJsonFromJArray(JArray jarray)
        {
            var propiedadesJson = jarray.First().Children().Select(c => new PropiedadJson(c as JProperty, NivelAnidamiento + 1));
            return propiedadesJson;
        }
    }

}
