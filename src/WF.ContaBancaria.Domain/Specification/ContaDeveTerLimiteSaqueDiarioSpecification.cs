﻿using WF.ContaBancaria.Domain.Interface.Repository;
using WF.ContaBancaria.Domain.Models;
using DomainValidation.Interfaces.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WF.ContaBancaria.Domain.Specification
{
    public class ContaDeveTerLimiteSaqueDiarioSpecification : ISpecification<Transacoes>
    {
        
        private readonly ITransacoesRepository _transacoesRepository;

        public ContaDeveTerLimiteSaqueDiarioSpecification(ITransacoesRepository transacoesRepository)
        {
            
            _transacoesRepository = transacoesRepository;
        }
        public bool IsSatisfiedBy(Transacoes transacoes)
        {
            var transacoesDoDia = _transacoesRepository.Buscar(t => t.ContaId == transacoes.ContaId
                && t.DataCadastro.Year == transacoes.DataCadastro.Year
                && t.DataCadastro.Month == transacoes.DataCadastro.Month
                && t.DataCadastro.Day == transacoes.DataCadastro.Day
                && (int)t.TipoTransacao == 1).Sum(t => t.Valor);
            
            var limiteDiario = transacoes.Conta.LimiteSaqueDiario;
            return limiteDiario >= (transacoesDoDia * -1) + transacoes.Valor;
        }
    }
}
