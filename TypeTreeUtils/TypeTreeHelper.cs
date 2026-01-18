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
}
