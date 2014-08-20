﻿using System;
using System.Data.OleDb;

namespace MP.PlenoBDNE.AppWin.Dados
{
	public class BancoDeDadosOleDbForAccess : BancoDeDados<OleDbConnection>
	{
		public override String Descricao { get { return "Ole DB For MS Access"; } }
		protected override String AllTablesSQL { get { return "Select Table_Name as Tabela, Table_Schema as Banco, System_Table_Name as NomeInterno From SysTables"; } }
		protected override String AllViewsSQL { get { return "Select T.Name as NomeView, DB_NAME() as Banco, Name as NomeInterno From SysObjects T Where (T.Type In ('V')) And (T.Name Like '{0}%')"; } }
		protected override String AllColumnsSQL { get { return "Select C.Name as Coluna From SysColumns C Where (C.Name = '{0}')"; } }
		protected override String StringConexaoTemplate { get { return @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=dBASE IV;"; } }
	}
}