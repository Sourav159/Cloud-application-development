using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgrammingTask1
{
    public partial class ErrorsForm : Form
    {
        // Get property.
        public WebBrowser GetWebBrowser
        {
            get
            {
                return errorsWebBrowser;
            }
        }

        public ErrorsForm()
        {
            InitializeComponent();
        }

        // Method to add all the errors and display at ErrorsForm
        public void AddErrors(List<string> errors)
        {
            string errorList = string.Join(Environment.NewLine, errors.ToArray());
            string errorsToShow = $"<h3>Errors List</h3> <p>{errorList}</p>";

            errorsWebBrowser.DocumentText = errorsToShow;
            
        }

        // Method to add all the errors and display at ErrorsForm
        public void ClearErrors()
        {

            errorsWebBrowser.DocumentText = "";

        }



        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
