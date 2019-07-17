﻿Funcionalidade: Deposito em Conta Corrente
				Operações de deposito em conta corrente de um cliente

@TesteAceitacaoContaCorrente

Cenario: Deposito Realizado com Sucesso
	Dado Uma Conta Corrente Ativa
	Quando Um valor for depositado
	E O valor e superior a zero
	Então Receberei uma mensagem de deposito realizado com sucesso 	


Cenario: Deposito com valor negativo 
	Dado Uma Conta Corrente Ativa
	Quando Um valor for depositado
	E O valor é negativo
	Então Receberei uma mensagem de Valor deve ser positivo	

Cenario: Deposito com valor 0 
	Dado Uma Conta Corrente Ativa
	Quando Um valor for depositado
	E O valor é zero
	Então Receberei uma mensagem de Valor não pode ser igual a zero


