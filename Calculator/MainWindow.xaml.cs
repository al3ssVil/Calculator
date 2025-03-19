using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using System.Windows.Media;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private Standard standardMode;
        private Programmer programmerMode;
        private int baseSelected = 10;
        private string clipboardText = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            standardMode = new Standard(textBoxStandard);
            programmerMode = new Programmer(textBoxProgrammer, hexTextBox, decimalTextBox, octalTextBox, binaryTextBox);
            this.KeyDown += (s, e) =>
            {
                standardMode.HandleKeyDown();
                programmerMode.HandleKeyDown();
                HighlightButton(e.Key);
            };

            this.PreviewKeyDown += (s, e) => HandleDeleteKey(e);
            this.PreviewKeyDown += (s, e) => HandleEnterKey(e);
            this.PreviewKeyDown += (s, e) => HandleEscKey(e);
            LoadLastUsedMode();
            UpdateBasesBasedOnSelectedBase();
        }

        private void LoadLastUsedMode()
        {
            string lastUsedMode = Properties.Settings.Default.lastUsedMode;
            bool isDigitGroupingEnabled = Properties.Settings.Default.DigitGroupingEnabled;

            baseSelected = Properties.Settings.Default.LastUsedBase;

            CheckBox? checkBox = FindName("DigitGroupingCheckBox") as CheckBox;

            if (checkBox != null)
            {
                checkBox.IsChecked = isDigitGroupingEnabled;
            }

            if (lastUsedMode == "Programmer")
            {
                ShowOnlyThisGrid(ProgrammerModeGrid);
            }
            else
            {
                ShowOnlyThisGrid(StandardModeGrid);
            }

            standardMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);
            programmerMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);

            UpdateBasesBasedOnSelectedBase();
            DisableInvalidButtons();
        }

        private void SaveLastUsedMode(string mode)
        {
            Properties.Settings.Default.lastUsedMode = mode;

            CheckBox? checkBox = FindName("DigitGroupingCheckBox") as CheckBox;

            if (checkBox != null)
            {
                Properties.Settings.Default.DigitGroupingEnabled = checkBox.IsChecked == true;
            }

            Properties.Settings.Default.Save();

            bool isDigitGroupingEnabled = Properties.Settings.Default.DigitGroupingEnabled;

            standardMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);
            programmerMode.HandleDigitGroupingChecked(isDigitGroupingEnabled);
        }

        //keyboard input validation
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                standardMode.HandlePreviewTextInput(e);
                if (Properties.Settings.Default.DigitGroupingEnabled)
                {
                    standardMode.UpdateTextBoxWithGrouping();
                }
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                programmerMode.HandlePreviewTextInput(e);
                if (Properties.Settings.Default.DigitGroupingEnabled)
                {
                    programmerMode.UpdateTextBoxWithGrouping();
                    UpdateBasesBasedOnSelectedBase();
                }
            }
        }

        private void DigitGroupingCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.IsChecked == true;

            Properties.Settings.Default.DigitGroupingEnabled = isChecked;
            Properties.Settings.Default.Save();

            standardMode.HandleDigitGroupingChecked(isChecked);
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

        //buttons input validation
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string? buttonText = button.Content.ToString();

            if (buttonText == "•")
            {
                buttonText = ".";
            }

            if (buttonText == "🔄")
            {
                if (textBoxStandard.Text.Length > 0)
                {
                    textBoxStandard.Text = textBoxStandard.Text.Substring(0, textBoxStandard.Text.Length - 1);
                    standardMode.TextBox_PreviewKeyDown();
                }
                else if (textBoxProgrammer.Text.Length > 0)
                {
                    textBoxProgrammer.Text = textBoxProgrammer.Text.Substring(0, textBoxProgrammer.Text.Length - 1);
                    programmerMode.TextBox_PreviewKeyDown();
                }
            }
            if (StandardModeGrid.Visibility == Visibility.Visible && buttonText != null)
            {
                standardMode.HandleButtonClick(buttonText);
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible && buttonText != null)
            {
                if (baseSelected == 2)
                {
                    if ("01+-*/%=^()".Contains(buttonText))
                    {
                        programmerMode.HandleButtonClick(buttonText);
                    }
                }
                else if (baseSelected == 10)
                {
                    if ("0123456789+-*/%=^()".Contains(buttonText))
                    {
                        programmerMode.HandleButtonClick(buttonText);
                    }
                }
                else if (baseSelected == 8)
                {
                    if ("01234567+-*/%=^()".Contains(buttonText))
                    {
                        programmerMode.HandleButtonClick(buttonText);
                    }
                }
                else if (baseSelected == 16)
                {
                    if ("0123456789ABCDEF+-*/%=^()".Contains(buttonText.ToUpper()))
                    {
                        programmerMode.HandleButtonClick(buttonText);
                    }
                }
                UpdateBasesBasedOnSelectedBase();

            }
        }

        private void DisableInvalidButtons()
        {
            // Disable buttons based on the selected base
            btnNumPad0.IsEnabled = (baseSelected == 2 || baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad1.IsEnabled = (baseSelected == 2 || baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad2.IsEnabled = (baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad3.IsEnabled = (baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad4.IsEnabled = (baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad5.IsEnabled = (baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad6.IsEnabled = (baseSelected == 10 || baseSelected == 8 || baseSelected == 16);
            btnNumPad7.IsEnabled = (baseSelected == 8 || baseSelected == 10 || baseSelected == 16);
            btnNumPad8.IsEnabled = (baseSelected == 10 || baseSelected == 16);
            btnNumPad9.IsEnabled = (baseSelected == 10 || baseSelected == 16);
            btnA.IsEnabled = (baseSelected == 16);
            btnB.IsEnabled = (baseSelected == 16);
            btnC.IsEnabled = (baseSelected == 16);
            btnNumPad.IsEnabled = (baseSelected == 16);
            btnE.IsEnabled = (baseSelected == 16);
            btnF.IsEnabled = (baseSelected == 16);

            // Disable operators and other special buttons based on the base
            btnAddP.IsEnabled = true;
            btnSubtractP.IsEnabled = true;
            btnMultiply.IsEnabled = true;
            btnDivide.IsEnabled = true;
            btnEqualP.IsEnabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveLastUsedMode(StandardModeGrid.Visibility == Visibility.Visible ? "Standard" : "Programmer");
        }

        //delete keyboard
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
                    UpdateBasesBasedOnSelectedBase();
                }

                e.Handled = true;
            }
        }

        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {
            Button? clickedButton = sender as Button;

            if (clickedButton != null)
            {
                if (clickedButton == btnHex)
                {
                    baseSelected = 16;
                    textBoxProgrammer.Text = hexTextBox.Text;
                }
                else if (clickedButton == btnDec)
                {
                    baseSelected = 10;
                    textBoxProgrammer.Text = decimalTextBox.Text;
                }
                else if (clickedButton == btnOct)
                {
                    baseSelected = 8;
                    textBoxProgrammer.Text = octalTextBox.Text;
                }
                else if (clickedButton == btnBin)
                {
                    baseSelected = 2;
                    textBoxProgrammer.Text=binaryTextBox.Text;
                }
                programmerMode.baseNumber= baseSelected;

                Properties.Settings.Default.LastUsedBase = baseSelected;
                Properties.Settings.Default.Save();

                MessageBox.Show($"Butonul:{clickedButton.Name}, baza:{baseSelected}");
                UpdateBasesBasedOnSelectedBase();
                DisableInvalidButtons();
            }
        }

        private void UpdateBasesBasedOnSelectedBase()
        {
            string inputValue = textBoxProgrammer.Text;

            //valid
            if (string.IsNullOrWhiteSpace(inputValue))
            {
                programmerMode.ResetTextBoxes();
                programmerMode.baseNumber = baseSelected;
                return;
            }

            try
            {
                programmerMode.UpdateAllBases(inputValue, baseSelected);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Conversion error: {ex.Message}");
                programmerMode.ResetTextBoxes();
            }
        }

        public void OnOperatorButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            string operatorText = string.Empty;

            switch (button.Name)
            {
                case "btnAdd":
                case "btnAddP":
                    operatorText = "+";
                    break;
                case "btnSubtract":
                case "btnSubtractP":
                    operatorText = "-";
                    break;
                case "btnMultiply":
                case "btnMultiplyP":
                    operatorText = "*";
                    break;
                case "btnDivide":
                case "btnDivideP":
                    operatorText = "/";
                    break;
                case "btnPercentage":
                case "btnPercentageP":
                    operatorText = "%";
                    break;
                case "btnDivideX":
                    operatorText = "1/x";
                    break;
                case "btnSquare":
                    operatorText = "^2";
                    break;

                case "btnSqrt":
                    operatorText = "sqrt";
                    break;
                case "btnPlusMinus":
                case "btnPlusMinusP":
                    operatorText = "+/-";
                    break;
                case "btnEqual":
                case "btnEqualP":
                    operatorText = "=";
                    break;
                default:
                    break;
            }

            //MessageBox.Show("Operator apasat: " + operatorText);

            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                standardMode.HandleOperatorClick(operatorText);
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                programmerMode.HandleOperatorClick(operatorText);
            }
        }

        private void HandleEnterKey(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (StandardModeGrid.Visibility == Visibility.Visible)
                {
                    standardMode.HandleOperatorClick("=");
                }
                else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    programmerMode.HandleOperatorClick("=");
                }
            }
        }

        private void HandleEscKey(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (StandardModeGrid.Visibility == Visibility.Visible)
                {
                    standardMode.ClearResult();
                }
                else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    programmerMode.ClearResult();
                }
            }
        }

        private void CutAction(object sender, RoutedEventArgs e)
        {
            string textToCut = string.Empty;

            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(textBoxStandard.SelectedText))
                {
                    textBoxStandard.SelectAll();
                }

                textToCut = textBoxStandard.SelectedText;
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(textBoxProgrammer.SelectedText))
                {
                    textBoxProgrammer.SelectAll();
                }

                textToCut = textBoxProgrammer.SelectedText;
            }

            if (!string.IsNullOrEmpty(textToCut))
            {
                clipboardText = textToCut;
                Clipboard.SetText(textToCut);

                if (StandardModeGrid.Visibility == Visibility.Visible)
                {
                    textBoxStandard.SelectedText = string.Empty;
                }
                else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    textBoxProgrammer.SelectedText = string.Empty;
                    UpdateBasesBasedOnSelectedBase();
                }
            }

        }

        private void CopyAction(object sender, RoutedEventArgs e)
        {
            string textToCopy = string.Empty;

            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(textBoxStandard.SelectedText))
                {
                    textBoxStandard.SelectAll();
                }
                textToCopy = textBoxStandard.SelectedText;
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(textBoxProgrammer.SelectedText))
                {
                    textBoxProgrammer.SelectAll();
                }
                textToCopy = textBoxProgrammer.SelectedText;
            }

            if (!string.IsNullOrEmpty(textToCopy))
            {
                clipboardText = textToCopy;
                Clipboard.SetText(textToCopy);
            }
        }

        private void PasteAction(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(clipboardText))
            {
                if (StandardModeGrid.Visibility == Visibility.Visible)
                {
                    string existingText = textBoxStandard.Text;

                    textBoxStandard.Text = existingText + clipboardText;

                    textBoxStandard.SelectionStart = textBoxStandard.Text.Length;
                    textBoxStandard.SelectionLength = 0;
                    standardMode.UpdateTextBoxWithGrouping();
                }
                else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    string existingText = textBoxProgrammer.Text;
                    textBoxProgrammer.Text = existingText + clipboardText;

                    textBoxProgrammer.SelectionStart = textBoxProgrammer.Text.Length;
                    textBoxProgrammer.SelectionLength = 0;
                    programmerMode.UpdateTextBoxWithGrouping();
                    UpdateBasesBasedOnSelectedBase();
                }
            }
        }

        private void C_Button(object sender, RoutedEventArgs e)
        {
            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                standardMode.ClearResult();
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                programmerMode.ClearResult();
            }
        }

        private void CE_Button(object sender, RoutedEventArgs e)
        {
            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                standardMode.ClearEntry();
            }
            else if (ProgrammerModeGrid.Visibility == Visibility.Visible)
            {
                programmerMode.ClearEntry();
            }
        }

        private void OnNotButtonClick(object sender, RoutedEventArgs e)
        {
            if (baseSelected == 2)
                programmerMode.HandleLogicalOperation("NOT");
        }

        private void MemoryClear_Click(object sender, RoutedEventArgs e)//MC
        {
            int selectedIndex = MemoryListBox.SelectedIndex;
            standardMode.MemoryClear(selectedIndex);
            UpdateMemoryList();
        }

        private void MemoryRecall_Click(object sender, RoutedEventArgs e)//MR
        {
            if (MemoryListBox.SelectedItem != null)
            {
                textBoxStandard.Text = MemoryListBox.SelectedItem.ToString();
            }
            else
            {
                textBoxStandard.Text = standardMode.MemoryRecall();
            }
        }

        private void MemoryAdd_Click(object sender, RoutedEventArgs e)//M+
        {
            int selectedIndex = MemoryListBox.SelectedIndex;
            standardMode.MemoryAdd(textBoxStandard.Text, selectedIndex);
            UpdateMemoryList();
        }

        private void MemorySubtract_Click(object sender, RoutedEventArgs e)//M-
        {
            int selectedIndex = MemoryListBox.SelectedIndex;
            standardMode.MemorySubtract(textBoxStandard.Text, selectedIndex);
            UpdateMemoryList();
        }

        private void MemoryStore_Click(object sender, RoutedEventArgs e)//MS
        {
            standardMode.MemoryStore(textBoxStandard.Text);
            UpdateMemoryList();
        }

        private void MemoryShowStack_Click(object sender, RoutedEventArgs e)//M▼
        {
            UpdateMemoryList(); 
            MemoryPanel.Visibility = Visibility.Visible;
        }

        private void ToggleMemoryPanel_Click(object sender, RoutedEventArgs e)
        {
            MemoryPanel.Visibility = (MemoryPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateMemoryList()
        {
            MemoryListBox.Items.Clear();
            foreach (var value in standardMode.GetMemoryStack())
            {
                MemoryListBox.Items.Add(value);
            }
        }

        private void HighlightButton(Key key)
        {
            string keyString = key.ToString();

            if (StandardModeGrid.Visibility == Visibility.Visible)
            {
                if (keyString.StartsWith("NumPad"))
                {
                    keyString = "D" + keyString.Substring(6);
                }
            }
            else
                if (ProgrammerModeGrid.Visibility == Visibility.Visible)
                {
                    if (keyString.StartsWith("D"))
                    {   
                        keyString = "NumPad" + keyString.Substring(1);
                    }
                }
            

            Button? buttonToHighlight = FindName($"btn{keyString}") as Button;

            if (buttonToHighlight != null)
            {
                Brush originalColor = buttonToHighlight.Background;
                buttonToHighlight.Background = Brushes.Gray;

                Task.Delay(200).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => buttonToHighlight.Background = originalColor);
                });
            }
        }

    }
}
