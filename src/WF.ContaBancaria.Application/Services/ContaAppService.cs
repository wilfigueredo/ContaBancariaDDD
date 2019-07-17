﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF.ContaBancaria.Infra.Data.UoW;
using WF.ContaBancaria.Application.Interface;
using WF.ContaBancaria.Application.ViewModels;
using WF.ContaBancaria.Domain.Interface.Repository;
using WF.ContaBancaria.Domain.Interface.Service;
using AutoMapper;
using WF.ContaBancaria.Domain.Models;
using WF.ContaBancaria.Domain.Enuns;

namespace WF.ContaBancaria.Application.Services
{
    public class ContaAppService : AppServices, IContaAppService
    {
        private readonly IContaRepository _contaRepository;
        private readonly ITransacoesRepository _transacoesRepository;
        private readonly IContaService _contaService;
        private readonly ITransacaoService _transacaoService;

        public ContaAppService(IContaRepository contaRepository,
                               IContaService contaService,
                               ITransacoesRepository transacoesRepository,
                               ITransacaoService transacaoService,
                               IUnitOfWork uow) : base(uow)
        {
            _contaRepository = contaRepository;
            _contaService = contaService;
            _transacoesRepository = transacoesRepository;
            _transacaoService = transacaoService;
        }

        public ContaViewModel Adicionar(ContaViewModel contaViewModel)
        {
            var conta = Mapper.Map<Conta>(contaViewModel);

            var ClienteRet = _contaService.Adicionar(conta);

            Commit();

            return contaViewModel;
        }

        public ContaViewModel Atualizar(ContaViewModel contaViewModel)
        {
            var conta = Mapper.Map<Conta>(contaViewModel);

            var ClienteRet = _contaService.Atualizar(conta);

            Commit();

            return contaViewModel;
        }

        public void AtivarConta(Guid Id)
        {
            var conta = _contaRepository.ObterPorId(Id);
            conta.Ativar();
            _contaRepository.Atualizar(conta);
            Commit();
        }

        

        public void BloquearConta(Guid Id)
        {
            var conta = _contaRepository.ObterPorId(Id);
            conta.Bloquear();
            _contaRepository.Atualizar(conta);
            Commit();
        }

        public SaqueViewModel Sacar(SaqueViewModel saqueViewModel)
        {            
            var conta = _contaRepository.ObterPorId(saqueViewModel.Id);
            
            Transacoes transacoes = new Transacoes(saqueViewModel.ValorSaque,TipoTransacao.Saque,conta.Id);
             
            var contaRet = _contaService.Sacar(conta,transacoes);
            saqueViewModel = Mapper.Map<SaqueViewModel>(contaRet);
            if (contaRet.ValidationResult.IsValid) {                 
                _transacaoService.Adicionar(transacoes);
            }
            else
            {
                contaRet.ValidationResult.Message = "Ocorreu um erro ao sacar!";
                return saqueViewModel;
            }

            if (transacoes.ValidationResult.IsValid)
            { 
                Commit();
                contaRet.ValidationResult.Message = "Saque realizado com sucesso!";
            }       
                        
            return saqueViewModel;
        }

        public DepositoViewModel Depositar(DepositoViewModel depositoViewModel)
        {

            var conta = _contaRepository.ObterPorId(depositoViewModel.Id);
           
            Transacoes transacoes = new Transacoes(depositoViewModel.ValorDeposito,TipoTransacao.Deposito,conta.Id);
            
            var contaRet = _contaService.Depositar(conta, transacoes);
            if (contaRet.ValidationResult.IsValid)
            {
                _transacaoService.Adicionar(transacoes);
            }


            if (transacoes.ValidationResult.IsValid)
            {
                Commit();
                contaRet.ValidationResult.Message = "Deposito realizado com sucesso!";
            }

            depositoViewModel = Mapper.Map<DepositoViewModel>(contaRet);

            return depositoViewModel;
        }

        public void Remover(Guid id)
        {
            _contaRepository.Remover(id);
            Commit();
        }             

        public ContaViewModel ObterPorId(Guid Id)
        {
            return Mapper.Map<ContaViewModel>(_contaRepository.ObterPorId(Id));
        }

        public IEnumerable<ContaViewModel> ObterTodos()
        {
            return Mapper.Map<IEnumerable<ContaViewModel>>(_contaRepository.ObterTodos());
        }
        public SaqueViewModel ObterDadosSaque(Guid Id)
        {
            return Mapper.Map<SaqueViewModel>(_contaRepository.ObterPorId(Id));
        }

        public DepositoViewModel ObterDadosDeposito(Guid Id)
        {
            return Mapper.Map<DepositoViewModel>(_contaRepository.ObterPorId(Id));
        }

        public IEnumerable<ContaViewModel> ObterPorSaldo(double valor)
        {
            return Mapper.Map<IEnumerable<ContaViewModel>>(_contaRepository.ObterPorSaldo(valor));
        }

        public IEnumerable<ExtratoViewModel> ObterExtrato(Guid Id)
        {
            return Mapper.Map<IEnumerable<ExtratoViewModel>>(_transacoesRepository.ObterExtrato(Id));
        }

        public ExtratoPeriodoViewModel ObterDadosExtratoPeriodo(Guid Id)
        {
            var conta = _contaRepository.ObterPorId(Id);
            ExtratoPeriodoViewModel extratoPeriodo = new ExtratoPeriodoViewModel();
            extratoPeriodo.Id = conta.Id;
            extratoPeriodo.Nome = conta.Cliente.Nome;
            return extratoPeriodo;
        }

        public IEnumerable<ExtratoViewModel> GerarExtratoPeriodo(Guid Id, DateTime dataInicial, DateTime dataFinal)
        {
            return Mapper.Map<IEnumerable<ExtratoViewModel>>(_transacoesRepository.ObterExtratoPeriodo(Id, dataInicial, dataFinal));
        }

        public double ConsultarSaldo(Guid Id)
        {
            var conta = ObterPorId(Id);
            double saldo = conta.Saldo;

            return saldo;
        }

        public void Dispose()
        {
            _contaRepository.Dispose();
        }       
    }
}
