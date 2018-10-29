﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF.ContaBancaria.Application.ViewModels
{
    public class ExtratoViewModel
    {
        public Guid Id { get; set; }

        public double Valor { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }  
        
        public ContaViewModel Contas { get; set; }
    }
}
