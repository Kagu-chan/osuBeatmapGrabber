using System;
using System.Collections.Generic;
using System.IO;

namespace kcUpdater.Classes
{
    /// <summary>
    /// Utilities class for configuration files (JSON format)
    /// </summary>
    internal class Configurator
    {
        private static readonly Lazy<Configurator> lazy = new Lazy<Configurator>(() => new Configurator());

        /// <summary>
        /// Returns the Instance of this class
        /// </summary>
        public static Configurator Instance { get { return lazy.Value; } }

        /// <summary>
        /// Make the constructor private!
        /// </summary>
        private Configurator() { }

        private bool ConfigExists(string configurationName)
        {
            return File.Exists(configurationName);
        }

        /// <summary>
        /// check if a configuration exists - otherwise it will saved with given default configuration
        /// </summary>
        /// <typeparam name="T">configuration type</typeparam>
        /// <param name="configurationName">file where the config lies</param>
        /// <param name="defaultConfiguration">default configuration</param>
        public void CheckOrCreateDefaultConfiguration<T>(string configurationName, T defaultConfiguration)
        {
            if (ConfigExists(configurationName)) return;
            
            WriteConfiguration(defaultConfiguration, configurationName);
        }

        /// <summary>
        /// save a given configuration
        /// </summary>
        /// <typeparam name="T">configuration type</typeparam>
        /// <param name="configuration">the configuration object</param>
        /// <param name="configurationName">file to save to</param>
        public void WriteConfiguration<T>(T configuration, string configurationName)
        {
            try
            {
                JsonObject.Instance.WriteObject(configuration, configurationName);
            } catch (Exceptions.JsonConversionFailedException)
            {
                throw new Exceptions.ConfigurationNotHandableException(Enums.ConfigurationIssueType.NotWritable);
            }
        }

        /// <summary>
        /// load a configuration
        /// </summary>
        /// <typeparam name="T">configuration type</typeparam>
        /// <param name="configurationName">file to read from</param>
        /// <returns></returns>
        public T ReadConfiguration<T>(string configurationName)
        {
            if (!ConfigExists(configurationName)) throw new Exceptions.ConfigurationNotHandableException(Enums.ConfigurationIssueType.NotExists);
            T configuration = JsonObject.Instance.ReadObject<T>(configurationName);

            if (EqualityComparer<T>.Default.Equals(configuration, default(T))) throw new Exceptions.ConfigurationNotHandableException(Enums.ConfigurationIssueType.NotReadable);
            return configuration;
        }
    }
}
