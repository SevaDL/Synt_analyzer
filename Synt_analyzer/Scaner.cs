using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Synt_analyzer
{
    class Scaner
    {
        public const int
            MaxLex = 50,

            //id
            type_int = 1,
            type_id = 2,
            type_for = 3,
            type_while = 4,
            type_do = 5,

            // + - =
            type_sum = 11,
            type_sub = 12,
            type_save = 13,

            //; , '
            type_semi = 21,
            type_com = 22,

            //< > == !=
            type_less = 31,
            type_more = 32,
            type_eq = 33,
            type_neq = 34,


            //() {} 
            type_lbkt = 41,
            type_rbkt = 42,
            type_flbkt = 43,
            type_frbkt = 44,

            //целочисленная константа
            type_const = 51;

        public const int type_error = 100, type_end = 200;

        public static int uk, str, stb;
        public static char[] prText;

        public static void SetUk(int num)
        {
            uk = num;
        }

        public int GetUk()
        {
            return uk;
        }

        public int GetStr()
        {
            return str;
        }

        public int GetCol()
        {
            return stb;
        }

        public void PrintError(String error)
        {
            Console.WriteLine("Строка " + str + ", столбец " + stb + ". Ошибка " + error + ".");
            Console.WriteLine("\r");
        }
        public static void GetData(String file)
        {
            try
            {
                StreamReader sr = new StreamReader(file);
                int counter = 1;
                String s;
                String ProgCode = "";
                String Code = "";

                while ((s = sr.ReadLine()) != null)
                {
                    ProgCode += s + '\n'; // считываем текст программы (построчно),пока не закончится файл
                    Code += counter + ") " + s + '\n'; // считываем текст программы (построчно),пока не закончится файл
                    counter++;
                }
                Console.WriteLine("Текст программы: ");
                Console.WriteLine(Code);

                Console.WriteLine("Отсканированные лексемы: ");

                ProgCode += '\0';
                prText = ProgCode.ToCharArray(); // Конвертируем String в Char[]
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла!\n Текст ошибки:" + e);
                Console.WriteLine("\r\n");
            }
        }

        public Scaner(String file)
        {
            GetData(file);
            SetUk(0);
            str = 1;
            stb = 0;
        }
        public int Program(char[] l)
        {
            int i;
            for (i = 0; i < MaxLex; i++)
            {
                l[i] = '\0';
            }
            i = 0;
            //Пропуск пробелов и переносов строк
            {
                while ((prText[uk] == ' ') || (prText[uk] == '\t') || (prText[uk] == '\n'))
                {
                    if (prText[uk] == '\n')
                    {
                        str++;
                        stb = 0;
                    }
                    stb++;
                    uk++;
                    if (uk == prText.Length)
                    {
                        PrintError("Не найден конец");
                        return type_error;
                    }
                }

                if (prText[uk] == '/' && prText[uk + 1] == '/')// пропуск комментариев
                {
                    stb = 1;
                    uk += 2;
                    while (true)
                    {
                        if (prText[uk] == '\n')
                        {
                            uk++;
                            str++;
                            stb = 1;
                            break;
                        }
                        if (prText[uk] == '\t' || prText[uk] == ' ')
                        { stb++; }
                        uk++;
                    }
                }
                if (prText[uk] == '/' && prText[uk + 1] == '*')
                {
                    uk += 2;
                    while (true)
                    {
                        if (prText[uk] == '\n')
                        {
                            str++;
                            stb = 1;
                        }
                        if (prText[uk] == '\t' || prText[uk] == ' ')
                        { stb++; }
                        if (prText[uk] == '#')
                        { PrintError("Конец файла"); return type_error; }
                        if (prText[uk] == '*' && prText[uk + 1] == '/')
                        {
                            uk += 2;
                            str++;
                            stb = 1;
                            break;
                        }
                        uk++; stb++;
                    }
                    uk++;
                }
            }
            if (prText[uk] == '\0')//конец программы
            {
                l[0] = '#';
                return type_end;
            }//конец программы

            else if ((prText[uk] <= '9') && (prText[uk] >= '0'))
            {
                if (prText[uk] == '0')
                {// константа с 0
                    l[i++] = prText[uk++];
                    stb++;
                    if ((prText[uk] <= '9') && (prText[uk] >= '0'))
                    {// проверка на константу 0[0-9]+[.][0-9]+
                        while ((prText[uk] <= '9') && (prText[uk] >= '0') || (prText[uk] == '.'))
                        {
                            if (i < MaxLex - 1)
                            {
                                l[i++] = prText[uk++];
                                stb++;
                            }
                        }
                        PrintError("Неккоректная константа");
                        return type_error;
                    }
                    return type_const;
                }
                else
                {// константа не с 0
                    l[i++] = prText[uk++];
                    stb++;
                    while ((prText[uk] <= '9') && (prText[uk] >= '0'))
                    {
                        if (i < MaxLex - 1)
                        {
                            l[i++] = prText[uk++];
                            stb++;
                        }
                    }
                    return type_const;
                }
            }
            else if ((prText[uk] >= 'a') && (prText[uk] <= 'z') || (prText[uk] >= 'A') && (prText[uk] <= 'Z'))
            {
                stb++;
                l[i++] = prText[uk++];
                while ((prText[uk] <= '9') && (prText[uk] >= '0') || (prText[uk] >= 'a') && (prText[uk]
<= 'z') || (prText[uk] >= 'A') && (prText[uk] <= 'Z'))
                {
                    if (i < MaxLex - 1)
                    {
                        stb++;
                        l[i++] = prText[uk++];
                    }
                    else
                    {
                        stb++;
                        uk++;
                    }
                }
                String s = new String(l);
                s = s.Trim().Replace("\0", "");
                if ("int".Equals(s))
                {
                    return type_int;
                }
                else if ("for".Equals(s))
                {
                    return type_for;
                }
                else if ("while".Equals(s))
                {
                    return type_while;
                }
                else if ("do".Equals(s))
                {
                    return type_do;
                }
                else
                {
                    return type_id;
                }
            }//тип инт и id
            else if (prText[uk] == '=')
            {
                l[i++] = prText[uk++];

                if (prText[uk] == '=')
                {
                    l[i++] = prText[uk++];
                    return type_eq;
                }
                return type_save;
            }
            else if (prText[uk] == '!')
            {
                l[i++] = prText[uk++];

                if (prText[uk] == '=')
                {
                    l[i++] = prText[uk++];
                    return type_neq;
                }
                PrintError("Неизвестный символ");
                return type_error;
            }
            else if (prText[uk] == '<')
            {
                l[i++] = prText[uk++];
                return type_less;
            }
            else if (prText[uk] == '>')
            {
                l[i++] = prText[uk++];
                return type_more;
            }
            else if (prText[uk] == '+')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_sum;
            }
            else if (prText[uk] == '-')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_sub;
            }
            else if (prText[uk] == ',')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_com;
            }
            else if (prText[uk] == ';')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_semi;
            }
            else if (prText[uk] == '{')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_flbkt;
            }
            else if (prText[uk] == '}')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_frbkt;
            }
            else if (prText[uk] == '(')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_lbkt;
            }
            else if (prText[uk] == ')')
            {
                stb++;
                l[i++] = prText[uk++];
                return type_rbkt;
            }
            else
            {
                PrintError("Неизвестный символ");
                stb++;
                uk++;
                return type_error;
            }
        }
    }
}
