﻿using DomainValidation.Validation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TechTalk.SpecFlow;
using WF.ContaBancaria.Domain.Interface.Repository;
using WF.ContaBancaria.Domain.Models;
using WF.ContaBancaria.Domain.Services;
using WF.ContaBancaria.Domain.Specification;
using WF.ContaBancaria.Domain.Validation;
using Xunit;

namespace WF.ContaBancaria.Domain.Teste.Contas.Service
{
    [Binding]
    public class OperacoesEmContaCorrenteSteps
    {

        Conta conta;
        Transacoes transacao;        
        List<string> errors;
        List<Transacoes> transacoes = new List<Transacoes>();

        [Given(@"Uma Conta Corrente Ativa com limite diario de (.*)")]
        public void DadoUmaContaCorrenteAtivaComLimiteDiarioDe(int p0)
        {
            conta = new Conta(1000, p0);
            conta.Ativar();
        }

        [Given(@"Uma Conta Corrente Ativa com limite diario de (.*) e saldo de (.*)")]
        public void DadoUmaContaCorrenteAtivaComLimiteDiarioDeESaldoDe(int p0, int p1)
        {
            conta = new Conta(p1, p0);
            conta.Ativar();
        }

        [When(@"Sacar um valor de (.*)")]
        public void QuandoSacarUmValorDe(int p0)
        {
            var repoConta = new Mock<IContaRepository>();
            var repoTransacao = new Mock<ITransacoesRepository>();            

            transacao = new Transacoes(p0, Enuns.TipoTransacao.Saque, conta.Id);
            transacao.Conta = conta;                     
                        
            repoTransacao.Setup(r => r.Buscar(It.IsAny<Expression<Func<Transacoes, bool>>>()))
            .Returns<Expression<Func<Transacoes, bool>>>(pred => transacoes.Where(pred.Compile()));            

            var contaService = new ContaService(repoConta.Object, repoTransacao.Object);
            contaService.Sacar(conta, transacao);
            transacao.Valor = transacao.Valor * -1;
            transacoes.Add(transacao);
        }       

        [Then(@"Receberei uma mensagem de Limite diario para saque excedido")]
        public void EntaoRecebereiUmaMensagemDeLimiteDiarioParaSaqueExcedido()
        {
            PopularErros(conta.ValidationResult);
            Assert.Equal("Limite diario para saque excedido", errors[0]);
        }

        [Then(@"Receberei uma mensagem de Saldo para saque indisponivel")]
        public void EntaoRecebereiUmaMensagemDeSaldoParaSaqueIndisponivel()
        {
            PopularErros(conta.ValidationResult);
            Assert.Equal("Saldo para saque indisponivel", errors[0]);
        }

        public void PopularErros(ValidationResult validationResult)
        {
            errors = new List<string>();
            foreach (var erro in validationResult.Erros)
            {
                errors.Add(erro.Message);
            }
        }
    }
}
