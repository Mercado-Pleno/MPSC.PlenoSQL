﻿using System;
using System.Linq;

namespace MPSC.PlenoSQL.AppWin.Infra
{
	public static class Strings
	{
		public const Char CR = '\r';
		public const Char LF = '\n';
		public const Char TB = '\t';
		public const Char PL = '\'';
		public const Char SPC = ' ';
		public static readonly String BREAK = new String(new Char[] { CR, LF, TB, PL, SPC, '"', '(', ')', '[', ']', '{', '}', '/', '*', '+', '-', ',', ';', '<', '>', '!', '=' });
		public static readonly Char[] ENTER = { CR, LF };
	}

	public class Trecho
	{
		private String _sql;
		private Int32 _posicao;

		private Trecho() { }
		~Trecho() { Dispose(); }

		private Trecho Load(String sql, Int32 posicao)
		{
			Dispose();
			_sql = sql;
			_posicao = posicao;
			return this;
		}

		public virtual void Dispose()
		{
			_sql = null;
		}

		public String LinhaAnterior
		{
			get
			{
				String retorno = null;
				if ((_posicao > 0) && (_posicao < _sql.Length))
				{
					var posicaoFinal = _sql.LastIndexOfAny(Strings.ENTER, _posicao) - 2;
					if (posicaoFinal >= 0)
					{
						var posicaoInicial = _sql.LastIndexOfAny(Strings.ENTER, posicaoFinal);
						if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
							retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial);
					}
				}

				return retorno;
			}
		}

		public String LinhaAtual
		{
			get
			{
				String retorno = null;

				if ((_posicao >= 0) && (_posicao <= _sql.Length))
				{
					var posicaoInicial = (_posicao < _sql.Length) ? _sql.LastIndexOfAny(Strings.ENTER, _posicao) : _sql.LastIndexOfAny(Strings.ENTER);
					if (posicaoInicial >= 0)
					{
						var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 1);
						if ((posicaoFinal >= 0) && (posicaoFinal > posicaoInicial))
							retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial - 1);
						else
							retorno = _sql.Substring(posicaoInicial + 1);
					}
					else
						retorno = _sql;
				}

				return retorno;
			}
		}

		public String LinhaPosterior
		{
			get
			{
				String retorno = null;
				if ((_posicao >= 0) && (_posicao < _sql.Length))
				{
					var posicaoInicial = _sql.IndexOfAny(Strings.ENTER, _posicao);
					if (posicaoInicial >= 0)
					{
						var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 2);
						if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
							retorno = _sql.Substring(posicaoInicial + 2, posicaoFinal - posicaoInicial - 2);
					}
				}

				return retorno;
			}
		}

		public String CaracterAtual { get { return (_posicao > 0) ? _sql.Substring(_posicao - 1, 1) : String.Empty; } }

		public Token Token { get { return Token.Get(_sql, _posicao); } }

		private static readonly Trecho trecho = new Trecho();
		public static Trecho Get(String sql, Int32 posicao)
		{
			return trecho.Load(sql, posicao);
		}
	}

	public class Token : IDisposable
	{
		private String _primeiro;
		private String _completo;
		private String _parcial;
		private String _tabela;
		public String Completo { get { return _completo; } }
		public String Parcial { get { return _parcial; } }
		public String Primeiro { get { return _primeiro; } }
		public String Tabela { get { return _tabela; } }

		private Token() { }
		~Token() { Dispose(); }

		private Token Load(String sql, Int32 posicao)
		{
			Dispose();
			var tamanho = sql.Length;
			if ((posicao >= 0) && (posicao <= tamanho))
			{
				var posicaoInicial = ObterPosicao(sql, posicao, -1);
				if ((posicaoInicial >= 0) && (posicaoInicial < posicao) && (posicaoInicial < tamanho))
				{
					_parcial = sql.Substring(posicaoInicial, CalcularQuantidade(posicaoInicial, posicao, tamanho, 0));
					var posicaoFinal = ObterPosicao(sql, posicao, +1);
					if (posicaoFinal > posicaoInicial)
						_completo = sql.Substring(posicaoInicial, CalcularQuantidade(posicaoInicial, posicaoFinal, tamanho, 1));
					else
						_completo = _parcial;
				}
				var posicaoPonto = _completo.IndexOf(".");
				_primeiro = (posicaoPonto > 0) ? _completo.Substring(0, posicaoPonto) : _completo;
				_tabela = ObterNomeTabelaPeloApelido(sql, posicao, _primeiro);
			}
			return this;
		}

		private Int32 CalcularQuantidade(Int32 posicaoInicial, Int32 posicaoFinal, Int32 tamanhoString, Int32 controle)
		{
			var quantidade = posicaoFinal - posicaoInicial;
			return quantidade + ((posicaoFinal == tamanhoString) || (posicaoInicial + quantidade >= tamanhoString) ? 0 : controle);
		}

		public virtual void Dispose()
		{
			_primeiro = String.Empty;
			_completo = String.Empty;
			_parcial = String.Empty;
			_tabela = String.Empty;
		}

		private Int32 ObterPosicao(String sql, Int32 posicao, Int32 controle)
		{
			while (PodeNavegar(sql, posicao, controle) && !IsToken(sql, posicao, controle))
				posicao += controle;
			return posicao;
		}

		private Boolean PodeNavegar(String sql, Int32 posicao, Int32 controle)
		{
			return ((controle < 0) && (posicao > 0)) || (posicao < sql.Length);
		}

		private Boolean IsToken(String sql, Int32 posicao, Int32 controle)
		{
			var retorno = (posicao >= 0) && (posicao < sql.Length);
			retorno = retorno && !IsBreakToken(sql, posicao, controle);
			retorno = retorno && IsBreakToken(sql, posicao + controle, controle);
			return retorno;
		}

		private Boolean IsBreakToken(String sql, Int32 posicao, Int32 controle)
		{
			var retorno = (posicao < 0) || (posicao >= sql.Length);
			retorno = retorno || (Strings.BREAK.Contains(sql[posicao].ToString()) && !IsBreakToken(sql, posicao + controle, controle));
			return retorno;
		}

		private String ObterNomeTabelaPeloApelido(String sql, Int32 posicao, String apelido)
		{
			String nomeDaTabela = String.Empty;
			var tokens = sql.Split(Strings.BREAK.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();

			var index = tokens.LastIndexOf(apelido);
			if (index < 0)
				index = tokens.Select(t => t.ToUpper()).ToList().LastIndexOf(apelido.ToUpper());

			if (index > 1)
			{
				if (tokens[index - 1].ToUpper().Equals("AS"))
					nomeDaTabela = tokens[index - 2];
				else if (tokens[index - 1].ToUpper().Equals("FROM") || tokens[index - 1].ToUpper().Equals("JOIN"))
					nomeDaTabela = tokens[index];
				else
					nomeDaTabela = tokens[index - 1];
			}

			return nomeDaTabela;
		}

		private static readonly Token token = new Token();
		internal static Token Get(String sql, Int32 posicao)
		{
			return token.Load(sql, posicao);
		}
	}
}