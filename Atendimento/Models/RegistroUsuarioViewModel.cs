using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDenunciaSSP.Atendimento.Models
{
    public class RegistroUsuarioViewModel
    {

        [Display(Name = "ID")]
        [UIHint("Hidden")]
        public int usuarioID { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string usuarioNome { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(5)]
        [Display(Name = "Login")]
        public string usuarioLogin { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string usuarioEmail { get; set; }

       
        [Display(Name = "RG")]
        [MaxLength(12)]
    
        public string usuarioRG { get; set; }

        [Display(Name = "RG UF")]        
        public string usuarioRG_UF { get; set; }

        [Required]
        [Display(Name = "CPF")]
        [UIHint("CPF")]
        public string usuarioCPF { get; set; }

       
        
        [Display(Name = "Grupo")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "Grupo")]
        [AdditionalMetadata("TextField", "DESCRICAO_GRUPO")]
        [AdditionalMetadata("ValueField", "ID_GRUPO")]
        [Required]
        public int GrupoId { get; set; }      


        [Display(Name = "Tipo Órgão")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "TipoOrgao")]
        [AdditionalMetadata("TextField", "DESCRICAO_TIPO_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_TIPO_ORGAO")]
        public int? OrgaoIdTipo { get; set; }

        [Display(Name = "Órgão Sub Tipo")]
        [UIHint("SelectOneFromEntity")]
        [AdditionalMetadata("EntityName", "SubtipoOrgao")]
        [AdditionalMetadata("TextField", "DESCRICAO_SUBTIPO_ORGAO")]
        [AdditionalMetadata("ValueField", "ID_TIPO_ORGAO")]
        public int? OrgaoIdSubTipo { get; set; }

        public List<int> Orgaos { get; set; }
       
        [Display(Name = "Órgão")]
        [UIHint("Hidden")]
        //[UIHint("SelectOneFromEntity")]
        //[AdditionalMetadata("EntityName", "Orgao")]
        //[AdditionalMetadata("TextField", "NOME_ORGAO")]
        //[AdditionalMetadata("ValueField", "ID_ORGAO")]
        public int? OrgaoId { get; set; }

        [Display(Name = "Primeiro Acesso")]
        public bool primAcesso { get; set; }

        [Display(Name = "Flg Inativo")]
        public bool ativo { get; set; }

 
        [UIHint("Switch")]
        public bool TipoUsuario { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        [Required]
        public String Senha { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        [Required]
        public String NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a nova senha")]
         [Required]
        [System.ComponentModel.DataAnnotations.Compare("NovaSenha", ErrorMessage = "A Senha e Confirmação de Senha devem coincidir.")]
        public String  ConfirmaNovaSenha {get;set;}




    }


}