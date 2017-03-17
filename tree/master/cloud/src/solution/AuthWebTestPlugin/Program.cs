using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;


namespace Matter.Legal.MatterCenter.PerfTestPlugins
{ 
    public class Program
    {

        public static void Main(string[] args)
        {
            try
            {
                var host = new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .Build();
                host.Run();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw;
            }
        }

    }
}
