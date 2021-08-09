using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AngleSharp.Dom;
using MudBlazor.Utilities;
using Newtonsoft.Json;
using Verify.AngleSharp;
using VerifyTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.ModifySerialization(settings =>
            settings.AddExtraSettings(jsonSettings =>
                jsonSettings.Converters.Add(new MudColorConverter())));
        HtmlPrettyPrint.All(list =>
        {
            foreach (var element in list.QuerySelectorAll("[id]"))
            {
                var idAttribute = element.GetAttribute("id");
                if (Guid.TryParse(idAttribute, out _))
                {
                    element.RemoveAttribute("id");
                }
            }
        });
        VerifyBunit.Initialize();
    }
}

public class MudColorConverter :
    WriteOnlyJsonConverter<MudColor>
{
    public override void WriteJson(JsonWriter writer, MudColor color, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(color.ToString(MudColorOutputFormats.RGB));
    }
}
