#region

using System.Collections.Generic;
using System.Linq;
using MagService.MagWeb;
using MagService.Models;

#endregion

namespace MagService.Helpers
{
    public static class MagHelper
    {
        public static MagEntity GetEntityById(long id)
        {
            var query = new Dictionary<string, object>
            {
                {"expr", $"Id={id}"},
                {"attributes", "Id,RId,F.FId,J.JId,C.CId,AA.AuId,AA.AfId"},
                {"subscription-key", AppConfig.SubscriptionKey},
                {"count", 1}
            };
            var result = ApiHelper.Get<MagResponse>(AppConfig.MagApi, query);
            var entity = result.MagEntities.FirstOrDefault();
            return entity;
        }

        public static IEnumerable<long> QueryRId(long id)
        {
            var query = new Dictionary<string, object>
            {
                {"expr", $"Id={id}"},
                {"attributes", "RId"},
                {"subscription-key", AppConfig.SubscriptionKey},
                {"count", 1}
            };
            var result = ApiHelper.Get<MagResponseRId>(AppConfig.MagApi, query);
            var entity = result.MagEntityRIds.FirstOrDefault();
            if (entity == null) return new long[0];
            return entity.ReferenceIds;
        }

        public static IEnumerable<MagEntityR> QueryLId(long rId)
        {
            var query = new Dictionary<string, object>
            {
                {"expr", $"RId={rId}"},
                {"attributes", "Id,F.FId,J.JId,C.CId,AA.AuId,AA.AfId"},
                {"subscription-key", AppConfig.SubscriptionKey},
                {"count", AppConfig.Count}
            };
            var entities = ApiHelper.Get<MagResponseLId>(AppConfig.MagApi, query).MagEntityLIds.ToArray();
            return entities;
        }

        public static IEnumerable<MagEntityAuId> QueryAuId(long auId)
        {
            var query = new Dictionary<string, object>
            {
                {"expr", $"Composite(AA.AuId={auId})"},
                {"attributes", "Id,AA.AuId,AA.AfId"},
                {"subscription-key", AppConfig.SubscriptionKey},
                {"count", AppConfig.Count}
            };
            var auIds = ApiHelper.Get<MagResponseAuId>(AppConfig.MagApi, query).MagEntityAuIds.ToArray();
            foreach (var single in auIds)
            {
                single.Authors.RemoveAll(au => au.Id != auId);
            }
            return auIds;
        }
    }
}