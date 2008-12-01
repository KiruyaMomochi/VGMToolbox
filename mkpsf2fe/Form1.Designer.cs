﻿namespace mkpsf2fe
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tbSourceDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSourceDirectoryBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbModulesDirectory = new System.Windows.Forms.TextBox();
            this.btnModulesDirectoryBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSourceDirectory
            // 
            this.tbSourceDirectory.Location = new System.Drawing.Point(12, 23);
            this.tbSourceDirectory.Name = "tbSourceDirectory";
            this.tbSourceDirectory.Size = new System.Drawing.Size(235, 20);
            this.tbSourceDirectory.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source Directory";
            // 
            // btnSourceDirectoryBrowse
            // 
            this.btnSourceDirectoryBrowse.Location = new System.Drawing.Point(253, 23);
            this.btnSourceDirectoryBrowse.Name = "btnSourceDirectoryBrowse";
            this.btnSourceDirectoryBrowse.Size = new System.Drawing.Size(25, 20);
            this.btnSourceDirectoryBrowse.TabIndex = 2;
            this.btnSourceDirectoryBrowse.Text = "...";
            this.btnSourceDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnSourceDirectoryBrowse.Click += new System.EventHandler(this.btnSourceDirectoryBrowse_Click);
            // 
            // tbOutput
            // 
            this.tbOutput.AcceptsReturn = true;
            this.tbOutput.Location = new System.Drawing.Point(12, 185);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbOutput.Size = new System.Drawing.Size(266, 120);
            this.tbOutput.TabIndex = 3;
            this.tbOutput.Text = "Source Directory: Path to BD/HD/SQ files.\r\nModules Directory: Path to IRX files.\r" +
                "\n\r\nNotes: Be sure matching BD/HD/SQ files have the same file name.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log";
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(12, 312);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(266, 23);
            this.btnExecute.TabIndex = 5;
            this.btnExecute.Text = "Go!";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Modules Directory";
            // 
            // tbModulesDirectory
            // 
            this.tbModulesDirectory.Location = new System.Drawing.Point(12, 62);
            this.tbModulesDirectory.Name = "tbModulesDirectory";
            this.tbModulesDirectory.Size = new System.Drawing.Size(235, 20);
            this.tbModulesDirectory.TabIndex = 7;
            // 
            // btnModulesDirectoryBrowse
            // 
            this.btnModulesDirectoryBrowse.Location = new System.Drawing.Point(253, 62);
            this.btnModulesDirectoryBrowse.Name = "btnModulesDirectoryBrowse";
            this.btnModulesDirectoryBrowse.Size = new System.Drawing.Size(25, 21);
            this.btnModulesDirectoryBrowse.TabIndex = 8;
            this.btnModulesDirectoryBrowse.Text = "...";
            this.btnModulesDirectoryBrowse.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 76);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(43, 15);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(19, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "reverb";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(72, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "depth";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(106, 15);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(41, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "16383";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 340);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnModulesDirectoryBrowse);
            this.Controls.Add(this.tbModulesDirectory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.btnSourceDirectoryBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSourceDirectory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "mkpsf2FE";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSourceDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSourceDirectoryBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbModulesDirectory;
        private System.Windows.Forms.Button btnModulesDirectoryBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
    }
}

