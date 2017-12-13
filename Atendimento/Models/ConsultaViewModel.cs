using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class ConsultaViewModel
    {        
        [Display(Name = "Número do Protocolo")]
        public string NumeroProtocolo { get; set; }

        [Display(Name = "Tipo de crime")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "TipoCrime")]
        [AdditionalMetadata("TextField", "DESCRICAO_TIPO_CRIME")]
        [AdditionalMetadata("ValueField", "ID_TIPO_CRIME")]
        public int? TipoCrimeId { get; set; }

        [Display(Name = "Situação")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "Situacao")]
        [AdditionalMetadata("TextField", "DESCRICAO_SITUACAO")]
        [AdditionalMetadata("ValueField", "ID_SITUACAO")]
        public int? SituacaoId { get; set; }

        [Display(Name = "Unidade Policial")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "Orgao")]
        [AdditionalMetadata("TextField", "NOME_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_ORGAO")]
        public int? UnidadeId { get; set; }

        [Display(Name = "Batalhão")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "Orgao")]
        [AdditionalMetadata("TextField", "NOME_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_ORGAO")]
        public int? UnidadeBatalhaoId { get; set; }

        [Display(Name = "Tipo Órgão")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "TipoUnidadePMPCModel")]
        [AdditionalMetadata("TextField", "DESCRICAO_TIPO_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_TIPO_ORGAO")]     
        [Required]   
        public int OrgaoIdTipo { get; set; }

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

        [Display(Name = "Data Expirar")]
        [UIHint("Date")]
        public DateTime? DataExpira { get; set; }

        [Display(Name = "CPF do denunciante")]
        [UIHint("CPFValido")]
        public string CpfDenunciante { get; set; }

        [Display(Name = "Especificar")]
        [UIHint("DescricaoTipoCrime")]
        public string DescricaoTipoCrime { get; set; }
    }

}