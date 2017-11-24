using System;

namespace Widm
{
    public enum Kelintas { Extra, First, Second, Third, Fourth }

    // arba struct, arba static class
    public struct Klnt
    {
        private Kelintas _kelintas;

        public Klnt(string kelintas)
        {
            _kelintas = (Kelintas)Enum.Parse(typeof(Kelintas), kelintas);
        }

        public Klnt(int kelintas)
        {
            _kelintas = (Kelintas)kelintas;
        }

        public Klnt(Kelintas kelintas)
        {
            _kelintas = kelintas;
        }

        public Kelintas Kelintas
        {
            get
            {
                return _kelintas;
            }
        }

        public int Digit
        {
            get { return (int)_kelintas; }
        }

        public string DgsVard
        {
            get
            {
                switch (_kelintas)
                {
                    case Kelintas.Extra:
                        return "papildomi";
                    case Kelintas.First:
                        return "pirmieji";
                    case Kelintas.Second:
                        return "antrieji";
                    case Kelintas.Third:
                        return "tretieji";
                    case Kelintas.Fourth:
                        return "ketvirtieji";
                }
                return "samsing weri vyrd chepened";
            }
        }

        public string VnsVard
        {
            get
            {
                switch (_kelintas)
                {
                    case Kelintas.Extra:
                        return "papildomas";
                    case Kelintas.First:
                        return "pirmasis";
                    case Kelintas.Second:
                        return "antrasis";
                    case Kelintas.Third:
                        return "trečiasis";
                    case Kelintas.Fourth:
                        return "ketvirtasis";
                }
                return "samsing weri vyrd chepened";
            }
        }

        public string Roman
        {
            get
            {
                switch(_kelintas)
                {
                    case Kelintas.Extra:
                        return "EXTRA";
                    case Kelintas.First:
                        return "I";
                    case Kelintas.Second:
                        return "II";
                    case Kelintas.Third:
                        return "III";
                    case Kelintas.Fourth:
                        return "IV";
                }
                return "samsing weri vyrd chepened";
            }
        }
    }



    public static class Kl
    {
        public static Kelintas Create (string kelintas)
        {
            return (Kelintas)Enum.Parse(typeof(Kelintas), kelintas);
        }

        public static Kelintas Create(int kelintas)
        {
            return (Kelintas)kelintas;
        }

        public static int Digit(Kelintas kel)
        {
            return (int)kel;
        }

        public static string DgsVard(Kelintas kel)
        {
            switch (kel)
                {
                    case Kelintas.Extra:
                        return "papildomi";
                    case Kelintas.First:
                        return "pirmieji";
                    case Kelintas.Second:
                        return "antrieji";
                    case Kelintas.Third:
                        return "tretieji";
                    case Kelintas.Fourth:
                        return "ketvirtieji";
                }
            return "samsing weri vyrd chepened";
        }

        public static string VnsVard(Kelintas kel)
        {
            switch (kel)
                {
                    case Kelintas.Extra:
                        return "papildomas";
                    case Kelintas.First:
                        return "pirmasis";
                    case Kelintas.Second:
                        return "antrasis";
                    case Kelintas.Third:
                        return "trečiasis";
                    case Kelintas.Fourth:
                        return "ketvirtasis";
                }
            return "samsing weri vyrd chepened";
        }

        public static string Roman(Kelintas kel)
        {
            switch (kel)
            {
                case Kelintas.Extra:
                    return "EXTRA";
                case Kelintas.First:
                    return "I";
                case Kelintas.Second:
                    return "II";
                case Kelintas.Third:
                    return "III";
                case Kelintas.Fourth:
                    return "IV";
            }
            return "samsing weri vyrd chepened";
        }
    }
}
