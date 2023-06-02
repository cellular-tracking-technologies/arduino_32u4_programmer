using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Feather32u4Programmer {
    public partial class Main : Form {
        public SerialPort Usb { get; set; } = new SerialPort();
        String SelectedPort { get; set; } = String.Empty;
        String FirmwarePath { get; set; } = String.Empty;

        public Main() {
            InitializeComponent();

            comboBoxComPort.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxComPort.SelectedIndexChanged += (o, e) => {
                SelectedPort = comboBoxComPort.SelectedItem.ToString();
            };

            buttonSelectFile.Click += (o, e) => {
                FirmwarePath = GetFileFromDialog("Firmware", ".hex");
                labelFile.Text = FirmwarePath.Equals(String.Empty) ?
                "Select Firmware" : FirmwarePath;
            };
            buttonPortRefresh.Click += (o, e) => {
                SelectedPort = RefreshUsbPortList(comboBoxComPort, SelectedPort);
            };
            buttonProgram.Click += async (o, e) => {
                buttonProgram.Enabled = false;
                await Program(SelectedPort, FirmwarePath);
                buttonProgram.Enabled = true;
            };

            this.Text = String.Format("{0} {1}", Application.ProductName, Application.ProductVersion);
            BuildConsoleWindow(textBoxConsole);
            BuildMenu();
        }
        private void BuildConsoleWindow(TextBox box) {
            box.ReadOnly = true;
            box.ScrollBars = ScrollBars.Both;
            box.WordWrap = false;
            box.Font = new Font("Times New Roman", 12);
            box.BackColor = Color.Black;
            box.ForeColor = Color.LimeGreen;
        }
        private static string GetFileFromDialog(string file_type, string extension) {
            string filterAll = "All files (*.*)|*.*";
            string filterSpecific = file_type + "|" + "*" + extension;
            OpenFileDialog selector = new OpenFileDialog {
                Filter = filterSpecific + "|" + filterAll,
                Title = "Choose " + file_type + " file to upload",
                InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString()
            };

            if (selector.ShowDialog() == DialogResult.OK) {
                return selector.FileName;
            }

            return String.Empty;
        }

        /// <summary>
        /// Updates the firmware for an Adafruit ATMega32u4 Feather. Access to the device
        /// bootloader required opening a USB connection to the device with a baudrate 
        /// if 1200, then immediately close the connection. Once the device is in bootloader
        /// mode, it will switch from its application COM port to a boot COM port. We must 
        /// discover the boot port prior to starting the firmware update process. Finally, we
        /// use <see href="https://github.com/avrdudes/avrdude">avrdude</see> to perform the update process.
        /// <example>
        /// For example:
        /// <code>
        /// Program("COM3", "C:\Users\MyUserName\Documents\Firmware\my_firmware.hex");
        /// </code>
        /// </example>
        /// </summary>
        private async Task Program(String port, String firmware_path) {
            if (String.IsNullOrEmpty(port)) {
                WriteToConsole("Invalid Port");
                return; 
            }
            if (String.IsNullOrEmpty(firmware_path)) {
                WriteToConsole("Invalid firmware path");
                return; 
            }else if(File.Exists(firmware_path) == false) {
                WriteToConsole(String.Format("File Doesn't Exist: {0}", firmware_path));
                return;
            }

            /*
             * Physical Port Assignment and configuration
             */

            Usb.PortName = port;

            try {
                Usb.Close();
            } catch (System.IO.IOException ex) {
                Console.WriteLine(ex);
                return;
            }

            /** 1200 Baud required to initiate bootloader */
            Usb.BaudRate = 1200;
            Usb.DataBits = 8;
            Usb.StopBits = StopBits.One;
            Usb.Parity = Parity.None;
            Usb.Handshake = Handshake.None;
            
            /*
             * DTR must be enabled to communicate with AtMega32u4 Feather. We 
             * may decide to expand features of this program to show output data,
             * therefore we set this value now.
             */
            Usb.DtrEnable = true; 

            String[] original_ports = GetSortedPortNames();

            /**
             * Open then immediately close port to trigger
             * boot sequence.
             */

            try {
                Usb.Open();
            } catch (UnauthorizedAccessException ex) {
                WriteToConsole(ex.Message);
                return;
            } catch (ArgumentOutOfRangeException ex) {
                WriteToConsole(ex.Message);
                return;
            } catch (ArgumentException ex) {
                WriteToConsole(ex.Message);
                return;
            } catch (System.IO.IOException ex) {
                WriteToConsole(ex.Message);
                return;
            } catch (InvalidOperationException ex) {
                WriteToConsole(ex.Message);
                return;
            }

            try {
                Usb.Close();
            } catch (System.IO.IOException ex) {
                Console.WriteLine(ex);
                return;
            }

            /** Discover the bootloader COM port. Under most circumstances this
             *  can be done by frequently sampling available ports and comparing 
             *  against the original port list. Take the first "new" COM port.
             */
            String DfuPort = String.Empty;

            /**
             * Run an asynchronus task to poll the new COM ports. This will prevent
             * locking up the UI while we poll for changes. This task will have an 
             * associated timeout to prevent the app from spinning forever.
             */

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var tasks = new ConcurrentBag<Task>();
            Task t = Task.Run(() => { DfuPort = FindBootloaderComPort(original_ports, token); }, token);
            tasks.Add(t);

            var Timeout = new System.Timers.Timer();
            Timeout.Interval = 10000;
            Timeout.Start();
            Timeout.Elapsed += (sender, args) => {
                Timeout.Stop();
                tokenSource.Cancel();
                Console.WriteLine("\nTask cancellation requested.");
            };

            await Task.WhenAll(tasks.ToArray());
            Timeout.Stop();
            tokenSource.Dispose();

            if (String.IsNullOrEmpty(DfuPort)) {
                WriteToConsole("COM Error: Couldnt find Dfu Port");
                return;
            }

            WriteToConsole(String.Format("Dfu Port: {0}", DfuPort));
            
            
            /**
             * Invoke Avrdude from the command line. This program is embedded in the output 
             * directory of this project to alleviate the users responsibility from downloading
             * it from github or through the Arduino IDE.
             */
            String AvrDude = String.Format("{0}/bin/avrdude", Properties.Resources.AvrDudePath);
            String Arguments = String.Format("\"-C{0}/etc/avrdude.conf\" -v -patmega32u4 -cavr109 -P{1} -b57600 -D \"-Uflash:w:{2}:i\"", Properties.Resources.AvrDudePath, DfuPort, firmware_path);

            var dfu = Task.Run(() => {
                RunCommand(AvrDude, Arguments);
            });

            if (dfu.Wait(TimeSpan.FromSeconds(15)) == false) {
                WriteToConsole("AvrDude Timeout");
            }
        }
        private String FindBootloaderComPort(String[] original_ports, CancellationToken token) {

            // Was cancellation already requested?
            if (token.IsCancellationRequested) {
                Console.WriteLine("Task was cancelled before it got started.");
                token.ThrowIfCancellationRequested();
            }

            while (true) {
                    String[] ports = GetSortedPortNames();
                    List<String> new_ports = ports.Except(original_ports).ToList();
                    String selected = String.Empty;
                    if (new_ports.Count > 0) {
                        selected =  new_ports.FirstOrDefault();
                    }

                    WriteToConsole(String.Format("[{0}] -> {1}", String.Join(",", ports), String.IsNullOrEmpty(selected) ? "Searching" : selected));

                    if (new_ports.Count > 0) {
                        return selected;
                    }
                if (token.IsCancellationRequested) {
                    return String.Empty;
                }
                Thread.Sleep(250);
            }
        }
        private String RefreshUsbPortList(ComboBox port_cb, String selected_port) {
            String selected = selected_port;

            string[] ports = GetSortedPortNames();
            port_cb.Items.Clear();
            if (ports.Length > 0) {
                port_cb.Items.AddRange(ports);
                if (ports.Contains(selected) == false) {
                    selected = ports.FirstOrDefault();
                }
                port_cb.SelectedItem = selected;
            }

            return selected;
        }
        private void BuildMenu() {

            MainMenu m = new MainMenu();

            MenuItem refresh = m.MenuItems.Add("&USB");
            refresh.MenuItems.Add(new MenuItem("&Refresh COM Ports", new EventHandler((o, e) => {
                SelectedPort = RefreshUsbPortList(comboBoxComPort, SelectedPort);
            }), Shortcut.F5));

            MenuItem about = m.MenuItems.Add("&About");
            about.MenuItems.Add(new MenuItem("&Version", new EventHandler((o, e) => {
                string str = String.Format("{0}{1}{2}{3}",
                    String.Format("Product: {0}", Application.ProductName) + Environment.NewLine,
                    String.Format("Version: {0}", Application.ProductVersion) + Environment.NewLine,
                    String.Format("Company: {0}", Application.CompanyName) + Environment.NewLine,
                    String.Format("Author: {0}", "Sheldon Blackshire") + Environment.NewLine);

                MessageBox.Show(str, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            })));

            this.Menu = m;
        }
        private void RunCommand(String program, String arguments) {
            //* Create your Process
            Process process = new Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            //* Set your output and error (asynchronous) handlers
            process.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceivedHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorDataReceivedHandler);

            //* Start process and handlers
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (process.WaitForExit(30000)) {
                    
            }
        }

        private void OutputDataReceivedHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            WriteToConsole(outLine.Data);
        }
        private void ErrorDataReceivedHandler(object sendingProcess, DataReceivedEventArgs outLine) {
            WriteToConsole(outLine.Data);            
        }
        void WriteToConsole(String data, bool new_line = true) {
            if (String.IsNullOrEmpty(data)) { return; }
            
            Console.WriteLine(data);

            textBoxConsole.InvokeIfRequired(() => {
                textBoxConsole.AppendText(data);
                if (new_line) {
                    textBoxConsole.AppendText(Environment.NewLine);
                }
            });
        }

        // Order the serial port names in numberic order (if possible)            
        string[] GetSortedPortNames() {
            try {

            } catch (Win32Exception ex) {
                Console.WriteLine(ex.Message);
                return new string[] { };
            }
            var ports = SerialPort.GetPortNames();
            if (ports.Count() == 0) {
                return ports;
            }

            ports.ToList().ForEach(p => Console.WriteLine(p));

            try {
                int num;
                return ports.OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
            } catch (ArgumentNullException ex) {
                return new string[] { };
            }
        }
    }
}
