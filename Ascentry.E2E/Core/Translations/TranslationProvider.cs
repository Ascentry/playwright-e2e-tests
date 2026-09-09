using Ascentry.E2E.Configurations;
using Ascentry.E2E.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Ascentry.E2E.Core.Translations
{
    internal static class TranslationProvider
    {
        private static readonly Dictionary<string, string> _translations;
        private static readonly string _culture;

        static TranslationProvider()
        {
            _culture = GetCulture();
            _translations = Load();
        }

        private static Dictionary<string, string> Load()
        {
            var assembly = typeof(TranslationProvider).Assembly;
            var resourceName = $"Ascentry.E2E.Core.Translations.{_culture}.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (translations == null)
                    {
                        translations = new Dictionary<string, string>();
                    }

                    return translations;
                }
            }
        }

        public static string Get(TranslationEnum key)
        {
            if (_translations.TryGetValue(key.ToString(), out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Translation '{key}' not found in culture '{_culture}'.");
        }

        private static string GetCulture()
        {
            string cultureEnvVar = PlaywrightSettings.GetDescription(nameof(PlaywrightSettings.Culture));
            var culture = Environment.GetEnvironmentVariable(cultureEnvVar);

            if (string.IsNullOrEmpty(culture))
            {
                culture = TestConfiguration.Settings.Culture;
            }

            if (string.IsNullOrEmpty(culture))
            {
                culture = "fr-FR";
            }

            return culture;
        }
    }
}

