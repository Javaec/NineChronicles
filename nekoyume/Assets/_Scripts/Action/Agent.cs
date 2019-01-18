using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Libplanet;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Tx;
using UnityEngine;
using Avatar = Nekoyume.Model.Avatar;

namespace Nekoyume.Action
{
    public class Agent
    {
        public event EventHandler<Avatar> DidReceiveAction;
        private readonly PrivateKey privateKey;
        private readonly float interval;
        public Address UserAddress => privateKey.PublicKey.ToAddress();
        public readonly ConcurrentQueue<ActionBase> queuedActions;


        internal readonly Blockchain<ActionBase> blocks;

        public Agent(PrivateKey privateKey, string path, float interval = 3.0f)
        {
            this.privateKey = privateKey;
            this.interval = interval;
            blocks = new Blockchain<ActionBase>(new FileStore(path));
            queuedActions = new ConcurrentQueue<ActionBase>();
        }

        public IEnumerator Sync()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                var task = Task.Run(() => blocks.GetStates(new[] {UserAddress}));
                yield return new WaitUntil(() => task.IsCompleted);
                var ctx = (Context) task.Result.GetValueOrDefault(UserAddress);
                if (ctx?.avatar != null)
                {
                    DidReceiveAction?.Invoke(this, ctx.avatar);
                }

                yield return null;
            }
        }

        public IEnumerator Mine()
        {
            while (true)
            {
                var processedActions = new List<ActionBase>();
                for (int i = 0; i < queuedActions.Count; i++)
                {
                    ActionBase action;
                    queuedActions.TryDequeue(out action);
                    if (action != null)
                    {
                        processedActions.Add(action);
                    }
                }
                StageTransaction(processedActions);
                var task = Task.Run(() => blocks.MineBlock(UserAddress));
                yield return new WaitUntil(() => task.IsCompleted);
                Debug.Log($"created block index: {task.Result.Index}");
            }
        }

        private void StageTransaction(IList<ActionBase> actions)
        {
            var tx = Transaction<ActionBase>.Make(
                privateKey,
                UserAddress,
                actions,
                DateTime.UtcNow
            );
            blocks.StageTransactions(new HashSet<Transaction<ActionBase>> { tx });
        }
    }
}
