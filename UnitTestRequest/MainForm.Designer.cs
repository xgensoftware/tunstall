namespace UnitTestRequest
{
    partial class MainForm
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
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.btnOnTest = new System.Windows.Forms.Button();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.btnOffTest = new System.Windows.Forms.Button();
            this.grdDevices = new System.Windows.Forms.DataGridView();
            this.UnitId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IMEI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Serial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestMode = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).BeginInit();
            this.SuspendLayout();
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.Location = new System.Drawing.Point(319, 12);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsole.Size = new System.Drawing.Size(352, 181);
            this.txtConsole.TabIndex = 0;
            // 
            // btnOnTest
            // 
            this.btnOnTest.Location = new System.Drawing.Point(12, 38);
            this.btnOnTest.Name = "btnOnTest";
            this.btnOnTest.Size = new System.Drawing.Size(96, 28);
            this.btnOnTest.TabIndex = 1;
            this.btnOnTest.Text = "Place on Test";
            this.btnOnTest.UseVisualStyleBackColor = true;
            this.btnOnTest.Visible = false;
            // 
            // txtUnit
            // 
            this.txtUnit.Location = new System.Drawing.Point(12, 12);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(301, 20);
            this.txtUnit.TabIndex = 2;
            // 
            // btnOffTest
            // 
            this.btnOffTest.Location = new System.Drawing.Point(150, 38);
            this.btnOffTest.Name = "btnOffTest";
            this.btnOffTest.Size = new System.Drawing.Size(96, 28);
            this.btnOffTest.TabIndex = 3;
            this.btnOffTest.Text = "Take off Test";
            this.btnOffTest.UseVisualStyleBackColor = true;
            this.btnOffTest.Visible = false;
            // 
            // grdDevices
            // 
            this.grdDevices.AllowUserToAddRows = false;
            this.grdDevices.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdDevices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grdDevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDevices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
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
            this.grdDevices.DefaultCellStyle = dataGridViewCellStyle2;
            this.grdDevices.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grdDevices.Location = new System.Drawing.Point(0, 199);
            this.grdDevices.Name = "grdDevices";
            this.grdDevices.ReadOnly = true;
            this.grdDevices.Size = new System.Drawing.Size(673, 348);
            this.grdDevices.TabIndex = 4;
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
            this.Serial.DataPropertyName = "SERIAL";
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 547);
            this.Controls.Add(this.grdDevices);
            this.Controls.Add(this.btnOffTest);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.btnOnTest);
            this.Controls.Add(this.txtConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.grdDevices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Button btnOnTest;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.Button btnOffTest;
        private System.Windows.Forms.DataGridView grdDevices;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitId;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitType;
        private System.Windows.Forms.DataGridViewTextBoxColumn IMEI;
        private System.Windows.Forms.DataGridViewTextBoxColumn Serial;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TestMode;
    }
}