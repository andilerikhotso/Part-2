using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Part_1_poe
{
    public partial class MainWindow : Window
    {
        private List<Claim> claims = new List<Claim>();
        private string uploadedFilePath; // Store the path of the uploaded file
        private const string fileStorageDirectory = "UploadedClaims"; // Directory for storing uploaded files

        public MainWindow()
        {
            InitializeComponent();
            dgClaims.ItemsSource = claims; // Bind DataGrid to the claim list

            // Create the directory for storing uploaded files if it doesn't exist
            if (!Directory.Exists(fileStorageDirectory))
            {
                Directory.CreateDirectory(fileStorageDirectory);
            }
        }

        private void UploadDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*", // Allow any file type to be uploaded
                Title = "Select a Supporting Document"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(openFileDialog.FileName);
                uploadedFilePath = Path.Combine(fileStorageDirectory, fileName);

                // Copy the file to the storage directory
                File.Copy(openFileDialog.FileName, uploadedFilePath, true);

                txtFileName.Text = fileName; // Display file name on the form
            }
        }

        private void SubmitClaimButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate that required fields are not empty
            if (string.IsNullOrWhiteSpace(txtLecturerName.Text) ||
                string.IsNullOrWhiteSpace(txtHoursWorked.Text) ||
                string.IsNullOrWhiteSpace(txtHourlyRate.Text))
            {
                MessageBox.Show("Please fill in all required fields before submitting the claim.",
                                "Incomplete Information",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Collect the data from the form and create a new claim
            var newClaim = new Claim
            {
                ClaimID = claims.Count + 1,
                LecturerName = txtLecturerName.Text,
                HoursWorked = double.Parse(txtHoursWorked.Text),
                HourlyRate = double.Parse(txtHourlyRate.Text),
                AdditionalNotes = txtAdditionalNotes.Text,
                TotalAmount = double.Parse(txtHoursWorked.Text) * double.Parse(txtHourlyRate.Text),
                Status = "Pending",
                SubmittedDate = DateTime.Now,
                SupportingDocumentPath = uploadedFilePath // Link the file to the claim
            };

            claims.Add(newClaim);
            dgClaims.Items.Refresh();

            // Update the list of submitted claims to display details
            lstSubmittedClaims.Items.Add(
                $"Lecturer: {newClaim.LecturerName}, Hours: {newClaim.HoursWorked}, Rate: {newClaim.HourlyRate}, Notes: {newClaim.AdditionalNotes}, Total: {newClaim.TotalAmount:C}"
            );

            // Update the claim status tracker
            UpdateClaimStatusTracker();

            MessageBox.Show("Claim Submitted Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Clear the form fields after submission
            txtLecturerName.Clear();
            txtHoursWorked.Clear();
            txtHourlyRate.Clear();
            txtAdditionalNotes.Clear();
            txtFileName.Text = string.Empty;
            uploadedFilePath = string.Empty;
        }

        private void UpdateClaimStatusTracker()
        {
            lstClaimStatus.Items.Clear(); // Clear previous statuses
            foreach (var claim in claims)
            {
                lstClaimStatus.Items.Add($"Claim ID: {claim.ClaimID}, Lecturer: {claim.LecturerName}, Status: {claim.Status}, Submitted Date: {claim.SubmittedDate}");
            }
        }

        private void ApproveClaim_Click(object sender, RoutedEventArgs e)
        {
            // Approve the selected claim
            if (dgClaims.SelectedItem is Claim selectedClaim)
            {
                selectedClaim.Status = "Approved";
                dgClaims.Items.Refresh();
                UpdateClaimStatusTracker(); // Refresh the status tracker
                MessageBox.Show($"Claim {selectedClaim.ClaimID} has been approved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RejectClaim_Click(object sender, RoutedEventArgs e)
        {
            // Reject the selected claim
            if (dgClaims.SelectedItem is Claim selectedClaim)
            {
                selectedClaim.Status = "Rejected";
                dgClaims.Items.Refresh();
                UpdateClaimStatusTracker(); // Refresh the status tracker
                MessageBox.Show($"Claim {selectedClaim.ClaimID} has been rejected.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class Claim
    {
        public int ClaimID { get; set; }
        public string LecturerName { get; set; }
        public double HoursWorked { get; set; }
        public double HourlyRate { get; set; }
        public double TotalAmount { get; set; }
        public string AdditionalNotes { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string SupportingDocumentPath { get; set; }
    }
}
