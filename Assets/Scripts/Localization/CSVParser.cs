using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StringLocalization
{
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

                if (row != null)
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

        bool PeekCharAndAdvance(char c)
        {
            if (PeekChar(c))
            {
                _i++;
                return true;
            }
            return false;
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
            bool firstChar = true;

            while (!HeadAtEOF() && !PeekChar(',') && !PeekChar('\n'))
            {
                if (PeekChar('"'))
                    column.Append(ConsumeUntilMatchingQuote(!firstChar));
                else
                    column.Append(ConsumeChar());
                
                firstChar = false;
            }

            PeekCharAndAdvance(',');

            return column.ToString();
        }

        string ConsumeUntilMatchingQuote(bool includeQuotes)
        {
            var result = new StringBuilder();

            ConsumeChar('"');

            if (includeQuotes)
                result.Append('"');

            do
            {
                char c = ConsumeChar();

                if (c == '"')
                {
                    if (includeQuotes)
                        result.Append('"');
                    
                    return result.ToString();
                }

                result.Append(c);
            }
            while (!HeadAtEOF());

            Debug.LogError($"Error parsing CSV at char {_i}. Consumed string so far:\n\"{result}\"");
            throw new System.Exception($"Error parsing CSV. Non-matching quotes.");
        }
    }
}