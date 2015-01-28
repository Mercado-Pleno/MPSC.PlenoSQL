﻿using System;

namespace MPSC.PlenoSQL.AppWin.Interface
{
	public interface IQueryResult
	{
		String NomeDoArquivo { get; }
		void Executar();
		void AlterarConexao();
		Boolean Salvar();
		Boolean PodeFechar();
		void Fechar();
		Boolean Focus();
	}

	public class NullQueryResult : IQueryResult
	{
		public String NomeDoArquivo { get { return String.Empty; } }
		public void Executar() { }
		public void AlterarConexao() { }
		public Boolean Focus() { return false; }
		public Boolean Salvar() { return false; }
		public Boolean PodeFechar() { return true; }
		public void Fechar() { }

		private static IQueryResult _instance;
		public static IQueryResult Instance { get { return _instance ?? (_instance = new NullQueryResult()); } }
	}
}