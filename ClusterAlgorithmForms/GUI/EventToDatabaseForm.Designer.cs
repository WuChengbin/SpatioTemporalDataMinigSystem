namespace ClusterAlgorithmForms.GUI
{
    partial class EventToDatabaseForm
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
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.aimTableComboBox = new System.Windows.Forms.ComboBox();
            this.corrTableComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableFieldComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.fileFieldComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.geoTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(86, 6);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(304, 23);
            this.pathTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "文件路径：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(396, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "选择";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "存储表名：";
            // 
            // aimTableComboBox
            // 
            this.aimTableComboBox.FormattingEnabled = true;
            this.aimTableComboBox.Location = new System.Drawing.Point(86, 35);
            this.aimTableComboBox.Name = "aimTableComboBox";
            this.aimTableComboBox.Size = new System.Drawing.Size(106, 25);
            this.aimTableComboBox.TabIndex = 5;
            // 
            // corrTableComboBox
            // 
            this.corrTableComboBox.FormattingEnabled = true;
            this.corrTableComboBox.Location = new System.Drawing.Point(328, 35);
            this.corrTableComboBox.Name = "corrTableComboBox";
            this.corrTableComboBox.Size = new System.Drawing.Size(118, 25);
            this.corrTableComboBox.TabIndex = 7;
            this.corrTableComboBox.SelectedIndexChanged += new System.EventHandler(this.corrTableComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(254, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "关联表名：";
            // 
            // tableFieldComboBox
            // 
            this.tableFieldComboBox.FormattingEnabled = true;
            this.tableFieldComboBox.Location = new System.Drawing.Point(328, 66);
            this.tableFieldComboBox.Name = "tableFieldComboBox";
            this.tableFieldComboBox.Size = new System.Drawing.Size(118, 25);
            this.tableFieldComboBox.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(242, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "表关联字段：";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(396, 97);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 149;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(340, 97);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 148;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // fileFieldComboBox
            // 
            this.fileFieldComboBox.FormattingEnabled = true;
            this.fileFieldComboBox.Location = new System.Drawing.Point(110, 66);
            this.fileFieldComboBox.Name = "fileFieldComboBox";
            this.fileFieldComboBox.Size = new System.Drawing.Size(113, 25);
            this.fileFieldComboBox.TabIndex = 152;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 17);
            this.label5.TabIndex = 151;
            this.label5.Text = "文件关联字段：";
            // 
            // geoTextBox
            // 
            this.geoTextBox.BackColor = System.Drawing.Color.White;
            this.geoTextBox.Location = new System.Drawing.Point(110, 97);
            this.geoTextBox.Name = "geoTextBox";
            this.geoTextBox.ReadOnly = true;
            this.geoTextBox.Size = new System.Drawing.Size(113, 23);
            this.geoTextBox.TabIndex = 157;
            this.geoTextBox.Text = "space";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 17);
            this.label6.TabIndex = 156;
            this.label6.Text = "空间字段名称：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(229, 100);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 17);
            this.label7.TabIndex = 158;
            this.label7.Text = "label7";
            this.label7.Visible = false;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(198, 35);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(50, 23);
            this.clearButton.TabIndex = 159;
            this.clearButton.Text = "清空";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // EventToDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 131);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.geoTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.fileFieldComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.tableFieldComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.corrTableComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.aimTableComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EventToDatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "事件入库";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox aimTableComboBox;
        private System.Windows.Forms.ComboBox corrTableComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox tableFieldComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ComboBox fileFieldComboBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox geoTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button clearButton;
    }
}