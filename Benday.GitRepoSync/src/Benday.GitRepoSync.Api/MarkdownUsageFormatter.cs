﻿
using Benday.CommandsFramework;

using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;

namespace Benday.GitRepoSync.Api;

public class MarkdownUsageFormatter
{
    public string Format(List<CommandInfo> usages, bool skipCommandAnchors)
    {
        StringBuilder builder = new StringBuilder();

        AppendCommandList(usages, builder, skipCommandAnchors);

        foreach (CommandInfo usage in usages)
        {
            AppendUsage(builder, usage, skipCommandAnchors);
        }

        return builder.ToString();
    }

    private void AppendCommandList(List<CommandInfo> usages, StringBuilder builder, bool skipCommandAnchors)
    {
        builder.AppendLine($"## Commands");

        builder.AppendLine("| Command Name | Description |");
        builder.AppendLine("| --- | --- |");

        foreach (CommandInfo usage in usages)
        {
            if (skipCommandAnchors)
            {
                builder.AppendLine($"| {usage.Name} | {usage.Description} |");
            }
            else
            {
                builder.AppendLine($"| [{usage.Name}](#{usage.Name}) | {usage.Description} |");
            }
        }
    }

    private void AppendUsage(StringBuilder builder, CommandInfo usage, bool skipCommandAnchors)
    {
        if (!skipCommandAnchors)
        {
            builder.AppendLine($"## <a name=\"{usage.Name}\"></a> {usage.Name}");
        }
        else
        {
            builder.AppendLine($"## {usage.Name}");
        }

        builder.AppendLine($"**{usage.Description}**");

        builder.AppendLine("### Arguments");

        builder.AppendLine("| Argument | Is Optional | Data Type | Description |");
        builder.AppendLine("| --- | --- | --- | --- |");

        foreach (IArgument arg in usage.Arguments)
        {
            builder.Append("| ");
            builder.Append(arg.Name);
            builder.Append(" | ");

            if (arg.IsRequired == true)
            {
                builder.Append("Required");
                builder.Append(" | ");
            }
            else
            {
                builder.Append("Optional");
                builder.Append(" | ");
            }

            builder.Append(arg.DataType);
            builder.Append(" | ");

            if (string.IsNullOrEmpty(arg.Description) == false)
            {
                builder.Append(arg.Description);
            }
            else
            {
                builder.Append(string.Empty);
            }

            builder.Append(" |");

            builder.AppendLine();
        }
    }
}
