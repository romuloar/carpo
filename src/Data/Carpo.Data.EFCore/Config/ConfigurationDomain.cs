using Carpo.Core.Interface.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Carpo.Data.EFCore.Config
{
    /// <summary>
    /// Configuration domain class
    /// </summary>
    public static class ConfigurationDomain
    {
        private static string GetOriginalAssembly()
        {
            string originName = "";
            // get current assembly
            Assembly assemblyDaBiblioteca = Assembly.GetExecutingAssembly();

            // get current stack 
            StackTrace stackTrace = new StackTrace();

            var framesNoOutroAssembly = stackTrace.GetFrames()
            .Where(frame => frame.GetMethod()?.DeclaringType?.Assembly != assemblyDaBiblioteca)
            .ToList();

            if (framesNoOutroAssembly.Any())
            {
                var chamadorFrame = framesNoOutroAssembly.First();
                var chamadorMethod = chamadorFrame.GetMethod();
                var chamadorType = chamadorMethod?.DeclaringType;

                if (chamadorType != null)
                {
                    originName = chamadorType.Assembly.FullName.Split(',')[0];
                }
            }

            return originName;
        }
        /// <summary>
        /// Set configurations class on DbModelBuilder 
        /// </summary>        
        public static void SetListConfigurations(ModelBuilder modelBuilder)
        {
            var type = typeof(IConfigDomain);
         
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json"); //todo -> needs corrections
            IConfiguration configuration = builder.Build();
            string contextProjectName = configuration["CarpoConfig:ProjectContext"];
            //GetOriginalAssembly();

            var typesToMapping = Assembly.Load(contextProjectName)
                .GetTypes().Where(p => type.IsAssignableFrom(p) && p.IsClass);

            foreach (var mappingClass in typesToMapping.Select(Activator.CreateInstance))
                modelBuilder.ApplyConfiguration(mappingClass as dynamic);
                //modelBuilder.Con.AddConfiguration(.Add(mappingClass as dynamic);
        }

        public static string GetConnectionString(string nameContext, string nameFile)
        {            
            var builder = new ConfigurationBuilder().AddJsonFile(nameFile);
            IConfiguration configuration = builder.Build();
            string con = configuration["CarpoConfig:ConnectionStrings:" + nameContext];
            return con;
        }

        /// <summary>
        /// GetConnectionString
        /// </summary>        
        public static string GetConnectionString(string nameContext)
        {
            string nameFile = "appSettings.json";           

            return GetConnectionString(nameContext, nameFile);
        }
    }
}
