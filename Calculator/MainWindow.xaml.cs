using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Setează focus-ul pe textBox, indiferent ce tastă a fost apăsată
            textBox.Focus();
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            int maxLength = 9;
            if (textBox != null)
            {
                if (textBox.Text.Length > maxLength)
                {
                    e.Handled = true; // Anulează inputul dacă nu este o cifră sau punct
                    return;
                }

                if (textBox.Text.StartsWith("0") && !textBox.Text.StartsWith("0."))
                {
                    textBox.Text = textBox.Text.Substring(1); // Elimina primul caracter (0)
                    textBox.Select(textBox.Text.Length, 0); // Plasează cursorul la sfârșit
                }

                if ((!char.IsDigit(e.Text, 0)) && e.Text[0] != '.')
                {
                    e.Handled = true; // Anulează inputul dacă nu este o cifră sau punct
                    return;
                }

                // Verifică dacă există deja un punct în text
                if (e.Text == "." && textBox.Text.Contains("."))
                {
                    e.Handled = true; // Anulează inputul dacă există deja un punct
                    return;
                }


                // Dacă se apasă ".", adăugăm automat un "0" înainte de punct
                if (e.Text == "." && textBox.Text.Length==0)
                {
                    textBox.Text = "0." + textBox.Text; // Adaugă "0" înainte de punct
                    textBox.Select(textBox.Text.Length, 0); // Plasează cursorul la sfârșit
                    e.Handled = true; // Anulează acțiunea de a adăuga punctul, pentru că l-am adăugat deja
                }
            }
        }
        private void Settings(object sender, RoutedEventArgs e)
        {
            if (SidebarMenu.Visibility == Visibility.Collapsed)
            {
                // Dacă meniul este ascuns, îl facem vizibil
                SidebarMenu.Visibility = Visibility.Visible;
            }
            else
            {
                // Dacă meniul este vizibil, îl ascundem
                SidebarMenu.Visibility = Visibility.Collapsed;
            }
        }
        private void Button_0Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button != null)
            {
                string? buttonContent = button.Content.ToString();
                TextBox textBox = this.textBox;
                // Simulează apelul la TextBox_PreviewTextInput pentru a valida înainte de a adăuga textul
                TextCompositionEventArgs args = new TextCompositionEventArgs(
                    Keyboard.PrimaryDevice, new TextComposition(InputManager.Current, textBox, buttonContent));
                args.RoutedEvent = TextBox.PreviewTextInputEvent;
                TextBox_PreviewTextInput(textBox, args); // Apelăm funcția pentru a aplica aceleași reguli

                // Dacă textul este valid, adaugă-l în TextBox
                if (!args.Handled)
                {
                    textBox.AppendText(button.Content.ToString());

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
            }
        }
        private void Button_PointClick(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            TextBox textBox = this.textBox;  // Asigură-te că textBox-ul este definit corect.
          
            if (button != null && textBox != null)
            {
                if (textBox.Text.Length == 0 || textBox.Text == "0")
                {
                    textBox.Text = "0.";  // Adaugă "0." la început

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
                // Verificăm dacă în text există deja un punct
                if (!textBox.Text.Contains("."))
                {
                    // Dacă nu există, adăugăm punctul
                    textBox.AppendText(".");

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
            }
        }
    }
}