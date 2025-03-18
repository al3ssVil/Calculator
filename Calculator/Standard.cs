using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calculator
{
    public class Standard
    {
        private bool isDigitGroupingEnabled = false;
        private TextBox textBox;

        private double currentValue = 0;  
        private string lastOperator = "";  
        private bool isNewEntry = true;

        private List<double> memoryStack = new List<double>();
        private double memoryValue = 0;

        public Standard(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void HandleKeyDown()
        {
            textBox.Focus();

            if (Keyboard.IsKeyDown(Key.Add) || Keyboard.IsKeyDown(Key.OemPlus)) // pentru "+"
            {
                HandleOperatorClick("+");
            }
            else if (Keyboard.IsKeyDown(Key.Subtract) || Keyboard.IsKeyDown(Key.OemMinus)) // pentru "-"
            {
                HandleOperatorClick("-");
            }
            else if (Keyboard.IsKeyDown(Key.Multiply)) // pentru "*"
            {
                HandleOperatorClick("*");
            }
            else if (Keyboard.IsKeyDown(Key.Divide) || Keyboard.IsKeyDown(Key.OemQuestion)) // pentru "/"
            {
                HandleOperatorClick("/");
            }
        }

        //keyboard input
        public void HandlePreviewTextInput(TextCompositionEventArgs e)
        {
            int maxLength = 9;
            if (textBox.Text.Length >= maxLength)
            {
                e.Handled = true;
                return;
            }

            if (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0."))//123 not 0123
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

                // Select all the text to avoid losing the cursor position
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;

                e.Handled = true; // Prevents the direct input of the character into the TextBox.
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

        //for delete input on the keyboard
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

        public void HandleOperatorClick(string operatorText)
        {
            double currentNumber;
            if (double.TryParse(textBox.Text, out currentNumber))//string to double( fail or not)
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

                if (operatorText == "^2" || operatorText == "sqrt" || operatorText == "+/-" || operatorText == "1/x")
                {
                    PerformUnaryCalculation(operatorText);
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
            currentValue = Math.Round(currentValue, 4);
            //MessageBox.Show($"Value:{currentValue}");
            textBox.Text = currentValue.ToString();
            UpdateTextBoxWithGrouping();
            isNewEntry = true;
        }

        public void PerformUnaryCalculation(string operatorText)
        {
            switch (operatorText)
            {
                case "^2":
                    currentValue = Math.Pow(currentValue, 2);  
                    break;
                case "sqrt":
                    if (currentValue >= 0)
                    {
                        currentValue = Math.Sqrt(currentValue);  
                    }
                    else
                    {
                        textBox.Text = "Error";  
                        return;
                    }
                    break;
                case "+/-":
                    currentValue = -currentValue; 
                    break;
                case "1/x":
                    if (currentValue != 0)
                    {
                        currentValue = 1 / currentValue;  
                    }
                    else
                    {
                        textBox.Text = "Error"; 
                        return;
                    }
                    break;
            }
            currentValue = Math.Round(currentValue, 4);
            //MessageBox.Show($"Value:{currentValue}");  
            textBox.Text = currentValue.ToString();
            UpdateTextBoxWithGrouping();
            isNewEntry = true;
        }

        public void ClearResult()
        {
            textBox.Clear(); 
            textBox.Text = "0";
            currentValue = 0; 
            isNewEntry = true;
        }

        public void ClearEntry()
        {
            textBox.Text = "0";  
        }

        public void MemoryClear(int selectedIndex)//MC
        {
            if (selectedIndex == -1) // Not selected, delete all
            {
                memoryStack.Clear();
                memoryValue = 0;
            }
            else
            {
                int index = GetSelectedMemoryIndex(selectedIndex);
                if (index >= 0)
                {
                    memoryStack.RemoveAt(index);
                    memoryValue = memoryStack.Count > 0 ? memoryStack[memoryStack.Count - 1] : 0;
                }
            }
        }

        public string MemoryRecall()//MR
        {
            return memoryValue.ToString();
        }

        public void MemoryAdd(string input, int selectedIndex)//M+
        {
            if (double.TryParse(input, out double value))
            {
                int index = GetSelectedMemoryIndex(selectedIndex);
                if (index >= 0)
                {
                    memoryStack[index] += value;
                    memoryValue = memoryStack[index];
                }
            }
        }

        public void MemorySubtract(string input, int selectedIndex)//M-
        {
            if (double.TryParse(input, out double value))
            {
                int index = GetSelectedMemoryIndex(selectedIndex);
                if (index >= 0)
                {
                    memoryStack[index] -= value;
                    memoryValue = memoryStack[index];
                }
            }
        }

        public void MemoryStore(string input)//MC
        {
            if (double.TryParse(input, out double value))
            {
                memoryValue = value;
                memoryStack.Add(value);
            }
        }

        private int GetSelectedMemoryIndex(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < memoryStack.Count)
                return selectedIndex; 

            return memoryStack.Count > 0 ? memoryStack.Count - 1 : -1; 
        }

        public List<double> GetMemoryStack()
        {
            return memoryStack;
        }

    }
}
