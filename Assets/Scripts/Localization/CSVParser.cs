using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVParser
{
    List<List<string>> _csvContent = new List<List<string>>();

    int _i = 0;
    string _text;

    public List<List<string>> Content => _csvContent;

    public CSVParser(string resourcePath)
    {
        var resource = Resources.Load<TextAsset>(resourcePath);

        _text = resource.text;

        do
        {
            var row = ConsumeRow();
            _csvContent.Add(row);
        }
        while (!HeadAtEOF());
    }

    bool HeadAtEOF()
    { return _i >= _text.Length; }

    List<string> ConsumeRow()
    {
        var row = new List<string>();

        do
        {
            var column = ConsumeColumn();
            row.Add(column);
        }
        while (!HeadAtNewLine());

        if (!HeadAtEOF())
        {
            ConsumeChar('\n');
        }

        return row;
    }

    bool PeekChar(char c)
    {
        return !HeadAtEOF() && _text[_i] == c;
    }

    bool HeadAtNewLine()
    { return PeekChar('\n'); }

    char ConsumeChar(char c)
    {
        if (!PeekChar(c))
        {
            throw new System.Exception($"Error parsing CSV. '{c}' expected but found '{_text[_i]}' instead.");
        }

        return ConsumeChar();
    }

    char ConsumeChar()
    {
        return _text[_i++];
    }

    string ConsumeColumn()
    {
        var column = new StringBuilder();

        do
        {
            if (PeekChar('"'))
            {
                column.Append(ConsumeUntilMatchingQuote());
            }
            else
            {
                column.Append(ConsumeChar());
            }
        }
        while (!HeadAtEOF() && !PeekChar(',') && !PeekChar('\n'));

        if (!HeadAtEOF() && PeekChar(','))
        {
            ConsumeChar(',');
        }

        return column.ToString();
    }

    string ConsumeUntilMatchingQuote()
    {
        var result = new StringBuilder();

        ConsumeChar('"');

        do
        {
            char c = ConsumeChar();

            if (c == '"')
            {
                return result.ToString();
            }

            result.Append(c);
        }
        while (!HeadAtEOF());

        throw new System.Exception($"Error parsing CSV. Non-matching quotes.");
    }
}