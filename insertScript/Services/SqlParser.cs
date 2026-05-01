using System.Text;
using System.Text.RegularExpressions;

namespace DbSyncTool.Services
{
    /// <summary>
    /// Splits raw SQL text into individual executable statements,
    /// respecting DELIMITER changes, stored procedures, triggers, functions, and events.
    /// </summary>
    public static class SqlParser
    {
        private static readonly HashSet<string> _ddlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "CREATE", "ALTER", "DROP", "TRUNCATE", "RENAME"
        };

        private static readonly HashSet<string> _dmlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "INSERT", "UPDATE", "DELETE", "REPLACE", "MERGE"
        };

        private static readonly HashSet<string> _dqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "SHOW", "DESCRIBE", "EXPLAIN", "CALL"
        };

        private static readonly HashSet<string> _txnKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "START", "BEGIN", "COMMIT", "ROLLBACK", "SAVEPOINT"
        };

        public static List<ParsedStatement> Parse(string sql)
        {
            var results = new List<ParsedStatement>();
            string currentDelimiter = ";";
            var lines = sql.Split('\n');
            var buffer = new StringBuilder();
            int lineNumber = 0;
            int statementStartLine = 1;

            foreach (var rawLine in lines)
            {
                lineNumber++;
                string line = rawLine.TrimEnd();

                // Handle DELIMITER command
                var delimMatch = Regex.Match(line, @"^\s*DELIMITER\s+(\S+)\s*$", RegexOptions.IgnoreCase);
                if (delimMatch.Success)
                {
                    // Flush any pending buffer
                    FlushBuffer(buffer, results, statementStartLine);
                    currentDelimiter = delimMatch.Groups[1].Value;
                    statementStartLine = lineNumber + 1;
                    continue;
                }

                buffer.AppendLine(rawLine);

                string bufferStr = buffer.ToString();
                int delimPos = bufferStr.LastIndexOf(currentDelimiter, StringComparison.Ordinal);
                if (delimPos >= 0)
                {
                    // Check if delimiter is inside a string or comment
                    string candidate = bufferStr.Substring(0, delimPos + currentDelimiter.Length);
                    if (!IsInsideStringOrComment(candidate, delimPos))
                    {
                        string statementText = candidate.Substring(0, delimPos).Trim();
                        if (!string.IsNullOrWhiteSpace(statementText))
                        {
                            results.Add(CreateStatement(statementText, statementStartLine));
                        }
                        // Remainder after delimiter
                        string remainder = bufferStr.Substring(delimPos + currentDelimiter.Length);
                        buffer.Clear();
                        buffer.Append(remainder);
                        statementStartLine = lineNumber + 1;
                    }
                }
            }

            // Flush anything remaining
            FlushBuffer(buffer, results, statementStartLine);
            return results;
        }

        private static void FlushBuffer(StringBuilder buffer, List<ParsedStatement> results, int lineNumber)
        {
            string remaining = buffer.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(remaining))
            {
                results.Add(CreateStatement(remaining, lineNumber));
            }
            buffer.Clear();
        }

        private static ParsedStatement CreateStatement(string text, int lineNumber)
        {
            string cleaned = StripLeadingComments(text).TrimStart();
            string firstWord = GetFirstWord(cleaned);
            string secondWord = GetSecondWord(cleaned);

            string type = "OTHER";
            if (_ddlKeywords.Contains(firstWord))
                type = $"DDL:{firstWord.ToUpper()} {secondWord.ToUpper()}".Trim();
            else if (_dmlKeywords.Contains(firstWord))
                type = $"DML:{firstWord.ToUpper()}";
            else if (_dqlKeywords.Contains(firstWord))
                type = $"DQL:{firstWord.ToUpper()}";
            else if (_txnKeywords.Contains(firstWord))
                type = $"TXN:{firstWord.ToUpper()}";
            else if (firstWord.Equals("USE", StringComparison.OrdinalIgnoreCase))
                type = "USE";
            else if (firstWord.Equals("SET", StringComparison.OrdinalIgnoreCase))
                type = "SET";
            else if (firstWord.Equals("GRANT", StringComparison.OrdinalIgnoreCase) ||
                     firstWord.Equals("REVOKE", StringComparison.OrdinalIgnoreCase))
                type = $"DCL:{firstWord.ToUpper()}";

            return new ParsedStatement
            {
                Text = text,
                Type = type,
                LineNumber = lineNumber,
                Preview = cleaned.Length > 80 ? cleaned[..80] + "..." : cleaned
            };
        }

        private static bool IsInsideStringOrComment(string sql, int position)
        {
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            bool inLineComment = false;
            bool inBlockComment = false;

            for (int i = 0; i < position && i < sql.Length; i++)
            {
                char c = sql[i];
                char next = i + 1 < sql.Length ? sql[i + 1] : '\0';

                if (inLineComment)
                {
                    if (c == '\n') inLineComment = false;
                    continue;
                }
                if (inBlockComment)
                {
                    if (c == '*' && next == '/') { inBlockComment = false; i++; }
                    continue;
                }
                if (!inSingleQuote && !inDoubleQuote)
                {
                    if (c == '-' && next == '-') { inLineComment = true; i++; continue; }
                    if (c == '/' && next == '*') { inBlockComment = true; i++; continue; }
                    if (c == '#') { inLineComment = true; continue; }
                }
                if (c == '\'' && !inDoubleQuote) inSingleQuote = !inSingleQuote;
                else if (c == '"' && !inSingleQuote) inDoubleQuote = !inDoubleQuote;
                else if (c == '\\') i++; // skip escaped char
            }

            return inSingleQuote || inDoubleQuote || inBlockComment;
        }

        private static string StripLeadingComments(string sql)
        {
            // Remove block and line comments at the start
            sql = Regex.Replace(sql, @"^(/\*.*?\*/\s*)+", "", RegexOptions.Singleline);
            sql = Regex.Replace(sql, @"^(--[^\n]*\n\s*)+", "", RegexOptions.Multiline);
            return sql.TrimStart();
        }

        private static string GetFirstWord(string text)
        {
            var m = Regex.Match(text, @"^\s*(\w+)");
            return m.Success ? m.Groups[1].Value : string.Empty;
        }

        private static string GetSecondWord(string text)
        {
            var m = Regex.Match(text, @"^\s*\w+\s+(\w+)");
            return m.Success ? m.Groups[1].Value : string.Empty;
        }
    }

    public class ParsedStatement
    {
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
        public int LineNumber { get; set; }
    }
}
