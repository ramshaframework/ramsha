using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Ramsha.Common.Domain
{
    public static class SearchExtension
    {
        private static readonly RegexCache _regexCache = new();

        private static Regex BuildRegex(string pattern)
        {
            var regexPattern = Regex
                .Escape(pattern)
                .Replace("%", ".*")
                .Replace("_", ".")
                .Replace(@"\[", "[")
                .Replace(@"\^", "^");

            regexPattern = "^" + regexPattern + "$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex;
        }

        public static bool Like(this string input, string pattern)
        {
            try
            {
                var regex = _regexCache.GetOrAdd(pattern, BuildRegex);
                return regex.IsMatch(input);
            }
            catch (Exception ex)
            {
                throw new InvalidSearchPatternException(pattern, ex);
            }
        }

        private class RegexCache
        {
            private const int MAX_SIZE = 10;
            private readonly ConcurrentDictionary<string, Regex> _dictionary = new();

            public Regex GetOrAdd(string key, Func<string, Regex> valueFactory)
            {
                if (_dictionary.TryGetValue(key, out var regex))
                    return regex;

                for (int i = _dictionary.Count - MAX_SIZE; i >= 0; i--)
                {
                    var firstKey = _dictionary.Keys.FirstOrDefault();
                    if (firstKey is not null)
                    {
                        _dictionary.TryRemove(firstKey, out _);
                    }
                }

                var newRegex = valueFactory(key);
                _dictionary.TryAdd(key, newRegex);
                return newRegex;
            }
        }

#pragma warning disable IDE0051
        [ExcludeFromCodeCoverage]
        private static bool SqlLikeOption2(string str, string pattern)
        {
            var isMatch = true;
            var isWildCardOn = false;
            var isCharWildCardOn = false;
            var isCharSetOn = false;
            var isNotCharSetOn = false;
            var lastWildCard = -1;
            var patternIndex = 0;
            var set = new List<char>();
            var p = '\0';
            bool endOfPattern;

            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                endOfPattern = (patternIndex >= pattern.Length);
                if (!endOfPattern)
                {
                    p = pattern[patternIndex];

                    if (!isWildCardOn && p == '%')
                    {
                        lastWildCard = patternIndex;
                        isWildCardOn = true;
                        while (patternIndex < pattern.Length &&
                            pattern[patternIndex] == '%')
                        {
                            patternIndex++;
                        }
                        p = patternIndex >= pattern.Length ? '\0' : pattern[patternIndex];
                    }
                    else if (p == '_')
                    {
                        isCharWildCardOn = true;
                        patternIndex++;
                    }
                    else if (p == '[')
                    {
                        if (pattern[++patternIndex] == '^')
                        {
                            isNotCharSetOn = true;
                            patternIndex++;
                        }
                        else isCharSetOn = true;

                        set.Clear();
                        if (pattern[patternIndex + 1] == '-' && pattern[patternIndex + 3] == ']')
                        {
                            var start = char.ToUpper(pattern[patternIndex]);
                            patternIndex += 2;
                            var end = char.ToUpper(pattern[patternIndex]);
                            if (start <= end)
                            {
                                for (var ci = start; ci <= end; ci++)
                                {
                                    set.Add(ci);
                                }
                            }
                            patternIndex++;
                        }

                        while (patternIndex < pattern.Length &&
                            pattern[patternIndex] != ']')
                        {
                            set.Add(pattern[patternIndex]);
                            patternIndex++;
                        }
                        patternIndex++;
                    }
                }

                if (isWildCardOn)
                {
                    if (char.ToUpper(c) == char.ToUpper(p))
                    {
                        isWildCardOn = false;
                        patternIndex++;
                    }
                }
                else if (isCharWildCardOn)
                {
                    isCharWildCardOn = false;
                }
                else if (isCharSetOn || isNotCharSetOn)
                {
                    var charMatch = (set.Contains(char.ToUpper(c)));
                    if ((isNotCharSetOn && charMatch) || (isCharSetOn && !charMatch))
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    isNotCharSetOn = isCharSetOn = false;
                }
                else
                {
                    if (char.ToUpper(c) == char.ToUpper(p))
                    {
                        patternIndex++;
                    }
                    else
                    {
                        if (lastWildCard >= 0) patternIndex = lastWildCard;
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }
                }
            }
            endOfPattern = (patternIndex >= pattern.Length);

            if (isMatch && !endOfPattern)
            {
                var isOnlyWildCards = true;
                for (var i = patternIndex; i < pattern.Length; i++)
                {
                    if (pattern[i] != '%')
                    {
                        isOnlyWildCards = false;
                        break;
                    }
                }
                if (isOnlyWildCards) endOfPattern = true;
            }
            return isMatch && endOfPattern;
        }
#pragma warning restore IDE0051
    }
}