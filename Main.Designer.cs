namespace Feather32u4Programmer {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxComPort = new System.Windows.Forms.ComboBox();
            this.textBoxConsole = new System.Windows.Forms.TextBox();
            this.buttonProgram = new System.Windows.Forms.Button();
            this.buttonPortRefresh = new System.Windows.Forms.Button();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.labelFile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxComPort
            // 
            this.comboBoxComPort.FormattingEnabled = true;
            this.comboBoxComPort.Location = new System.Drawing.Point(133, 46);
            this.comboBoxComPort.Name = "comboBoxComPort";
            this.comboBoxComPort.Size = new System.Drawing.Size(111, 21);
            this.comboBoxComPort.TabIndex = 1;
            // 
            // textBoxConsole
            // 
            this.textBoxConsole.Location = new System.Drawing.Point(12, 74);
            this.textBoxConsole.Multiline = true;
            this.textBoxConsole.Name = "textBoxConsole";
            this.textBoxConsole.ReadOnly = true;
            this.textBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxConsole.Size = new System.Drawing.Size(1203, 538);
            this.textBoxConsole.TabIndex = 2;
            this.textBoxConsole.WordWrap = false;
            // 
            // buttonProgram
            // 
            this.buttonProgram.Location = new System.Drawing.Point(12, 618);
            this.buttonProgram.Name = "buttonProgram";
            this.buttonProgram.Size = new System.Drawing.Size(1203, 49);
            this.buttonProgram.TabIndex = 4;
            this.buttonProgram.Text = "Program";
            this.buttonProgram.UseVisualStyleBackColor = true;
            // 
            // buttonPortRefresh
            // 
            this.buttonPortRefresh.Location = new System.Drawing.Point(12, 43);
            this.buttonPortRefresh.Name = "buttonPortRefresh";
            this.buttonPortRefresh.Size = new System.Drawing.Size(111, 25);
            this.buttonPortRefresh.TabIndex = 5;
            this.buttonPortRefresh.Text = "Refresh";
            this.buttonPortRefresh.UseVisualStyleBackColor = true;
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(12, 12);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(111, 25);
            this.buttonSelectFile.TabIndex = 6;
            this.buttonSelectFile.Text = "Select File";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            // 
            // labelFile
            // 
            this.labelFile.AutoSize = true;
            this.labelFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFile.Location = new System.Drawing.Point(129, 13);
            this.labelFile.Name = "labelFile";
            this.labelFile.Size = new System.Drawing.Size(152, 20);
            this.labelFile.TabIndex = 7;
            this.labelFile.Text = "Select Firmware File";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 679);
            this.Controls.Add(this.labelFile);
            this.Controls.Add(this.buttonSelectFile);
            this.Controls.Add(this.buttonPortRefresh);
            this.Controls.Add(this.buttonProgram);
            this.Controls.Add(this.textBoxConsole);
            this.Controls.Add(this.comboBoxComPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxComPort;
        private System.Windows.Forms.TextBox textBoxConsole;
        private System.Windows.Forms.Button buttonProgram;
        private System.Windows.Forms.Button buttonPortRefresh;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.Label labelFile;
    }
}

