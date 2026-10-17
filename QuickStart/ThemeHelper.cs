using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace QuickStart
{
    // 主题公共工具：系统明暗检测 + 逐窗口标题栏明暗（主窗体 / About 窗体 / 暗色弹框共用）
    static class ThemeHelper
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        // 系统当前是否暗色主题（UWP 应用明暗设置）；读不到按亮色
        public static bool IsSystemDarkMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value is int)
                        {
                            return (int)value == 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        // 标题栏明暗（官方 DWM 属性，Win10 1809+；20H1 起属性号 20，之前为 19）
        public static void ApplyTitleBarTheme(IntPtr hwnd, bool dark)
        {
            if (hwnd == IntPtr.Zero)
            {
                return;
            }

            try
            {
                int useDark = dark ? 1 : 0;
                int hr = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
                if (hr != 0)
                {
                    DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref useDark, sizeof(int));
                }
            }
            catch (EntryPointNotFoundException)
            {
                // 极旧系统无 dwmapi 该导出：忽略，仅标题栏不跟随
            }
            catch (Exception)
            {
            }
        }
    }
}
