﻿namespace bb_sim
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
            this.p1f0 = new System.Windows.Forms.ComboBox();
            this.p1f1 = new System.Windows.Forms.ComboBox();
            this.p1f2 = new System.Windows.Forms.ComboBox();
            this.p1f3 = new System.Windows.Forms.ComboBox();
            this.p1f4 = new System.Windows.Forms.ComboBox();
            this.p2f4 = new System.Windows.Forms.ComboBox();
            this.p2f3 = new System.Windows.Forms.ComboBox();
            this.p2f2 = new System.Windows.Forms.ComboBox();
            this.p2f1 = new System.Windows.Forms.ComboBox();
            this.p2f0 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // p1f0
            // 
            this.p1f0.FormattingEnabled = true;
            this.p1f0.Location = new System.Drawing.Point(16, 116);
            this.p1f0.Name = "p1f0";
            this.p1f0.Size = new System.Drawing.Size(189, 21);
            this.p1f0.Sorted = true;
            this.p1f0.TabIndex = 0;
            // 
            // p1f1
            // 
            this.p1f1.FormattingEnabled = true;
            this.p1f1.Location = new System.Drawing.Point(211, 116);
            this.p1f1.Name = "p1f1";
            this.p1f1.Size = new System.Drawing.Size(189, 21);
            this.p1f1.Sorted = true;
            this.p1f1.TabIndex = 1;
            // 
            // p1f2
            // 
            this.p1f2.FormattingEnabled = true;
            this.p1f2.Location = new System.Drawing.Point(406, 116);
            this.p1f2.Name = "p1f2";
            this.p1f2.Size = new System.Drawing.Size(189, 21);
            this.p1f2.Sorted = true;
            this.p1f2.TabIndex = 2;
            // 
            // p1f3
            // 
            this.p1f3.FormattingEnabled = true;
            this.p1f3.Location = new System.Drawing.Point(601, 116);
            this.p1f3.Name = "p1f3";
            this.p1f3.Size = new System.Drawing.Size(189, 21);
            this.p1f3.Sorted = true;
            this.p1f3.TabIndex = 3;
            // 
            // p1f4
            // 
            this.p1f4.FormattingEnabled = true;
            this.p1f4.Location = new System.Drawing.Point(796, 116);
            this.p1f4.Name = "p1f4";
            this.p1f4.Size = new System.Drawing.Size(189, 21);
            this.p1f4.Sorted = true;
            this.p1f4.TabIndex = 4;
            // 
            // p2f4
            // 
            this.p2f4.FormattingEnabled = true;
            this.p2f4.Location = new System.Drawing.Point(796, 34);
            this.p2f4.Name = "p2f4";
            this.p2f4.Size = new System.Drawing.Size(189, 21);
            this.p2f4.Sorted = true;
            this.p2f4.TabIndex = 9;
            // 
            // p2f3
            // 
            this.p2f3.FormattingEnabled = true;
            this.p2f3.Location = new System.Drawing.Point(601, 34);
            this.p2f3.Name = "p2f3";
            this.p2f3.Size = new System.Drawing.Size(189, 21);
            this.p2f3.Sorted = true;
            this.p2f3.TabIndex = 8;
            // 
            // p2f2
            // 
            this.p2f2.FormattingEnabled = true;
            this.p2f2.Location = new System.Drawing.Point(406, 34);
            this.p2f2.Name = "p2f2";
            this.p2f2.Size = new System.Drawing.Size(189, 21);
            this.p2f2.Sorted = true;
            this.p2f2.TabIndex = 7;
            // 
            // p2f1
            // 
            this.p2f1.FormattingEnabled = true;
            this.p2f1.Location = new System.Drawing.Point(211, 34);
            this.p2f1.Name = "p2f1";
            this.p2f1.Size = new System.Drawing.Size(189, 21);
            this.p2f1.Sorted = true;
            this.p2f1.TabIndex = 6;
            // 
            // p2f0
            // 
            this.p2f0.FormattingEnabled = true;
            this.p2f0.Location = new System.Drawing.Point(16, 34);
            this.p2f0.Name = "p2f0";
            this.p2f0.Size = new System.Drawing.Size(189, 21);
            this.p2f0.Sorted = true;
            this.p2f0.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(476, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Player 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(476, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Player 1";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(465, 198);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 12;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 261);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.p2f4);
            this.Controls.Add(this.p2f3);
            this.Controls.Add(this.p2f2);
            this.Controls.Add(this.p2f1);
            this.Controls.Add(this.p2f0);
            this.Controls.Add(this.p1f4);
            this.Controls.Add(this.p1f3);
            this.Controls.Add(this.p1f2);
            this.Controls.Add(this.p1f1);
            this.Controls.Add(this.p1f0);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox p1f0;
        private System.Windows.Forms.ComboBox p1f1;
        private System.Windows.Forms.ComboBox p1f2;
        private System.Windows.Forms.ComboBox p1f3;
        private System.Windows.Forms.ComboBox p1f4;
        private System.Windows.Forms.ComboBox p2f4;
        private System.Windows.Forms.ComboBox p2f3;
        private System.Windows.Forms.ComboBox p2f2;
        private System.Windows.Forms.ComboBox p2f1;
        private System.Windows.Forms.ComboBox p2f0;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button startButton;
    }
}

