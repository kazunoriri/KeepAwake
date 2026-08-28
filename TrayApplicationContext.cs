namespace KeepAwake;

internal sealed class TrayApplicationContext : ApplicationContext
{
    private static readonly TimeSpan ActivityInterval = TimeSpan.FromSeconds(30);

    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly ToolStripMenuItem _preventLockItem;
    private readonly ToolStripMenuItem _startupItem;
    private bool _preventLock = true;

    public TrayApplicationContext()
    {
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
        _timer.Start();

        KeepActive();
    }

    private void SetPreventLock(bool enabled)
    {
        _preventLock = enabled;
        _preventLockItem.Checked = enabled;

        if (enabled)
        {
            _timer.Start();
            KeepActive();
            _notifyIcon.Text = "KeepAwake - ロック防止中";
        }
        else
        {
            _timer.Stop();
            _notifyIcon.Text = "KeepAwake - 停止中";
        }
    }

    private void TogglePreventLock()
    {
        SetPreventLock(!_preventLock);
    }

    private void KeepActive()
    {
        if (!_preventLock)
            return;

        NativeMethods.SendTinyMouseMove();
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

    private void ExitApplication()
    {
        _timer.Stop();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _timer.Dispose();
        ExitThread();
    }
}
