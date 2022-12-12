using System;
using System.Collections.Generic;
using System.Text;

namespace Synt_analyzer
{
    class Analyzer
    {
        private readonly int[] _scanResult = new int[1000];
        private readonly int[] _scanResultStr = new int[1000];
        private int curlevel = 0;
        private int _uk;
        bool live = true;

        public Analyzer(string file)
        {
            live = true;
            var scanner = new Scaner(file);
            int type;
            var i = 0;
            do
            {
                var l = new char[50];
                type = scanner.Program(l);
                var s = new string(l);
                Console.WriteLine(i + ") Тип " + type + " -> " + s.Trim());
                _scanResult[i] = type;
                _scanResultStr[i] = scanner.GetStr();
                i++;
            } while (type != Scaner.type_end);
            i = 0;
            _uk = 0;
            S();
            if (live)
                Print("Конец программы");
        }

        private void S()
        {
            do
            {
                switch (_scanResult[_uk])
                {
                    case Scaner.type_int:
                        B();
                        break;
                    case Scaner.type_id:
                        C();
                        break;
                    case Scaner.type_for:
                        W();
                        break;
                    case Scaner.type_while:
                        W();
                        break;
                    case Scaner.type_do:
                        W();
                        break;
                    default:
                        Print("Ошибка");
                        live = false;
                        break;
                }
            } while (_scanResult[_uk] != Scaner.type_end && _scanResult[_uk] != Scaner.type_frbkt && live);
        }


        private void B()
        {
            Print("Вход в описание переменных");
            do
            {
                _uk++;
                if (_scanResult[_uk] == Scaner.type_id)
                {
                    _uk++;
                    if (_scanResult[_uk] == Scaner.type_save)
                        A();
                    if (_scanResult[_uk] == Scaner.type_semi)
                        _uk++;
                    else
                    {
                        Print("Ожидалась ;");
                        live = false;
                    }
                }
                else
                {
                    Print("Ожидался идентификатор");
                    live = false;
                }
            } while (_scanResult[_uk] == Scaner.type_com);
            Print("Выход из описания переменных");
        }

        private void C()
        {
            Print("Вход в описание переменных");
            if (_scanResult[_uk] == Scaner.type_id)
            {
                _uk++;
                if (_scanResult[_uk] == Scaner.type_save)
                {
                    A();
                    if (_scanResult[_uk] == Scaner.type_semi)
                        _uk++;
                    else
                    {
                        Print("Ожидалась ; или оператор арифметически/сравнения");
                        live = false;
                    }
                }
                else
                {
                    Print("Ошибка: неверный оператор");
                    live = false;
                }
            }
            Print("Выход из описание переменных");
        }


