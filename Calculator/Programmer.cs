using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Calculator
{
    public class Programmer
    {
        private bool isDigitGroupingEnabled = false;

        private TextBox textBox;
        public TextBox hexTextBox;
        public TextBox decimalTextBox;
        public TextBox octalTextBox;
        public TextBox binaryTextBox;

        private double currentValue = 0;
        private string lastOperator = "";
        private bool isNewEntry = true;

        public int baseNumber;

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

            if (Keyboard.IsKeyDown(Key.Add) || Keyboard.IsKeyDown(Key.OemPlus))
            {
                HandleOperatorClick("+");
            }
            else if (Keyboard.IsKeyDown(Key.Subtract) || Keyboard.IsKeyDown(Key.OemMinus))
            {
                HandleOperatorClick("-");
            }
            else if (Keyboard.IsKeyDown(Key.Multiply))
            {
                HandleOperatorClick("*");
            }
            else if (Keyboard.IsKeyDown(Key.Divide))
            {
                HandleOperatorClick("/");
            }
        }

        public void UpdateAllBases(string value, int baseSelected)
        {
            try
            {
                value = value.Replace(",", "");
                //MessageBox.Show($"Valoare introdusă: {value}, Baza selectată: {baseSelected}");
                int decimalValue = 0;

                if (baseSelected == 10)
                {
                    decimalValue = int.Parse(value);
                }
                else
                {
                    switch (baseSelected)
                    {
                        case 16: // HEX
                            decimalValue = Convert.ToInt32(value, 16);
                            break;
                        case 8: // OCT
                            decimalValue = Convert.ToInt32(value, 8);
                            break;
                        case 2: // BIN
                            decimalValue = Convert.ToInt32(value, 2);
                            break;
                        default:
                            decimalValue = 0;
                            break;
                    }
                }

                //MessageBox.Show($"Valoarea convertită: {decimalValue}");

                hexTextBox.Text = decimalValue.ToString("X");  // HEX
                decimalTextBox.Text = decimalValue.ToString();  // DEC
                octalTextBox.Text = Convert.ToString(decimalValue, 8);  // OCT
                binaryTextBox.Text = Convert.ToString(decimalValue, 2);  // BIN

                //MessageBox.Show($"HEX: {hexTextBox.Text}, DEC: {decimalTextBox.Text}, OCT: {octalTextBox.Text}, BIN: {binaryTextBox.Text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la conversie: {ex.Message}");
                ResetTextBoxes();
            }
        }

        public void ResetTextBoxes()
        {
            hexTextBox.Text = "0";
            decimalTextBox.Text = "0";
            octalTextBox.Text = "0";
            binaryTextBox.Text = "0";
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

            if (!IsValidCharacter(e.Text))
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
                textBox.Text += e.Text;

                UpdateTextBoxWithGrouping();

                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                e.Handled = true;
            }
        }

        public void HandleDigitGroupingChecked(bool isChecked)
        {
            isDigitGroupingEnabled = isChecked;
            if (isChecked)
                UpdateTextBoxWithGrouping();
            else
            {
                textBox.Text = textBox.Text.Replace(",", "");
            }

            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
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

            bool isNegative = input.StartsWith("-");
            if (isNegative)
            {
                // Dacă este negativ, elimină semnul pentru a aplica gruparea pe partea pozitivă
                input = input.Substring(1);
            }

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

            if (isNegative)
            {
                grouped.Insert(0, "-");
            }

            return grouped.ToString();
        }

        public void TextBox_PreviewKeyDown()
        {
            UpdateTextBoxWithGrouping();
        }

        private bool IsValidCharacter(string character)
        {
            if (character == "." && textBox.Text.Contains("."))
            {
                return false;
            }

            if (textBox.Text.Length >= 9)
            {
                return false;
            }

            if (baseNumber == 2)
            {
                return character == "0" || character == "1";
            }
            // Dacă baza este octală (baza 8), permitem doar caracterele 0-7
            else if (baseNumber == 8)
            {
                return "01234567".Contains(character);
            }
            // Dacă baza este zecimală (baza 10), permitem doar caracterele 0-9
            else if (baseNumber == 10)
            {
                return "0123456789".Contains(character);
            }
            // Dacă baza este hexazecimală (baza 16), permitem caracterele 0-9 și A-F
            else if (baseNumber == 16)
            {
                return "0123456789ABCDEFabcdef".Contains(character.ToUpper());
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

            if (textBox.Text.Length < 12)
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

        public void HandleOperatorClick(string operatorText)
        {
            double currentNumber = Convert.ToDouble(decimalTextBox.Text);
            //MessageBox.Show($"Numar in baza 10 curent:{currentNumber}");

            if (double.TryParse(decimalTextBox.Text, out currentNumber))
            {
                if (isNewEntry)
                {
                    currentValue = currentNumber;
                    isNewEntry = false;
                    textBox.Clear();
                }
                else
                {
                    PerformCalculation(currentNumber);
                }

                if (operatorText == "+/-")
                {
                    PerformUnaryCalculation(operatorText);
                    decimalTextBox.Text = currentValue.ToString();
                    isNewEntry = true;
                }
                else
                {
                    lastOperator = operatorText;
                }
            }
            //MessageBox.Show($"Numar in baza 10 curent:{currentNumber}");
        }

        public void PerformCalculation(double secondOperand)
        {
            switch (lastOperator)
            {
                case "+":
                    currentValue += secondOperand;
                    break;
                case "-":
                    currentValue -= secondOperand;
                    break;
                case "*":
                    currentValue *= secondOperand;
                    break;
                case "/":
                    if (secondOperand != 0)
                    {
                        currentValue /= secondOperand;
                    }
                    else
                    {
                        textBox.Text = "Error";
                        return;
                    }
                    break;
                case "%":
                    currentValue %= secondOperand;
                    break;
                case "=":
                    break;
                default:
                    break;
            }
            currentValue = Math.Round(currentValue, 9);
            //MessageBox.Show($"Value:{currentValue}");
            UpdateAllBases(currentValue.ToString(), 10);
            if (baseNumber == 2)
                textBox.Text = binaryTextBox.Text;
            if (baseNumber == 8)
                textBox.Text = octalTextBox.Text;
            if (baseNumber == 10)
                textBox.Text = decimalTextBox.Text;
            if (baseNumber == 16)
                textBox.Text = hexTextBox.Text;
            isNewEntry = true;
        }

        public void PerformUnaryCalculation(string operatorText)
        {
            switch (operatorText)
            {

                case "+/-":
                    currentValue = -currentValue;
                    break;
            }
            currentValue = Math.Round(currentValue, 9);
            //MessageBox.Show($"Value:{currentValue}");
            UpdateAllBases(currentValue.ToString(), 10);
            if (baseNumber == 2)
                textBox.Text = binaryTextBox.Text;
            if (baseNumber == 8)
                textBox.Text = octalTextBox.Text;
            if (baseNumber == 10)
                textBox.Text = decimalTextBox.Text;
            if (baseNumber == 16)
                textBox.Text = hexTextBox.Text;
        }

        public void ClearResult()
        {
            textBox.Clear();
            textBox.Text = "0";
            currentValue = 0;
            isNewEntry = true;
            UpdateAllBases("0", baseNumber);
        }

        public void ClearEntry()
        {
            textBox.Text = "0";
            UpdateAllBases("0", baseNumber);
        }

        public void HandleLogicalOperation(string operatorText)
        {
            try
            {
                string sanitizedInput = textBox.Text.Replace(",", "");

                if (operatorText == "NOT")
                {
                    int value = Convert.ToInt32(sanitizedInput, 2);

                    //8 bit
                    int result = ~value & 0xFF;

                    string binaryResult = Convert.ToString(result, 2).PadLeft(8, '0');

                    binaryTextBox.Text = binaryResult;
                    decimalTextBox.Text = result.ToString();
                    octalTextBox.Text = Convert.ToString(result, 8);
                    hexTextBox.Text = Convert.ToString(result, 16).ToUpper();

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message);
            }
        }
    }
}
