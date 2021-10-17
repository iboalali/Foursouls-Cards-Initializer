using System;

namespace Foursouls_Cards_Initializer
{
    public class CardModel
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string CardPack { set; get; }
        public string Type { set; get; }

        public CardModel(string name, string expension, string type, int id)
        {
            Name = name;
            CardPack = expension;
            Type = type;
            Id = id;
        }

        public string ToLine()
        {
            return string.Format("{0}\t{1}\t{2}", Id, Name, CardPack) + Environment.NewLine;
        }
    }
}
