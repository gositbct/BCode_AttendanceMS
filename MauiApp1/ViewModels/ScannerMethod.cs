using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiApp1.ViewModels
{
    //VIBE CODE MALALA ULIIIIIIIIIIIIIIIIT
    public class ScanLogEntry
    {
        public string StudentName { get; set; }
        public string StudentId { get; set; }
        public string StatusText { get; set; }
        public string StatusColor { get; set; }
        public string TimeText { get; set; }
    }

    // Handles the "Back to Dashboard" button and holds the visible scan
    // log on AdminScanner.xaml. The actual barcode scan handling lives in
    // AdminScanner.xaml.cs's OnBarcodeDetected, since it needs direct
    // access to the camera view; it appends to ScanLog after each scan.
    public class ScannerMethod
    {
        // Newest scan first, so the admin can see recent results without
        // scrolling. Capped in AdminScanner.xaml.cs to avoid growing forever
        // during a long scanning session.
        public ObservableCollection<ScanLogEntry> ScanLog { get; } = new();

        public ICommand DashboardNav { get; }

        //SCANNING METHOD ETO YUNG TINATAWAG PAG MAG SSCAN
        public ScannerMethod()
        {
            DashboardNav = new Command(async () => await GoToDashboard());
        }

        //ROUTING LANG DIN SA BUTTON
        private async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("///AdminDashboard");
        }
    }
}
