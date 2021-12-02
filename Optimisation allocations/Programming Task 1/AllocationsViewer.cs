using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PT1;
using System.Linq;
using WcfServiceLibrary;
using System.Threading;
using System.ServiceModel;

namespace AllocationsApplication
{
    partial class AllocationsViewerForm : Form
    {
        #region properties
        private Allocations PT1Allocations;
        private Configuration PT1Configuration;
        private ErrorsViewer ErrorListViewer = new ErrorsViewer();
        #endregion

        #region constructors
        public AllocationsViewerForm()
        {
            InitializeComponent();

            this.Text += String.Format(" ({0})", Application.ProductVersion);
        }
        #endregion

        #region File menu event handlers
        private void OpenAllocationsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearGUI();

            // Process allocations and configuration files.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get both filenames.
                String allocationsFileName = openFileDialog1.FileName;
                String configurationFileName = Allocations.ConfigurationFileName(allocationsFileName);

                // Parse configuration file.
                if (configurationFileName == null)
                    PT1Configuration = new Configuration();
                else
                {
                    using (WebClient configWebClient = new WebClient())
                    using (Stream configStream = configWebClient.OpenRead(configurationFileName))
                    using (StreamReader configFile = new StreamReader(configStream))
                    {
                        Configuration.TryParse(configFile, configurationFileName,
                            out PT1Configuration, out List<String> configurationErrors);
                    }
                }

                // Parse allocations file.
                using (WebClient allocationsWebClient = new WebClient())
                using (Stream allocationsStream = allocationsWebClient.OpenRead(allocationsFileName))
                using (StreamReader allocationsFile = new StreamReader(allocationsStream))
                {
                    Allocations.TryParse(allocationsFile, allocationsFileName, PT1Configuration,
                        out PT1Allocations, out List<String> allocationsErrors);
                }

                // Refesh GUI and Log errors.
                UpdateGUI();
                PT1Allocations.LogFileErrors(PT1Allocations.FileErrorsTXT);
                PT1Allocations.LogFileErrors(PT1Configuration.FileErrorsTXT);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region  Clear and Update GUI
        private void ClearGUI()
        {
            // As we are opening a Configuration file,
            // indicate allocations are not valid, and clear GUI.
            allocationToolStripMenuItem.Enabled = false;

            if (allocationWebBrowser.Document != null)
                allocationWebBrowser.Document.OpenNew(true);
            allocationWebBrowser.DocumentText = String.Empty;

            if (ErrorListViewer.WebBrowser.Document != null)
                ErrorListViewer.WebBrowser.Document.OpenNew(true);
            ErrorListViewer.WebBrowser.DocumentText = String.Empty;
        }

        private void UpdateGUI()
        {
            // Update GUI:
            // - enable menu
            // - display Allocations data (whether valid or invalid)
            // - display Allocations and Configuration file errors.
            if (PT1Allocations != null && PT1Allocations.FileValid &&
                PT1Configuration != null && PT1Configuration.FileValid)
                allocationToolStripMenuItem.Enabled = true;

            if (allocationWebBrowser.Document != null)
                allocationWebBrowser.Document.OpenNew(true);
            if (ErrorListViewer.WebBrowser.Document != null)
                ErrorListViewer.WebBrowser.Document.OpenNew(true);

            if (PT1Allocations != null)
            {
                allocationWebBrowser.DocumentText = PT1Allocations.ToStringHTML();
                ErrorListViewer.WebBrowser.DocumentText =
                    PT1Allocations.FileErrorsHTML +
                    PT1Configuration.FileErrorsHTML +
                    PT1Allocations.AllocationsErrorsHTML;
            }
        }
        #endregion

        #region Validate menu event handlers
        private void AllocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the allocations are valid.
            PT1Allocations.Validate();

            // Update GUI - display allocations file data (whether valid or invalid), 
            // allocations file errors, config file errors, and allocation errors.
            allocationWebBrowser.DocumentText = PT1Allocations.ToStringHTML();
            ErrorListViewer.WebBrowser.DocumentText =
                PT1Allocations.FileErrorsHTML +
                PT1Configuration.FileErrorsHTML +
                PT1Allocations.AllocationsErrorsHTML;

            // Log errors.
            PT1Allocations.LogFileErrors(PT1Allocations.AllocationsErrorsTXT);
        }
        #endregion

