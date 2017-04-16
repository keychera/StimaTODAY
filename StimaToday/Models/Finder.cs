using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace StimaToday.Models
{
    public class Finder
    {
        public Boolean kmpMatch(string t, string p, ref string searchResult)
        {
            int match = 0;
            Boolean found = false;
            string text = t.ToLower();
            string pattern = p.ToLower();
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
            {
                int kiri = match;
                int kanan = match + m - 1;
                while (!text[kiri].Equals('.') && !text[kiri].Equals('>') && (kiri > 0))
                    kiri--;
                while (!text[kanan].Equals('.') && !text[kanan].Equals('<'))
                    kanan++;
                searchResult = t.Substring(kiri + 1, kanan - kiri + 1);
            }
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
    }
}