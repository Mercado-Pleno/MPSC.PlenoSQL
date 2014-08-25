﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FastColoredTextBoxNS;

namespace MP.PlenoBDNE.AppWin.Infra
{
	public static class Extensions
	{
		public static String Concatenar<T>(this IEnumerable<T> source, String join)
		{
			return String.Join<T>(join, source);
		}

		public static String ObterPrefixo(this FastColoredTextBox textBox)
		{
			Int32 selectionStart = textBox.SelectionStart;
			String query = textBox.Text.Substring(0, selectionStart).ToUpper();

			Int32 i = selectionStart + 1;
			while (!Util.TokenKeys.Contains(query[--i - 1])) ;

			var tamanho = selectionStart - i;
			textBox.SelectionStart = i;
			textBox.SelectionLength = tamanho;
			return query.Substring(i, tamanho).Trim();
		}

		public static IDbCommand CriarComando(this IDbConnection iDbConnection, String query)
		{
			if (iDbConnection.State != ConnectionState.Open)
				iDbConnection.Open();
			IDbCommand iDbCommand = iDbConnection.CreateCommand();
			iDbCommand.CommandText = query;
			iDbCommand.CommandType = query.ToLower().StartsWith("exec") || (query.IndexOfAny("\r\n\t ".ToCharArray()) < 0) ? CommandType.StoredProcedure : CommandType.Text;
			iDbCommand.CommandTimeout = 3600;
			return iDbCommand;
		}

		public static Boolean IsOpen(this IDataReader iDataReader)
		{
			return (iDataReader != null) && !iDataReader.IsClosed;
		}
	}
}