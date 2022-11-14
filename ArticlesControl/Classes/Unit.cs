namespace ArticlesControl.Klasy
{
    internal class Unit
    {
        #region Fields
        public int? ID_Jednostki { get; set; }
        public string? Sybmol { get; set; }
        public string? Nazwa { get; set; }
        public int? Precyzja { get; set; }
        #endregion
        public Unit()
        {

        }
        public Unit(string Symbol, string Nazwa, int Precyzja)
        {
            Sybmol = Symbol;
            this.Nazwa = Nazwa;
            this.Precyzja = Precyzja;
        }

        public void Insert()
        {
            //TODO Insert function for Unit
        }
    }
}
