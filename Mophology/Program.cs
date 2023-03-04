using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Morphology.Utils;

namespace Morphology
{
    public static class Program
    {
        public static string DictionaryPath => Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!,
            @"Resources\dict.opcorpora.txt"
        );

        private static void Main()
        {
            //PrintGrammems(DictionaryPath);

            var sw = Stopwatch.StartNew();
            var morpher = SentenceMorpher.Create(FileLinesEnumerable.Create(DictionaryPath));
            Console.WriteLine($"Init took {sw.Elapsed}");
            RunLoop(morpher);
        }

        public static void RunLoop(SentenceMorpher morpher)
        {
            var input = "мама{noun,anim,femn,sing,gent} мыла РАМА{noun,inan,femn,sing,accs}";
            var sw = new Stopwatch();

            sw.Restart();
            var result = morpher.Morph(input);
            Console.WriteLine($"[took {sw.Elapsed}]   {result}");

            input = @"ОДНАЖДЫ{ADVB}
В{NOUN,anim,ms-f,Sgtm,Fixd,Abbr,Init,nomn}
СТУДЁНЫЙ{ADJF,Qual,femn,sing,accs}
ЗИМНИЙ{ADJF,femn,accs}
ПОРА{sing,accs}
Я{NOUN,anim,ms-f,Sgtm,Fixd,Abbr,Patr,Init,sing,nomn}
ИЗА{NOUN,anim,plur,gent}
ЛЕСА{NOUN,inan,femn,sing,accs}
ВЫШЕЛ{VERB,perf,intr,sing,indc}
ЕСТЬ{VERB,impf,intr,masc,sing,past,indc}
СИЛЬНЫЙ{Qual,masc,nomn}
МОРОЗ{anim,femn,Sgtm,Surn,sing,nomn}
ГЛЯЖУ{VERB,impf,tran,sing,pres,indc}
ПОДНИМАЮСЬ{VERB,impf,intr,3per,pres,indc}
МЕДЛЕН{Qual,neut}
В{NOUN,ms-f,Fixd,Abbr,Patr,Init,sing,nomn}
ГОРА{NOUN,inan,femn,sing,accs}
ЛОШАДКА{NOUN,anim,femn,sing}
ВЕЗУЩИЙ{impf,pres,actv,femn,sing,nomn}
ХВОРОСТ{NOUN,gen2}
ВОЗ{NOUN,femn,Fixd,Abbr,Orgn,sing,nomn}";
            do
            {
                sw.Restart();
                result = morpher.Morph(input);
                Console.WriteLine($"[took {sw.Elapsed}]   {result}");
            } while ((input = Console.ReadLine()) is { Length: > 0 });
        }

        public static void PrintGrammems(string path)
        {
            HashSet<string> grammems = new();
            grammems.EnsureCapacity(100);
            GrammemReadState state = GrammemReadState.Start;
            foreach (var line in File.ReadLines(path))
            {
                switch (state)
                {
                    case GrammemReadState.Start:
                        if (int.TryParse(line, out int number))
                        {
                            state = GrammemReadState.StartMiddle;
                            Console.WriteLine(number);
                        }
                        break;
                    case GrammemReadState.StartMiddle:
                        grammems.UnionWith(line.Split(" \t".ToCharArray()).Skip(1).SelectMany(str => str.Split(',')));
                        state = GrammemReadState.Middle;
                        break;
                    case GrammemReadState.Middle:
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            state = GrammemReadState.Start;
                            break;
                        }
                        grammems.UnionWith(line.Split(" \t".ToCharArray()).Skip(2).SelectMany(str => str.Split(',')));
                        break;
                }
            }
            foreach (var grammem in grammems)
                Console.WriteLine(grammem);
        }

        private enum GrammemReadState
        {
            Start, Middle, End,
            StartMiddle
        }

        
    }
}