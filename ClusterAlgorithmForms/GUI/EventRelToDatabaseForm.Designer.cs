namespace ClusterAlgorithmForms.GUI
{
    partial class EventRelToDatabaseForm
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
            this.label7 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.tableComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(291, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 17);
            this.label7.TabIndex = 174;
            this.label7.Text = "label7";
            this.label7.Visible = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(396, 35);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 169;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(340, 35);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 168;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // tableComboBox
            // 
            this.tableComboBox.FormattingEnabled = true;
            this.tableComboBox.Location = new System.Drawing.Point(86, 35);
            this.tableComboBox.Name = "tableComboBox";
            this.tableComboBox.Size = new System.Drawing.Size(143, 25);
            this.tableComboBox.TabIndex = 163;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 162;
            this.label2.Text = "目标表名：";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(396, 6);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(50, 23);
            this.addButton.TabIndex = 161;
            this.addButton.Text = "选择";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 160;
            this.label1.Text = "文件路径：";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(86, 6);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(304, 23);
            this.pathTextBox.TabIndex = 159;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(235, 35);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(50, 23);
            this.clearButton.TabIndex = 175;
            this.clearButton.Text = "清空";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // EventRelToDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 70);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.tableComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EventRelToDatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "事件关系入库";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ComboBox tableComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button clearButton;
    }
}