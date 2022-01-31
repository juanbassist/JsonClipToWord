using System;
using System.Diagnostics;

namespace JsonClipToWord
{
    [DebuggerDisplay("{NumeroFila}|{TagCampos}|{TipoDato}|{NivelAnidamiento}|{Requerido}")]
    public class RenglonEspecificacion
    {
        public int NumeroFila { get; set; }
        public string TagCampos { get; set; }
        public string TipoDato { get; set; }
        public int NivelAnidamiento { get; set; }
        public string Requerido { get; set; }

        public RenglonEspecificacion(int numeroFila, string tagCampos, string tipoDato, int nivelAnidamiento=0, string requerido = "SI")
        {
            NumeroFila = numeroFila;
            TagCampos = tagCampos.PadLeft(tagCampos.Length+nivelAnidamiento, '-');
            TipoDato = tipoDato;
            NivelAnidamiento = nivelAnidamiento;
            Requerido = requerido;
            Console.WriteLine($"numerofila: {NumeroFila}. TagCampos: {TagCampos}. tipoDato: {TipoDato}. nivelAnidamiento: {NivelAnidamiento}. requerido: {Requerido}");
        }
    }
}
