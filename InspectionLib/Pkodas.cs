using InspectionLib;
using System;

namespace InspectionLib
{
    public enum Pkodas { None, D3, D2, D1, DP, ID, L }

    // arba struct, arba static class
    public struct Pavojingumas
    {
        private Pkodas _pkodas;

        public Pavojingumas(string pavojingumas)
        {
            if (pavojingumas == null || pavojingumas.Trim() == string.Empty)
            {
                _pkodas = Pkodas.None;
            }
            else
            {
                _pkodas = (Pkodas)Enum.Parse(typeof(Pkodas), pavojingumas);
            }
        }

        public Pavojingumas(Pkodas pkodas)
        {
            _pkodas = pkodas;
        }

        public Pkodas Pkodas
        {
            get
            {
                return _pkodas;
            }
        }

        public override string ToString()
        {
            if (_pkodas == Pkodas.None)
                return string.Empty;
            else
                return _pkodas.ToString();
        }
    }
}
