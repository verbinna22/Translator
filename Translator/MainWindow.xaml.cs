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

        string ProgramName = "program1";
        List<string> varNames = new List<string>();
        public enum variator
        {
            vari,
            beginy,
            noney
        }

        string exVars = String.Empty;
        variator step = variator.noney;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(
            textBlockIn.Document.ContentStart,
            textBlockIn.Document.ContentEnd);
            var input = textRange.Text;
            exVars = String.Empty;
            step = variator.noney;
            varNames = new List<string>();
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
                if (outputData[outputData.Length - 1] != '\n')
                {
                    outputData += ";\n";
                }
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
                if (lexems[0] == "if") comOut = DoIfIf(lexems);
                else if (lexems[0] == "for") comOut = DoIfFor(lexems);
                else if (lexems[0] == "else") comOut = DoIfElse(lexems);
                else if (lexems[0] == "program") comOut = DoIfProgram(lexems);
                else if (lexems[0] == "begin") comOut = DoIfBegin(lexems);
                else if (lexems[0] == "end.") comOut = DoIfEnd(lexems);
                else if (lexems[0] == "end") comOut = DoIfEnd2(lexems);
                else if (lexems[0] == "var") comOut = DoIfVar(lexems);
                
                else if (CondiSet(lexems)) comOut = DoIfSet(lexems);
                else if (CondiSeter(lexems)) comOut = DoIfSeter(lexems);
                else if (lexems.Contains("writeln")) comOut = DoIfWriteln(lexems);
                else if (lexems.Contains("write")) comOut = DoIfWrite(lexems);
                
                else if (lexems[0] == "") comOut = "";
                else comOut = DoIfLine(lexems);
            }
            else comOut = "";
            return comOut;
        }

        public bool CondiSet(List<string> lexems)
        {
            if (lexems.Contains(":"))
            {
                if (lexems.IndexOf(":") != (lexems.Count - 1))
                {
                    if (lexems[lexems.IndexOf(":") + 1] != "=")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CondiSeter(List<string> lexems)
        {
            if (lexems.Contains(":"))
            {
                if (lexems.IndexOf(":") != (lexems.Count - 1))
                {
                    if (lexems[lexems.IndexOf(":") + 1] == "=")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public List<string> GenerateLexem(string line)
        {
            var lexemList = new List<string>();
            var tempLine = String.Empty;
            foreach (var cha in line)
            {
                if ((cha == ' ') || (cha == '\n') || (cha == '\r'))
                {
                    lexemList.Add(tempLine.ToLower());
                    //MessageBox.Show(tempLine);
                    tempLine = String.Empty;
                }
                else if ("+:=*/-(),<>".Contains(cha))
                {
                    lexemList.Add(tempLine.ToLower());
                    //MessageBox.Show(tempLine);
                    lexemList.Add(cha.ToString());
                    tempLine = String.Empty;
                }
                else
                {
                    tempLine += cha.ToString();
                }
            }
            lexemList.Add(tempLine);
            //MessageBox.Show(tempLine);
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
            return output + "\n";
        }

        public string DoIfBegin(List<string> lexems)
        {
            var output = "using System;\nusing System.Collections.Generic;\n" +
                "using System.Linq;\nusing System.Text;\n" +
                $"namespace {ProgramName};\n" + "{\n" +
                $"    class {ProgramName}\n" + "    {\n" +
                "\tstatic void Main(string[] args)\n" + "\t{\n";
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            step = variator.beginy;
            output += exVars;
            foreach (var lexem in lexems) tempout += (" " + lexem);
            output += TranslateCommand(tempout);
            return output;
        }

        public string DoIfEnd(List<string> lexems)
        {
            var output = "\t}\n    }\n}\n";
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            foreach (var lexem in lexems) tempout += (" " + lexem);
            output += TranslateCommand(tempout);
            return output;
        }

        public string DoIfEnd2(List<string> lexems)
        {
            var output = "}\n";
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            foreach (var lexem in lexems) tempout += (" " + lexem);
            output += TranslateCommand(tempout);
            return output;
        }

        public string DoIfVar(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var tempout = String.Empty;
            foreach (var lexem in lexems) tempout += (' ' + lexem);
            return TranslateCommand(tempout);
        }

        public string DoIfSet(List<string> lexems)
        {
            var output = String.Empty;
            var validList = new List<string>();
            validList.Add("char");
            validList.Add("string");
            validList.Add("integer");
            validList.Add("real");
            validList.Add("boolean");
            string type;
            if (validList.Contains(lexems[lexems.Count - 1].ToLower()))
            {
                type = TranslateType(lexems[lexems.Count - 1]);
            }
            else
            {
                return "Error type\n";
            }
            int i = 0;
            while (i < lexems.Count)
            {
                var lexem = lexems[i];
                if (lexem == ":") break;
                else if (lexem != ",")
                {
                    output += (type + " " + lexem + ";\n");
                    varNames.Add(lexem);
                }
                i++;
            }
            if (step == variator.beginy) return output;
            exVars += output;
            //MessageBox.Show(output);
            return String.Empty;
        }

        public string TranslateType(string paskalType)
        {
            if (paskalType == "char") return "char";
            else if (paskalType == "integer") return "int";
            else if (paskalType == "real") return "double";
            else if (paskalType == "string") return "string";
            else return "bool";
        }
        public string DoIfSeter(List<string> lexems)
        {
            var output = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == ":") break;
                output += (lexem);
            }
            lexems.RemoveAt(0);
            output += " = ";
            var tempOut = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (" " + lexem);
            }
            output += TranslateCommand(tempOut);
            return output;
        }

        public string DoIfWriteln(List<string> lexems)
        {
            var output = String.Empty;
            var tempOut = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "writeln") break;
                tempOut += (lexem);
            }
            output += TranslateCommand(tempOut);
            output += "Console.WriteLine";
            tempOut = String.Empty;

            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (lexem);
            }
            output += TranslateCommand(tempOut);
            return output;
        }

        public string DoIfWrite(List<string> lexems)
        {
            var output = String.Empty;
            var tempOut = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "write") break;
                tempOut += (" " + lexem);
            }
            output += TranslateCommand(tempOut);
            output += "Console.Write";
            tempOut = String.Empty;

            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (" " + lexem);
            }
            output += TranslateCommand(tempOut);
            return output;
        }

        public string DoIfIf(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "if ";
            var tempOut = String.Empty;
            //MessageBox.Show(lexems.Count.ToString());
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                //MessageBox.Show(lexem);

                if (lexem == "then") break;
                tempOut += (" " + lexem);
                lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                lexems.RemoveAt(0);
                output += (" " + TranslateCommand(tempOut));
            }
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin") output += "\n{\n";
                lexems.RemoveAt(0);
            }
            tempOut = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "end") break;
                if (lexem == "else") break;
                tempOut += (" " + lexem);
            }
            output += (" " + TranslateCommand(tempOut));
            if (lexems.Count != 0)
            {
                if (lexems[0] == "end") output += "}\n";
                lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                if (lexems[0] == "else") output += DoIfElse(lexems);
            }
            return output;
        }

        public string DoIfFor(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "for ( ";
            var tempOut = String.Empty;
            //MessageBox.Show(lexems.Count.ToString());
            var tempVar = String.Empty;
            List <string> lastLexems = new List<string> ();
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                //MessageBox.Show(lexem);
                lastLexems.Add(lexem);
                if (lexem == "to") break;
                else if (lexem == "downto") break;
                tempOut += (" " + lexem);
                lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                output += (" " + TranslateCommand(tempOut));
                foreach (var lexem in lastLexems)
                {
                    if (varNames.Contains(lexem)) tempVar = lexem;
                }
                if (lexems[0] == "to")
                {
                    output += $";{tempVar} <=";
                    lexems.RemoveAt(0);
                    while (lexems.Count > 0)
                    {
                        if (lexems[0] == "do") break;
                        output += (" " + lexems[0]);
                        lexems.RemoveAt(0);
                    }
                    output += $"; {tempVar} ++)";
                }
                else if (lexems[0] == "downto")
                {
                    output += $";{tempVar} >=";
                    lexems.RemoveAt(0);
                    while (lexems.Count > 0)
                    {
                        if (lexems[0] == "do") break;
                        output += (" " + lexems[0]);
                        lexems.RemoveAt(0);
                    }
                    output += $"; {tempVar} --)";
                }
            }
            if (lexems.Count != 0)
            {
                //MessageBox.Show(lexems[0]);
                if (lexems[0] == "do") lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                //MessageBox.Show(lexems[0]);
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
            }
            tempOut = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "end") break;
                tempOut += (" " + lexem);
            }
            output += (" " + TranslateCommand(tempOut));
            if (lexems.Count != 0)
            {
                if (lexems[0] == "end") output += "}\n";
                lexems.RemoveAt(0);
            }
            return output;
        }

        public string DoIfElse(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "else ";
            var tempOut = String.Empty;
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
            }
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "end") break;
                if (lexem == "else") break;
                tempOut += (" " + lexem);
            }
            output += (" " + TranslateCommand(tempOut));
            if (lexems.Count != 0)
            {
                if (lexems[0] == "end") output += "}\n";
            }
            if (lexems.Count != 0)
            {
                if (lexems[0] == "else") output += DoIfElse(lexems);
            }
            return output;
        }
        public string DoIfLine(List<string> lexems)
        {
            var output = String.Empty;
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "and") output += "&&";
                else if (lexem == "or") output += "||";
                else if (lexem == "xor") output += "^";
                else if (lexem == "not") output += "!";
                else if (lexem == "div") output += "/";
                else if (lexem == "mod") output += "%";
                else if (lexem == "=") output += "==";
                else if (varNames.Contains(lexem)) output += lexem;
                else if ("+-/*();><,".Contains(lexem)) output += lexem;
                else if ("' \u0022".Contains(lexem[0])) output += lexem;
                else if ("0123456789".Contains(lexem[0])) output += lexem;
                else output = "\nError lexem";
            }
            return output;
        }
    }
}
