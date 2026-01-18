namespace AssetParser.TypeTreeUtils;

public static class TypeTreeHelper
{
    public static TypeTreeNode BuildTypeTree(List<TypeTreeNode> nodes)
    {
        TypeTreeNode rootNode = nodes[0];
        Stack<TypeTreeNode> stack = new();
        foreach (var node in nodes)
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
        stack.Clear();
        return rootNode;
    }
}
