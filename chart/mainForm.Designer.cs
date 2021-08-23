namespace MathChart
{
    partial class mainForm
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
            this.bAddLine = new System.Windows.Forms.Button();
            this.bDeleteLine = new System.Windows.Forms.Button();
            this.bBuildLine = new System.Windows.Forms.Button();
            this.bEditPoint = new System.Windows.Forms.Button();
            this.DataLinesInfo = new MathChart.TableList.TableListView();
            this.DataTablePoints = new MathChart.TableList.TableListView();
            this.MainChart = new MathChart.Chart.Chart();
            this.SuspendLayout();
            // 
            // AddData
            // 
            this.AddData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddData.Location = new System.Drawing.Point(815, 21);
            this.AddData.Name = "AddData";
            this.AddData.Size = new System.Drawing.Size(246, 23);
            this.AddData.TabIndex = 1;
            this.AddData.Text = "Add data line";
            this.AddData.UseVisualStyleBackColor = true;
            this.AddData.Click += new System.EventHandler(this.AddData_click);
            // 
            // bAddLine
            // 
            this.bAddLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bAddLine.Location = new System.Drawing.Point(28, 656);
            this.bAddLine.Name = "bAddLine";
            this.bAddLine.Size = new System.Drawing.Size(123, 23);
            this.bAddLine.TabIndex = 4;
            this.bAddLine.Text = "Добавить кривую";
            this.bAddLine.UseVisualStyleBackColor = true;
            this.bAddLine.Click += new System.EventHandler(this.bAddLine_Click);
            // 
            // bDeleteLine
            // 
            this.bDeleteLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bDeleteLine.Location = new System.Drawing.Point(157, 656);
            this.bDeleteLine.Name = "bDeleteLine";
            this.bDeleteLine.Size = new System.Drawing.Size(102, 23);
            this.bDeleteLine.TabIndex = 5;
            this.bDeleteLine.Text = "Удалить кривую";
            this.bDeleteLine.UseVisualStyleBackColor = true;
            // 
            // bBuildLine
            // 
            this.bBuildLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bBuildLine.Location = new System.Drawing.Point(265, 656);
            this.bBuildLine.Name = "bBuildLine";
            this.bBuildLine.Size = new System.Drawing.Size(120, 23);
            this.bBuildLine.TabIndex = 6;
            this.bBuildLine.Text = "Построить кривую";
            this.bBuildLine.UseVisualStyleBackColor = true;
            // 
            // bEditPoint
            // 
            this.bEditPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bEditPoint.Location = new System.Drawing.Point(391, 656);
            this.bEditPoint.Name = "bEditPoint";
            this.bEditPoint.Size = new System.Drawing.Size(135, 23);
            this.bEditPoint.TabIndex = 7;
            this.bEditPoint.Text = "Редактировать точки";
            this.bEditPoint.UseVisualStyleBackColor = true;
            // 
            // DataLinesInfo
            // 
            this.DataLinesInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataLinesInfo.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.DataLinesInfo.BorderColor = System.Drawing.Color.Black;
            this.DataLinesInfo.ColumnHeaderBack = System.Drawing.Color.White;
            this.DataLinesInfo.ColumnHeaderHeigth = 20;
            this.DataLinesInfo.CountRow = 0;
            this.DataLinesInfo.HearderFont = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DataLinesInfo.Location = new System.Drawing.Point(28, 482);
            this.DataLinesInfo.Name = "DataLinesInfo";
            this.DataLinesInfo.NumerableRows = true;
            this.DataLinesInfo.RowHeaderWidth = 33;
            this.DataLinesInfo.SelectionCellColor = System.Drawing.Color.Gray;
            this.DataLinesInfo.SelectionHeaderColor = System.Drawing.Color.Empty;
            this.DataLinesInfo.Size = new System.Drawing.Size(1033, 168);
            this.DataLinesInfo.TabIndex = 3;
            // 
            // DataTablePoints
            // 
            this.DataTablePoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTablePoints.BackColor = System.Drawing.SystemColors.ControlDark;
            this.DataTablePoints.BorderColor = System.Drawing.Color.Black;
            this.DataTablePoints.ColumnHeaderBack = System.Drawing.Color.White;
            this.DataTablePoints.ColumnHeaderHeigth = 20;
            this.DataTablePoints.CountRow = 0;
            this.DataTablePoints.HearderFont = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DataTablePoints.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.DataTablePoints.Location = new System.Drawing.Point(815, 50);
            this.DataTablePoints.Name = "DataTablePoints";
            this.DataTablePoints.NumerableRows = true;
            this.DataTablePoints.RowHeaderWidth = 33;
            this.DataTablePoints.SelectionCellColor = System.Drawing.SystemColors.ButtonFace;
            this.DataTablePoints.SelectionHeaderColor = System.Drawing.SystemColors.ControlLight;
            this.DataTablePoints.Size = new System.Drawing.Size(246, 410);
            this.DataTablePoints.TabIndex = 2;
            // 
            // MainChart
            // 
            this.MainChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainChart.AutoSize = true;
            this.MainChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainChart.Location = new System.Drawing.Point(28, 21);
            this.MainChart.Name = "MainChart";
            this.MainChart.Size = new System.Drawing.Size(770, 439);
            this.MainChart.TabIndex = 0;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1082, 691);
            this.Controls.Add(this.bEditPoint);
            this.Controls.Add(this.bBuildLine);
            this.Controls.Add(this.bDeleteLine);
            this.Controls.Add(this.bAddLine);
            this.Controls.Add(this.DataLinesInfo);
            this.Controls.Add(this.DataTablePoints);
            this.Controls.Add(this.AddData);
            this.Controls.Add(this.MainChart);
            this.Name = "mainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Chart.Chart MainChart;
        private System.Windows.Forms.Button AddData;
        private TableList.TableListView DataTablePoints;
        private TableList.TableListView DataLinesInfo;
        private System.Windows.Forms.Button bAddLine;
        private System.Windows.Forms.Button bDeleteLine;
        private System.Windows.Forms.Button bBuildLine;
        private System.Windows.Forms.Button bEditPoint;
    }
}

