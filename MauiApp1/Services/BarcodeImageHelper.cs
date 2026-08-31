using ZXing.Net.Maui;

namespace MauiApp1.Services
{
    /*vibe code malalaaaaaaaaa sa ano to naghahandle ng barcode*/
    public static class BarcodeImageHelper
    {
        private static readonly Dictionary<Image, int> _requestTokens = new();

        public static async Task SetBarcodeAsync(Image target, string? value)
        {
            int token;
            lock (_requestTokens)
            {
                token = _requestTokens.TryGetValue(target, out var current) ? current + 1 : 1;
                _requestTokens[target] = token;
            }

            if (string.IsNullOrEmpty(value))
            {
                await ApplyIfStillCurrent(target, token, null);
                return;
            }

            var bytes = await TryGenerateAsync(value);

            if (bytes == null)
            {
                // One quiet retry - covers transient issues (e.g. a hiccup on
                // first camera/renderer init) without bothering the user.
                await Task.Delay(250);
                bytes = await TryGenerateAsync(value);
            }

            var source = bytes != null
                ? ImageSource.FromStream(() => new MemoryStream(bytes))
                : null;

            await ApplyIfStillCurrent(target, token, source);
        }

        private static async Task<byte[]?> TryGenerateAsync(string value)
        {
            try
            {
                using var stream = new MemoryStream();

                await BarcodeGenerator.WriteToStreamAsync(
                    value,
                    stream,
                    new BarcodeGeneratorOptions
                    {
                        Format = BarcodeFormat.Code128,
                        Width = 300,
                        Height = 120,
                        Margin = 2,
                        ForegroundColor = Colors.Black,
                        BackgroundColor = Colors.White
                    },
                    new BarcodeImageOptions
                    {
                        Format = BarcodeImageFormat.Png
                    });

                return stream.ToArray();
            }
            catch (Exception ex)
            {
                // Previously this vanished silently because the caller used
                // `_ = RefreshBarcodeAsync();`. Now it's at least visible in
                // the debug output so a real, repeated failure can be diagnosed.
                System.Diagnostics.Debug.WriteLine(
                    $"[BarcodeImageHelper] Failed to generate barcode for '{value}': {ex}");
                return null;
            }
        }

        private static async Task ApplyIfStillCurrent(Image target, int token, ImageSource? source)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                lock (_requestTokens)
                {
                    // A newer request for this same Image has already been
                    // issued since this one started - drop this result instead
                    // of letting a stale/out-of-order completion win.
                    if (!_requestTokens.TryGetValue(target, out var current) || current != token)
                        return;

                    target.Source = source;
                }
            });
        }
    }
}
