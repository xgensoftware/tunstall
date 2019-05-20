namespace UnitTestRequest
{
    partial class frmSearch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUnitId = new System.Windows.Forms.TextBox();
            this.txtIMEI = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSerialNum = new System.Windows.Forms.TextBox();
            this.grdSearchResult = new System.Windows.Forms.DataGridView();
            this.UnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IMEI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Serial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestMode = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUnitType = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.grdSearchResult)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(435, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unit Id:";
            this.label1.Visible = false;
            // 
            // txtUnitId
            // 
            this.txtUnitId.Location = new System.Drawing.Point(482, 80);
            this.txtUnitId.Name = "txtUnitId";
            this.txtUnitId.Size = new System.Drawing.Size(252, 20);
            this.txtUnitId.TabIndex = 1;
            this.txtUnitId.Visible = false;
            // 
            // txtIMEI
            // 
            this.txtIMEI.Location = new System.Drawing.Point(73, 36);
            this.txtIMEI.Name = "txtIMEI";
            this.txtIMEI.Size = new System.Drawing.Size(252, 20);
            this.txtIMEI.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IMEI:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mode:";
            // 
            // cmbMode
            // 
            this.cmbMode.FormattingEnabled = true;
            this.cmbMode.Location = new System.Drawing.Point(73, 84);
            this.cmbMode.Name = "cmbMode";
            this.cmbMode.Size = new System.Drawing.Size(252, 21);
            this.cmbMode.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Serial #:";
            // 
            // txtSerialNum
            // 
            this.txtSerialNum.Location = new System.Drawing.Point(73, 60);
            this.txtSerialNum.Name = "txtSerialNum";
            this.txtSerialNum.Size = new System.Drawing.Size(252, 20);
            this.txtSerialNum.TabIndex = 7;
            // 
            // grdSearchResult
            // 
            this.grdSearchResult.AllowUserToAddRows = false;
            this.grdSearchResult.AllowUserToDeleteRows = false;
            this.grdSearchResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdSearchResult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grdSearchResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdSearchResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UnitId,
            this.UnitType,
            this.IMEI,
            this.Serial,
            this.TestMode});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdSearchResult.DefaultCellStyle = dataGridViewCellStyle2;
            this.grdSearchResult.Location = new System.Drawing.Point(14, 111);
            this.grdSearchResult.Name = "grdSearchResult";
            this.grdSearchResult.ReadOnly = true;
            this.grdSearchResult.Size = new System.Drawing.Size(759, 397);
            this.grdSearchResult.TabIndex = 8;
            // 
            // UnitId
            // 
            this.UnitId.DataPropertyName = "UNIT_ID";
            this.UnitId.HeaderText = "Unit";
            this.UnitId.Name = "UnitId";
            this.UnitId.ReadOnly = true;
            // 
            // UnitType
            // 
            this.UnitType.DataPropertyName = "OTHER";
            this.UnitType.HeaderText = "Unit Type";
            this.UnitType.Name = "UnitType";
            this.UnitType.ReadOnly = true;
            // 
            // IMEI
            // 
            this.IMEI.DataPropertyName = "IMEI";
            this.IMEI.HeaderText = "IMEI";
            this.IMEI.Name = "IMEI";
            this.IMEI.ReadOnly = true;
            // 
            // Serial
            // 
            this.Serial.DataPropertyName = "SERIALNO";
            this.Serial.HeaderText = "Serial";
            this.Serial.Name = "Serial";
            this.Serial.ReadOnly = true;
            // 
            // TestMode
            // 
            this.TestMode.DataPropertyName = "TEST";
            this.TestMode.HeaderText = "Test Status";
            this.TestMode.Name = "TestMode";
            this.TestMode.ReadOnly = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(615, 514);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(696, 514);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Unit Type:";
            // 
            // txtUnitType
            // 
            this.txtUnitType.Location = new System.Drawing.Point(73, 12);
            this.txtUnitType.Name = "txtUnitType";
            this.txtUnitType.Size = new System.Drawing.Size(252, 20);
            this.txtUnitType.TabIndex = 12;
            // 
            // frmSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 545);
            this.Controls.Add(this.txtUnitType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.grdSearchResult);
            this.Controls.Add(this.txtSerialNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIMEI);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUnitId);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSearch";
            this.Text = "Search Units";
            ((System.ComponentModel.ISupportInitialize)(this.grdSearchResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUnitId;
        private System.Windows.Forms.TextBox txtIMEI;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSerialNum;
        private System.Windows.Forms.DataGridView grdSearchResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitType;
        private System.Windows.Forms.DataGridViewTextBoxColumn IMEI;
        private System.Windows.Forms.DataGridViewTextBoxColumn Serial;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TestMode;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUnitType;
    }
}