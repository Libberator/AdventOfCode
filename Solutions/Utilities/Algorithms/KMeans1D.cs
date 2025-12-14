using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AoC.Utilities.Extensions;

namespace AoC.Utilities.Algorithms;

public static class KMeans1D
{
    public static ClusterResult<T> Cluster<T>(IList<T> data, int k, int maxIterations = 100) where T : INumber<T>
    {
        if (data == null || data.Count == 0)
            throw new ArgumentException("Data cannot be empty.");

        if (k < 1)
            throw new ArgumentException("k must be >= 1.");

        if (k > data.Count)
            throw new ArgumentException("k cannot exceed number of data points.");

        // --- 1. Initialize centers evenly across the sorted values ---
        var centers = InitializeCenters(data, k);

        // Allocate clusters
        var clusters = new List<T>[k];

        for (var iter = 0; iter < maxIterations; iter++)
        {
            // reset clusters
            for (var i = 0; i < k; i++)
                clusters[i] = [];

            // --- 2. Assign each point to nearest center ---
            foreach (var x in data)
            {
                var nearest = 0;
                var bestDist = T.Abs(x - centers[0]);

                for (var i = 1; i < k; i++)
                {
                    var dist = T.Abs(x - centers[i]);
                    if (dist >= bestDist) continue;
                    bestDist = dist;
                    nearest = i;
                }

                clusters[nearest].Add(x);
            }

            // --- 3. Recompute centers (mean of clusters) ---
            var newCenters = new T[k];
            for (var i = 0; i < k; i++)
                if (clusters[i].Count > 0)
                    newCenters[i] = clusters[i].Average();
                else
                    newCenters[i] = centers[i]; // keep old to avoid collapse

            // --- 4. Check convergence ---
            var unchanged = true;
            for (var i = 0; i < k; i++)
            {
                if (newCenters[i] == centers[i]) continue;
                unchanged = false;
                break;
            }

            centers = newCenters;

            if (unchanged)
                break;
        }

        // --- 5. Sort clusters by center value ---
        var pairs = centers
            .Select((c, i) => (center: c, cluster: clusters[i]))
            .OrderBy(p => p.center)
            .ToArray();

        return new ClusterResult<T>(
            pairs.Select(p => p.cluster).ToArray(),
            pairs.Select(p => p.center).ToArray()
        );
    }

    // Helper: initialize k centers using evenly-distributed quantiles
    private static T[] InitializeCenters<T>(IList<T> data, int k) where T : INumber<T>
    {
        var sorted = data.OrderBy(x => x).ToList();
        var centers = new T[k];

        for (var i = 0; i < k; i++)
        {
            // quantile index
            var pos = (sorted.Count - 1) * (i / (double)(k - 1));
            var idx = (int)Math.Round(pos);
            centers[i] = sorted[idx];
        }

        return centers;
    }

    public record ClusterResult<T>(List<T>[] Clusters, T[] Centers) where T : INumber<T>;
}