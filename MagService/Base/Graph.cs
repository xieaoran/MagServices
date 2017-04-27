#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagService.Helpers;
using MagService.Models;

#endregion

namespace MagService.Base
{
    public static class Graph
    {
        public static IEnumerable<long[]> Start(long start, long end)
        {
            var networkActions = new Action[4];
            MagEntity startEntity = null, endEntity = null;
            var startEntityAuIds = new MagEntityAuId[0];
            var endEntityAuIds = new MagEntityAuId[0];
            networkActions[0] = () => { startEntity = MagHelper.GetEntityById(start); };
            networkActions[1] = () => { endEntity = MagHelper.GetEntityById(end); };
            networkActions[2] = () => { startEntityAuIds = MagHelper.QueryAuId(start).ToArray(); };
            networkActions[3] = () => { endEntityAuIds = MagHelper.QueryAuId(end).ToArray(); };
            Parallel.Invoke(networkActions);

            if (startEntity?.ReferenceIds.Count == 0
                && startEntity.Authors == null
                && startEntity.Fields == null
                && startEntity.Journal == null
                && startEntity.Conference == null)
                startEntity = null;

            if (endEntity?.ReferenceIds.Count == 0
                && endEntity.Authors == null
                && endEntity.Fields == null
                && endEntity.Journal == null
                && endEntity.Conference == null)
                endEntity = null;

            var queryActions = new List<Action>();
            var results = new ConcurrentBag<long[]>();
            if (startEntity != null && endEntity != null)
                queryActions.Add(() =>
                {
                    foreach (var result in IdToId(start, end, startEntity, endEntity))
                    {
                        results.Add(result);
                    }
                });
            if (startEntity != null && endEntityAuIds.Length > 0)
                queryActions.Add(() =>
                {
                    foreach (var result in IdToAuId(start, end, startEntity, endEntityAuIds))
                    {
                        results.Add(result);
                    }
                });
            if (startEntityAuIds.Length > 0 && endEntity != null)
                queryActions.Add(() =>
                {
                    foreach (var result in AuIdToId(start, end, startEntityAuIds, endEntity))
                    {
                        results.Add(result);
                    }
                });
            if (startEntityAuIds.Length > 0 && endEntityAuIds.Length > 0)
                queryActions.Add(() =>
                {
                    foreach (var result in AuIdToAuId(start, end, startEntityAuIds, endEntityAuIds))
                    {
                        results.Add(result);
                    }
                });
            Parallel.Invoke(queryActions.ToArray());
            return results;
        }

