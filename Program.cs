using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;

namespace methods3
{
    class Program
    {
        public static Random rand = new Random();
        public static BigInteger p, A, NR, r, l;
        public static int w = 0;
        public static string m;
        public static bool flag, flag2 = true;
        public static List <BigInteger> NR_; 
        public static List <BigInteger> Q = new List<BigInteger>();
        public static List <BigInteger> P;
        private static BigInteger NumMessage(string m)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < m.Length; i++)
                str.Append((int)m[i]);
            return BigInteger.Parse(str.ToString());
        }
        public static void DeleteFiles(bool flag = false)
        {
            foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (flag == false) 
                    continue;
                if (Path.GetExtension(item) == ".txt")
                    File.Delete(item);
            }
        }
        public static void ReadOptions()
        {
            using (var sr = new StreamReader(File.Open("ElipticCurve.txt", FileMode.Open), Encoding.Default))
            {
                p = BigInteger.Parse(sr.ReadLine());
                A = BigInteger.Parse(sr.ReadLine());
                r = BigInteger.Parse(sr.ReadLine());
                Q.Add(BigInteger.Parse(sr.ReadLine()));
                Q.Add(BigInteger.Parse(sr.ReadLine()));
            }
        }
        private static void Start()
        {
            if (!flag2)
                return;
            Console.WriteLine("Введите: ");
            Console.WriteLine("\t1 - Если хотите сгенерировать эллиптическую кривую");
            Console.WriteLine("\t2 - Если хотите сгенерировать значение l");
            Console.WriteLine("\t3 - Если хотите создать электронную монету");
            Console.WriteLine("\t4 - Если хотите погасить электронную монету");
            Console.WriteLine("\t0 - Выход");
        }
        private static void GenerateL()
        {
            Console.Write("Введите значение l: ");
            l = BigInteger.Parse(Console.ReadLine());
            try
            {
                using (var write = new StreamWriter(File.Open("l.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(l);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static void Protocol()
        {
            Q.Clear();
            ReadOptions();
            try
            {
                l = int.Parse(File.ReadAllText("l.txt"));
            }
            catch
            {
                Console.WriteLine("Ошибка! Не задано значение l");
                Console.ReadLine();
                flag = false;
                return;
            }
            P = QuickSumPoint(Q, l, A, p);
            try
            {
                using (var write = new StreamWriter(File.Open("P.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(P[0]);
                    write.WriteLine(P[1]);
                }
            }
            catch (FileNotFoundException)
            {
                 throw new FileNotFoundException();
            }
            try
            {
                Q.Clear();
                ReadOptions();
                int step = 0;
                while (true)
                {
                    Console.WriteLine("Введите номер шага, который хотите исполнить: ");
                    Console.WriteLine("\t1 - Если хотите сгенерировать случайный показатель k'");
                    Console.WriteLine("\t2 - Если хотите сгенерировать случайный показатель альфа");
                    Console.WriteLine("\t3 - Если хотите вычислить точку R");
                    Console.WriteLine("\t4 - Если хотите вычислить коэффициент бета");
                    Console.WriteLine("\t5 - Если хотите наложить на сообщение маску");
                    Console.WriteLine("\t6 - Если хотите вычислить подпись");
                    Console.WriteLine("\t7 - Если хотите проверить подпись");
                    Console.WriteLine("\t8 - Если хотите получить монету");
                    Console.WriteLine("\t0 - Выход");
                    int mode;
                    flag = true;
                    try
                    {
                        mode = int.Parse(Console.ReadLine());
                        m = File.ReadAllText("m.txt");
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Ошибка! Файл m.txt не создан");
                        Console.ReadLine();
                        return;
                    }
                    try
                    {
                        switch (mode)
                        {
                            case 1:
                                {
                                    Step1();
                                    step = 1;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 1");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 1 && step != 2)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 1");
                                        flag = false;
                                        break;
                                    }
                                    Step2();
                                    step = 2;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 2");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 2 && step != 3)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 2!");
                                        flag = false;
                                        break;
                                    }
                                    while (!Step3())
                                    {
                                        Step2();
                                        Step3();
                                    }
                                    step = 3;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 3");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 3 && step != 4)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 3!");
                                        flag = false;
                                        break;
                                    }
                                    Step4();
                                    step = 4;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 4");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 4 && step != 5)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 4!");
                                        flag = false;
                                        break;
                                    }
                                    Step5();
                                    step = 5;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 6:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 5");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 5 && step != 6)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 5!");
                                        flag = false;
                                        break;
                                    }
                                    Step6();
                                    step = 6;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 7:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 6");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 6 && step != 7)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните шаг 6!");
                                        flag = false;
                                        break;
                                    }
                                    if (Step7()) 
                                        Console.WriteLine("Подпись принята");
                                    else 
                                    {
                                        Console.WriteLine("Подпись не принята");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    step = 7;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 8:
                                {
                                    foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
                                    {
                                        if (Path.GetFileNameWithoutExtension(item) == "m" || Path.GetFileNameWithoutExtension(item) == "R" || Path.GetFileNameWithoutExtension(item) == "s" || Path.GetFileNameWithoutExtension(item) == "l" || Path.GetFileNameWithoutExtension(item) == "ElipticCurve" || Path.GetFileNameWithoutExtension(item) == "P")
                                            continue;
                                        if (Path.GetExtension(item) == ".txt")
                                            File.Delete(item);
                                    }
                                    step = 8;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    Console.WriteLine("Монета получена");
                                    break;
                                }
                            case 0:
                                {
                                    return;
                                }
                            default:
                            {
                                Console.WriteLine("Введена некорректная команда");
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            Console.WriteLine("Выполнен шаг " + mode);
                            Console.ReadLine();
                        }
                    }
                    catch (FileNotFoundException er)
                    {
                        Console.WriteLine(er.Message);
                        DeleteFiles();
                        return;
                    }
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void Protocol2()
        {   
            try
            {
                int step = 8;
                while (true)
                {
                    Console.WriteLine("Введите номер шага, который хотите исполнить: ");
                    Console.WriteLine("\t1 - Если хотите проверить, что m не равно 0");
                    Console.WriteLine("\t2 - Если хотите проверить, что f(R) не равно 0");
                    Console.WriteLine("\t3 - Если хотите проверить подлинность подписи");
                    Console.WriteLine("\t0 - Выход");
                    int mode;
                    try
                    {
                        mode = int.Parse(Console.ReadLine());
                        m = File.ReadAllText("m.txt");
                        var R = new List <BigInteger>();
                        using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                        {
                            R.Add(BigInteger.Parse(sr.ReadLine()));
                            R.Add(BigInteger.Parse(sr.ReadLine()));
                        }
                        var s = BigInteger.Parse(File.ReadAllText("s.txt"));
                    }
                    catch (FormatException) 
                    { 
                        continue; 
                    }
                    catch (FileNotFoundException)
                    {
                        Console.WriteLine("Ошибка! Сгенерируйте монету");
                        Console.ReadLine();
                        return;
                    }
                    try
                    {
                        switch (mode)
                        {
                            case 1:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 8 && step != 9)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг!");
                                        flag = false;
                                        break;
                                    }
                                    if (!Step8()) 
                                    {
                                        Console.WriteLine("Подпись не действительна");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    step = 9;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
 
                            case 2:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 9 && step != 10)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг!");
                                        flag = false;
                                        break;
                                    }
                                    if (!Step9()) 
                                    {
                                        Console.WriteLine("Подпись не действительна");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    step = 10;
                                    using (var write = new StreamWriter(File.Open("Step.txt", FileMode.OpenOrCreate)))
                                    {
                                        write.WriteLine(step);
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    try
                                    {
                                        step = int.Parse(File.ReadAllText("Step.txt"));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг");
                                        flag = false;
                                        break;
                                    }
                                    if (step != 10)
                                    {
                                        Console.WriteLine("Ошибка! Сначала выполните предыдущий шаг!");
                                        flag = false;
                                        break;
                                    }
                                    if (Step10()) 
                                    { 
                                        Console.WriteLine("Подпись действительна");
                                        foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
                                        {
                                            if (Path.GetFileName(item) == "R.txt" || Path.GetFileName(item) == "s.txt" || Path.GetFileName(item) == "Step.txt")
                                                     File.Delete(item);
                                        }
                                        Console.WriteLine("Монета погашена");
                                    }
                                    else 
                                    {
                                        Console.WriteLine("Подпись не действительна");
                                        File.Delete("l.txt");
                                        flag2 = false;
                                        return;
                                    }
                                    return;
                                }
                            case 0:
                                {
                                  return;
                                }
                            default:
                                break;
                        }
                        if (flag)
                        {
                            Console.WriteLine("Выполнен шаг " + mode);
                            Console.ReadLine();
                        }
                    }
                    catch (FileNotFoundException er)
                    {
                        Console.WriteLine(er.Message);
                        return;
                    }
              }
            }
            catch (FormatException) 
            { 
                throw; 
            }
            catch (FileNotFoundException) 
            { 
                throw; 
            }
            catch (Exception) 
            { 
                throw; 
            }
        }
        public static void Step1()
        {
            List <BigInteger> _R;
            while (true)
            {
                BigInteger _k;
                do
                {
                    _k = BigInteger.ModPow(new Random().Next(99, 9999), new Random().Next(99, 9999), r);
                } while (_k == 0 || _k == r);

                _R = QuickSumPoint(Q, _k, A, p);
                if (_R[0] != 0)
                {
                    try
                    {
                        using (var write = new StreamWriter(File.Open("_k.txt", FileMode.OpenOrCreate)))
                        {
                            write.WriteLine(_k);
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        throw new FileNotFoundException();
                    }
                    break;
                }
            }
            try
            {
                using (var write = new StreamWriter(File.Open("_R.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(_R[0]);
                    write.WriteLine(_R[1]);
                }
            }
            catch (FileNotFoundException)
            {
                 throw new FileNotFoundException();
            }
        }
        public static bool Step2()
        {
            BigInteger alpha;
            do
            { 
                alpha = BigInteger.ModPow(new Random().Next(99, 9999), new Random().Next(99, 9999), r);
            } while (alpha == 0 || alpha == r);
            try
            {
                using (var write = new StreamWriter(File.Open("alpha.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(alpha);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
            return true;
        }
        public static bool Step3()
        {
            try
            {
                var alpha = BigInteger.Parse(File.ReadAllText("alpha.txt"));
                var _R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("_R.txt", FileMode.Open), Encoding.Default))
                {
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                List <BigInteger> R;
                R = QuickSumPoint(_R, alpha, A, p);
                if (R[0] == 0)
                    return false;
                using (var write = new StreamWriter(File.Open("R.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(R[0]);
                    write.WriteLine(R[1]);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static bool Step4()
        {
            try
            {
                var _R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("_R.txt", FileMode.Open), Encoding.Default))
                {
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                {
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var Bb = R[0] * ReverseElement(_R[0], r) % r;
                using (var write = new StreamWriter(File.Open("B.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(Bb);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static bool Step5()
        {
            try
            {
                var Bb = BigInteger.Parse(File.ReadAllText("B.txt"));
                var alpha = BigInteger.Parse(File.ReadAllText("alpha.txt"));
                var _m = ReverseElement(alpha, r) * Bb * NumMessage(m) % r;
                using (var write = new StreamWriter(File.Open("_m.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(_m);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static bool Step6()
        {
            try
            {
                var l = BigInteger.Parse(File.ReadAllText("l.txt"));
                var _m = BigInteger.Parse(File.ReadAllText("_m.txt"));
                var _k = BigInteger.Parse(File.ReadAllText("_k.txt"));
                var _R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("_R.txt", FileMode.Open), Encoding.Default))
                {
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var _s = _k + l * _m * _R[0] % r;
                using (var write = new StreamWriter(File.Open("_s.txt", FileMode.OpenOrCreate)))
                {
                    write.WriteLine(_s);
                }
                return true;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
        }
        public static bool Step7()
        {
            try
            {
                var _m = BigInteger.Parse(File.ReadAllText("_m.txt"));
                var _s = BigInteger.Parse(File.ReadAllText("_s.txt"));
                var alpha = BigInteger.Parse(File.ReadAllText("alpha.txt"));
                var _R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("_R.txt", FileMode.Open), Encoding.Default))
                {
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                    _R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var P = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("P.txt", FileMode.Open), Encoding.Default))
                {
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var mult1 = QuickSumPoint(Q, _s, A, p);
                var mult2 = QuickSumPoint(P, (_R[0] * _m), A, p);
                var sum1 = SumPoints(_R, mult2, A, p);
                if (mult1[0] == sum1[0] && mult1[1] == sum1[1])
                {
                    var s = alpha * _s % r;
                    using (var write = new StreamWriter(File.Open("s.txt", FileMode.OpenOrCreate)))
                    {
                        write.WriteLine(s);
                    }
                    return true;
                }
                else 
                    return false;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }            
        }
        public static bool Step8()
        {
            var M = NumMessage(m);
            if (M == 0) 
                return false;
            return true;
        }
        public static bool Step9()
        {
            try
            {
                var R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                {
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                if (R[0] == 0) 
                    return false;
                return true;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }   
        }
        public static bool Step10()
        {   
            Q.Clear();
            ReadOptions();
            try
            {
                var R = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("R.txt", FileMode.Open), Encoding.Default))
                {
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                    R.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var P = new List <BigInteger>();
                using (var sr = new StreamReader(File.Open("P.txt", FileMode.Open), Encoding.Default))
                {
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                    P.Add(BigInteger.Parse(sr.ReadLine()));
                }
                var s = BigInteger.Parse(File.ReadAllText("s.txt"));
                var M = NumMessage(m);
                var mult1 = QuickSumPoint(Q, s, A, p);
                var mult2 = QuickSumPoint(P, (R[0] * M), A, p);
                var sum1 = SumPoints(R, mult2, A, p);
                return (mult1[0] == sum1[0] && mult1[1] == sum1[1]);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }   
        }
        public static void GenerateEC()
        {
            int m = 5;
            BigInteger D = 1;
            bool t = true;
            Console.Write("Введите длину характеристики поля l = ");
            int l1 = int.Parse(Console.ReadLine());
            if (l1 <= 4)
            {
                Console.WriteLine("Длина характеристики поля <= 4. Введите корректную длину!");
                return;
            }

            w = m;
            while (true)
            {
         step1: while (true)
                {
                    t = true;
                    List<BigInteger> decomposition;
                    FindPrimeNumberOfLength(l1);
                    decomposition = Decomposition_P(D, p);

                    NR_ = NR_function(decomposition[0], decomposition[1], p);

                    if (!NR_.Any()) 
                        goto step1;

                    if (p != NR_[1])
                    {
                        for (int i = 1; i <= m; i++)
                        {
                            if (BigInteger.ModPow(p, i, NR_[1]) == 0)
                            {
                                w = w + 1;
                                if (w > 10)
                                {
                                    Console.WriteLine("Нет p, удовлятворяющего всем условиям");
                                    return;
                                }
                                goto step1;
                            }
                        }
                        break;
                    }
                    goto step1;
                }
                int j = 1;
                BigInteger k = 2, counter = 0;
                List <BigInteger> x0y0 = new List <BigInteger>();

         step5: while (true)
                {
                    if (NR_[0] == NR_[1] * 2)
                        CheckA(ref t, ref k, 2);
                    
                    else
                        CheckA(ref t, ref k, 4);
                    
                    if (!t) 
                        goto step6;
                    x0y0 = new List<BigInteger>();
                    for (int i = j; ; i++, j++)
                    {
                        BigInteger z = (A * i + i * i * i) % p;
                        if (Legendre(z, p) == 1)
                        {
                            if (SqrtMod(z, p) == 0) 
                                continue;
                            else
                            {
                                x0y0.Add(i);
                                x0y0.Add(SqrtMod(z, p));
                            }
                            i++;
                            j++;
                            break;
                        }
                    }

                    BigInteger cnt = NR_[0];
                    List <BigInteger> Nx0y0 = QuickSumPoint(x0y0, cnt, A, p);

                    if (Nx0y0.Any())
                    {
                        if (counter++ < 100)
                            goto step5;
                        else
                        {
                            goto step1;
                        }
                    }
                    break;
                }

            step6:
                if (!t)
                    continue;
                NR = NR_[0] / NR_[1];

                List <BigInteger> Q = QuickSumPoint(x0y0, NR, A, p);

                if (!Q.Any() || Q[1] == 0)
                    continue;
                
                Console.WriteLine("p = " + p);
                Console.WriteLine("r = " + NR_[1]);
                Console.WriteLine("A = " + A);
                Console.WriteLine("Q = (" + Q[0] + ", " + Q[1] + ")");
                WriteToFiles(Q);
                break;
            }
        }
        static BigInteger Pow(BigInteger a, BigInteger b)
        {
            BigInteger result = 1;
            for (BigInteger i = 0; i < b; i++)
                result *= a;
        
            return result;
        }
        static bool IsPrime(BigInteger p)
        {
            BigInteger rounds = 30, t = p - 1;
            if (p == 2 || p == 3)
                return true;

            if (p < 2 || p % 2 == 0)
                return false;

            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            for (int i = 0; i < rounds; i++)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] _a = new byte[p.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= p - 2);

                BigInteger x = BigInteger.ModPow(a, t, p);
                if (x == 1 || x == p - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, p);

                    if (x == 1)
                        return false;

                    if (x == p - 1)
                        break;
                }

                if (x != p - 1)
                    return false;
            }
            return true;
        }
        static BigInteger GenerateNumberOfLength(int l)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] _result = new byte[BigInteger.Pow(2, l).ToByteArray().LongLength];
            BigInteger result;
            do
            {
                rng.GetBytes(_result);
                result = new BigInteger(_result);
            } while (result <= BigInteger.Pow(2, (l - 1)) || result >= BigInteger.Pow(2, l));
            
            return result;
        }
        static BigInteger GenerateSimpleNumberOfLength(int l)
        {   
            BigInteger result;
            result = GenerateNumberOfLength(l);
            while (!IsPrime(result))
            {   
                result = GenerateNumberOfLength(l);
            }
            return result;
        }
        private static void FindPrimeNumberOfLength(int l)
        {
            do
            {
                p = GenerateSimpleNumberOfLength(l);
            } while ((p % 4) != 1);
        }
        private static void WriteToFiles(List <BigInteger> Q)
        {
            using (StreamWriter f = new StreamWriter(File.Open("ElipticCurve.txt", FileMode.Create), Encoding.Default))
            {
                f.WriteLine(p);
                f.WriteLine(A);
                f.WriteLine(NR_[1]);
                f.WriteLine(Q[0]);
                f.WriteLine(Q[1]);
            }
            return;
        }
        private static Boolean CheckSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root, upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
        public static BigInteger Sqrt_N(BigInteger N)
        {
            if (N == 0) 
                return 0;

            if (N > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(N, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!CheckSqrt(N, root))
                {
                    root += N / root;
                    root /= 2;
                }
                return root;
            }
            throw new ArithmeticException("NaN");
        }
        static BigInteger BinaryEuclide(BigInteger a, BigInteger b)
        {
            BigInteger g = 1;
            while (a % 2 == 0 && b % 2 == 0)
            {
                a = a / 2;
                b = b / 2;
                g = 2 * g;
            }
            BigInteger u = a, v = b;
            while (u != 0)
            {
                while (u % 2 == 0) u = u / 2;
                while (v % 2 == 0) v = v / 2;
                if (u >= v) u = u - v;
                else v = v - u;
            }
            return g * v;
        }
        static Tuple<BigInteger,BigInteger> ExtendedEuclide(BigInteger a, BigInteger b)
        {
            BigInteger r0 = a, r1 = b, x0 = 1, x1 = 0, y0 = 0, y1 = 1, x, y, d;
            while (true)
            {
                BigInteger q = r0 / r1, r = r0 % r1;
                if (r == 0) 
                    break;
                else
                {
                    r0 = r1;
                    r1 = r;
                    x = x0 - q * x1;
                    x0 = x1;
                    x1 = x;
                    y = y0 - q * y1;
                    y0 = y1;
                    y1 = y;
                }
            }
            d = r1;
            x = x1;
            y = y1;
            return new Tuple<BigInteger,BigInteger>(x, y);
        }
        public static int Legendre(BigInteger a, BigInteger p)
        {
            if (p < 2) 
                Console.WriteLine("P должно быть больше 2");
            if (a == 0)
                return 0;
            
            if (a == 1)
                return 1;
            
            int result;
            if (a < 0)
            {
                result = Legendre(-a, p);
                BigInteger deg = (p - 1) / 2;
                if (deg % 2 != 0) result = -result;
            }
            else
            {
                if (a % 2 == 0)
                {
                    result = Legendre(a / 2, p);
                    BigInteger deg = (p * p - 1) / 8;
                    if (deg % 2 != 0) result = -result;
                }
                else
                {
                    result = Legendre(p % a, a);
                    BigInteger deg = (a - 1) * ((p - 1) / (4));
                    if (deg % 2 != 0) result = -result;
                }
            }
            return result;
        }
        static BigInteger Jacobi(BigInteger a, BigInteger n)
        {
            if (BinaryEuclide(a, n) != 1)
                return 0;
            
            BigInteger r = 1;
            if (a < 0)
            {
                a = -a;
                if (n % 4 == 3)
                    r = -r;
            }
            while (a != 0)
            {
                BigInteger k = 0;
                while (a % 2 == 0)
                {
                    k++;
                    a = a / 2;
                }
                if (k % 2 != 0)
                {
                    if (n % 8 == 3 || n % 8 == 5)
                        r = -r;
                }
                if (n % 4 == 3 && a % 4 == 3)
                    r = -r;
                
                BigInteger temp = a;
                a = n % a;
                n = temp;
            }
            return r;
        }
        static List<BigInteger> ComparisonSolution(BigInteger a, BigInteger b, BigInteger m)
        {
            List<BigInteger> answer = new List<BigInteger>();
            BigInteger d = BinaryEuclide(a, m);
            if (b % d != 0)
                return answer;
            
            else
            {
                BigInteger a1 = a / d, b1 = b / d, m1 = m / d;
                Tuple<BigInteger, BigInteger> xy = ExtendedEuclide(a1, m1);
                BigInteger x0 = (b1 * xy.Item1) % m1;
                while (x0 < 0) 
                    x0 = x0 + m1;
                answer.Add(x0 % m1);
            }
            return answer;
        }
        static BigInteger ReverseElement(BigInteger a, BigInteger m)
        {
            BigInteger d = BinaryEuclide(a, m);
            if (d != 1)
                return -1;
            
            else
            {
                List<BigInteger> answer = ComparisonSolution(a, 1, m);
                return answer[0];
            }
        }
        static BigInteger SqrtMod(BigInteger a, BigInteger p)
        {
            a += p;
            BigInteger jacobi = Jacobi(a, p);
            if (jacobi == -1)
                return 0;
            
            int N = 0;
            if (jacobi == 1)
            {
                for (int i = 2; i < p; i++)
                {
                    if (Jacobi(i, p) == -1)
                    {
                        N = i;
                        break; 
                    }
                }
            }
            BigInteger h = p - 1;
            int k = 0;
            while (h % 2 == 0)
            {
                k++;
                h = h / 2;
            }
            BigInteger a1 = (int)BigInteger.ModPow(a, (h + 1) / 2, p);
            BigInteger a2 = ReverseElement(a, p);
            BigInteger N1 = BigInteger.ModPow(N, h, p);
            BigInteger N2 = 1;
            BigInteger[] j = new BigInteger[k - 1];
            for (int i = 0; i <= k - 2; i++)
            {
                BigInteger b = (a1 * N2) % p;
                BigInteger c = (a2 * b * b) % p;
                BigInteger pow = Pow(2, k - 2 - i);
                BigInteger d = BigInteger.ModPow(c, pow, p);
                if (d == 1)
                    j[i] = 0;
                
                if (d == p - 1 || d - p == -1)
                    j[i] = 1;
                
                N2 = (N2 * (BigInteger.ModPow(N1, BigInteger.Pow(2, i) * j[i], p))) % p;
            }
            BigInteger answer = (a1 * N2) % p;
            BigInteger answer1 = (-answer + p) % p;
            return answer;
        }
        public static List<BigInteger> Decomposition_P(BigInteger D, BigInteger p)
        {
            if (Legendre(-D, p) == -1) 
                return new List<BigInteger>();
            BigInteger R = SqrtMod(-D, p);
            int i = 0;
            List<BigInteger> U = new List<BigInteger>();
            List<BigInteger> M = new List<BigInteger>();
            U.Add(R);
            M.Add(p);
            do
            {
                M.Add((U[i] * U[i] + D) / M[i]);
                U.Add(BigInteger.Min(U[i] % M[i + 1], M[i + 1] - U[i] % M[i + 1]));
                i++;
            } while (M[i] != 1);
            i--;
            List<BigInteger> a = new List<BigInteger>();
            List<BigInteger> b = new List<BigInteger>();
            for (int j = 0; j <= i; j++)
            {
                a.Add(0);
                b.Add(0);
            }
            a[i] = U[i];
            b[i] = 1;
            while (i != 0)
            {
                BigInteger znamenatel = a[i] * a[i] + D * b[i] * b[i];
                if ((U[i - 1] * a[i] + D * b[i]) % znamenatel == 0)
                    a[i - 1] = (U[i - 1] * a[i] + D * b[i]) / znamenatel;
                
                else
                    a[i - 1] = (-U[i - 1] * a[i] + D * b[i]) / znamenatel;
                
                if ((-a[i] + U[i - 1] * b[i]) % znamenatel == 0)
                    b[i - 1] = (-a[i] + U[i - 1] * b[i]) / znamenatel;
                else
                    b[i - 1] = (-a[i] - U[i - 1] * b[i]) / znamenatel;
                
                i--;
            }
            List<BigInteger> answer = new List<BigInteger>();
            answer.Add(a[0]);
            answer.Add(b[0]);
            return answer;
        }
        public static bool CheckQuadraticResidue(BigInteger A, BigInteger p) => 
        BigInteger.ModPow(A, (p - 1) / 2, p) == 1;
        private static void CheckA(ref bool t, ref BigInteger k, int n)
        {
            if (NR_[0] == NR_[1] * n)
            {
                for (BigInteger i = k; ; i++)
                {
                    if (i > 1000000)
                    {
                        t = false;
                        break;
                    }
                    bool flag = CheckQuadraticResidue((i + 1) % p, NR_[0]);
                    if (n == 4 && flag || n == 2 && !flag)
                    {
                        A = i;
                        k = A + 1;
                        break;
                    }
                }
            }
        }
        private static List<BigInteger> NR_function(BigInteger a, BigInteger b, BigInteger p)
        {
            List<BigInteger> T = new List<BigInteger>();
            T.Add(b * (-2));
            T.Add(a * 2);
            T.Add(b * 2);
            T.Add(a * (-2));
            for (int i = 0; i < T.Count; i++)
            {
                T[i] += (1 + p);

                if ((T[i] % 2).Equals(0) && IsPrime((T[i] / 2)))
                    return new List <BigInteger>() {T[i],T[i] / 2};
                
                else 
                    if ((T[i] % 4).Equals(0) && IsPrime((T[i] / 4)))
                        return new List <BigInteger>() {T[i],T[i] / 4};
            }
            return new List <BigInteger>();
        }
         public static List<BigInteger> SumPoints(List<BigInteger> P1, List<BigInteger> P2, BigInteger A, BigInteger p)
        {
            List <BigInteger> answer = new List <BigInteger>();
            BigInteger x1 = P1[0],y1 = P1[1], x2 = P2[0], y2 = P2[1], alpha;
            if (x1 == x2 && y1 == y2)
            {
                BigInteger numerator = (3 * x1 * x1 + A) % p, denomerator = (2 * y1) % p;
                if (denomerator == 0) 
                    return answer;
                alpha = numerator * ReverseElement(denomerator, p) % p;
            }
            else
            {
                BigInteger numerator = (y2 - y1) % p, denomerator = (x2 - x1) % p;
                denomerator = denomerator >= 0 ? denomerator : denomerator + p;
                if (denomerator == 0) 
                    return answer;
                alpha = numerator * ReverseElement(denomerator, p) % p;
            }
            BigInteger xr = (alpha * alpha - x1 - x2) % p, yr = (-y1 + alpha * (x1 - xr)) % p;
            xr = xr >= 0 ? xr : xr + p;
            yr = yr >= 0 ? yr : yr + p;
            answer.Add(xr);
            answer.Add(yr);
            return answer;
        }
        public static string c10to2(BigInteger i)
        {
            string s = "";
            while (i > 0) { s = (i % 2).ToString() + s; i /= 2; }
            return s == "" ? "0" : s;
        }
        private static int CountBit(BigInteger P)
        {
            int count = 0;
            while (P > 0)
            {
                P >>= 1;
                count++;
            }
            return count;
        }
        private static List <BigInteger> QuickSumPoint(List<BigInteger> P, BigInteger cnt, BigInteger A, BigInteger p)
        {
            BigInteger lengthBase = CountBit(cnt);
            string c = c10to2(cnt);
            char[] b = c.ToCharArray();
            Array.Reverse(b);
            c = new string(b);
            List<List<BigInteger>> basePoints = new List<List<BigInteger>>();
            List<List<BigInteger>> result = new List<List<BigInteger>>();
            basePoints.Add(P);
            int k = 0;
            for (BigInteger i = 1; i <= cnt; i *= 2)
            {
                if (!basePoints[k].Any())
                {
                    break;
                }
                else
                {
                    basePoints.Add(SumPoints(basePoints[k], basePoints[k], A, p));
                }

                if (c[k] == '1')
                {
                    result.Add(basePoints[k]);
                }
                k++;
            }

            if (!result.Any())
                return new List <BigInteger>();
            List <BigInteger> resultPoint = result[0];

            for (int i = 1; i < result.Count; i++)
            {
                if (!resultPoint.Any())
                    resultPoint = result[i];
                else
                    resultPoint = SumPoints(resultPoint, result[i], A, p);
            }

            return resultPoint;
        }
        static void Main(string[] args)
        {
            while (true)
            {
                step11:
                Start();
                int mode;
                try
                {
                    mode = int.Parse(Console.ReadLine());
                }
                catch (FormatException)
                {
                    continue;
                }
                try
                {
                    switch (mode)
                    {
                        case 1:
                            {
                                foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
                                {
                                    if (Path.GetFileName(item) == "l.txt" || Path.GetFileName(item) == "m.txt") 
                                        continue;
                                    if (Path.GetExtension(item) == ".txt")
                                        File.Delete(item);
                                }
                                GenerateEC();
                                Console.ReadLine();
                                break;
                            }
                        case 2:
                            {
                                foreach (var item in Directory.GetFiles(Environment.CurrentDirectory))
                                {
                                    if (Path.GetFileName(item) == "ElipticCurve.txt" || Path.GetFileName(item) == "m.txt") 
                                        continue;
                                    if (Path.GetExtension(item) == ".txt")
                                        File.Delete(item);
                                }
                                GenerateL();
                                Console.ReadLine();
                                break;
                            }
                        case 3:
                            {
                                Protocol();
                                if(!flag2)
                                    return;
                                Console.ReadLine();
                                break;
                            }
                        case 4:
                            {
                                Protocol2();
                                if(!flag2)
                                    return;
                                Console.ReadLine();
                                break;
                            }
                        case 0:
                            {
                                return;
                            }
                        default:
                            break;
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Ошибка! Нет параметров эллиптической кривой!");
                    Console.ReadLine();
                    goto step11;
                }
            }
        }
    }
}