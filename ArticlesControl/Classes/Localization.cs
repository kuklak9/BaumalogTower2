namespace ArticlesControl.Klasy
{
    internal class Localization
    {
        #region Fields

        public int? ID_Lokalizacji { get; set; }
        public int? ID_Polka { get; set; }

        public string? Symbol { get; set; }
        public string? Nazwa { get; set; }
        public string? Opis { get; set; }
        public int? WymiarX { get; set; }
        public int? WymiarY { get; set; }
        public int? WymiarZ { get; set; }
        public int? PozX { get; set; }
        public int? PozY { get; set; }

        public int? PolkaX { get; set; }
        public int? PolkaY { get; set; }
        #endregion
        public Localization(int? ID_Lokalizacji, int? ID_Polka, string? Symbol, string? Nazwa, string? Opis, int? WymiarX, int? WymiarY, int? WymiarZ, int? PozX, int? PozY, int? PolkaX, int? PolkaY)
        {
            this.ID_Lokalizacji = ID_Lokalizacji;
            this.ID_Polka = ID_Polka;
            this.Symbol = Symbol;
            this.Nazwa = Nazwa;
            this.Opis = Opis;
            this.WymiarX = WymiarX;
            this.WymiarY = WymiarY;
            this.WymiarZ = WymiarZ;
            this.PozX = PozX;
            this.PozY = PozY;
            this.PolkaX = PolkaX;
            this.PolkaY = PolkaY;
        }
        public Localization(int? ID_Lokalizacji, string? Symbol, int? WymiarX, int? WymiarY, int? WymiarZ, int? PozX, int? PozY, int? PolkaX, int? PolkaY)
        {
            this.ID_Lokalizacji = ID_Lokalizacji;
            this.Symbol = Symbol;
            this.WymiarX = WymiarX;
            this.WymiarY = WymiarY;
            this.WymiarZ = WymiarZ;
            this.PozX = PozX;
            this.PozY = PozY;
            this.PolkaX = PolkaX;
            this.PolkaY = PolkaY;
        }
    }
}
