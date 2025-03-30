namespace EntropicZest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.txtBinSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLoadExcel = new System.Windows.Forms.Button();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.txtStatistics = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lstDistribution = new System.Windows.Forms.RichTextBox();
            this.btnSaveResults = new System.Windows.Forms.Button();
            this.btnCompareResults = new System.Windows.Forms.Button();
            this.txtEntropyDifference = new System.Windows.Forms.RichTextBox();
            this.btnLoadResults = new System.Windows.Forms.Button();
            this.chartDistribution = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnBuildGraph = new System.Windows.Forms.Button();
            this.panelGraph = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.chartDistribution)).BeginInit();
            this.panelGraph.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtBinSize
            // 
            resources.ApplyResources(this.txtBinSize, "txtBinSize");
            this.txtBinSize.Name = "txtBinSize";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnLoadExcel
            // 
            resources.ApplyResources(this.btnLoadExcel, "btnLoadExcel");
            this.btnLoadExcel.Name = "btnLoadExcel";
            this.btnLoadExcel.UseVisualStyleBackColor = true;
            this.btnLoadExcel.Click += new System.EventHandler(this.btnLoadExcel_Click);
            // 
            // btnCalculate
            // 
            resources.ApplyResources(this.btnCalculate, "btnCalculate");
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // txtStatistics
            // 
            resources.ApplyResources(this.txtStatistics, "txtStatistics");
            this.txtStatistics.Name = "txtStatistics";
            this.txtStatistics.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lstDistribution
            // 
            resources.ApplyResources(this.lstDistribution, "lstDistribution");
            this.lstDistribution.Name = "lstDistribution";
            this.lstDistribution.ReadOnly = true;
            // 
            // btnSaveResults
            // 
            resources.ApplyResources(this.btnSaveResults, "btnSaveResults");
            this.btnSaveResults.Name = "btnSaveResults";
            this.btnSaveResults.UseVisualStyleBackColor = true;
            this.btnSaveResults.Click += new System.EventHandler(this.btnSaveResults_Click);
            // 
            // btnCompareResults
            // 
            resources.ApplyResources(this.btnCompareResults, "btnCompareResults");
            this.btnCompareResults.Name = "btnCompareResults";
            this.btnCompareResults.UseVisualStyleBackColor = true;
            this.btnCompareResults.Click += new System.EventHandler(this.btnCompareResults_Click);
            // 
            // txtEntropyDifference
            // 
            resources.ApplyResources(this.txtEntropyDifference, "txtEntropyDifference");
            this.txtEntropyDifference.Name = "txtEntropyDifference";
            this.txtEntropyDifference.ReadOnly = true;
            // 
            // btnLoadResults
            // 
            resources.ApplyResources(this.btnLoadResults, "btnLoadResults");
            this.btnLoadResults.Name = "btnLoadResults";
            this.btnLoadResults.UseVisualStyleBackColor = true;
            this.btnLoadResults.Click += new System.EventHandler(this.btnLoadResults_Click);
            // 
            // chartDistribution
            // 
            chartArea2.Name = "ChartArea1";
            this.chartDistribution.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chartDistribution.Legends.Add(legend2);
            resources.ApplyResources(this.chartDistribution, "chartDistribution");
            this.chartDistribution.Name = "chartDistribution";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chartDistribution.Series.Add(series2);
            // 
            // btnBuildGraph
            // 
            resources.ApplyResources(this.btnBuildGraph, "btnBuildGraph");
            this.btnBuildGraph.Name = "btnBuildGraph";
            this.btnBuildGraph.UseVisualStyleBackColor = true;
            this.btnBuildGraph.Click += new System.EventHandler(this.btnBuildGraph_Click);
            // 
            // panelGraph
            // 
            resources.ApplyResources(this.panelGraph, "panelGraph");
            this.panelGraph.Controls.Add(this.chartDistribution);
            this.panelGraph.Name = "panelGraph";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelGraph);
            this.Controls.Add(this.btnBuildGraph);
            this.Controls.Add(this.btnLoadResults);
            this.Controls.Add(this.txtEntropyDifference);
            this.Controls.Add(this.btnCompareResults);
            this.Controls.Add(this.btnSaveResults);
            this.Controls.Add(this.lstDistribution);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtStatistics);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.btnLoadExcel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBinSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.chartDistribution)).EndInit();
            this.panelGraph.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBinSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadExcel;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.RichTextBox txtStatistics;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox lstDistribution;
        private System.Windows.Forms.Button btnSaveResults;
        private System.Windows.Forms.Button btnCompareResults;
        private System.Windows.Forms.RichTextBox txtEntropyDifference;
        private System.Windows.Forms.Button btnLoadResults;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDistribution;
        private System.Windows.Forms.Button btnBuildGraph;
        private System.Windows.Forms.Panel panelGraph;
    }
}

