using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]

namespace ProgrammingTask1
{
    public partial class AllocationsForm: Form
    {
        // Variables.
        Configuration configuration;
        TaskAllocations taskAllocations;
      
        public AllocationsForm()
        {
            InitializeComponent();

            // Open File Dialog to accept TAFF files only.
            fileDialog = new OpenFileDialog
            {
                Title = "Select Task Allocation File (TAFF)",
                DefaultExt = "taff",
                Filter = "Task Allocation (*.taff)|*.taff",
                RestoreDirectory = true
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Resetting the data.
            ClearData();

            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string taffFilename = fileDialog.FileName;

                taskAllocations = new TaskAllocations(taffFilename);
                configuration = new Configuration();

                Boolean CffFileValid;
                Boolean TaffFileValid;

                if (taskAllocations.GetCffFilename())
                {
                    // Validating both the files.
                    CffFileValid = configuration.Validate(taskAllocations.CffFilename);
                    TaffFileValid = taskAllocations.Validate();

                    // Both the file are valid.
                    if (CffFileValid && TaffFileValid)
                    {                             

                        // Enable the Allocation menu.
                        allocationsToolStripMenuItem.Enabled = true;   
                    }
                    else
                    {

                        // Populate the errors for both files.
                        // Combined the errors of TaskAllocations to Configuration errors.
                        configuration.Errors.AddRange(taskAllocations.Errors);                 
                   
                    }

                    // Appending the error list to the log file.
                    AppendToLogfile();

                    //Displaying the output in GUI.
                    webBrowser1.DocumentText = taskAllocations.ShowAllocations(CffFileValid, TaffFileValid);
                }

               
            }
           
        }

        // Method to display the about box form.
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        // Method to display the errors box form.
        private void errorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorsForm errorWindow = new ErrorsForm();

            errorWindow.AddErrors(configuration.Errors);
            errorWindow.Show();
        }
        
        // Method to exit the application.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Method to validate the allocations for valid TAFF and CFF file.
        private void allocationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ValidateAllocations allocationsWindow = new ValidateAllocations();

            List<string> allocationsErrors = new List<string>();

            // Validating each allocation in the list of allocations.
            foreach (Allocation allocation in taskAllocations.Allocations)
            {
                List<string> errors = allocation.ValidateAllocation();

                allocationsErrors.AddRange(errors);
            }

            // Displaying the validated allocations error list.
            allocationsWindow.AddErrors(allocationsErrors);
            allocationsWindow.Show();
        }

        // Method to clear the data on GUI.
        private void ClearData()
        {
            allocationsToolStripMenuItem.Enabled = false;        
        }

        // Method to append the errors list to the log file.
        private void AppendToLogfile()
        {
           File.AppendAllLines(configuration.LogFilename, configuration.Errors);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
