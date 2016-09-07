﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MPSC.PlenoSQL.TestesUnitarios.Conexao
{
	public interface IConexao
	{
		IDataReader Executar(String cmdSql);
	}

	public class AutoFactory
	{
		private IConexao Conexao;
		public AutoFactory(IConexao conexao)
		{
			Conexao = conexao;
		}

		public IEnumerable<TEntidade> Query<TEntidade>(String cmdSql)
		{
			return QueryImpl<TEntidade>(cmdSql).ToArray();
		}

		public IEnumerable<TEntidade> QueryImpl<TEntidade>(String cmdSql)
		{
			var dataReader = Executar(cmdSql);
			while (dataReader.Read())
				yield return Filler.New<TEntidade>(dataReader);
		}

		private IDataReader Executar(String cmdSql)
		{
			return Conexao.Executar(cmdSql);
		}
	}
}