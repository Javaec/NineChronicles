using System.Collections.Generic;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Action;
using Nekoyume.Model;

namespace Nekoyume.Action
{
    [ActionType("create_novice")]
    public class CreateNovice : ActionBase
    {
        private string _name = "tester";
        // TODO use constructor
        // Avoid MissingMethodException in Tx.ToAction
//        public CreateNovice(string name)
//        {
//            _name = name;
//        }
        public override void LoadPlainValue(IImmutableDictionary<string, object> plainValue)
        {
            _name = (string) plainValue["name"];
        }

        public override AddressStateMap Execute(Address @from, Address to, AddressStateMap states)
        {
            var avatar = new Avatar
            {
                Name = _name,
                Level = 1,
                EXP = 0,
                HPMax = 0,
                WorldStage = 1,
                CurrentHP = 0,
            };
            var ctx = new Context(avatar);
            return (AddressStateMap) states.SetItem(to, ctx);
        }
        public override IImmutableDictionary<string, object> PlainValue => new Dictionary<string, object>()
        {
            ["name"] = _name,
        }.ToImmutableDictionary();
    }
}