        public static IEnumerable<long[]> IdToId(long start, long end,
            MagEntity startEntity, MagEntity endEntity)
        {
            var result = new ConcurrentBag<long[]>();

            if (startEntity == null || endEntity == null) return new long[0][];

            var startRIdEntities = new ConcurrentBag<MagEntity>();
            var endLIdEntities = new MagEntityR[0];
            var queryActions =
                startEntity.ReferenceIds.Select(
                    rId => (Action)(() => { startRIdEntities.Add(MagHelper.GetEntityById(rId)); })).ToList();
            queryActions.Add(() => { endLIdEntities = MagHelper.QueryLId(endEntity.Id).ToArray(); });
            Parallel.Invoke(queryActions.ToArray());

            var calcActions = new Action[2];
            calcActions[0] = () =>
            {
                if (startRIdEntities.Any(entity => entity.Id == end))
                    // Id - Id
                    result.Add(new[] { start, end });

                foreach (var singleResult in startRIdEntities.Select(se => se.Id)
                    .Intersect(endLIdEntities.Select(ee => ee.Id))
                    .Select(id => new[] { start, id, end }))
                {
                    // Id - Id - Id
                    result.Add(singleResult);
                }
            };
            calcActions[1] = () =>
            {
                var possibleSIds = new List<long>();
                if (startEntity.Fields != null && endEntity.Fields != null)
                    possibleSIds.AddRange(startEntity.Fields.Select(sf => sf.Id).Intersect(
                        endEntity.Fields.Select(ef => ef.Id)));
                if (startEntity.Authors != null && endEntity.Authors != null)
                    possibleSIds.AddRange(startEntity.Authors.Select(sau => sau.Id).Intersect(
                        endEntity.Authors.Select(eau => eau.Id)));
                if (startEntity.Conference != null && endEntity.Conference != null)
                    if (startEntity.Conference.Id == endEntity.Conference.Id)
                        possibleSIds.Add(startEntity.Conference.Id);
                if (startEntity.Journal != null && endEntity.Journal != null)
                    if (startEntity.Journal.Id == endEntity.Journal.Id)
                        possibleSIds.Add(startEntity.Journal.Id);
                foreach (var singleResult in possibleSIds.Where(id => id != 0)
                    .Select(id => new[] { start, id, end }))
                {
                    // Id - FId/AuId/CId/JId - Id
                    result.Add(singleResult);
                }
            };
            Parallel.Invoke(calcActions);

            Parallel.ForEach(startRIdEntities, startRIdEntity =>
            {
                foreach (var singleResult in startRIdEntity.ReferenceIds
                    .Intersect(endLIdEntities.Select(ee => ee.Id))
                    .Select(id => new[] { start, startRIdEntity.Id, id, end }))
                {
                    // Id - Id - Id - Id
                    result.Add(singleResult);
                }
                var possibleIds = new List<long>();
                if (endEntity.Fields != null && startRIdEntity.Fields != null)
                    possibleIds.AddRange(endEntity.Fields.Select(ef => ef.Id).Intersect(
                        startRIdEntity.Fields.Select(sf => sf.Id)));
                if (endEntity.Authors != null && startRIdEntity.Authors != null)
                    possibleIds.AddRange(endEntity.Authors.Select(eau => eau.Id).Intersect(
                        startRIdEntity.Authors.Select(sau => sau.Id)));
                if (endEntity.Conference != null && startRIdEntity.Conference != null)
                    if (endEntity.Conference.Id == startRIdEntity.Conference.Id)
                        possibleIds.Add(endEntity.Conference.Id);
                if (endEntity.Journal != null && startRIdEntity.Journal != null)
                    if (endEntity.Journal.Id == startRIdEntity.Journal.Id)
                        possibleIds.Add(endEntity.Journal.Id);
                foreach (var singleResult in possibleIds.Where(id => id != 0)
                    .Select(id => new[] { start, startRIdEntity.Id, id, end }))
                {
                    // Id - Id - FId/AuId/CId/JId - Id
                    result.Add(singleResult);
                }
            });

            Parallel.ForEach(endLIdEntities, endLIdEntity =>
            {
                var possibleIds = new List<long>();
                if (startEntity.Fields != null && endLIdEntity.Fields != null)
                    possibleIds.AddRange(startEntity.Fields.Select(sf => sf.Id).Intersect(
                        endLIdEntity.Fields.Select(ef => ef.Id)));
                if (startEntity.Authors != null && endLIdEntity.Authors != null)
                    possibleIds.AddRange(startEntity.Authors.Select(sau => sau.Id).Intersect(
                        endLIdEntity.Authors.Select(eau => eau.Id)));
                if (startEntity.Conference != null && endLIdEntity.Conference != null)
                    if (startEntity.Conference.Id == endLIdEntity.Conference.Id)
                        possibleIds.Add(startEntity.Conference.Id);
                if (startEntity.Journal != null && endLIdEntity.Journal != null)
                    if (startEntity.Journal.Id == endLIdEntity.Journal.Id)
                        possibleIds.Add(startEntity.Journal.Id);
                foreach (var singleResult in possibleIds.Where(id => id != 0)
                    .Select(id => new[] { start, id, endLIdEntity.Id, end }))
                {
                    // Id - FId/AuId/CId/JId - Id - Id
                    result.Add(singleResult);
                }
            });

            return result;
        }

        public static IEnumerable<long[]> IdToAuId(long start, long end,
            MagEntity startEntity, MagEntityAuId[] endEntityAuIds)
        {
            var result = new ConcurrentBag<long[]>();

            if (startEntity == null) return new long[0][];

            if (startEntity.Authors != null)
            {
                if (startEntity.Authors.Any(au => au.Id == end))
                    // Id - AuId
                    result.Add(new[] { start, end });

                var afIds = endEntityAuIds.SelectMany(auId => auId.Authors
                    .Select(afId => afId.AffiliationId)).ToArray();

                Parallel.ForEach(startEntity.Authors, author =>
                {
                    if (author.AffiliationId != 0 && afIds.Contains(author.AffiliationId))
                        // Id - AuId - AfId - AuId
                        result.Add(new[] { start, author.Id, author.AffiliationId, end });
                });
            }

            var middleIds = endEntityAuIds.Select(entity => entity.Id).ToArray();
            Parallel.ForEach(middleIds, middleId =>
            {
                var middleEntity = MagHelper.GetEntityById(middleId);

                if (startEntity.ReferenceIds != null)
                {
                    if (startEntity.ReferenceIds.Contains(middleId))
                        // Id - Id - AuId
                        result.Add(new[] { start, middleId, end });

                    foreach (var rId in startEntity.ReferenceIds)
                    {
                        var ridRids = MagHelper.QueryRId(rId);
                        if (ridRids == null) continue;
                        if (ridRids.Contains(middleId))
                            // Id - Id - Id - AuId
                            result.Add(new[] { start, rId, middleId, end });
                    }
                }

                var possibleIds = new List<long>();
                if (startEntity.Fields != null && middleEntity.Fields != null)
                    possibleIds.AddRange(startEntity.Fields.Select(sf => sf.Id).Intersect(
                        middleEntity.Fields.Select(ef => ef.Id)));
                if (startEntity.Authors != null && middleEntity.Authors != null)
                    possibleIds.AddRange(startEntity.Authors.Select(sau => sau.Id).Intersect(
                        middleEntity.Authors.Select(eau => eau.Id)));
                if (startEntity.Conference != null && middleEntity.Conference != null)
                    if (startEntity.Conference.Id == middleEntity.Conference.Id)
                        possibleIds.Add(startEntity.Conference.Id);
                if (startEntity.Journal != null && middleEntity.Journal != null)
                    if (startEntity.Journal.Id == middleEntity.Journal.Id)
                        possibleIds.Add(startEntity.Journal.Id);
                foreach (var singleResult in possibleIds.Where(id => id != 0)
                    .Select(id => new[] { start, id, middleId, end }))
                {
                    // Id - FId/AuId/CId/JId - Id - AuId
                    result.Add(singleResult);
                }
            });

            return result;
        }

