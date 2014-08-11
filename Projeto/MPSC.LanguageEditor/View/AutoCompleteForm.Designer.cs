using System.Collections.Specialized;

namespace MPSC.LanguageEditor
{
	/// <summary>
	/// Summary description for AutoCompleteForm.
	/// </summary>
	public partial class AutoCompleteForm : System.Windows.Forms.Form
	{
		private StringCollection mItems = new StringCollection();
		private System.Windows.Forms.ListView lstCompleteItems;
		private System.Windows.Forms.ColumnHeader columnHeader1;



		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;



		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstCompleteItems = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// lstCompleteItems
			// 
			this.lstCompleteItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.lstCompleteItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstCompleteItems.FullRowSelect = true;
			this.lstCompleteItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lstCompleteItems.HideSelection = false;
			this.lstCompleteItems.LabelWrap = false;
			this.lstCompleteItems.Location = new System.Drawing.Point(0, 0);
			this.lstCompleteItems.MultiSelect = false;
			this.lstCompleteItems.Name = "lstCompleteItems";
			this.lstCompleteItems.Size = new System.Drawing.Size(128, 174);
			this.lstCompleteItems.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lstCompleteItems.TabIndex = 1;
			this.lstCompleteItems.UseCompatibleStateImageBehavior = false;
			this.lstCompleteItems.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 148;
			// 
			// AutoCompleteForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(128, 174);
			this.ControlBox = false;
			this.Controls.Add(this.lstCompleteItems);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(128, 176);
			this.MinimizeBox = false;
			this.Name = "AutoCompleteForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "AutoCompleteForm";
			this.TopMost = true;
			this.VisibleChanged += new System.EventHandler(this.AutoCompleteForm_VisibleChanged);
			this.Resize += new System.EventHandler(this.AutoCompleteForm_Resize);
			this.ResumeLayout(false);

		}
		#endregion

	}
}
