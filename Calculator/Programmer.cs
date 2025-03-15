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
        public TextBox hexTextBox;
        public TextBox decimalTextBox;
        public TextBox octalTextBox;
        public TextBox binaryTextBox;

        private double currentValue = 0;
        private string lastOperator = "";
        private bool isNewEntry = true;

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
            double currentNumber;
            if (double.TryParse(textBox.Text, out currentNumber))
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

                if (operatorText == "+/-" )
                {
                    PerformUnaryCalculation(operatorText);
                    textBox.Text = currentValue.ToString();
                    isNewEntry = true;
                }
                else
                {
                    lastOperator = operatorText;
                }
            }
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
            textBox.Text = currentValue.ToString();
            UpdateAllBases(currentValue.ToString(), 10);
            textBox.Text = ApplyGrouping(currentValue.ToString());
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
            textBox.Text = currentValue.ToString();
            textBox.Text = ApplyGrouping(currentValue.ToString());
        }

        public void ClearResult()
        {
            textBox.Clear(); 
            textBox.Text = "0"; 
            currentValue = 0; 
            isNewEntry = true; 
        }

    }
}
