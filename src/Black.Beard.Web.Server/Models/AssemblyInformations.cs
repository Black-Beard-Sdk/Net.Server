using Bb.ComponentModel;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Bb.Models
{


    public static class AssemblyInformationExtension
    {

        /// <summary>
        /// return the assembly informations
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static AssemblyInformations GetAssemblyInformation(this Assembly assembly)
        {
            return new AssemblyInformations(assembly);
        }

    }

    public class AssemblyInformations
    {

        public AssemblyInformations(Assembly assembly)
        {

            var _dependencies = new HashSet<string>();

            assembly.CustomAttributes.ToList().ForEach(c =>
            {

                var n = c.AttributeType.Name;

                Debug.WriteLine(n);

                switch (c.AttributeType)
                {

                    case Type t1 when t1 == typeof(AssemblyDescriptionAttribute):
                        AssemblyDescription = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t1 when t1 == typeof(CompilationRelaxationsAttribute):
                        CompilationRelaxations = (Int32)c.ConstructorArguments[0].Value;
                        break;

                    case Type t2 when t2 == typeof(RuntimeCompatibilityAttribute):
                        RuntimeCompatibility = c.NamedArguments[0].TypedValue.ToString();
                        break;

                    case Type t3 when t3 == typeof(DebuggableAttribute):
                        Debuggable = (DebuggableAttribute.DebuggingModes)c.ConstructorArguments[0].Value;
                        break;

                    case Type t4 when t4 == typeof(TargetFrameworkAttribute):
                        TargetFramework = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t5 when t5 == typeof(AssemblyCompanyAttribute):
                        AssemblyCompany = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t6 when t6 == typeof(AssemblyConfigurationAttribute):
                        AssemblyConfiguration = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t7 when t7 == typeof(AssemblyFileVersionAttribute):
                        AssemblyFileVersion = new Version(c.ConstructorArguments[0].Value.ToString());
                        break;

                    case Type t8 when t8 == typeof(AssemblyInformationalVersionAttribute):
                        AssemblyInformationalVersion = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t9 when t9 == typeof(AssemblyProductAttribute):
                        AssemblyProduct = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t10 when t10 == typeof(ApplicationPartAttribute):
                        var o10 = c.ConstructorArguments[0].Value.ToString();
                        _dependencies.Add(o10);
                        break;

                    case Type t11 when t11 == typeof(AssemblyTitleAttribute):
                        AssemblyTitle = c.ConstructorArguments[0].Value.ToString();
                        break;

                    case Type t11 when t11 == typeof(ExtensionAttribute):
                    case Type t12 when t12 == typeof(UserSecretsIdAttribute):
                        break;

                    default:
                        break;

                }

            });

            this.Dependencies = _dependencies.ToArray();

        }

        public Int32 CompilationRelaxations { get; private set; }
        public string RuntimeCompatibility { get; private set; }
        public DebuggableAttribute.DebuggingModes Debuggable { get; private set; }
        public string? TargetFramework { get; private set; }
        public string? AssemblyCompany { get; private set; }
        public string? AssemblyConfiguration { get; private set; }
        public Version AssemblyFileVersion { get; private set; }
        public string? AssemblyInformationalVersion { get; private set; }
        public string? AssemblyProduct { get; private set; }
        public string? AssemblyTitle { get; private set; }
        public string[] Dependencies { get; }
        public string AssemblyDescription { get; private set; }
    }


}
