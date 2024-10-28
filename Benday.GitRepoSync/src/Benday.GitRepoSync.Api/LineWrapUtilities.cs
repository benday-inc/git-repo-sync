using System;
using System.Text;

namespace Benday.GitRepoSync.Api;

public static class LineWrapUtilities
{
    public static void AppendWrappedValue(this StringBuilder builder, string valueToWrap,
            int wrappedValueMaxLength, int commandNameColumnWidth)
    {
        var firstLine = valueToWrap.Substring(0, wrappedValueMaxLength);

        var firstLineLastIndexOfSpace = firstLine.LastIndexOf(' ');

        string secondLine = string.Empty;
        string thirdLine = string.Empty;

        if (firstLineLastIndexOfSpace == -1)
        {
            // not sure how to handle a value with no spaces...
            // ...give up and write the unwrapped value
            builder.AppendLine(valueToWrap);
        }
        else
        {
            firstLine = valueToWrap.Substring(0, firstLineLastIndexOfSpace);
            secondLine = valueToWrap[firstLineLastIndexOfSpace..];

            if (secondLine.Length > wrappedValueMaxLength)
            {
                var secondLineLastIndexOfSpace = secondLine.LastIndexOf(' ');

                thirdLine = secondLine[secondLineLastIndexOfSpace..];

                secondLine = secondLine.Substring(0,
                    secondLineLastIndexOfSpace);
            }

            builder.AppendLine(firstLine.Trim());

            builder.Append(' ', commandNameColumnWidth);
            builder.Append(secondLine.Trim());

            if (string.IsNullOrWhiteSpace(thirdLine) == false)
            {
                builder.AppendLine();
                builder.Append(' ', commandNameColumnWidth);
                builder.Append(thirdLine.Trim());
            }
        }
    }

    public static string GetNameWithPadding(string name, int padToLength)
    {
        var builder = new StringBuilder();

        builder.Append(name);
        builder.Append(' ', padToLength - name.Length);

        return builder.ToString();
    }
}

