using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class DenunciaFiltroViewModel
    {
        [Required]
        [Display(Name = "Número do Protocolo")]
        public string NumeroProtocolo { get; set; }

        [Display(Name = "Tipo de crime")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "TipoCrime")]
        [AdditionalMetadata("TextField", "DESCRICAO_TIPO_CRIME")]
        [AdditionalMetadata("ValueField", "ID_TIPO_CRIME")]
        public int TipoCrimeId { get; set; }


        [Display(Name = "Tipo Órgão")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "TipoOrgao")]
        [AdditionalMetadata("TextField", "DESCRICAO_TIPO_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_TIPO_ORGAO")]
        public int? OrgaoIdTipo { get; set; }

        [Display(Name = "Data de Cadastro - De")]
        [UIHint("Date")]

        public DateTime? DataCadastroInicial { get; set; }
        [Display(Name = "Data de Cadastro - Até")]
        [UIHint("Date")]

        public DateTime? DataCadastroFinal { get; set; }

        [Display(Name = "Data de Encaminhamento - De")]
        [UIHint("Date")]

        public DateTime? DataEncaminhamentoInicial { get; set; }
        [Display(Name = "Data de Encaminhamento - Até")]
        [UIHint("Date")]

        public DateTime? DataEncaminhamentoFinal { get; set; }

    }
}