using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yaya_butonu_test
{
    public class TimeClass
    {
        public  string zaman;

        public string Zaman
        {
            get { return zaman; }
            set { zaman = value; }
        }
        private string pazartesi;

        public string Pazartesi
        {
            get { return pazartesi; }
            set { pazartesi = value; }
        }
        private string sali;

        public string Sali
        {
            get { return sali; }
            set { sali = value; }
        }
        private string carsamba;

        public string Carsamba
        {
            get { return carsamba; }
            set { carsamba = value; }
        }
        private string persembe;

        public string Persembe
        {
            get { return persembe; }
            set { persembe = value; }
        }
        private string cuma;

        public string Cuma
        {
            get { return cuma; }
            set { cuma = value; }
        }
        private string cumartesi;

        public string Cumartesi
        {
            get { return cumartesi; }
            set { cumartesi = value; }
        }
        private string pazar;

        public string Pazar
        {
            get { return pazar; }
            set { pazar = value; }
        }

        public override string ToString()
        {
            return Zaman + Pazartesi + Sali + Carsamba + Persembe + Cuma + Cumartesi + Pazar;
        }
    }
}
