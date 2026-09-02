using Microsoft.Win32;

namespace KeepAwake;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private static readonly TimeSpan ActivityInterval = TimeSpan.FromSeconds(30);
    private static readonly string TestLogPath = Path.Combine(Path.GetTempPath(), "KeepAwake-test.log");

    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly ToolStripMenuItem _preventLockItem;
    private readonly ToolStripMenuItem _startupItem;
    private bool _preventLock = true;
    private bool _sessionLocked;

    public TrayApplicationContext()
    {
        Log("Application started");

        _preventLockItem = new ToolStripMenuItem("ロックを防止する")
        {
            Checked = true,
            CheckOnClick = true
        };
        _preventLockItem.CheckedChanged += (_, _) => SetPreventLock(_preventLockItem.Checked);

        _startupItem = new ToolStripMenuItem("Windows起動時に開始")
        {
            Checked = StartupManager.IsEnabled(),
            CheckOnClick = true
        };
        _startupItem.CheckedChanged += StartupItem_CheckedChanged;

        var exitItem = new ToolStripMenuItem("終了");
        exitItem.Click += (_, _) => ExitApplication();

        var menu = new ContextMenuStrip();
        menu.Items.Add(_preventLockItem);
        menu.Items.Add(_startupItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!)
                   ?? SystemIcons.Application,
            Text = "KeepAwake - ロック防止中",
            ContextMenuStrip = menu,
            Visible = true
        };

        _notifyIcon.DoubleClick += (_, _) => TogglePreventLock();

        _timer = new System.Windows.Forms.Timer
        {
            Interval = (int)ActivityInterval.TotalMilliseconds
        };
        _timer.Tick += (_, _) => KeepActive();

        SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

        UpdateTimerState();
    }

    private void SetPreventLock(bool enabled)
    {
        _preventLock = enabled;
        _preventLockItem.Checked = enabled;
        Log($"Prevent lock {(enabled ? "enabled" : "disabled")}");
        UpdateTimerState();
    }

    private void TogglePreventLock()
    {
        SetPreventLock(!_preventLock);
    }

    private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason == SessionSwitchReason.SessionLock)
        {
            _sessionLocked = true;
            Log("Session locked");
            UpdateTimerState();
        }
        else if (e.Reason == SessionSwitchReason.SessionUnlock)
        {
            _sessionLocked = false;
            Log("Session unlocked");
            UpdateTimerState();
        }
    }

    private void UpdateTimerState()
    {
        if (_preventLock && !_sessionLocked)
        {
            _timer.Start();
            KeepActive();
            _notifyIcon.Text = "KeepAwake - ロック防止中";
        }
        else
        {
            _timer.Stop();
            _notifyIcon.Text = _sessionLocked && _preventLock
                ? "KeepAwake - ロック中"
                : "KeepAwake - 停止中";
        }
    }

    private void KeepActive()
    {
        if (!_preventLock || _sessionLocked)
            return;

        var sent = NativeMethods.SendTinyMouseMove();
        Log(sent ? "Mouse input sent" : "Mouse input failed");
    }

    private void StartupItem_CheckedChanged(object? sender, EventArgs e)
    {
        try
        {
            StartupManager.SetEnabled(_startupItem.Checked);
        }
        catch (Exception ex)
        {
            _startupItem.CheckedChanged -= StartupItem_CheckedChanged;
            _startupItem.Checked = !_startupItem.Checked;
            _startupItem.CheckedChanged += StartupItem_CheckedChanged;

            MessageBox.Show(
                $"スタートアップ設定の変更に失敗しました。\n\n{ex.Message}",
                "KeepAwake",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static void Log(string message)
    {
        try
        {
            File.AppendAllText(
                TestLogPath,
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}{Environment.NewLine}");
        }
        catch
        {
            // テスト用ログの失敗で本体動作を止めない。
        }
    }

    private void ExitApplication()
    {
        Log("Application exiting");
        SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
        _timer.Stop();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _timer.Dispose();
        ExitThread();
    }
}