        private void W()
        {
            switch (_scanResult[_uk])
            {
                case Scaner.type_for:
                    Print("Оператор цикла for");
                    _uk++;
                    if (_scanResult[_uk] == Scaner.type_lbkt)
                    {
                        _uk++;
                        C();

                        A1();
                        if (_scanResult[_uk] == Scaner.type_semi)
                        {
                            _uk++;
                            A3();
                            if (_scanResult[_uk] == Scaner.type_rbkt)
                            {
                                _uk++;
                                if (_scanResult[_uk] == Scaner.type_flbkt)
                                {
                                    Print("Вход в for");
                                    curlevel++;
                                    _uk++;
                                    S();
                                    if (_scanResult[_uk] == Scaner.type_frbkt)
                                    {
                                        curlevel--;
                                        Print("Выход из for");
                                        _uk++;
                                    }
                                    else
                                    {
                                        Print("Ожидалась }, не закончен блок for");
                                        live = false;
                                    }
                                }
                                else
                                {
                                    Print("Ожидалась {");
                                    live = false;
                                }
                            }
                            else
                            {
                                Print("Ожидалась )");
                                live = false;
                            }
                        }
                        else
                        {
                            Print("Ожидалось перечисление параметров");
                            live = false;
                        }

                    }
                    else
                    {
                        Print("Ожидалась (");
                        live = false;
                    }
                    break;
                case Scaner.type_while:
                    Print("Оператор цикла while");
                    _uk++;
                    if (_scanResult[_uk] == Scaner.type_lbkt)
                    {
                        _uk++;
                        A1();
                        if (_scanResult[_uk] == Scaner.type_rbkt)
                        {
                            _uk++;
                            Print("Вход в While");
                            curlevel++;
                            if (_scanResult[_uk] == Scaner.type_flbkt)
                            {
                                _uk++;
                                S();
                                if (_scanResult[_uk] == Scaner.type_frbkt)
                                {
                                    curlevel--;
                                    Print("Выход из While");
                                    _uk++;
                                }
                                else
                                {
                                    Print("Ожидалось }, не закончен блок while");
                                    live = false;
                                }
                            }
                            else
                            {
                                Print("Ожидалось {");
                                live = false;
                            }

                        }
                        else
                        {
                            Print("Ожидалось )");
                            live = false;
                        }
                    }
                    else
                    {
                        Print("Ожидалось (");
                        live = false;
                    }
                    break;
                case Scaner.type_do:
                    Print("Оператор цикла do");
                    _uk++;
                    if (_scanResult[_uk] == Scaner.type_flbkt)
                    {

                        Print("Вход в Do While");
                        curlevel++;
                        _uk++;
                        S();
                        if (_scanResult[_uk] == Scaner.type_frbkt)
                        {
                            _uk++;
                            if (_scanResult[_uk] == Scaner.type_while)
                            {
                                Print("Оператор цикла while");
                                _uk++;
                                if (_scanResult[_uk] == Scaner.type_lbkt)
                                {
                                    _uk++;
                                    A1();
                                    if (_scanResult[_uk] == Scaner.type_rbkt)
                                    {
                                        curlevel--;
                                        Print("Выход из Do While");
                                        _uk++;
                                    }
                                    else
                                    {
                                        Print("Ожидалось )");
                                        live = false;
                                    }
                                }
                                else
                                {
                                    Print("Ожидалось (");
                                    live = false;
                                }
                            }
                            else
                            {
                                Print("Ожидалось While");
                                live = false;
                            }
                        }
                        else
                        {
                            Print("Ожидалось }, не закончен блок do while");
                            live = false;
                        }
                    }
                    else
                    {
                        Print("Ожидалось {");
                        live = false;
                    }
                    break;
            }
        }
        private void A()
        {
            if (_scanResult[_uk] == Scaner.type_save)
            {
                _uk++;
                A1();
            }
            else
            {
                Print("Ожидалось =");
                live = false;
            }
        }

        private void A1()
        {
            A2();
            if (_scanResult[_uk] == Scaner.type_eq || _scanResult[_uk] == Scaner.type_neq)
            {
                _uk++;
                A2();
            }
        }
        private void A2()
        {
            A3();
            if (_scanResult[_uk] == Scaner.type_less || _scanResult[_uk] == Scaner.type_more)
            {
                _uk++;
                A3();
            }
        }
        private void A3()
        {
            A4();
            while (_scanResult[_uk] == Scaner.type_sum || _scanResult[_uk] == Scaner.type_sub)
            {
                _uk++;
                A4();
            }
        }
        private void A4()
        {
            switch (_scanResult[_uk])
            {
                case Scaner.type_const:
                    _uk++;
                    break;
                case Scaner.type_id:
                    _uk++;
                    break;
                default:
                    {
                        Print("Неизвестный операнд" + _scanResult[_uk].ToString());
                        live = false;
                    }
                    break;
            }
        }
        private void Print(string text)
        {
            string str = "";
            for (int i = 0; i < curlevel; i++)
                str += "\t";

            if (live)
                Console.WriteLine(str + "Строка " + _scanResultStr[_uk] + ": " + text);

        }
    }
}
