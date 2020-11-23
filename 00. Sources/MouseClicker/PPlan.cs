using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace MouseClicker
{
    public class PPlan
    {
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public UInt32 cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        /// <summary>
        /// 작동 예정 시간
        /// </summary>
        public DateTime ReserveTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 마우스 X 좌표
        /// </summary>
        public int PosX { get; set; } = 0;

        /// <summary>
        /// 마우스 Y 좌표
        /// </summary>
        public int PosY { get; set; } = 0;

        /// <summary>
        /// 클릭 횟수
        /// </summary>
        public int Count { get; set; } = 1;
        
        /// <summary>
        /// 간격 (ms)
        /// </summary>
        public int Interval { get; set; } = 100;

        /// <summary>
        /// 입력 문자열
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 날짜는 무시되고 시간으로만 반응
        /// </summary>
        public bool IgnoreDay { get; set; } = false;

        public PPlan()
        {
            ThreadPool.SetMinThreads(1, 1);
        }

        public PPlan(DateTime reserveTime, int posX, int posY, int count, int interval, string message, bool ignoreDay)
        {
            this.ReserveTime = reserveTime;
            this.PosX = posX;
            this.PosY = posY;
            this.Count = count;
            this.Interval = interval;
            this.Message = message;
            this.IgnoreDay = ignoreDay;
        }

        public bool IsNeedAction(DateTime dateTime)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(-5000);

            if (IgnoreDay)
                span = dateTime.TimeOfDay - ReserveTime.TimeOfDay;
            else
                span = dateTime - ReserveTime;

            if (span > TimeSpan.FromMilliseconds(-125) && span < TimeSpan.FromMilliseconds(125))
                return true;

            return false;
        }

        public void Act()
        {
            ThreadPool.QueueUserWorkItem(DoAction);

            //Thread actThread = new Thread(DoAction);
            //actThread.Start();
        }

        private void DoAction(object obj)
        {
            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < Count; i++)
            {
                try
                {
                    sw.Restart();

                    Cursor.Position = new System.Drawing.Point(PosX, PosY);

                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    Thread.Sleep(25);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(25);

                    if (Message != String.Empty)
                        SendKeys.SendWait(Message);

                    sw.Stop();

                    int remainInterval = Interval - (int)sw.ElapsedMilliseconds;
                    if (remainInterval > 0)
                        Thread.Sleep(remainInterval);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