        #region View menu event handlers
        private void ErrorListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorListViewer.WindowState = FormWindowState.Normal;
            ErrorListViewer.Show();
            ErrorListViewer.Activate();
        }
        #endregion

        #region Help menu event handlers
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AboutBox.ShowDialog();
        }
        #endregion

        #region Constant deadline value of 5 mins (300000 milliseconds)
        const int DEADLINE = 300000;
        #endregion

        #region AutoResetEvent for waiting.
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        #endregion

        #region Collection of results.
        List<AllocationsData> allocationsList;
        #endregion

        #region Counters for completed and timed-out WCFS operations.
        int numberOfCalls;
        int completedCalls;
        int localTimeouts;
        int remoteTimeouts;
        int commExceptions;
        int webExceptions;
        #endregion

        #region Readonly object for locking.
        readonly object aLock = new object();
        #endregion

        #region Generate Allocations
        private void generateAllocationsButton_Click(object sender, EventArgs e)
        {
            String CffFilePath = comboBox1.Text;

            using (WebClient configWebClient = new WebClient())
            using (Stream configStream = configWebClient.OpenRead(CffFilePath))
            using (StreamReader configFile = new StreamReader(configStream))
            {
                Configuration.TryParse(configFile, CffFilePath,
                    out PT1Configuration, out List<String> configurationErrors);
            }

            // WCFS library's data contract object
            ConfigData configData = new ConfigData();

            configData.Duration = PT1Configuration.Program.Duration;
            configData.NumberOfTasks = PT1Configuration.Program.Tasks;
            configData.NumberOfProcessors = PT1Configuration.Program.Processors;
            configData.Energies = PT1Configuration.Energies;
            configData.Runtimes = PT1Configuration.Runtimes;
            configData.ProcessorUploadSpeed = PT1Configuration.ProcessorUploadSpeed;
            configData.ProcessorDownloadSpeed = PT1Configuration.ProcessorDownloadSpeed;
            configData.ProcessorRAM = PT1Configuration.ProcessorRAM;
            configData.TaskDownloadSpeed = PT1Configuration.TaskDownloadSpeed;
            configData.TaskUploadSpeed = PT1Configuration.TaskUploadSpeed;
            configData.TaskRAM = PT1Configuration.TaskRAM;
            configData.FilePath = Path.GetFileName(PT1Configuration.FilePath);
            configData.LocalCommunication = PT1Configuration.LocalCommunication;
            configData.RemoteCommunication = PT1Configuration.RemoteCommunication;

            // Create 2nd thread.
            System.Threading.Tasks.Task.Run(() => RemoteALBAsyncCalls(configData));

            // GUI thread waits for 5 mins.
            autoResetEvent.WaitOne(DEADLINE);

            // Process all results that are returned within 5 mins and find best allocation with min energy.
            lock (aLock)
            {
                AllocationsData bestAllocation = allocationsList[0];

                foreach (AllocationsData allocation in allocationsList)
                {
                   if (allocation.Energy < bestAllocation.Energy)
                    {
                        bestAllocation = allocation;
                    }
                }

                Allocations.TryParse(bestAllocation.Description, PT1Configuration,
                                      out PT1Allocations, out List<String> allocationsErrors);
            }

            // Refesh GUI.
            UpdateGUI();
        }
        #endregion

        #region Remote ALB Async calls
        private void RemoteALBAsyncCalls(ConfigData configData)
        {
            // Create WCFS objects.
            using (RemoteALBGreedy.ServiceClient remoteGreedyWS = new RemoteALBGreedy.ServiceClient())
            using (RemoteALBHeuristic.ServiceClient remoteHeuristicWS = new RemoteALBHeuristic.ServiceClient())
            {
                // Set the event handler.
                remoteGreedyWS.GetAllocationsCompleted += RemoteGreedyWS_GetAllocationsCompleted1;
                remoteHeuristicWS.GetHeuristicAllocationsCompleted += RemoteHeuristicWS_GetHeuristicAllocationsCompleted1;

                // Initiliase results collection and counters.
                lock (aLock)
                {
                    allocationsList = new List<AllocationsData>();
                    numberOfCalls = 3;
                    completedCalls = 0;
                    localTimeouts = 0;
                    remoteTimeouts = 0;
                    commExceptions = 0;
                    webExceptions = 0;
                }

                // Asynchronous calls.
                for (int ID = 0; ID < numberOfCalls; ID++)
                {
                    remoteGreedyWS.GetAllocationsAsync(configData, DEADLINE);
                    remoteHeuristicWS.GetHeuristicAllocationsAsync(configData, DEADLINE);
                }

            }
        }

        private void RemoteHeuristicWS_GetHeuristicAllocationsCompleted1(object sender, RemoteALBHeuristic.GetHeuristicAllocationsCompletedEventArgs e)
        {
            try
            {
                lock (aLock)
                {
                    AllocationsData data = e.Result;
                    completedCalls++;

                    if (data != null)
                    {
                        allocationsList.Add(data);
                    }

                    // If all completed, stop waiting.
                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Local sendTimeout exception.
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    localTimeouts++;

                    // Display the exception message
                    MessageBox.Show("Local Timeout Exception");

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }

            }
            // Remote timeout exception.
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    remoteTimeouts++;

                    MessageBox.Show("Remote Timeout Exception - " + fex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Communication exception.
            catch (Exception ex) when (ex.InnerException is CommunicationException cex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    commExceptions++;

                    MessageBox.Show("Comm Exception - " + cex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Web exception.
            catch (Exception ex) when (ex.InnerException is WebException wex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    webExceptions++;

                    MessageBox.Show("Web Exception - " + wex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
        }

        private void RemoteGreedyWS_GetAllocationsCompleted1(object sender, RemoteALBGreedy.GetAllocationsCompletedEventArgs e)
        {
            try
            {
                lock (aLock)
                {
                    AllocationsData data = e.Result;
                    completedCalls++;

                    if (data != null)
                    {
                        allocationsList.Add(data);
                    }

                    // If all completed, stop waiting.
                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Local sendTimeout exception.
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    localTimeouts++;

                    // Display the exception message
                    MessageBox.Show("Local Timeout Exception");

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }

            }
            // Remote timeout exception.
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    remoteTimeouts++;

                    MessageBox.Show("Remote Timeout Exception - " + fex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Communication exception.
            catch (Exception ex) when (ex.InnerException is CommunicationException cex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    commExceptions++;

                    MessageBox.Show("Comm Exception - " + cex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
            // Web exception.
            catch (Exception ex) when (ex.InnerException is WebException wex)
            {
                lock (aLock)
                {
                    completedCalls++;
                    webExceptions++;

                    MessageBox.Show("Web Exception - " + wex.Message);

                    if (completedCalls == numberOfCalls)
                        autoResetEvent.Set();
                }
            }
        }
        #endregion
    }
}
