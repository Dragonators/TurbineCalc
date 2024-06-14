using System;
using System.Linq;
using Microsoft.SolverFoundation.Services;

namespace ConsoleApp1
{
    public class turbine
    {
        public const int MAX_BLADES = 28; //最大涡轮叶片数=最大转子数 x 2
        public const int MAX_ROTORS = 14; //最大转子数

        private readonly int turbineDisperserGasFlow; //单个泄压阀处理蒸汽速率
        private readonly int turbineVentGasFlow; //单个排气口处理蒸汽速率

        private int length; //涡轮机底面为正方形
        private int height; //涡轮机高度
        private int numOfRotors; //涡轮转子数
        private int numOfDispersers; //泄压阀数量
        private int numOfVent; //排气口数量
        private int volume; //储罐容积

        public turbine(int length, int height, int turbineDisperserGasFlow = 1280, int turbineVentGasFlow = 32000)
        {
            this.length = length;
            this.height = height;
            this.turbineDisperserGasFlow = turbineDisperserGasFlow;
            this.turbineVentGasFlow = turbineVentGasFlow;
            numOfDispersers = (length - 2) * (length - 2) - 1; //泄压阀数量=涡轮内部底面积-1(中央有一个复杂旋钮装置)
            numOfRotors = height - 4; //涡轮转子数=高度-4，上下表面，分压原件，电磁线圈共四层
            volume = numOfRotors * length * length; //储罐容积=蒸汽层高度(和涡轮转子数相等) x 涡轮底面积(非内部底面而是外部底面）
        }

        /// <summary>
        /// 泄压阀(分压元件) 处理蒸汽速率 = 储罐容积 x 泄压阀数量 x 单个泄压阀处理蒸汽速率;
        /// 涡轮排气口处理蒸汽速率 = 排气口数量 x 单个排气口处理蒸汽速率;
        /// 最大蒸汽流量(mB/t) = 以上两者中较低的一个
        /// </summary>
        /// <returns>返回涡轮机的最大气流。同时设定所需最小排气口数</returns>
        public int maxGasFlow()
        {
            int maxDisperserGasFlow = volume * numOfDispersers * turbineDisperserGasFlow;
            numOfVent = Convert.ToInt32(Math.Ceiling((double)maxDisperserGasFlow / turbineVentGasFlow));
            return maxDisperserGasFlow;
        }

        public static void minimumTurbineSizeForSteamFlow(int steamFlow)
        {
            // 目标值G
            int targetG = Convert.ToInt32(Math.Ceiling((double)steamFlow/1280));
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            // 定义决策变量 l 和 h
            Decision l = new Decision(Domain.IntegerRange(5, 17), "l");
            Decision h = new Decision(Domain.IntegerRange(5, 18), "h");
            model.AddDecisions(l, h);

            Term G = (l * l * l * l - 4 * l * l * l + 3 * l * l) * (h - 4);
            Term B = 2 * l * l + 4 * (h - 2) * (l - 1);
            
            // 定义目标函数 B
            model.AddGoal("MinimizeB", GoalKind.Minimize, B);

            // 添加约束 G = (l^4 - 4l^3 + 3l^2) * (h - 4)
            model.AddConstraint("ConstraintG", G >= targetG);

            // 解决模型
            Solution solution = context.Solve();

            // 获取结果
            Report report = solution.GetReport();
            Console.WriteLine("所用材料最少的最优解如下:");
            Console.WriteLine($"l(边长) = {l.GetDouble()}");
            Console.WriteLine($"h(高度) = {h.GetDouble()}");
            Console.WriteLine($"B(框架方块数量最小值) = {solution.Goals.First().ToDouble()}");

            // 打印详细报告
            // Console.WriteLine(report);
        }
    }
}