using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((Paragraph)textBlockIn.Document.Blocks.FirstBlock).LineHeight = 5;
            ((Paragraph)textBlockOut.Document.Blocks.FirstBlock).LineHeight = 5;
            textBlockOut.IsReadOnly = true;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(
            textBlockIn.Document.ContentStart,
            textBlockIn.Document.ContentEnd);
            var input = textRange.Text;
            var output = Translate(input);
            
            textBlockOut.Document.Blocks.Clear();
            textBlockOut.AppendText("// Program in C#\r");
            ((Paragraph)textBlockOut.Document.Blocks.FirstBlock).LineHeight = 5;
            textBlockOut.AppendText(output);
        }
        public string Translate(string inputData)
        {
            return inputData;
        }
    }
}
