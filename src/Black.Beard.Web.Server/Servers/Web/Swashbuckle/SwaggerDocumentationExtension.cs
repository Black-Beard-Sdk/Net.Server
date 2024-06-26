﻿using Bb.Extensions;
using Bb.Servers.Web.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Bb.Servers.Web.Swashbuckle
{

    public static class SwaggerDocumentationExtension
    {

        /// <summary>
        /// Adds the documentation in the open api document from assemblies comments
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="patternGlobing">The pattern globing.</param>
        public static void AddDocumentation(this SwaggerGenOptions self, Action<OpenApiInfoGenerator<OpenApiInfo>> builder, string patternGlobing = "*.xml")
        {

            Action<OpenApiInfoGenerator<OpenApiInfo>> defaultBuilder = GetDefaultBuilder();

            var info = defaultBuilder.Generate(builder);
            self.SwaggerDoc("v1", info);

            var doc = LoadXmlFiles(patternGlobing);

            if (doc != null)
                self.IncludeXmlComments(() => doc);

        }

        /// <summary>
        /// Loads and concat all XML documentation files.
        /// </summary>
        /// <param name="patternGlobing">The pattern globing.</param>
        /// <returns></returns>
        internal static XPathDocument? LoadXmlFiles(string patternGlobing = "*.xml")
        {

            XElement? xml = null;

            var directory = new FileInfo(Assembly.GetEntryAssembly().Location).Directory;
            //var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            directory.Refresh();


            // Build one large xml with all comments files
            var files = directory.GetFiles(patternGlobing).ToList();
            if (files.Count == 0)
                Console.WriteLine($"no documentation file found in the folder {directory.FullName}");

            // Build one large xml with all comments files
            foreach (var file in files)
            {

                try
                {

                    Console.WriteLine($"try to load documentation {file.FullName}");
                    XElement dependentXml = XElement.Load(file.FullName);

                    if (xml == null)
                        xml = dependentXml;

                    else
                        foreach (var ele in dependentXml.Descendants())
                            xml.Add(ele);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to parse '{file}'. maybe the file don't contains documentation. ({e.ToString()})");
                }
            }

            if (xml != null)
                return new XPathDocument(xml.CreateReader());

            return null;

        }

        #region default values

        private static Action<OpenApiInfoGenerator<OpenApiInfo>> GetDefaultBuilder()
        {

            var infos = Assembly.GetEntryAssembly().GetAssemblyInformation();

            var version = infos.AssemblyFileVersion.ToString();
            if (!string.IsNullOrEmpty(infos.AssemblyTitle) && infos.AssemblyInformationalVersion != null)
                version = $"{infos.AssemblyTitle} {version.ToString()}";

            Action<OpenApiInfoGenerator<OpenApiInfo>> builder1 = b =>
            {

                if (!string.IsNullOrEmpty(infos.AssemblyTitle))
                    b.Add(c => c.Title = infos.AssemblyTitle);

                if (!string.IsNullOrEmpty(version))
                    b.Add(c => c.Version = version);

                if (!string.IsNullOrEmpty(infos.AssemblyDescription))
                    b.Add(c => c.Description = infos.AssemblyDescription);

            };

            return builder1;

        }

        private static string? GetDescription(List<CustomAttributeData> c)
        {
            return GetValue(c, typeof(AssemblyDescriptionAttribute));
        }


        private static string? GetValue(List<CustomAttributeData> c, params Type[] types)
        {

            foreach (var type in types)
            {
                var o = c.Where(d => type == d.AttributeType).Select(e => e.ConstructorArguments.First().Value?.ToString()).FirstOrDefault();
                if (o != null)
                    return o;
            }

            return null;

        }

        #endregion default values

    }


}


