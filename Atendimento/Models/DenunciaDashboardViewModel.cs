using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class DenunciaDashboardViewModel
    {
        public int denunciasTotal { get; set; }
        public int denunciasNaoAtendidasTotal { get; set; }
        public int denunciasPM { get; set; }
        public int denunciasPMAguardando { get; set; }
        public int denunciasPC { get; set; }
        public int denunciasPCAguardando { get; set; }
        public int denunciasEmAnalisePM { get; set; }
        public int denunciasEmAnalisePC { get; set; }
        public int denunciasDescatardasPM { get; set; }
        public int denunciasDescatardasPC { get; set; }
        public int denunciasFinalizadasPositivaPM { get; set; }
        public int denunciasFinalizadasNegativaPM { get; set; }
        public int denunciasFinalizadasPositivaPC { get; set; }
        public int denunciasFinalizadasNegativaPC { get; set; }
    }
}