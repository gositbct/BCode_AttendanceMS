using MauiApp1.Services;
using MauiApp1.ViewModels;
using ZXing.Net.Maui;
using static MauiApp1.Models.StudentModel;

namespace MauiApp1.Views.AdminPages;

public partial class AdminScanner : ContentPage
{

    //VIBE CODE HIRAP TO SCANNER EH
    private readonly DatabaseService _db = new();
    private readonly ScannerMethod _viewModel;

    // Keep the on-screen log from growing forever during a long scanning
    // session - only the most recent entries are useful to see at a glance.
    private const int MaxLogEntries = 20;

    // Guards against the handful of duplicate BarcodesDetected events that
    // fire in the split-second before IsDetecting actually turns off.
    private bool _isProcessing = false;

    public AdminScanner()
    {
        InitializeComponent();
        _viewModel = new ScannerMethod();
        BindingContext = _viewModel;
    }

    private async void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing)
            return;

        var result = e.Results.FirstOrDefault();
        if (result == null)
            return;

        _isProcessing = true;

        // Stop the camera from reading anything else until we're done
        // handling this scan and the admin is ready for the next one.
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            cameraBarcodeReaderView.IsDetecting = false;
        });

        string studentId = result.Value;

        try
        {
            Student? student = await _db.GetStudentById(studentId);

            if (student == null)
            {
                AddLogEntry(new ScanLogEntry
                {
                    StudentName = "Unrecognized barcode",
                    StudentId = studentId,
                    StatusText = "Not Found",
                    StatusColor = "#D32F2F", // red
                    TimeText = DateTime.Now.ToString("h:mm tt")
                });
            }
            else
            {
                AttendanceResult attendanceResult = await _db.HandleAttendance(studentId);

                var (statusText, statusColor) = attendanceResult switch
                {
                    AttendanceResult.TimeIn => ("Timed In", "#27AE60"),           // green
                    AttendanceResult.TimeOut => ("Timed Out", "#2F80ED"),         // blue
                    AttendanceResult.AlreadyCompleted => ("Already Done Today", "#F59E0B"), // orange
                    _ => ("Scanned", "#9E9E9E")
                };

                AddLogEntry(new ScanLogEntry
                {
                    StudentName = student.Name,
                    StudentId = student.Id,
                    StatusText = statusText,
                    StatusColor = statusColor,
                    TimeText = DateTime.Now.ToString("h:mm tt")
                });
            }
        }
        catch (Exception ex)
        {
            AddLogEntry(new ScanLogEntry
            {
                StudentName = "Scan failed",
                StudentId = studentId,
                StatusText = ex.Message,
                StatusColor = "#D32F2F", // red
                TimeText = DateTime.Now.ToString("h:mm tt")
            });
        }
        finally
        {
            // Resume scanning for the next student, even if something above failed.
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cameraBarcodeReaderView.IsDetecting = true;
            });

            _isProcessing = false;
        }
    }

    // Adds the scan result to the top of the visible log instead of
    // popping a DisplayAlert, so scanning a line of students isn't
    // interrupted by having to tap "OK" after every single scan.
    private void AddLogEntry(ScanLogEntry entry)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _viewModel.ScanLog.Insert(0, entry);

            while (_viewModel.ScanLog.Count > MaxLogEntries)
                _viewModel.ScanLog.RemoveAt(_viewModel.ScanLog.Count - 1);
        });
    }
}
