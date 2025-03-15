using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private Standard standardMode;
        private Programmer programmerMode;
        public MainWindow()
        {
            InitializeComponent();
            standardMode = new Standard(textBoxStandard);
            programmerMode = new Programmer(textBoxProgrammer, hexTextBox, decimalTextBox, octalTextBox, binaryTextBox);
            this.KeyDown += (s, e) => standardMode.HandleKeyDown();  // Asigură că tastatura funcționează direct pe textBox
            this.KeyDown += (s, e) => programmerMode.HandleKeyDown();
            this.KeyDown += (s, e) => HandleDeleteKey(e);
            LoadLastUsedMode();
        }

        private void LoadLastUsedMode()
        {
            // Citește valoarea din setările aplicației
            string lastUsedMode = Properties.Settings.Default.lastUsedMode;

            // Dacă ultima valoare salvată a fost "Programmer", trecem în modul respectiv
            if (lastUsedMode == "Programmer")
            {
                ShowOnlyThisGrid(ProgrammerModeGrid);
            }
            else
            {
                ShowOnlyThisGrid(StandardModeGrid);
            }
            bool isDigitGroupingEnabled = Properties.Settings.Default.DigitGroupingEnabled;
            // Aplică starea la CheckBox și setările modului
            CheckBox checkBox = FindName("DigitGroupingCheckBox") as CheckBox;

            if (StandardModeGrid.Visibility == Visibility.Visible)
                standardMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                programmerMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);
        }
        private void SaveLastUsedMode(string mode)
        {
            // Salvează modul curent în setările aplicației
            Properties.Settings.Default.lastUsedMode = mode;
            Properties.Settings.Default.Save();  // Salvează setările
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                standardMode.HandlePreviewTextInput(e);
                if (Properties.Settings.Default.DigitGroupingEnabled)  // Verifică dacă gruparea este activată
                {
                    standardMode.UpdateTextBoxWithGrouping();  // Actualizează gruparea
                }
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                programmerMode.HandlePreviewTextInput(e);
                if (Properties.Settings.Default.DigitGroupingEnabled)
                {
                    programmerMode.UpdateTextBoxWithGrouping();  // Aplică gruparea și în modul Programmer
                }
            }
        }

        private void DigitGroupingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.IsChecked == true;

            Properties.Settings.Default.DigitGroupingEnabled = isChecked;
            Properties.Settings.Default.Save();  // Salvează setările

            if (StandardModeGrid.Visibility == Visibility.Visible)
                standardMode.HandleDigitGroupingChecked(isChecked);
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                programmerMode.HandleDigitGroupingChecked(isChecked);
        }

        private void ShowOnlyThisGrid(Grid gridToShow)
        {
            StandardModeGrid.Visibility = Visibility.Collapsed;
            ProgrammerModeGrid.Visibility = Visibility.Collapsed;
            gridToShow.Visibility = Visibility.Visible;
        }

        private void StandardMode(object sender, RoutedEventArgs e)
        {
            ShowOnlyThisGrid(StandardModeGrid);
            SidebarMenu.Visibility = Visibility.Collapsed;
        }

        private void ProgrammerMode(object sender, RoutedEventArgs e)
        {
            ShowOnlyThisGrid(ProgrammerModeGrid);
            SidebarMenu.Visibility = Visibility.Collapsed;
        }

        private void HelpMode(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This was made by Vilcu Alessandra, from group 10LF234");
        }

        private void Settings(object sender, RoutedEventArgs e)
        {
            if (SidebarMenu.Visibility == Visibility.Collapsed)
            {
                SidebarMenu.Visibility = Visibility.Visible;
            }
            else
            {
                SidebarMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string? buttonText = button.Content.ToString();
            if (buttonText == "•")
            {
                buttonText = ".";
            }
            if (buttonText == "🔄")
                if (textBoxStandard.Text.Length > 0)
                {
                    textBoxStandard.Text = textBoxStandard.Text.Substring(0, textBoxStandard.Text.Length - 1);
                    standardMode.TextBox_PreviewKeyDown();
                }
                else
               if (textBoxProgrammer.Text.Length > 0)
                {
                    textBoxProgrammer.Text = textBoxProgrammer.Text.Substring(0, textBoxProgrammer.Text.Length - 1);
                    programmerMode.TextBox_PreviewKeyDown();
                }

            if (StandardModeGrid.Visibility == Visibility.Visible && buttonText != null)
            {
                standardMode.HandleButtonClick(buttonText);
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible && buttonText != null)
            {
                programmerMode.HandleButtonClick(buttonText);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Salvează setările atunci când aplicația este închisă
            SaveLastUsedMode(StandardModeGrid.Visibility == Visibility.Visible ? "Standard" : "Programmer");
        }
        private void HandleDeleteKey(KeyEventArgs e)
        { 
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (StandardModeGrid.Visibility == Visibility.Visible)
                {
                    standardMode.HandleDeleteKey();
                }
                else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    programmerMode.HandleDeleteKey();
                }

                e.Handled = true;
            }
        }
    }
}
