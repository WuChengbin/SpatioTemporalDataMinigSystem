namespace MarineSTMiningSystem
{
    partial class MarineHeatwavesTimeAnomalyForm
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
            this.selectButton1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inPathTextBox = new System.Windows.Forms.TextBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.selectButton3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.outPathTextBox = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.selectButton2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.normalTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // selectButton1
            // 
            this.selectButton1.Location = new System.Drawing.Point(417, 6);
            this.selectButton1.Name = "selectButton1";
            this.selectButton1.Size = new System.Drawing.Size(50, 23);
            this.selectButton1.TabIndex = 66;
            this.selectButton1.Text = "选择";
            this.selectButton1.UseVisualStyleBackColor = true;
            this.selectButton1.Click += new System.EventHandler(this.selectButton1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 65;
            this.label1.Text = "输入文件夹：";
            // 
            // inPathTextBox
            // 
            this.inPathTextBox.Location = new System.Drawing.Point(98, 6);
            this.inPathTextBox.Name = "inPathTextBox";
            this.inPathTextBox.Size = new System.Drawing.Size(313, 23);
            this.inPathTextBox.TabIndex = 64;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(417, 93);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 63;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 93);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(343, 23);
            this.progressBar1.TabIndex = 62;
            this.progressBar1.Visible = false;
            // 
            // selectButton3
            // 
            this.selectButton3.Location = new System.Drawing.Point(417, 64);
            this.selectButton3.Name = "selectButton3";
            this.selectButton3.Size = new System.Drawing.Size(50, 23);
            this.selectButton3.TabIndex = 61;
            this.selectButton3.Text = "选择";
            this.selectButton3.UseVisualStyleBackColor = true;
            this.selectButton3.Click += new System.EventHandler(this.selectButton3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 60;
            this.label2.Text = "输出文件夹：";
            // 
            // outPathTextBox
            // 
            this.outPathTextBox.Location = new System.Drawing.Point(98, 64);
            this.outPathTextBox.Name = "outPathTextBox";
            this.outPathTextBox.Size = new System.Drawing.Size(313, 23);
            this.outPathTextBox.TabIndex = 59;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(361, 93);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 58;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // selectButton2
            // 
            this.selectButton2.Location = new System.Drawing.Point(417, 35);
            this.selectButton2.Name = "selectButton2";
            this.selectButton2.Size = new System.Drawing.Size(50, 23);
            this.selectButton2.TabIndex = 69;
            this.selectButton2.Text = "选择";
            this.selectButton2.UseVisualStyleBackColor = true;
            this.selectButton2.Click += new System.EventHandler(this.selectButton2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 17);
            this.label3.TabIndex = 68;
            this.label3.Text = "常值文件夹：";
            // 
            // normalTextBox
            // 
            this.normalTextBox.Location = new System.Drawing.Point(98, 35);
            this.normalTextBox.Name = "normalTextBox";
            this.normalTextBox.Size = new System.Drawing.Size(313, 23);
            this.normalTextBox.TabIndex = 67;
            // 
            // MarineHeatwavesTimeAnomalyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 131);
            this.Controls.Add(this.selectButton2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.normalTextBox);
            this.Controls.Add(this.selectButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inPathTextBox);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.selectButton3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.outPathTextBox);
            this.Controls.Add(this.okBtn);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MarineHeatwavesTimeAnomalyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "时间距平";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inPathTextBox;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button selectButton3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outPathTextBox;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button selectButton2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox normalTextBox;
    }
}