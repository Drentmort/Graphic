namespace chart
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddData = new System.Windows.Forms.Button();
            this.chart1 = new chart.Chart.Chart();
            this.RectangleTransform = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AddData
            // 
            this.AddData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddData.Location = new System.Drawing.Point(648, 21);
            this.AddData.Name = "AddData";
            this.AddData.Size = new System.Drawing.Size(123, 23);
            this.AddData.TabIndex = 1;
            this.AddData.Text = "Add data line";
            this.AddData.UseVisualStyleBackColor = true;
            this.AddData.Click += new System.EventHandler(this.AddData_click);
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.AutoSize = true;
            this.chart1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chart1.Location = new System.Drawing.Point(28, 21);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(614, 507);
            this.chart1.TabIndex = 0;
            // 
            // RectangleTransform
            // 
            this.RectangleTransform.Location = new System.Drawing.Point(648, 50);
            this.RectangleTransform.Name = "RectangleTransform";
            this.RectangleTransform.Size = new System.Drawing.Size(123, 23);
            this.RectangleTransform.TabIndex = 2;
            this.RectangleTransform.Text = "Rectangle Transform";
            this.RectangleTransform.UseVisualStyleBackColor = true;
            this.RectangleTransform.Click += new System.EventHandler(this.RectangleTransform_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 561);
            this.Controls.Add(this.RectangleTransform);
            this.Controls.Add(this.AddData);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Chart.Chart chart1;
        private System.Windows.Forms.Button AddData;
        private System.Windows.Forms.Button RectangleTransform;
    }
}

