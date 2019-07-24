using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Libplanet;
using Libplanet.Action;
using Nekoyume.State;

namespace Nekoyume.Action
{
    [ActionType("delete_avatar")]
    public class DeleteAvatar : GameAction
    {
        public int index;
        public Address avatarAddress;

        protected override IImmutableDictionary<string, object> PlainValueInternal => new Dictionary<string, object>
        {
            ["index"] = index.ToString(),
            ["avatarAddress"] = avatarAddress.ToByteArray(),
        }.ToImmutableDictionary();

        protected override void LoadPlainValueInternal(IImmutableDictionary<string, object> plainValue)
        {
            index = int.Parse(plainValue["index"].ToString());
            avatarAddress = new Address((byte[]) plainValue["avatarAddress"]);
        }

        public override IAccountStateDelta Execute(IActionContext ctx)
        {
            var states = ctx.PreviousStates;
            if (ctx.Rehearsal)
            {
                states = states.SetState(ctx.Signer, MarkChanged);
                return states.SetState(avatarAddress, MarkChanged);
            }

            var agentState = (AgentState) states.GetState(ctx.Signer);
            if (agentState == null)
            {
                return states;
            }

            if (!agentState.avatarAddresses.ContainsKey(index))
            {
                return states;
            }

            if (!agentState.avatarAddresses[index].Equals(avatarAddress))
            {
                return states;
            }

            agentState.avatarAddresses.Remove(index);

            var deletedAvatarState = new DeletedAvatarState(
                (AvatarState) states.GetState(avatarAddress),
                DateTimeOffset.UtcNow);

            states = states.SetState(ctx.Signer, agentState);
            return states.SetState(avatarAddress, deletedAvatarState);
        }
    }
}
