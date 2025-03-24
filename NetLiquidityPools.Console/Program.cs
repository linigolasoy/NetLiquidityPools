using NetLiquidityPools.Bot;
using NetLiquidityPools.Factory;
using NetLiquidityPools.Interface;
using NetLiquidityPools.Interface.Bot;

namespace NetLiquidityPools.Console
{
    public class Program
    {

        private enum eAction
        {
            None,
            Cancel,
            Close
        }

        // private const string SETUP_FILE = "d:/Data/NetLiquidityPools/NetLiquidityPoolsSetup.json";

        /// <summary>
        /// Return if user hit <F> key to end
        /// </summary>
        /// <returns></returns>
        private static eAction NeedsAction()
        {
            eAction eResult = eAction.None;
            if (System.Console.KeyAvailable)
            {
                ConsoleKeyInfo oKeyInfo = System.Console.ReadKey();
                if (oKeyInfo.KeyChar == 'F' || oKeyInfo.KeyChar == 'f') eResult = eAction.Cancel;
                if (oKeyInfo.KeyChar == 'C' || oKeyInfo.KeyChar == 'c') eResult = eAction.Close;
            }
            return eResult;

        }

        public static async Task<int> Main(string[] args)
        {
            if (args.Length <= 0) throw new Exception("Debe incluir el path del setup");
            System.Console.WriteLine($"Parametro [{args[0]}]");
            ICryptoSetup oSetup = CommonDexFactory.CreateSetup(args[0]);

            ILiquidityBot oBot = BotFactory.CreateBot(oSetup);

            oBot.Logger.Info("Start main progam");
            bool bStarted = await oBot.Start();

            if (!bStarted) return 1;

            eAction eResult = eAction.None;
            while (eResult != eAction.Cancel)
            {
                eResult = NeedsAction();
                await Task.Delay(500);
            }



            oBot.Logger.Info("End main progam");
            await Task.Delay(1000);
            await oBot.Stop();
            return 0;
        }
    }
}
