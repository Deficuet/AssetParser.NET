using System.Diagnostics.CodeAnalysis;

namespace AssetParser.TypeTreeUtils;

public static class TypeTreeHelper
{
    public static void AddNode(this Stack<TypeTreeNode> stack, TypeTreeNode node)
    {
        while (stack.Count > 0 && stack.Peek().level >= node.level)
        {
            stack.Pop();
        }
        if (stack.Count > 0)
        {
            stack.Peek().children.Add(node);
        }
        stack.Push(node);
    }

    public static TypeTreeNode BuildTypeTree(List<TypeTreeNode> nodes)
    {
        TypeTreeNode rootNode = nodes[0];
        Stack<TypeTreeNode> stack = new();
        foreach (var node in nodes)
        {
            stack.AddNode(node);
        }
        stack.Clear();
        return rootNode;
    }

    public static bool TryParseExportNodeLine(string line, [NotNullWhen(true)] out TypeTreeNode? node, char indentCharacter = '\t', int indentSize = 1)
    {
        node = null;
        if (line is null)
        {
            return false;
        }
        int leadingIndent = 0;
        while (leadingIndent < line.Length && line[leadingIndent] == indentCharacter)
        {
            leadingIndent++;
        }
        int indentLevel = leadingIndent / indentSize;

        var rest = line.Trim();
        var seg = rest.Split(' ');
        if (seg.Length < 4)
        {
            return false;
        }
        var boolToken = seg.Last();
        if (!bool.TryParse(boolToken, out var doAlignReader))
        {
            return false;
        }
        var sizeToken = seg[^2];
        if (!int.TryParse(sizeToken, out var byteSize))
        {
            return false;
        }
        var classNameToken = seg[0];
        var classNameTokenSize = 1;
        if (classNameToken == "long" && seg[1] == "long")
        {
            classNameToken = "long long";
            classNameTokenSize = 2;
        }
        else if (classNameToken == "unsigned")
        {
            if (seg[1] == "long" && seg[2] == "long")
            {
                classNameToken = "unsigned long long";
                classNameTokenSize = 3;
            }
            else
            {
                classNameToken = string.Join(" ", seg.Take(2));
                classNameTokenSize = 2;
            }
        }
        var nameSeg = seg.Skip(classNameTokenSize).Take(seg.Length - classNameTokenSize - 2).ToArray();
        var name = string.Join(" ", nameSeg);
        node = new TypeTreeNode()
        {
            name = name,
            type = classNameToken,
            byteSize = byteSize,
            metaFlag = doAlignReader ? 0x4000 : 0,
            level = indentLevel
        };
        return true;
    }
}
