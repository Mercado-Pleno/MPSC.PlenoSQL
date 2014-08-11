﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MP.PlenoBDNE.AppWin.Dados;
using MP.PlenoBDNE.AppWin.Infra;
using MP.PlenoBDNE.AppWin.View.Interface;
using MPSC.LanguageEditor;
using System.Drawing;
using MPSC.LanguageEditor.Syntax;

//TODO: Colocar informações no StatusBar
//TODO: Listar os objetos do banco de dados na coluna da esquerda (TreeView)
//TODO: Exportar o resultado da query para TXT, XLS, XML, PDF, etc.
//TODO: Permitir escolher a fonte e o tamanho da mesma.
//TODO: Melhorar tratamento para Colorir query
//TODO: Mensagem de aguarde, processsando
//TODO: Permitir o cancelamento da query.
//TODO: Criar um grupo de Favoritos (Cada grupo poderá agrupar vários arquivos)
//TODO: Close All But This.

namespace MP.PlenoBDNE.AppWin.View
{
	public partial class QueryResult : TabPage, IQueryResult
	{
		private static Int32 _quantidade = 0;
		private IBancoDeDados _bancoDeDados = null;
		public IBancoDeDados BancoDeDados { get { return _bancoDeDados ?? (_bancoDeDados = BancoDeDados<IDbConnection>.Conectar()); } }

		private String originalQuery = String.Empty;
		public String NomeDoArquivo { get; private set; }
		private String QueryAtiva { get { return ((txtQuery.SelectedText.Length > 1) ? txtQuery.SelectedText : txtQuery.Text); } }

		public QueryResult(String nomeDoArquivo)
		{
			InitializeComponent();
			Abrir(nomeDoArquivo);
			Colorir();
		}

		public void Abrir(String nomeDoArquivo)
		{
			if (!String.IsNullOrWhiteSpace(nomeDoArquivo) && File.Exists(nomeDoArquivo))
			{
				txtQuery.Text = File.ReadAllText(NomeDoArquivo = nomeDoArquivo);
				originalQuery = txtQuery.Text;
			}
			else
				NomeDoArquivo = String.Format("Query{0}.sql", ++_quantidade);
			UpdateDisplay();
		}

		public Boolean Salvar()
		{
			if (String.IsNullOrWhiteSpace(NomeDoArquivo) || NomeDoArquivo.StartsWith("Query") || !File.Exists(NomeDoArquivo))
				NomeDoArquivo = Util.GetFileToSave("Arquivos de Banco de Dados|*.sql") ?? NomeDoArquivo;

			if (!String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && (originalQuery != txtQuery.Text))
			{
				File.WriteAllText(NomeDoArquivo, txtQuery.Text);
				originalQuery = txtQuery.Text;
			}
			UpdateDisplay();

			return !String.IsNullOrWhiteSpace(NomeDoArquivo) && !NomeDoArquivo.StartsWith("Query") && File.Exists(NomeDoArquivo);
		}

