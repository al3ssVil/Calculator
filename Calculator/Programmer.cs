using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Calculator
{
    public class Programmer
    {
        private bool isDigitGroupingEnabled = false;
        private TextBox textBox;
        public TextBox hexTextBox { get; set; }
        public TextBox decimalTextBox { get; set; }
        public TextBox octalTextBox { get; set; }
        public TextBox binaryTextBox { get; set; }

        public Programmer(TextBox textBox, TextBox hex, TextBox dec, TextBox oct, TextBox bin)
        {
            this.textBox = textBox;
            hexTextBox = hex;
            decimalTextBox = dec;
            octalTextBox = oct;
            binaryTextBox = bin;
        }

        public void HandleKeyDown()
        {
            textBox.Focus();
        }
        private void UpdateAllBases(string value)
        {
            if (int.TryParse(value, out int decimalValue))
            {
                hexTextBox.Text = decimalValue.ToString("X");
                decimalTextBox.Text = decimalValue.ToString();
                octalTextBox.Text = Convert.ToString(decimalValue, 8);
                binaryTextBox.Text = Convert.ToString(decimalValue, 2);
            }
        }
        public void HandlePreviewTextInput(TextCompositionEventArgs e)
        {
            int maxLength = 12;
            if (textBox.Text.Length > maxLength)
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

            if (isDigitGroupingEnabled)
            {
                UpdateTextBoxWithGrouping();
            }
        }

        public void HandlePointButtonClick(Button button)
        {
            if (button == null || textBox == null) return;

            // Dacă textBox-ul este gol sau conține doar "0", adăugăm "0."
            if (textBox.Text.Length == 0 || textBox.Text == "0")
            {
                textBox.Text = "0.";
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
            }

            // Dacă textBox-ul nu conține un punct, adăugăm un punct
            if (!textBox.Text.Contains("."))
            {
                textBox.AppendText(".");
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
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

            for (int i = input.Length; i >= 0; i--)
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
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Verificăm dacă tasta apăsată este Backspace sau Delete
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                // După ce se șterge un caracter, actualizăm gruparea
                UpdateTextBoxWithGrouping();
            }
        }
        private bool IsValidCharacter(string character)
        {
            // Verifică dacă caracterul este un digit sau punct
            if (!char.IsDigit(character, 0) && character != ".")
            {
                return false;
            }

            // Verifică dacă textul conține deja un punct
            if (character == "." && textBox.Text.Contains("."))
            {
                return false;  // Previne adăugarea unui alt punct
            }

            // Verifică dacă numărul de caractere depășește limita maximă
            if (textBox.Text.Length >= 9)
            {
                return false;  // Previne adăugarea mai multor caractere
            }

            return true;
        }

        public void HandleButtonClick(string buttonText)
        {
            // Verifică dacă caracterul este valid
            if (!IsValidCharacter(buttonText))
            {
                return;  // Dacă nu este valid, nu facem nimic
            }

            // Dacă este un punct și textul este gol, adăugăm "0."
            if (buttonText == "." && textBox.Text.Length == 0)
            {
                textBox.Text = "0.";  // Dacă este primul caracter, adăugăm 0.
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
                return;
            }

            // Dacă este un punct și textul conține deja un punct, nu adăugăm alt punct
            if (buttonText == "." && textBox.Text.Contains("."))
            {
                return;  // Previne adăugarea unui alt punct
            }

            // Dacă textul începe cu "0" și nu are punct, îl eliminăm
            if (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0.") && buttonText != ".")
            {
                textBox.Text = textBox.Text.Substring(1);  // Elimină "0" de la început
                textBox.Select(textBox.Text.Length, 0);  // Plasează cursorul la final
            }

            // Dacă textul are mai puțin de 9 caractere, adăugăm butonul apăsat
            if (textBox.Text.Length < 12)
            {
                textBox.Text += buttonText;  // Adăugăm caracterul la final
            }

            // Aplică gruparea numerelor dacă este activată
            if (isDigitGroupingEnabled)
            {
                UpdateTextBoxWithGrouping();  // Grupăm numerele, dacă e activat
            }

            // După adăugarea textului, poziționează cursorul la final
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
        }
        public void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox? changedTextBox = sender as TextBox;

            if (changedTextBox != null)
            {
                string input = changedTextBox.Text;

                // Dacă câmpul este gol, considerăm valoarea "0"
                if (string.IsNullOrEmpty(input))
                {
                    input = "0"; // dacă nu s-a introdus nimic, punem 0
                }

                // Aici determinăm care TextBox a fost modificat
                if (changedTextBox == hexTextBox)
                {
                    ConvertFromHex(input);
                }
                else if (changedTextBox == decimalTextBox)
                {
                    ConvertFromDecimal(input);
                }
                else if (changedTextBox == octalTextBox)
                {
                    ConvertFromOctal(input);
                }
                else if (changedTextBox == binaryTextBox)
                {
                    ConvertFromBinary(input);
                }
            }
        }

        // Conversie din HEX în celelalte baze
        private void ConvertFromHex(string hexValue)
        {
            try
            {
                int decimalValue = Convert.ToInt32(hexValue, 16); // Convertim HEX în DEC
                decimalTextBox.Text = decimalValue.ToString();  // Actualizăm DEC
                octalTextBox.Text = Convert.ToString(decimalValue, 8); // Actualizăm OCT
                binaryTextBox.Text = Convert.ToString(decimalValue, 2); // Actualizăm BIN
            }
            catch
            {
                decimalTextBox.Text = "0";
                octalTextBox.Text = "0";
                binaryTextBox.Text = "0";
            }
        }

        // Conversie din DEC în celelalte baze
        private void ConvertFromDecimal(string decValue)
        {
            try
            {
                int decimalNumber = int.Parse(decValue);
                hexTextBox.Text = decimalNumber.ToString("X");  // Convertim DEC în HEX
                octalTextBox.Text = Convert.ToString(decimalNumber, 8); // Convertim DEC în OCT
                binaryTextBox.Text = Convert.ToString(decimalNumber, 2); // Convertim DEC în BIN
            }
            catch
            {
                hexTextBox.Text = "0";
                octalTextBox.Text = "0";
                binaryTextBox.Text = "0";
            }
        }

        private void ConvertFromOctal(string octValue)
        {
            try
            {
                int decimalValue = Convert.ToInt32(octValue, 8); // Convertim OCT în DEC
                hexTextBox.Text = decimalValue.ToString("X");  // Actualizăm HEX
                decimalTextBox.Text = decimalValue.ToString(); // Actualizăm DEC
                binaryTextBox.Text = Convert.ToString(decimalValue, 2); // Actualizăm BIN
            }
            catch
            {
                hexTextBox.Text = "0";
                decimalTextBox.Text = "0";
                binaryTextBox.Text = "0";
            }
        }

        private void ConvertFromBinary(string binValue)
        {
            try
            {
                int decimalValue = Convert.ToInt32(binValue, 2); // Convertim BIN în DEC
                hexTextBox.Text = decimalValue.ToString("X"); // Actualizăm HEX
                decimalTextBox.Text = decimalValue.ToString(); // Actualizăm DEC
                octalTextBox.Text = Convert.ToString(decimalValue, 8); // Actualizăm OCT
            }
            catch
            {
                hexTextBox.Text = "0";
                decimalTextBox.Text = "0";
                octalTextBox.Text = "0";
            }
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
