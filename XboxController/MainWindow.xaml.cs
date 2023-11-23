using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Windows.Shell;
using System.Windows.Media.Effects;

namespace XboxController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;

        private Dictionary<string, byte> commandButtons = new Dictionary<string, byte>();
        private Dictionary<string, int> controllerButtons = new Dictionary<string, int>();

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            controller.Start();
            MaxHeight = SystemParameters.WorkArea.Height;
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            //nIcon.Visible = false;
            controller.Quit();
        }

        #region Initialize components

        private void Init()
        {
            InitCommandButtons();
            InitControllerButttons();
        }

        private void InitCommandButtons()
        {
            commandButtons.Add("Delete", (byte)Keys.Delete);
            commandButtons.Add("Left alt", (byte)Keys.LeftAlt);
            commandButtons.Add("Left shift", (byte)Keys.LeftShift);
            commandButtons.Add("Right alt", (byte)Keys.RightAlt);
            commandButtons.Add("Right shift", (byte)Keys.RightShift);
            commandButtons.Add("Left control", (byte)Keys.LeftControl);
            commandButtons.Add("Right control", (byte)Keys.RightControl);
            commandButtons.Add("Caps lock", (byte)Keys.CapsLock);
            commandButtons.Add("Tab", (byte)Keys.Tab);
            commandButtons.Add("Windows key", (byte)Keys.LeftWindows);
            commandButtons.Add("Enter", (byte)Keys.Enter);
            commandButtons.Add("End", (byte)Keys.End);
            commandButtons.Add("Home", (byte)Keys.Home);
            commandButtons.Add("Page down", (byte)Keys.PageDown);
            commandButtons.Add("Page up", (byte)Keys.PageUp);
            commandButtons.Add("Print screen", (byte)Keys.PrintScreen);
            commandButtons.Add("Play / Pause", (byte)Keys.MediaPlayPause);
            commandButtons.Add("Next track", (byte)Keys.MediaNextTrack);
            commandButtons.Add("Previous track", (byte)Keys.MediaPreviousTrack);
            commandButtons.Add("Stop", (byte)Keys.MediaStop);
            commandButtons.Add("Num lock", (byte)Keys.NumLock);
            commandButtons.Add("F1", (byte)Keys.F1);
            commandButtons.Add("F2", (byte)Keys.F2);
            commandButtons.Add("F3", (byte)Keys.F3);
            commandButtons.Add("F4", (byte)Keys.F4);
            commandButtons.Add("F5", (byte)Keys.F5);
            commandButtons.Add("F6", (byte)Keys.F6);
            commandButtons.Add("F7", (byte)Keys.F7);
            commandButtons.Add("F8", (byte)Keys.F8);
            commandButtons.Add("F9", (byte)Keys.F9);
            commandButtons.Add("F10", (byte)Keys.F10);
            commandButtons.Add("F11", (byte)Keys.F11);
            commandButtons.Add("F12", (byte)Keys.F12);
            commandButtons.Add("Insert", (byte)Keys.Insert);
            commandButtons.Add("Escape", (byte)Keys.Escape);
            commandButtons.Add("Left", (byte)Keys.Left);
            commandButtons.Add("Right", (byte)Keys.Right);
            commandButtons.Add("Up", (byte)Keys.Up);
            commandButtons.Add("Down", (byte)Keys.Down);
            commandButtons.Add("Volume mute", (byte)Keys.VolumeMute);
            commandButtons.Add("Volume up", (byte)Keys.VolumeUp);
            commandButtons.Add("Volume down", (byte)Keys.VolumeDown);
            commandButtons.Add("Left click", controller.LEFTCLICK);
            commandButtons.Add("Right click", controller.RIGHTCLICK);
            commandButtons.Add("Scroll up", controller.SCROLLUP);
            commandButtons.Add("Scroll down", controller.SCROLLDOWN);
            commandButtons.Add("On-screen keyboard", controller.ONSCREENKEYBOARD);

            var sortedDict = from entry in commandButtons orderby entry.Key ascending select entry;
            commandButtons = sortedDict.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void InitControllerButttons()
        {
            controllerButtons.Add("A button", controller.BUTTON_A_INDEX);
            controllerButtons.Add("B button", controller.BUTTON_B_INDEX);
            controllerButtons.Add("X button", controller.BUTTON_X_INDEX);
            controllerButtons.Add("Y button", controller.BUTTON_Y_INDEX);
            controllerButtons.Add("D-pad up", controller.BUTTON_UP_INDEX);
            controllerButtons.Add("D-pad right", controller.BUTTON_RIGHT_INDEX);
            controllerButtons.Add("D-pad down", controller.BUTTON_DOWN_INDEX);
            controllerButtons.Add("D-pad left", controller.BUTTON_LEFT_INDEX);
            controllerButtons.Add("Left bumper", controller.BUTTON_LEFT_BUMPER_INDEX);
            controllerButtons.Add("Right bumper", controller.BUTTON_RIGHT_BUMPER_INDEX);
            controllerButtons.Add("Left trigger", controller.BUTTON_LEFT_TRIGGER_INDEX);
            controllerButtons.Add("Right trigger", controller.BUTTON_RIGHT_TRIGGER_INDEX);
            controllerButtons.Add("Left stick click", controller.BUTTON_LEFT_ANALOG_INDEX);
            controllerButtons.Add("Right stick click", controller.BUTTON_RIGHT_ANALOG_INDEX);
            controllerButtons.Add("Back", controller.BUTTON_BACK_INDEX);
            controllerButtons.Add("Start", controller.BUTTON_START_INDEX);
        }

        #endregion

        #region WINDOW EVENTS
        private void XboxWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
                ((Border)XboxWindow.Template.FindName("maximizeBorder", XboxWindow)).BorderThickness = new Thickness(0);
            else if (WindowState == WindowState.Maximized)
                ((Border)XboxWindow.Template.FindName("maximizeBorder", XboxWindow)).BorderThickness = new Thickness(7);
        }

        private void XboxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void XboxWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controller.Quit();
        }
        #endregion

        #region WINDOW BUTTONS
        private void close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/close.png"));
        }

        private void close_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/close_hover.png"));
        }

        private void close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/close.png"));
        }

        private void maximize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/maximize.png"));
        }

        private void maximize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void maximize_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/maximize_hover.png"));
        }

        private void maximize_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/maximize.png"));
        }

        private void minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/minimize.png"));
        }

        private void minimize_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void minimize_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/minimize_hover.png"));
        }

        private void minimize_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri(@"pack://application:,,,/XboxController;component/imgs/minimize.png"));
        }
        #endregion


        #region ASIGN KEY/COMMAND EVENTS

        private void textbox_setKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    // get the new key
                    byte value = Convert.ToByte((Key)(new System.Windows.Forms.KeysConverter().ConvertFromString(((TextBox)sender).Text.ToUpper())));
                    
                    // get the controller button to map the key to
                    int index = controllerButtons.Values.ElementAt(((ComboBox)XboxWindow.Template.FindName("comboBox_buttons", XboxWindow)).SelectedIndex);

                    // set it
                    controller.buttons[index] = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void comboBox_buttons_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = controllerButtons.Keys;

            comboBox.IsTextSearchCaseSensitive = false;
            comboBox.IsTextSearchEnabled = true;

            comboBox.SelectedIndex = 0;
        }

        private void comboBox_keys_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = commandButtons.Keys;

            comboBox.IsTextSearchCaseSensitive = false;
            comboBox.IsTextSearchEnabled = true;

            int index = controllerButtons.Values.ElementAt(((ComboBox)XboxWindow.Template.FindName("comboBox_buttons", XboxWindow)).SelectedIndex);
            byte value = controller.buttons[index];
            string key = commandButtons.FirstOrDefault(x => x.Value == value).Key;
            for (int i = 0; i < commandButtons.Count; i++)
            {
                if (comboBox.Items[i].ToString().Equals(key))
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void comboBox_buttons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = XboxWindow.Template.FindName("comboBox_keys", XboxWindow) as ComboBox;

            int index = controllerButtons.Values.ElementAt(((ComboBox)sender).SelectedIndex);
            byte value = controller.buttons[index];
            string key = commandButtons.FirstOrDefault(x => x.Value == value).Key;
            try // TODO: fix - try/catch is temporary
            {
                for (int i = 0; i < commandButtons.Count; i++)
                {
                    if (comboBox.Items[i].ToString().Equals(key))
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void comboBox_keys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            // get the new key
            byte value = commandButtons.Values.ElementAt(comboBox.SelectedIndex);
            
            // get the controller button to map the key to
            int index = controllerButtons.Values.ElementAt(((ComboBox)XboxWindow.Template.FindName("comboBox_buttons", XboxWindow)).SelectedIndex);

            // set it
            controller.buttons[index] = value;
        }

        private void button_Restore_Click(object sender, RoutedEventArgs e)
        {
            controller.RestoreDefault();
            comboBox_buttons_SelectionChanged(XboxWindow.Template.FindName("comboBox_buttons", XboxWindow) as ComboBox, null);
        }
        #endregion

    }
}
