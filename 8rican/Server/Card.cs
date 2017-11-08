using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Card
    {
        public String[] strtab;
        int nb;

        public Card(int nb_)
        {
            nb = nb_;
            strtab = new String[nb];
            for (int i = 0; i < nb; i++)
            {
                strtab[i] = "";
            }
        }

        public Boolean IsAvailable(String tmp)
        {
            int count = -1;

                while (++count < nb)
                {
                    if (strtab[count].Contains(tmp))
                        return (false);
                }
                return (true);
        }

        public String distribution()
        {
            String str = "";
            Random rand = new Random();
            int nb = 0;
            int letter = 0;
            int count = 0;

            for (String tmp = ""; count < 8; tmp = "")
            {
                nb = rand.Next(8) + 7;
                letter = rand.Next(4) + 65;
                tmp = tmp + nb.ToString();
                tmp = tmp + (char)letter;
                if (this.IsAvailable(tmp) && !str.Contains(tmp))
                {
                    str = str + tmp;
                    if (count + 1 < 8)
                    {
                        str = str + ' ';
                    }

                    ++count;
                }
            }

            str = str + '\u0000';
            return str;
        }
    }
}