		private void txtQuery_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyValue == 190) || (e.KeyValue == 194))
				AutoCompletar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtQuery.SelectAll();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.S))
				Salvar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.T))
				ListarTabelas();
			else if (e.KeyCode == Keys.F5)
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Y))
				Executar();
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Space))
			{
				e.SuppressKeyPress = true;
				if ((txtQuery.SelectionStart > 0) && txtQuery.Text[txtQuery.SelectionStart - 1].Equals('.'))
					AutoCompletar();
				else
					ListarTabelas();
			}
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.R))
			{
				e.SuppressKeyPress = true;
				(new ExpressaoRegularBuilder()).ShowDialog();
			}
		}

		private void UpdateDisplay()
		{
			Text = Path.GetFileName(NomeDoArquivo) + (txtQuery.Text != originalQuery ? " *" : "");
		}

		private void txtQuery_TextChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void Colorir()
		{
			txtQuery.FilterAutoComplete = false;
			txtQuery.Seperators.Add(' ');
			txtQuery.Seperators.Add('\r');
			txtQuery.Seperators.Add('\n');
			txtQuery.Seperators.Add('\t');
			txtQuery.Seperators.Add(',');
			txtQuery.Seperators.Add('.');
			txtQuery.Seperators.Add('*');
			txtQuery.Seperators.Add('/');
			txtQuery.Seperators.Add('-');
			txtQuery.Seperators.Add('+');
			txtQuery.Seperators.Add('(');
			txtQuery.Seperators.Add(')');

			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Select", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Distinct", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("From", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Inner", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Left", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Right", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Outter", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Join", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Where", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Group", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Order", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Between", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Case", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("When", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Having", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Update", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Set", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Insert Into", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Delete", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Truncate", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Drop", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Table", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Column", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("End", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Then", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Else", Color.Blue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Null", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Is", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("As", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("In", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("On", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("And", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Or", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Not", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Like", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Union", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("By", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Asc", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("Desc", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("=", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor(">", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<=", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("=>", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("<>", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("!=", Color.CadetBlue, null, DescriptorType.Word, DescriptorRecognition.WholeWord, true));

			txtQuery.HighlightDescriptors.Add(new HighlightDescriptor("/*", "*/", Color.Green, null, DescriptorType.ToCloseToken, DescriptorRecognition.StartsWith, false));
			txtQuery.Colorir();
		}

		private INavegador FindNavegador()
		{
			return (FindForm() as INavegador) ?? NavegadorNulo.Instancia;
		}

		private void btBinding_Click(object sender, EventArgs e)
		{
			Binding();
		}

		public void Executar()
		{
			var query = QueryAtiva;
			if (!String.IsNullOrWhiteSpace(query))
			{
				try
				{
					var form = FindForm() as INavegador;
					if ((form != null) && form.SalvarAoExecutar)
						Salvar();

					query = Util.ConverterParametrosEmConstantes(txtQuery.Text, query);
					dgResult.DataSource = null;
					BancoDeDados.Executar(query);
					Binding();
					tcResultados.SelectedIndex = 1;
					dgResult.Focus();
					dgResult.AutoResizeColumns();
				}
				catch (Exception vException)
				{
					MessageBox.Show("Houve um problema ao executar a query. Detalhes:\n" + vException.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		public void Binding()
		{
			var result = BancoDeDados.Transformar();
			if (dgResult.DataSource == null)
			{
				var lista = result.ToList();
				dgResult.Enabled = lista.Count > 0;
				dgResult.DataSource = (lista.Count == 0) ? BancoDeDados.Cabecalho().ToList() : lista;
				if (lista.Count == 0)
					MessageBox.Show("A query não retornou resultados!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				var linha = dgResult.FirstDisplayedScrollingRowIndex;
				dgResult.DataSource = (dgResult.DataSource as IEnumerable<Object>).Union(result).ToList();
				if (linha >= 0)
					dgResult.FirstDisplayedScrollingRowIndex = linha;
			}
		}

		private void ListarTabelas()
		{
			try
			{
				var apelido = txtQuery.ObterPrefixo();
				var campos = BancoDeDados.ListarTabelas(apelido);
				ListaDeCampos.Exibir(campos, this, txtQuery.CurrentCharacterPosition(), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
		}

		private void AutoCompletar()
		{
			try
			{
				var apelido = Util.ObterApelidoAntesDoPonto(txtQuery.Text, txtQuery.SelectionStart);
				var tabela = Util.ObterNomeTabelaPorApelido(txtQuery.Text, txtQuery.SelectionStart, apelido);
				var campos = BancoDeDados.ListarColunasDasTabelas(tabela);
				ListaDeCampos.Exibir(campos, this, txtQuery.CurrentCharacterPosition(), OnSelecionarAutoCompletar);
			}
			catch (Exception) { }
		}

		private void OnSelecionarAutoCompletar(String item)
		{
			if (!String.IsNullOrWhiteSpace(item))
			{
				var old = Clipboard.GetText();
				Clipboard.SetText(item);
				txtQuery.Paste();
				if (!String.IsNullOrWhiteSpace(old))
					Clipboard.SetText(old);
			}
			txtQuery.Focus();
		}

		public Boolean PodeFechar()
		{
			Boolean fechar = true;

			if (txtQuery.Text != originalQuery)
			{
				var dialogResult = MessageBox.Show(String.Format("O arquivo '{0}' foi alterado. Deseja Salvá-lo?", NomeDoArquivo), "Confirmação", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (dialogResult == DialogResult.Yes)
					fechar = Salvar();
				else
					fechar = (dialogResult == DialogResult.No);
			}

			return fechar;
		}

		public void Fechar()
		{
			if (_bancoDeDados != null)
			{
				_bancoDeDados.Dispose();
				_bancoDeDados = null;
			}

			originalQuery = null;

			txtQuery.Clear();
			txtQuery.Dispose();

			dgResult.DataSource = null;
			dgResult.Dispose();

			base.Dispose();
			GC.Collect();
		}

		public new Boolean Focus()
		{
			return txtQuery.Focus();
		}
	}
}