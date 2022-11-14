using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesControl.Klasy
{
    internal class Item
    {
        public readonly string Nazwa;
        public readonly string Id;
        public readonly string? ParentId;

        public Item(string Nazwa, string id, string? parent = null)
        {
            this.Nazwa = Nazwa;
            Id = id;
            ParentId = parent;
        }
        public readonly List<Item> Children = new();
    }
}
