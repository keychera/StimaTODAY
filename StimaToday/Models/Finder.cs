using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace StimaToday.Models
{
    public class Finder
    {
        const int numberOfChars = 256;
        private string pattern;
        private string text;

        int max(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        public void getSentence(ref string searchResult, int match, string originalText)
        {
            int kiri = match;
            int kanan = match + pattern.Length - 1;
            while (!text[kiri].Equals('.') && !text[kiri].Equals('>') && (kiri > 0))
                kiri--;
            while (!text[kanan].Equals('.') && !text[kanan].Equals('<'))
                kanan++;
            searchResult = originalText.Substring(kiri + 1, kanan - kiri);
        }

        public void computeLastOccurence(string pattern, int[] b)
        {
            for (int i = 0; i < numberOfChars; i++)
                b[i] = -1;
            for (int i = 0; i < pattern.Length; i++)
                b[(int)pattern[i]] = i;
        }

        public Boolean booyerMoore(string t, string p, ref string searchResult)
        {
            int match = 0;
            Boolean found = false;
            text = t.ToLower();
            pattern = p.ToLower();
            int n = text.Length;
            int m = pattern.Length;
            int[] b = new int[numberOfChars];
            computeLastOccurence(pattern, b);
            int s = 0;
            while ((s <= (n - m)) && !found)
            {
                int j = m - 1;
                while (j >= 0 && pattern[j] == text[j + s])
                    j--;
                if (j < 0)
                {
                    found = true;
                    match = s;
                }
                else
                    s += max(1, j - b[(int)text[j + s]]);
            }
            if (found)
                getSentence(ref searchResult, match, t);
            return found;
        }

        public void computeFail(string pattern, int[] b)
        {
            b[0] = 0;
            int m = pattern.Length;
            int j = 0;
            int i = 1;
            while (i < m)
            {
                if (pattern[i].Equals(pattern[j]))
                {
                    b[i] = j + 1;
                    i++;
                    j++;
                }
                else if (j > 0)
                    j = b[j - 1];
                else
                {
                    b[i] = 0;
                    i++;
                }
            }
        }

        public Boolean kmpMatch(string t, string p, ref string searchResult)
        {
            int match = 0;
            Boolean found = false;
            text = t.ToLower();
            pattern = p.ToLower();
            int n = text.Length;
            int m = pattern.Length;
            int[] b = new int[m];
            computeFail(pattern, b);
            int i = 0;
            int j = 0;
            while ((i < n) && (!found))
            {
                if (pattern[j].Equals(text[i]))
                {
                    if (j == m - 1)
                    {
                        found = true;
                        match = i - m + 1;
                    }
                    i++;
                    j++;
                }
                else if (j > 0)
                    j = b[j - 1];
                else
                    i++;
            }
            if (found)
                getSentence(ref searchResult, match, t);
            return found;
        }

        
    }
}