

using System.Xml;

public class Mapping
{
    public string FilePath { get; set; }

    public List<MapNode> RootNodes { get; set; }
}

public class MapNode
{
    public string Code { get; init; }
    
    /// <summary>
    /// Before sending line to be interpreted, collect all modifiers of this node.
    /// </summary>
    /// <param name="line"></param>
    public MapNode(string line)
    {
        Code = line;
        InterpretCode();
    }

    public Value Name { get; set; }
    public Value? Value { get; set; }
    public MapNode? Parent { get; set; }
    public List<MapNode>? Children { get; set; }

    public List<ElementModifier>? Mods { get; set; }

    private void InterpretCode()
    {

    }

    public XmlElement Map() { throw new NotImplementedException(); }
}

public class ElementModifier
{
    public enum ElementModifierAction { Attr, List, Relative, Try }
    public ElementModifierAction Action { get; set; }
    public List<Value>? Args { get; set; }
}

public class Value
{
    public enum ValueEvaluator { None, Xml, Date, If, _Eq, Sum, Num_F, Split_and_Take, Split_and_Remove }
    public ValueEvaluator Evaluator { get; set; }
    public List<Value>? Args { get; set; }
    public string PlainValue { get; set; }
    private Value Eval(XmlDocument doc) { throw new NotImplementedException(); }
    public string Evaluate(XmlDocument doc) => Evaluator is ValueEvaluator.None ? PlainValue : Eval(doc).Evaluate(doc);
}