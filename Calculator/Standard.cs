using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Standard
    {
        private bool isDigitGroupingEnabled = false;
        private TextBox textBox;

        public Standard(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void HandleKeyDown()
        {
            textBox.Focus();
        }

        public void HandlePreviewTextInput(TextCompositionEventArgs e)
        {
            int maxLength = 9;
            if (textBox.Text.Length >= maxLength)
            {
                e.Handled = true;
                return;
            }

            if (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0."))
            {
                textBox.Text = textBox.Text.Substring(1);
                textBox.Select(textBox.Text.Length, 0);
            }

            if (!char.IsDigit(e.Text, 0) && e.Text[0] != '.')
            {
                e.Handled = true;
                return;
            }

            if (e.Text == "." && textBox.Text.Contains('.'))
            {
                e.Handled = true;
                return;
            }

            if (e.Text == "." && textBox.Text.Length == 0)
            {
                textBox.Text = "0.";
                textBox.Select(textBox.Text.Length, 0);
                e.Handled = true;
            }

            // Aplica gruparea dupa fiecare schimbare a textului
            if (isDigitGroupingEnabled)
            {
                // Adauga caracterul nou in text
                textBox.Text += e.Text;

                // Aplica gruparea
                UpdateTextBoxWithGrouping();

                // Selecteaza tot textul pentru a nu pierde pozitia cursorului
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                e.Handled = true; // Impiedica introducerea directa a caracterului in TextBox
            }
        }


        public void HandleDigitGroupingChecked(bool isChecked)
        {
            isDigitGroupingEnabled = isChecked;
            UpdateTextBoxWithGrouping();
        }

        public void UpdateTextBoxWithGrouping()
        {
            if (isDigitGroupingEnabled)
            {
                string rawText = textBox.Text.Replace(",", "");
                string[] parts = rawText.Split('.');

                parts[0] = ApplyGrouping(parts[0]);
                textBox.Text = parts.Length > 1 ? parts[0] + "." + parts[1] : parts[0];
            }
            else
            {
                textBox.Text = textBox.Text.Replace(",", "");
            }

            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }

        private string ApplyGrouping(string input)
        {
            StringBuilder grouped = new StringBuilder();
            int count = 0;

            for (int i = input.Length - 1; i >= 0; i--)
            {
                char currentChar = input[i];

                if (currentChar == '.')
                {
                    grouped.Insert(0, currentChar);
                    break;
                }
                grouped.Insert(0, currentChar);
                count++;

                if (count % 3 == 0 && i != 0 && char.IsDigit(currentChar))
                {
                    grouped.Insert(0, ",");
                }
            }

            return grouped.ToString();
        }

        public void TextBox_PreviewKeyDown()
        {
              UpdateTextBoxWithGrouping();
        }

        private bool IsValidCharacter(string character)
        {
            if (!char.IsDigit(character, 0) && character != ".")
            {
                return false;
            }

            if (character == "." && textBox.Text.Contains("."))
            {
                return false;
            }

            if (textBox.Text.Length >= 9)
            {
                return false;
            }

            return true;
        }

        public void HandleButtonClick(string buttonText)
        {
            if (!IsValidCharacter(buttonText))
            {
                return;
            }

            if (buttonText == "." && textBox.Text.Length == 0)
            {
                textBox.Text = "0.";
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
                return;
            }

            if (buttonText == "." && textBox.Text.Contains("."))
            {
                return;
            }

            if (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0.") && buttonText != ".")
            {
                textBox.Text = textBox.Text.Substring(1);
                textBox.Select(textBox.Text.Length, 0);
            }

            if (textBox.Text.Length < 9)
            {
                textBox.Text += buttonText;
            }

            if (isDigitGroupingEnabled)
            {
                UpdateTextBoxWithGrouping();
            }

            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }
        public void HandleDeleteKey()
        {
            if (textBox.Text.Length > 0)
            {
                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);

                if (isDigitGroupingEnabled)
                {
                    UpdateTextBoxWithGrouping();
                }

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }
        }
    }
}
