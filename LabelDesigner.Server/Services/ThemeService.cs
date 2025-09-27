using Microsoft.JSInterop;

namespace LabelDesigner.Server.Services
{
    public class ThemeService
    {
        private readonly IJSRuntime _js;
        public string CurrentTheme { get; private set; } = "light";

        public event Action? OnChange;

        public ThemeService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task InitializeAsync()
        {
            CurrentTheme = await _js.InvokeAsync<string>("themeManager.getTheme");
            await SetTheme(CurrentTheme);
        }

        public async Task SetTheme(string theme)
        {
            CurrentTheme = theme;
            await _js.InvokeVoidAsync("themeManager.setTheme", theme);
            OnChange?.Invoke();
        }
    }
}
