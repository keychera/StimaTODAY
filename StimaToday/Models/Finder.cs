using System;
using System.Text.RegularExpressions;

namespace StimaToday.Models
{
    public class Finder
    {
        const int numberOfChars = 256; //kode karakter dalam ASCII

        int max(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        public void getSentence(ref string searchResult, int match, string text, string pattern)
        //Prosedur yang fungsinya mendapatkan sebuah kalimat yang mengandung keyword
        //Hasil ditaruh di searchResult
        //match merupakan lokasi awal pattern pada text
        {    
            int kiri = match;
            int kanan = match + pattern.Length - 1;
            while (kiri >= 0 && !text[kiri].Equals('.') && !text[kiri].Equals('>'))
                kiri--;
            while (kanan < text.Length && !text[kanan].Equals('.') && !text[kanan].Equals('<'))
                kanan++;
            if (kiri < 0) kiri = 0;
            if (kanan >= text.Length) kanan = text.Length - 1;
            searchResult = text.Substring(kiri + 1, kanan - kiri); 
        }

        public void computeLastOccurence(string pattern, int[] b)
        //Mengkomputasi last occurence karakter-karakter pattern
        //Digunakan untuk algoritma Booyer Moore
        {
            for (int i = 0; i < numberOfChars; i++)
                b[i] = -1;
            for (int i = 0; i < pattern.Length; i++)
                b[(int)pattern[i]] = i;
        }

        public Boolean booyerMoore(string t, string p, ref string searchResult)
        {
            int match = 0;
            int temp;
            Boolean found = false;
            string text = t.ToLower();
            string pattern = p.ToLower();
            int n = text.Length;
            int m = pattern.Length;
            int[] b = new int[numberOfChars];
            computeLastOccurence(pattern, b);
            int s = 0;
            while ((s <= (n - m)) && !found)
            {
                int j = m - 1;
                while (j >= 0 && pattern[j].Equals(text[j + s])) //selama pattern cocok dengan text
                    j--;  //terus mundur
                if (j < 0)  //pattern ditemukan pada text
                {
                    found = true;
                    match = s;  //lokasi awal pattern pada text
                }
                else
                {
                    temp = (int) text[j + s];
                    if ((temp >= 0) && (temp <= 255))
                        s += max(1, j - b[temp]);  //case 1 atau case 2 dari booyer moore
                    else
                        s += 1;
                }
            }
            if (found)
                getSentence(ref searchResult, match, t, p);
            return found;
        }

        public void computeFail(string pattern, int[] b)
        //Mengkomputasi panjang substring suffix pada prefix
        //Digunakan untuk algoritma KMP
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
                if (pattern[j].Equals(text[i]))  //jika karakter j pattern cocok dengan text
                {
                    if (j == m - 1)  //pattern ditemukan pada text
                    {
                        found = true;
                        match = i - m + 1;  //lokasi awal pattern pada text
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
                getSentence(ref searchResult, match, t, p);
            return found;
        }

        public Boolean regex(string t, string p, ref string searchResult)
        {
            Boolean found = false;
            Regex rgx = new Regex(p, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(t);

            //kalau ketemu
            if (matches.Count > 0)
            {
                found = true;
            }

            Boolean pertama_ketemu = false; //buat dapet match pertama
            if (found)
            {
                int ketemu=0;
                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;
                    if (!pertama_ketemu)
                    {
                        pertama_ketemu = true;
                        ketemu = groups[0].Index;
                    }
                }

                getSentence(ref searchResult, ketemu, t, p);
            }
            return found;
        }
    }
}