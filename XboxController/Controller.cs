using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace XboxController
{
    public class Controller
    {

        #region dll imports
        // set mouse position
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        // get mouse position
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        // key press
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        const int KEYEVENTF_KEYUP = 0x0002;

        // get foreground windowhandle
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // mouse
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;

        public const int MOUSEEVENTF_WHEEL = 0x0800;
        #endregion

        private bool running;
        public float sensetivity { get; set; }
        private int ups;
        private int sleepInterval;

        private Window1 keyboardWindow;

        #region BUTTONS
        public byte[] buttons = new byte[16];
        private bool[] buttonsDown = new bool[16];
        #endregion

        #region BUTTON INDICES
        public readonly int BUTTON_A_INDEX = 0;
        public readonly int BUTTON_B_INDEX = 1;
        public readonly int BUTTON_X_INDEX = 2;
        public readonly int BUTTON_Y_INDEX = 3;

        public readonly int BUTTON_UP_INDEX = 4;
        public readonly int BUTTON_RIGHT_INDEX = 5;
        public readonly int BUTTON_DOWN_INDEX = 6;
        public readonly int BUTTON_LEFT_INDEX = 7;

        public readonly int BUTTON_LEFT_ANALOG_INDEX = 8;
        public readonly int BUTTON_RIGHT_ANALOG_INDEX = 9;

        public readonly int BUTTON_LEFT_BUMPER_INDEX = 10;
        public readonly int BUTTON_RIGHT_BUMPER_INDEX = 11;
        public readonly int BUTTON_LEFT_TRIGGER_INDEX = 12;
        public readonly int BUTTON_RIGHT_TRIGGER_INDEX = 13;

        public readonly int BUTTON_BACK_INDEX = 14;
        public readonly int BUTTON_START_INDEX = 15;
        #endregion

        #region functions
        // when adding a function it must be added to commandButtons in MainWindow.xaml.cs too
        private Dictionary<byte, Action> functions = new Dictionary<byte, Action>();

        public readonly byte LEFTCLICK = 0x01;
        public readonly byte RIGHTCLICK = 0x02;
        public readonly byte SCROLLUP = 0x03;
        public readonly byte SCROLLDOWN = 0x04;
        public readonly byte ONSCREENKEYBOARD = 0x05;
        #endregion

        public Controller()
        {
            // init arrays
            for (int i = 0; i < buttonsDown.Length; i++)
            {
                buttons[i] = 0;
                buttonsDown[i] = false;
            }

            // add functions
            functions.Add(LEFTCLICK, LeftClick);
            functions.Add(RIGHTCLICK, RightClick);
            functions.Add(SCROLLUP, ScrollUp);
            functions.Add(SCROLLDOWN, ScrollDown);
            functions.Add(ONSCREENKEYBOARD, StartOnScreenKeyboard);
        }

        public void Start()
        {
            ReadSettings();
            running = true;
            sensetivity = 1.0f;
            ups = 60;
            sleepInterval = (int)(1000.0f / ups);
            new Thread(() => Run()).Start();

            keyboardWindow = new Window1();
            keyboardWindow.Show();
        }

        private void Run()
        {
            double xa = 0;
            double ya = 0;

            float dt = 0;
            Stopwatch watch = Stopwatch.StartNew();
            while (running)
            {
                GamePadState currentState = GamePad.GetState(PlayerIndex.One);

                if (currentState.IsConnected)
                {
                    if (currentState.ThumbSticks.Left.Y != 0 || currentState.ThumbSticks.Left.X != 0)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            keyboardWindow.SelectPoly(Math.Atan2(currentState.ThumbSticks.Left.Y, currentState.ThumbSticks.Left.X));
                        }));
                    }

                    xa += currentState.ThumbSticks.Right.X * sensetivity * dt;
                    ya += currentState.ThumbSticks.Right.Y * sensetivity * dt;
                    POINT lpPoint;
                    GetCursorPos(out lpPoint);
                    SetCursorPos(lpPoint.X + (int)Math.Truncate(xa), lpPoint.Y - (int)Math.Truncate(ya));
                    xa -= Math.Truncate(xa);
                    ya -= Math.Truncate(ya);

                    if (currentState.ThumbSticks.Left.Y != 0) Scroll(currentState.ThumbSticks.Left.Y * dt * 5);

                    if (currentState.Buttons.A == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_A_INDEX]) KeyDown(BUTTON_A_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_A_INDEX]) KeyUp(BUTTON_A_INDEX);
                    }
                    if (currentState.Buttons.B == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_B_INDEX]) KeyDown(BUTTON_B_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_B_INDEX]) KeyUp(BUTTON_B_INDEX);
                    }
                    if (currentState.Buttons.X == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_X_INDEX]) KeyDown(BUTTON_X_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_X_INDEX]) KeyUp(BUTTON_X_INDEX);
                    }
                    if (currentState.Buttons.Y == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_Y_INDEX]) KeyDown(BUTTON_Y_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_Y_INDEX]) KeyUp(BUTTON_Y_INDEX);
                    }
                    if (currentState.Buttons.LeftStick == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_LEFT_ANALOG_INDEX]) KeyDown(BUTTON_LEFT_ANALOG_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_LEFT_ANALOG_INDEX]) KeyUp(BUTTON_LEFT_ANALOG_INDEX);
                    }
                    if (currentState.Buttons.RightStick == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_RIGHT_ANALOG_INDEX]) KeyDown(BUTTON_RIGHT_ANALOG_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_RIGHT_ANALOG_INDEX]) KeyUp(BUTTON_RIGHT_ANALOG_INDEX);
                    }
                    if (currentState.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_LEFT_BUMPER_INDEX]) KeyDown(BUTTON_LEFT_BUMPER_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_LEFT_BUMPER_INDEX]) KeyUp(BUTTON_LEFT_BUMPER_INDEX);
                    }
                    if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_RIGHT_BUMPER_INDEX]) KeyDown(BUTTON_RIGHT_BUMPER_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_RIGHT_BUMPER_INDEX]) KeyUp(BUTTON_RIGHT_BUMPER_INDEX);
                    }
                    if (currentState.Buttons.Back == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_BACK_INDEX]) KeyDown(BUTTON_BACK_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_BACK_INDEX]) KeyUp(BUTTON_BACK_INDEX);
                    }
                    if (currentState.Buttons.Start == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_START_INDEX]) KeyDown(BUTTON_START_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_START_INDEX]) KeyUp(BUTTON_START_INDEX);
                    }
                    if (currentState.DPad.Up == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_UP_INDEX]) KeyDown(BUTTON_UP_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_UP_INDEX]) KeyUp(BUTTON_UP_INDEX);
                    }
                    if (currentState.DPad.Right == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_RIGHT_INDEX]) KeyDown(BUTTON_RIGHT_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_RIGHT_INDEX]) KeyUp(BUTTON_RIGHT_INDEX);
                    }
                    if (currentState.DPad.Down == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_DOWN_INDEX]) KeyDown(BUTTON_DOWN_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_DOWN_INDEX]) KeyUp(BUTTON_DOWN_INDEX);
                    }
                    if (currentState.DPad.Left == ButtonState.Pressed)
                    {
                        if (!buttonsDown[BUTTON_LEFT_INDEX]) KeyDown(BUTTON_LEFT_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_LEFT_INDEX]) KeyUp(BUTTON_LEFT_INDEX);
                    }
                    if (currentState.Triggers.Left != 0.0f)
                    {
                        if (!buttonsDown[BUTTON_LEFT_TRIGGER_INDEX]) KeyDown(BUTTON_LEFT_TRIGGER_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_LEFT_TRIGGER_INDEX]) KeyUp(BUTTON_LEFT_TRIGGER_INDEX);
                    }
                    if (currentState.Triggers.Right != 0.0f)
                    {
                        if (!buttonsDown[BUTTON_RIGHT_TRIGGER_INDEX]) KeyDown(BUTTON_RIGHT_TRIGGER_INDEX);
                    }
                    else
                    {
                        if (buttonsDown[BUTTON_RIGHT_TRIGGER_INDEX]) KeyUp(BUTTON_RIGHT_TRIGGER_INDEX);
                    }
                }

                Thread.Sleep(sleepInterval);
                dt = ((float)watch.ElapsedMilliseconds);
                watch.Restart();
            }
        }

        private void KeyDown(int index)
        {
            buttonsDown[index] = true;
            Console.WriteLine(buttons[index]);
            if (Enum.IsDefined(typeof(Keys), (int)buttons[index]))
            {
                keybd_event(buttons[index], 0, 0, 0);
            }
            else
            {
                Action func;
                functions.TryGetValue(buttons[index], out func);
                func();
            }
        }

        private void KeyUp(int index)
        {
            buttonsDown[index] = false;
            if (!Enum.IsDefined(typeof(Keys), (int)buttons[index]))
                return;
            keybd_event(buttons[index], 0, KEYEVENTF_KEYUP, 0);
        }

        public void AsignButton(int index, byte key)
        {
            buttons[index] = key;
            Console.WriteLine(buttons[index]);
        }

        private void LeftClick()
        {
            POINT p;
            GetCursorPos(out p);
            mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, 0);
        }

        private void RightClick()
        {
            POINT p;
            GetCursorPos(out p);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, 0);
        }

        private void ScrollUp()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -120, 0);
        }

        private void ScrollDown()
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, 120, 0);
        }

        private void Scroll(float velocity)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (int)velocity, 0);
        }

        private void StartOnScreenKeyboard()
        {
            Process.Start("osk.exe");
        }

        #region settings
        public void RestoreDefault()
        {
            buttons[BUTTON_A_INDEX] = LEFTCLICK;
            buttons[BUTTON_B_INDEX] = RIGHTCLICK;
            buttons[BUTTON_X_INDEX] = ONSCREENKEYBOARD;
            buttons[BUTTON_Y_INDEX] = (byte)Keys.LeftWindows;

            buttons[BUTTON_UP_INDEX] = (byte)Keys.VolumeUp;
            buttons[BUTTON_RIGHT_INDEX] = (byte)Keys.Right;
            buttons[BUTTON_DOWN_INDEX] = (byte)Keys.VolumeDown;
            buttons[BUTTON_LEFT_INDEX] = (byte)Keys.Left;

            buttons[BUTTON_LEFT_ANALOG_INDEX] = (byte)Keys.MediaPreviousTrack;
            buttons[BUTTON_RIGHT_ANALOG_INDEX] = (byte)Keys.MediaNextTrack;

            buttons[BUTTON_LEFT_BUMPER_INDEX] = (byte)Keys.LeftAlt;
            buttons[BUTTON_RIGHT_BUMPER_INDEX] = (byte)Keys.Tab;
            buttons[BUTTON_LEFT_TRIGGER_INDEX] = (byte)Keys.Home;
            buttons[BUTTON_RIGHT_TRIGGER_INDEX] = (byte)Keys.End;

            buttons[BUTTON_BACK_INDEX] = (byte)Keys.VolumeMute;
            buttons[BUTTON_START_INDEX] = (byte)Keys.MediaPlayPause;
        }

        private void ReadSettings()
        {
            buttons[BUTTON_A_INDEX] = Properties.Settings.Default.A_button;
            buttons[BUTTON_B_INDEX] = Properties.Settings.Default.B_button;
            buttons[BUTTON_X_INDEX] = Properties.Settings.Default.X_button;
            buttons[BUTTON_Y_INDEX] = Properties.Settings.Default.Y_button;

            buttons[BUTTON_UP_INDEX] = Properties.Settings.Default.Dpad_up;
            buttons[BUTTON_RIGHT_INDEX] = Properties.Settings.Default.Dpad_right;
            buttons[BUTTON_DOWN_INDEX] = Properties.Settings.Default.Dpad_down;
            buttons[BUTTON_LEFT_INDEX] = Properties.Settings.Default.Dpad_left;

            buttons[BUTTON_LEFT_ANALOG_INDEX] = Properties.Settings.Default.Left_stick_click;
            buttons[BUTTON_RIGHT_ANALOG_INDEX] = Properties.Settings.Default.Right_stick_click;

            buttons[BUTTON_LEFT_BUMPER_INDEX] = Properties.Settings.Default.Left_bumper;
            buttons[BUTTON_RIGHT_BUMPER_INDEX] = Properties.Settings.Default.Right_bumper;
            buttons[BUTTON_LEFT_TRIGGER_INDEX] = Properties.Settings.Default.Left_trigger;
            buttons[BUTTON_RIGHT_TRIGGER_INDEX] = Properties.Settings.Default.Right_trigger;

            buttons[BUTTON_BACK_INDEX] = Properties.Settings.Default.Back;
            buttons[BUTTON_START_INDEX] = Properties.Settings.Default.Start;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.A_button = buttons[BUTTON_A_INDEX];
            Properties.Settings.Default.B_button = buttons[BUTTON_B_INDEX];
            Properties.Settings.Default.X_button = buttons[BUTTON_X_INDEX];
            Properties.Settings.Default.Y_button = buttons[BUTTON_Y_INDEX];

            Properties.Settings.Default.Dpad_up = buttons[BUTTON_UP_INDEX];
            Properties.Settings.Default.Dpad_right = buttons[BUTTON_RIGHT_INDEX];
            Properties.Settings.Default.Dpad_down = buttons[BUTTON_DOWN_INDEX];
            Properties.Settings.Default.Dpad_left = buttons[BUTTON_LEFT_INDEX];

            Properties.Settings.Default.Left_bumper = buttons[BUTTON_LEFT_BUMPER_INDEX];
            Properties.Settings.Default.Right_bumper = buttons[BUTTON_RIGHT_BUMPER_INDEX];
            Properties.Settings.Default.Left_trigger = buttons[BUTTON_LEFT_TRIGGER_INDEX];
            Properties.Settings.Default.Right_trigger = buttons[BUTTON_RIGHT_TRIGGER_INDEX];

            Properties.Settings.Default.Left_stick_click = buttons[BUTTON_LEFT_ANALOG_INDEX];
            Properties.Settings.Default.Right_stick_click = buttons[BUTTON_RIGHT_ANALOG_INDEX];

            Properties.Settings.Default.Back = buttons[BUTTON_BACK_INDEX];
            Properties.Settings.Default.Start = buttons[BUTTON_START_INDEX];

            Properties.Settings.Default.Save();
        }
        #endregion

        public void Quit()
        {
            SaveSettings();
            running = false;
        }
    }
}