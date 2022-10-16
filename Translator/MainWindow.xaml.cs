﻿using System;
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

        string ProgramName = "program1";

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
            var commands = new List<string>();
            var templine = String.Empty;
            foreach (var cha in inputData)
            {
                if (cha == ';')
                {
                    commands.Add(templine);
                    
                    templine = String.Empty;
                }
                else
                {
                    templine += cha.ToString();
                }
            }
            commands.Add(templine);
            
            var outputData = String.Empty;
            foreach (var cmd in commands)
            {
                outputData += TranslateCommand(cmd);
            }
            return outputData;
        }
        public string TranslateCommand(string command)
        {
            var lexems = GenerateLexem(command);
            string comOut;
            //MessageBox.Show(lexems[0]);
            if (lexems.Count > 0)
            {
                if (lexems[0].ToLower() == "program") comOut = DoIfProgram(lexems);
                else if (lexems[0].ToLower() == "begin") comOut = DoIfBegin(lexems);
                else if (lexems[0].ToLower() == "end.") comOut = DoIfEnd(lexems);
                else if (lexems[0] == "") comOut = "";
                else comOut = "\nError Lexem";
            }
            else comOut = "";
            return comOut;
        }

        public List<string> GenerateLexem(string line)
        {
            var lexemList = new List<string>();
            var tempLine = String.Empty;
            foreach (var cha in line)
            {
                if ((cha == ' ')||(cha == '\n') || (cha == '\r'))
                {
                    lexemList.Add(tempLine);
                    tempLine = String.Empty;
                }
                else if ("+:=*/-(),".Contains(cha))
                {
                    lexemList.Add(tempLine);
                    lexemList.Add(cha.ToString());
                    tempLine = String.Empty;
                }
                else
                {
                    tempLine += cha.ToString();
                }
            }
            lexemList.Add(tempLine);
            int i = 0;
            while (i < lexemList.Count)
            {
                if ("\n\t ".Contains(lexemList[i])) lexemList.RemoveAt(i);
                else i++;
            }
            
            return lexemList;
        }
        public string DoIfProgram(List<string> lexems)
        {
            var output = "// This program named: ";
            lexems.RemoveAt(0);
            var name = String.Empty;
            foreach (var lexem in lexems) name += lexem;
            ProgramName = name;
            output += name;
            return output;
        }

        public string DoIfBegin(List<string> lexems)
        {
            var output = "\nusing System;\nusing System.Collections.Generic;\n" +
                "using System.Linq;\nusing System.Text;\n" +
                $"namespace {ProgramName};\n"+"{\n" +
                $"    class {ProgramName}\n" + "    {\n" +
                "\tstatic void Main(string[] args)\n" + "\t{\n";
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            foreach (var lexem in lexems) tempout += lexem;
            output += TranslateCommand(tempout);
            return output;
        }

        public string DoIfEnd(List<string> lexems)
        {
            var output = "\t}\n    }\n}";
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            foreach (var lexem in lexems) tempout += lexem;
            output += TranslateCommand(tempout);
            return output;
        }
    }
}
