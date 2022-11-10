using StormWarfare.Model;

namespace StormWarfare.Models
{
    public class DrawCard : BaseSpecialEffect
    {
        public int Count;

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Count = json.ToInt("count");
        }
    }
}