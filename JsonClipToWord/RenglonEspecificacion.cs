namespace JsonClipToWord
{
    public class RenglonEspecificacion
    {
        public int NumeroFila { get; set; }
        public string TagCampos { get; set; }
        public string TipoDato { get; set; }
        public string Requerido { get; set; }

        public RenglonEspecificacion(int numeroFila, string tagCampos, string tipoDato, string requerido = "SI")
        {
            NumeroFila = numeroFila;
            TagCampos = tagCampos;
            TipoDato = tipoDato;
            Requerido = requerido;
        }
    }
}
