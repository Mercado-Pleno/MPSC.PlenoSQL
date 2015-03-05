﻿using System;

namespace MPSC.PlenoSQL.TestesUnitarios
{
	public static class Strings
	{
		public const Char CR = '\r';
		public const Char LF = '\n';
		public const Char TB = '\t';
		public const Char PL = '\'';
		public const Char SPC = ' ';
		public static readonly String BREAK = new String(new Char[] { CR, LF, TB, PL, SPC, '"', '(', ')', '[', ']', '{', '}', '/', '*', '+', '-', ',', ';' });
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
				var retorno = String.Empty;
				var posicaoFinal = _sql.LastIndexOfAny(Strings.ENTER, _posicao) - 2;
				if (posicaoFinal >= 0)
				{
					var posicaoInicial = _sql.LastIndexOfAny(Strings.ENTER, posicaoFinal);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial);
				}

				return retorno;
			}
		}

		public String LinhaAtual
		{
			get
			{
				var retorno = String.Empty;
				var posicaoInicial = _sql.LastIndexOfAny(Strings.ENTER, _posicao);
				if (posicaoInicial >= 0)
				{
					var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 1);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 1, posicaoFinal - posicaoInicial - 1);
				}

				return retorno;
			}
		}

		public String LinhaPosterior
		{
			get
			{
				var retorno = String.Empty;
				var posicaoInicial = _sql.IndexOfAny(Strings.ENTER, _posicao);
				if (posicaoInicial >= 0)
				{
					var posicaoFinal = _sql.IndexOfAny(Strings.ENTER, posicaoInicial + 2);
					if ((posicaoInicial >= 0) && (posicaoInicial < posicaoFinal))
						retorno = _sql.Substring(posicaoInicial + 2, posicaoFinal - posicaoInicial - 2);
				}

				return retorno;
			}
		}

		public String CaracterAtual { get { return _sql.Substring(_posicao, 1); } }

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
		public String Completo { get { return _completo; } }
		public String Parcial { get { return _parcial; } }
		public String Primeiro { get { return _primeiro; } }

		private Token() { }
		~Token() { Dispose(); }

		private Token Load(String sql, Int32 posicao)
		{
			Dispose();
			var posicaoInicial = ObterPosicao(sql, posicao, -1);
			if ((posicaoInicial >= 0) && (posicaoInicial < posicao))
			{
				_parcial = sql.Substring(posicaoInicial, posicao - posicaoInicial + 1);
				var posicaoFinal = ObterPosicao(sql, posicao, +1);
				if (posicaoFinal > posicaoInicial)
					_completo = sql.Substring(posicaoInicial, posicaoFinal - posicaoInicial + 1);
				else
					_completo = _parcial;
			}
			else
			{
				_parcial = String.Empty;
				_completo = String.Empty;
			}
			var posicaoPonto = _completo.IndexOf(".");
			_primeiro = (posicaoPonto > 0) ? _completo.Substring(0, posicaoPonto) : _completo;
			return this;
		}

		public virtual void Dispose()
		{
			_primeiro = null;
			_completo = null;
			_parcial = null;
		}

		private Int32 ObterPosicao(String sql, Int32 posicao, Int32 controle)
		{
			while (!IsToken(sql, posicao, controle))
				posicao += controle;
			return posicao;
		}

		private Boolean IsToken(String sql, Int32 posicao, Int32 controle)
		{
			var retorno = (posicao >= 0) && (posicao < sql.Length);
			retorno &= !IsBreakToken(sql, posicao, controle);
			retorno &= IsBreakToken(sql, posicao + controle, controle);
			return retorno;
		}

		private Boolean IsBreakToken(String sql, Int32 posicao, Int32 controle)
		{
			var retorno = (posicao < 0) || (posicao >= sql.Length);
			retorno |= Strings.BREAK.Contains(sql[posicao].ToString()) && !IsBreakToken(sql, posicao + controle, controle);
			return retorno;
		}

		private static readonly Token token = new Token();
		internal static Token Get(String sql, Int32 posicao)
		{
			return token.Load(sql, posicao);
		}
	}
}