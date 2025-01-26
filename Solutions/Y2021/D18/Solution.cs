using System;
using System.Collections.Generic;

namespace AoC.Solutions.Y2021.D18;

public class Solution : ISolver
{
    private const int MaxDepth = 4;
    private readonly List<Node> _nodes = [];

    public void Setup(string[] input)
    {
        foreach (var line in input)
            _nodes.Add(ParseNodes(line));
    }

    public object SolvePart1()
    {
        var node = _nodes[0];
        for (var i = 1; i < _nodes.Count; i++)
            node = (node + _nodes[i]).Reduce();

        return node.Magnitude;
    }

    public object SolvePart2()
    {
        var max = int.MinValue;

        for (var i = 0; i < _nodes.Count - 1; i++)
            for (var j = i + 1; j < _nodes.Count; j++)
            {
                max = Math.Max(max, (_nodes[i] + _nodes[j]).Reduce().Magnitude);
                max = Math.Max(max, (_nodes[j] + _nodes[i]).Reduce().Magnitude);
            }

        return max;
    }

    private static Node ParseNodes(string line)
    {
        Node? node = null;
        var isLeft = true;

        foreach (var symbol in line)
            switch (symbol)
            {
                case '[':
                    isLeft = true;
                    var child = new Node(node);
                    if (node != null)
                    {
                        if (node.Left == null) node.Left = child;
                        else node.Right = child;
                    }

                    node = child;
                    break;
                case ']': node = node!.Parent ?? node; break;
                case ',': isLeft = false; break;
                default:
                    if (isLeft) node!.Left = new Node(node) { Number = symbol - '0' };
                    else node!.Right = new Node(node) { Number = symbol - '0' };
                    break;
            }

        return node!;
    }

    private class Node(Node? parent = null)
    {
        public Node? Left, Right, Parent = parent;
        public int? Number;

        public int Magnitude => Number ?? 3 * Left!.Magnitude + 2 * Right!.Magnitude;

        public Node Reduce()
        {
            while (TryExplode() || TrySplit())
            {
                // no-op
            }

            return this;
        }

        private bool TryGetAdjacent(out Node node, bool left)
        {
            node = this;

            // traverse up the tree, checking in direction for new sibling 
            while (node.Parent is { } parent && (left ? parent.Left : parent.Right) == node)
                node = parent;

            // reached root without finding an adjacent node
            if (node.Parent == null) return false;

            // found common ancestor; go down that side
            node = left ? node.Parent.Left! : node.Parent.Right!;

            // traverse down to first value node, hugging the inner (opposite) side
            while (node is { Number: null })
                node = left ? node.Right! : node.Left!;

            return true;
        }

        private bool TryExplode()
        {
            if (!DfsCheck(this, CanExplode, out var target)) return false;
            target.Explode();
            return true;

            static bool CanExplode(Node node, int depth) => depth >= MaxDepth && !node.Number.HasValue &&
                                                     node is { Left.Number: not null, Right.Number: not null };
        }

        private void Explode()
        {
            if (TryGetAdjacent(out var left, true)) left.Number += Left!.Number;
            if (TryGetAdjacent(out var right, false)) right.Number += Right!.Number;

            Left = null;
            Right = null;
            Number = 0;
        }

        private bool TrySplit()
        {
            if (!DfsCheck(this, ShouldSplit, out var target)) return false;
            target.Split();
            return true;

            static bool ShouldSplit(Node node, int _) => node.Number is >= 10;
        }

        private void Split()
        {
            Left = new Node(this) { Number = Number / 2 };
            Right = new Node(this) { Number = (Number + 1) / 2 };
            Number = null;
        }

        private static bool DfsCheck(Node node, Func<Node, int, bool> predicate, out Node target, int depth = 0)
        {
            target = node;
            if (predicate(node, depth)) return true;

            return (L: node.Left, R: node.Right) switch
            {
                (not null, null) => DfsCheck(node.Left, predicate, out target, depth + 1),
                (null, not null) => DfsCheck(node.Right, predicate, out target, depth + 1),
                (not null, not null) =>
                    DfsCheck(node.Left, predicate, out target, depth + 1) ||
                    DfsCheck(node.Right, predicate, out target, depth + 1),
                _ => false
            };
        }

        private Node Clone() => DfsClone(this, new Node());

        private static Node DfsClone(Node original, Node copy)
        {
            if (original.Number.HasValue)
            {
                copy.Number = original.Number;
                return copy;
            }

            copy.Left = new Node(copy);
            copy.Right = new Node(copy);
            DfsClone(original.Left!, copy.Left);
            DfsClone(original.Right!, copy.Right);
            return copy;
        }

        // This creates a clone in order to not modify the original
        public static Node operator +(Node left, Node right)
        {
            var root = new Node
            {
                Left = left.Clone(),
                Right = right.Clone()
            };
            root.Left.Parent = root;
            root.Right.Parent = root;
            return root;
        }
    }
}