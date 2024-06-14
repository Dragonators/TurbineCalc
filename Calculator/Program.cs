using System;
using ConsoleApp1;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int steamFlow;
            bool isValidInput;
            do
            {
                Console.Write("请输入预期蒸汽流量(单位:mb): ");
                isValidInput = Int32.TryParse(Console.ReadLine(), out steamFlow);
                if (isValidInput)
                {
                    if(steamFlow <= 0) {
                        Console.WriteLine("何意呢");
                        isValidInput = false;
                    }
                    else if (steamFlow < 256000)
                    {
                        Console.WriteLine("涡轮机至少可以处理256000mb/t的蒸汽流量，请直接搭建5X5x5的最小涡轮机。");
                    }
                    else if (steamFlow > 1160069120)
                    {
						Console.WriteLine("涡轮机最多可以处理1160069120mb/t的蒸汽流量，请直接搭建17X18x17的最大涡轮机。");
					}
                    else
                    {
                        turbine.minimumTurbineSizeForSteamFlow(steamFlow);
                    }
                }
                else
                {
                    Console.WriteLine("输入无效，请输入一个整数。");
                }
            } while (!isValidInput);
			Console.WriteLine("请按任意键退出...");
			Console.ReadKey();
		}
    }
}