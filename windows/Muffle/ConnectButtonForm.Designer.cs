
namespace Muffle
{
    partial class ConnectButtonForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.buttonPair = new System.Windows.Forms.Button();
            this.baudSelector = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(354, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please ensure the button is unplugged, then press the Pair button.";
            // 
            // buttonPair
            // 
            this.buttonPair.Location = new System.Drawing.Point(41, 41);
            this.buttonPair.Name = "buttonPair";
            this.buttonPair.Size = new System.Drawing.Size(86, 28);
            this.buttonPair.TabIndex = 1;
            this.buttonPair.Text = "Pair";
            this.buttonPair.UseVisualStyleBackColor = true;
            this.buttonPair.Click += new System.EventHandler(this.buttonPair_Click);
            // 
            // baudSelector
            // 
            this.baudSelector.FormattingEnabled = true;
            this.baudSelector.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "31250",
            "38400",
            "57600",
            "115200"});
            this.baudSelector.Location = new System.Drawing.Point(165, 45);
            this.baudSelector.Name = "baudSelector";
            this.baudSelector.Size = new System.Drawing.Size(166, 23);
            this.baudSelector.TabIndex = 2;
            // 
            // ConnectButtonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 103);
            this.Controls.Add(this.baudSelector);
            this.Controls.Add(this.buttonPair);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectButtonForm";
            this.Text = "ConnectButtonForm";
            this.Load += new System.EventHandler(this.ConnectButtonForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonPair;
        private System.Windows.Forms.ComboBox baudSelector;
    }
}