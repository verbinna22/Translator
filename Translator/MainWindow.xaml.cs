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
            SetFirstValue();
            textBlockOut.IsReadOnly = true;

        }

        private void SetFirstValue()
        {
            textBlockIn.AppendText(@"
var y:integer;
    z: string;
    x: real;
begin
    x := 5;
    if (x = 0) then x:= 2;
    else x:= 2;
    for y:= 50 downto 7 do Writeln(5);
end.");
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
            try
            {
                TextRange textRange = new TextRange(
                textBlockIn.Document.ContentStart,
                textBlockIn.Document.ContentEnd);
                var input = textRange.Text;
                exVars = String.Empty;
                step = variator.noney;
                Inicializer(input);
            }
            catch (Exception)
            {

            }
        }

        private void Inicializer(string input)
        {
            varNames = new List<string>();
            if (new TextRange(
            textBlockOut.Document.ContentStart,
            textBlockOut.Document.ContentEnd).Text.Count() < 100) ShowMessage();
            var output = Translate(input);
            textBlockOut.Document.Blocks.Clear();
            textBlockOut.AppendText("// Program in C#\r");
            ((Paragraph)textBlockOut.Document.Blocks.FirstBlock).LineHeight = 5;
            textBlockOut.AppendText(output);
        }

        private static void ShowMessage()
        {
            MessageBox.Show("Транслятор переводит с языка Pascal на С#." +
                            "Транслятор умеет: объявлять 5 типов переменных:" +
                            "char, integer, real, string, boolean; использовать базовые" +
                            "арифметические операции: *, /, +, -, DIV, MOD;" +
                            "логические: AND, OR, NOT, XOR; операцию присваивания :=" +
                            "конструкции if-then-else, while и for; " +
                            "сравнения =, >, <. " +
                            "Программа может не начинаться со слова program." +
                            "Переменные могут быть объявлены до блока begin");
        }

        public string Translate(string inputData)
        {
            var commands = new List<string>();
            var templine = String.Empty;
            templine = ParseCommand(inputData, commands, templine);
            var outputData = String.Empty;
            outputData = BeginTranslation(commands, outputData);
            return outputData;
        }

        private string BeginTranslation(List<string> commands, string outputData)
        {
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

        private static string ParseCommand(string inputData, List<string> commands, string templine)
        {
            foreach (var cha in inputData)
            {
                if (cha == ';')
                {
                    commands.Add(templine);
                    templine = String.Empty;
                }
                else templine += cha.ToString();
            }
            commands.Add(templine);
            return templine;
        }

        public string TranslateCommand(string command)
        {
            var lexems = GenerateLexem(command);
            string comOut;
            if (lexems.Count > 0)
            {
                comOut = ChoseLexem(lexems);
            }
            else comOut = "";
            return comOut;
        }

        private string ChoseLexem(List<string> lexems)
        {
            string comOut;
            if (lexems[0] == "if") comOut = DoIfIf(lexems);
            else if (lexems[0] == "for") comOut = DoIfFor(lexems);
            else if (lexems[0] == "while") comOut = DoIfWhile(lexems);
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
            return comOut;
        }

        public bool CondiSet(List<string> lexems)
        {
            return lexems.Contains(":") && 
                   lexems.IndexOf(":") != (lexems.Count - 1) &&
                   lexems[lexems.IndexOf(":") + 1] != "=";
        }

        public bool CondiSeter(List<string> lexems)
        {
            if ((lexems.Contains(":")) && (lexems.IndexOf(":") != (lexems.Count - 1)) &&
                (lexems[lexems.IndexOf(":") + 1] == "=")) return true;
            return false;
        }
        public List<string> GenerateLexem(string line)
        {
            var lexemList = new List<string>();
            var tempLine = String.Empty;
            tempLine = ParseLexems(line, lexemList, tempLine);
            lexemList.Add(tempLine);
            int i = 0;
            while (i < lexemList.Count)
            {
                if ("\n\t ".Contains(lexemList[i])) lexemList.RemoveAt(i);
                else i++;
            }
            return lexemList;
        }

        private static string ParseLexems(string line, List<string> lexemList, string tempLine)
        {
            foreach (var cha in line)
            {
                if ((cha == ' ') || (cha == '\n') || (cha == '\r'))
                {
                    lexemList.Add(tempLine.ToLower());
                    tempLine = String.Empty;
                }
                else if ("+:=*/-(),<>".Contains(cha))
                {
                    lexemList.Add(tempLine.ToLower());
                    lexemList.Add(cha.ToString());
                    tempLine = String.Empty;
                }
                else
                {
                    tempLine += cha.ToString();
                }
            }
            return tempLine;
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
                $"namespace {ProgramName}\n" + "{\n" +
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
            CreateListValid(validList);
            string type;
            if (validList.Contains(lexems[lexems.Count - 1].ToLower()))
            {
                type = TranslateType(lexems[lexems.Count - 1]);
            }
            else
            {
                return "Error type\n";
            }
            output = TypeTranslate(lexems, output, type);
            if (step == variator.beginy) return output;
            exVars += output;
            return String.Empty;
        }

        private string TypeTranslate(List<string> lexems, string output, string type)
        {
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

            return output;
        }

        private static void CreateListValid(List<string> validList)
        {
            validList.Add("char");
            validList.Add("string");
            validList.Add("integer");
            validList.Add("real");
            validList.Add("boolean");
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
            output = BeforeTranslate(lexems, output);
            lexems.RemoveAt(0);
            output += " = ";
            var tempOut = String.Empty;
            tempOut = AfterTranslate(lexems, tempOut);
            output += TranslateCommand(tempOut);
            return output;
        }

        private static string AfterTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (" " + lexem);
            }

            return tempOut;
        }

        private static string BeforeTranslate(List<string> lexems, string output)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == ":") break;
                output += (lexem);
            }

            return output;
        }

        public string DoIfWriteln(List<string> lexems)
        {
            var output = String.Empty;
            var tempOut = String.Empty;
            tempOut = BeforeWritelnTranslate(lexems, tempOut);
            output += TranslateCommand(tempOut);
            output += "Console.WriteLine";
            tempOut = String.Empty;
            tempOut = AfterWritelnTranslate(lexems, tempOut);
            output += TranslateCommand(tempOut);
            return output;
        }

        private static string AfterWritelnTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (lexem);
            }

            return tempOut;
        }

        private static string BeforeWritelnTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "writeln") break;
                tempOut += (lexem);
            }

            return tempOut;
        }

        public string DoIfWrite(List<string> lexems)
        {
            var output = String.Empty;
            var tempOut = String.Empty;
            tempOut = BeforeWriteTranslate(lexems, tempOut);
            output += TranslateCommand(tempOut);
            output += "Console.Write";
            tempOut = String.Empty;
            tempOut = AfterWriteTranslate(lexems, tempOut);
            output += TranslateCommand(tempOut);
            return output;
        }

        private static string AfterWriteTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                tempOut += (" " + lexem);
            }

            return tempOut;
        }

        private static string BeforeWriteTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "write") break;
                tempOut += (" " + lexem);
            }

            return tempOut;
        }

        public string DoIfIf(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "if ";
            var tempOut = String.Empty;
            tempOut = BeforeIfTranslate(lexems, tempOut);
            if (lexems.Count != 0)
            {
                lexems.RemoveAt(0);
                output += (" " + TranslateCommand(tempOut));
            }
            output = BeginIfTranslate(lexems, output);
            tempOut = String.Empty;
            tempOut = AfterIfTranslate(lexems, tempOut);
            output += (" " + TranslateCommand(tempOut));
            output = EndElseTranslate(lexems, output);
            return output;
        }

        private string EndElseTranslate(List<string> lexems, string output)
        {
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

        private static string AfterIfTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lexems.RemoveAt(0);
                if (lexem == "end") break;
                if (lexem == "else") break;
                tempOut += (" " + lexem);
            }

            return tempOut;
        }

        private static string BeginIfTranslate(List<string> lexems, string output)
        {
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
            }

            return output;
        }

        private static string BeforeIfTranslate(List<string> lexems, string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                if (lexem == "then") break;
                tempOut += (" " + lexem);
                lexems.RemoveAt(0);
            }

            return tempOut;
        }

        public string DoIfWhile(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "while ";
            var tempOut = String.Empty;
            BeforeWhileTranslate(lexems, ref output, ref tempOut);
            output = BeginWhileTranslate(lexems, output);
            tempOut = AfterWhileTranslate(lexems, ref output);
            return output;
        }

        private string AfterWhileTranslate(List<string> lexems, ref string output)
        {
            string tempOut = String.Empty;
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
            return tempOut;
        }

        private static string BeginWhileTranslate(List<string> lexems, string output)
        {
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
            }
            return output;
        }

        private void BeforeWhileTranslate(List<string> lexems, ref string output, ref string tempOut)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                if (lexem == "do") break;
                tempOut += (" " + lexem);
                lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                lexems.RemoveAt(0);
                output += (" " + TranslateCommand(tempOut));
            }
        }

        public string DoIfFor(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "for ( ";
            var tempOut = String.Empty;
            var tempVar = String.Empty;
            List<string> lastLexems = new List<string>();
            tempOut = BeforForTranslate(lexems, tempOut, lastLexems);
            UpgradingDowngrading(lexems, ref output, tempOut, ref tempVar, lastLexems);
            output = BeginTranslate(lexems, output);
            tempOut = AfterForTranslate(lexems, ref output);
            return output;
        }

        private string AfterForTranslate(List<string> lexems, ref string output)
        {
            string tempOut = String.Empty;
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
            return tempOut;
        }

        private static string BeginTranslate(List<string> lexems, string output)
        {
            if (lexems.Count != 0)
            {
                if (lexems[0] == "do") lexems.RemoveAt(0);
            }
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
            }
            return output;
        }

        private void UpgradingDowngrading(List<string> lexems,
            ref string output, string tempOut, ref string tempVar, List<string> lastLexems)
        {
            if (lexems.Count != 0)
            {
                output += (" " + TranslateCommand(tempOut));
                foreach (var lexem in lastLexems)
                {
                    if (varNames.Contains(lexem)) tempVar = lexem;
                }
                if (lexems[0] == "to") output = Upgrading(lexems, output, tempVar);
                else if (lexems[0] == "downto") output = DownGrading(lexems, output, tempVar);
            }
        }

        private static string DownGrading(List<string> lexems, string output, string tempVar)
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
            return output;
        }

        private static string Upgrading(List<string> lexems, string output, string tempVar)
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
            return output;
        }

        private static string BeforForTranslate(List<string> lexems, string tempOut,
            List<string> lastLexems)
        {
            while (lexems.Count > 0)
            {
                var lexem = lexems[0];
                lastLexems.Add(lexem);
                if (lexem == "to") break;
                else if (lexem == "downto") break;
                tempOut += (" " + lexem);
                lexems.RemoveAt(0);
            }

            return tempOut;
        }

        public string DoIfElse(List<string> lexems)
        {
            lexems.RemoveAt(0);
            var output = "else ";
            var tempOut = String.Empty;
            output = BeforeElseTranslate(lexems, output);
            AfterElseTranslate(lexems, ref output, ref tempOut);
            if (lexems.Count != 0)
            {
                if (lexems[0] == "else") output += DoIfElse(lexems);
            }
            return output;
        }

        private void AfterElseTranslate(List<string> lexems, ref string output, ref string tempOut)
        {
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
        }

        private static string BeforeElseTranslate(List<string> lexems, string output)
        {
            if (lexems.Count != 0)
            {
                if (lexems[0] == "begin")
                {
                    output += "\n{\n";
                    lexems.RemoveAt(0);
                }
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
                output = LineReturn(output, lexem);
            }
            return output;
        }

        private string LineReturn(string output, string lexem)
        {
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
            return output;
        }
    }
}
