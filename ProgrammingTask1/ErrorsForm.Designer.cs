
namespace ProgrammingTask1
{
    partial class ErrorsForm
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
            this.errorsWebBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // errorsWebBrowser
            // 
            this.errorsWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorsWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.errorsWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.errorsWebBrowser.Name = "errorsWebBrowser";
            this.errorsWebBrowser.Size = new System.Drawing.Size(800, 450);
            this.errorsWebBrowser.TabIndex = 0;
            this.errorsWebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // ErrorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.errorsWebBrowser);
            this.Name = "ErrorsForm";
            this.Text = "Errors";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser errorsWebBrowser;
    }
}