        public static IEnumerable<long[]> AuIdToId(long start, long end,
            MagEntityAuId[] startEntityAuIds, MagEntity endEntity)
        {
            var result = new ConcurrentBag<long[]>();

            if (endEntity == null) return new long[0][];

            if (endEntity.Authors != null)
            {
                if (endEntity.Authors.Any(au => au.Id == start))
                    // AuId - Id
                    result.Add(new[] { start, end });
                var afIds = startEntityAuIds.SelectMany(auId => auId.Authors
                    .Select(afId => afId.AffiliationId)).ToArray();
                Parallel.ForEach(endEntity.Authors, author =>
                {
                    if (author.AffiliationId != 0 && afIds.Contains(author.AffiliationId))
                        // AuId - AfId - AuId - Id
                        result.Add(new[] { start, author.AffiliationId, author.Id, end });
                });
            }

            var middleIds = startEntityAuIds.Select(entity => entity.Id).Distinct().ToArray();
            Parallel.ForEach(middleIds, middleId =>
            {
                var middleEntity = MagHelper.GetEntityById(middleId);

                if (middleEntity.ReferenceIds != null)
                {
                    if (middleEntity.ReferenceIds.Contains(end))
                        // AuId - Id - Id
                        result.Add(new[] { start, middleId, end });

                    foreach (var rId in middleEntity.ReferenceIds)
                    {
                        var ridRids = MagHelper.QueryRId(rId);
                        if (ridRids == null) continue;
                        if (ridRids.Contains(middleId))
                            // AuId - Id - Id - Id
                            result.Add(new[] { start, middleId, rId, end });
                    }
                }

                var possibleIds = new List<long>();
                if (middleEntity.Fields != null && endEntity.Fields != null)
                    possibleIds.AddRange(middleEntity.Fields.Select(mf => mf.Id).Intersect(
                        endEntity.Fields.Select(ef => ef.Id)));
                if (middleEntity.Authors != null && endEntity.Authors != null)
                    possibleIds.AddRange(middleEntity.Authors.Select(mau => mau.Id).Intersect(
                        endEntity.Authors.Select(eau => eau.Id)));
                if (middleEntity.Conference != null && endEntity.Conference != null)
                    if (middleEntity.Conference.Id == endEntity.Conference.Id)
                        possibleIds.Add(middleEntity.Conference.Id);
                if (middleEntity.Journal != null && endEntity.Journal != null)
                    if (middleEntity.Journal.Id == endEntity.Journal.Id)
                        possibleIds.Add(middleEntity.Journal.Id);
                foreach (var singleResult in possibleIds.Where(id => id != 0)
                    .Select(id => new[] { start, middleId, id, end }))
                {
                    // AuId - Id - FId/AuId/CId/JId - Id
                    result.Add(singleResult);
                }
            });
            return result;
        }

        public static IEnumerable<long[]> AuIdToAuId(long start, long end,
            MagEntityAuId[] startEntityAuIds, MagEntityAuId[] endEntityAuIds)
        {
            var result = new ConcurrentBag<long[]>();

            var startIds = startEntityAuIds.Select(sau => sau.Id).ToArray();
            var endIds = endEntityAuIds.Select(eau => eau.Id).ToArray();

            var calcActions = new Action[2];

            calcActions[0] = () =>
            {
                foreach (var singleResult in startIds
                    .Intersect(endIds)
                    .Select(id => new[] { start, id, end }))
                {
                    // AuId - Id - AuId
                    result.Add(singleResult);
                }
            };

            calcActions[1] = () =>
            {
                foreach (
                    var singleResult in startEntityAuIds.SelectMany(sau => sau.Authors).Select(au => au.AffiliationId)
                        .Intersect(endEntityAuIds.SelectMany(eau => eau.Authors).Select(au => au.AffiliationId))
                        .Where(afId => afId != 0).Select(id => new[] { start, id, end }))
                {
                    // AuId - AfId - AuId
                    result.Add(singleResult);
                }
            };
            Parallel.Invoke(calcActions);

            Parallel.ForEach(startIds, startId =>
            {
                var startIdRIds = MagHelper.QueryRId(startId);

                foreach (var singleResult in startIdRIds.Intersect(endIds)
                    .Select(id => new[] { start, startId, id, end }))
                {
                    // AuId - Id - Id - AuId
                    result.Add(singleResult);
                }
            });
            return result;
        }
    }
}