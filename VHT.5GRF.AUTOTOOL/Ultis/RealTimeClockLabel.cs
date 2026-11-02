using System;
using System.Drawing;
using System.Windows.Forms;

namespace _5GAutoTool
{
    public class RealTimeClockLabel : Label
    {
        private readonly Timer _timer;

        public string TimeFormat { get; set; } = "HH:mm:ss";

        public RealTimeClockLabel()
        {
            AutoSize = true;
            Font = new Font("Segoe UI", 14, FontStyle.Bold);
            TextAlign = ContentAlignment.MiddleCenter;

            _timer = new Timer();
            _timer.Interval = 1000; // cập nhật mỗi giây
            _timer.Tick += (s, e) => { Text = DateTime.Now.ToString(TimeFormat); };
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode) // tránh chạy trong chế độ thiết kế
            {
                Text = DateTime.Now.ToString(TimeFormat);
                _timer.Start();
            }
        }

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
    }
}
