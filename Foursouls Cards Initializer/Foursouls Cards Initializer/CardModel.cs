using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foursouls_Cards_Initializer
{
    class CardModel
    {
        public string Name { set; get; }
        public string Expansion { set; get; }
        public int Id { set; get; }
        public string Type { set; get; }

        public CardModel()
        {

        }

        public CardModel(string name, string expension, string type, int id)
        {
            Name = name;
            Expansion = expension;
            Type = type;
            Id = id;
        }
    }
}
