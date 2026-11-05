using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            LogTextBox.Clear();
            Log("[Главный поток] Старт приложения и запуск потоков.");

            Thread thread1 = new Thread(RunNotepadTask);
            Thread thread2 = new Thread(RunCmdTask);

            thread1.Start();
            thread2.Start();
        }

        // Метод для безопасного вывода логов из любого потока в ваш LogTextBox
        private void Log(string message)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => Log(message));
                return;
            }

            LogTextBox.AppendText(message + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }

        private void RunNotepadTask()
        {
            Log("[Поток 1] Запуск потока.");
            try
            {
                Log("[Поток 1] Запуск Notepad.......");

                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };

                    process.Start();

                    Log("[Поток 1] Notepad запущен и автоматическое завершение через 3 секунды");

                    Thread.Sleep(3000);

                    Log("[Поток 1] Ожидание завершения Notepad...");

                    var realProcess = Process.GetProcessesByName("Notepad")[0];
                    realProcess.Kill();

                    Log("[Поток 1] Notepad корректно завершен");
                }
            }
            catch (Exception ex)
            {
                Log($"[Поток 1] Ошибка: {ex.Message}");
            }
            Log("[Поток 1] Завершение работы потока.");
        }

        private void RunCmdTask()
        {
            Log("[Поток 2] Запуск потока.");
            try
            {
                Log("[Поток 2] Запуск командной строки (cmd.exe) с командой dir.");

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dir",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.GetEncoding(866)
                };

                using (Process cmdProcess = Process.Start(startInfo))
                {
                    if (cmdProcess != null)
                    {
                        // Считываем весь результат работы команды dir
                        string result = cmdProcess.StandardOutput.ReadToEnd();
                        cmdProcess.WaitForExit();

                        Log("[Поток 2] Результаты выполнения команды dir:");
                        Log(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[Поток 2] Ошибка: {ex.Message}");
            }
            Log("[Поток 2] Завершение работы потока.");
        }
    }
}
