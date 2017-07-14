using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Challenges
{
    public class CloningService : ICloningService
    {
        public T Clone<T>(T source)
        {
            // Please implement this method
            Dictionary<object, object> mapping = new Dictionary<object, object>();
            return ToClone(source, mapping);
        }

        private T ToClone<T>(T source, Dictionary<object, object> mapping)
        {
            var type = typeof(T);
            List<object> objects = GetAllRefObjects(source);

            foreach (object o in objects)
            {
                if (o.GetType().IsArray)
                {
                    var enumerable = o as IEnumerable;
                    if (enumerable != null)
                    {
                        foreach (var e in enumerable)
                        {

                        }
                    }

                }
            }
            return (T)new object();
        }

        private List<object> GetAllRefObjects(object source)
        {
            var vis = new HashSet<object>();
            var queue = new Queue<object>();
            vis.Add(source);
            queue.Enqueue(source);

            while (queue.Count != 0)
            {
                var data = queue.Dequeue();
                if (data is IEnumerable)
                {
                    var enumeable = data as IEnumerable;
                    foreach (var t in enumeable)
                    {
                        if (!t.GetType().IsPrimitive)
                        {
                            if (!vis.Contains(t))
                            {
                                queue.Enqueue(t);
                                vis.Add(t);
                            }
                        }
                    }
                }
            }
            return new List<object>(vis);
        }

        private bool CheckSameNode(CloningServiceTest.Node oldNode, CloningServiceTest.Node currentClonedNode)
        {
            if (oldNode == null && currentClonedNode == null) return true;
            if (oldNode == null || currentClonedNode == null) return false;
            if (oldNode.Value == currentClonedNode.Value && oldNode.TotalNodeCount == currentClonedNode.TotalNodeCount)
            {
                return CheckSameNode(oldNode.Left, currentClonedNode.Left) &&
                       CheckSameNode(oldNode.Right, currentClonedNode.Right);
            }
            return false;
        }

        private CloningServiceTest.Node[] CloneNodeCollection(CloningServiceTest.Node[] source)
        {
            var enumerable = source as IEnumerable<CloningServiceTest.Node>;
            if (enumerable != null)
            {
                var enumerList = enumerable.ToList();
                var dictionary = new Dictionary<int, List<CloningServiceTest.Node>>();
                var resultArr = new CloningServiceTest.Node[enumerList.Count];
                for (var i = 0; i < enumerList.Count; i++)
                {
                    var currentClonedNode = Clone(enumerList[i]);
                    if (dictionary.Count == 0 || !dictionary.ContainsKey(currentClonedNode.TotalNodeCount))
                    {
                        resultArr[i] = currentClonedNode;
                        var newList = new List<CloningServiceTest.Node>();
                        newList.Add(currentClonedNode);
                        dictionary.Add(currentClonedNode.TotalNodeCount, newList);
                    }
                    else if (dictionary.ContainsKey(currentClonedNode.TotalNodeCount))
                    {
                        var nodeSet = dictionary[currentClonedNode.TotalNodeCount];
                        foreach (var node in nodeSet)
                        {
                            if (CheckSameNode(node, currentClonedNode))
                            {
                                resultArr[i] = node;
                            }
                            else
                            {
                                nodeSet.Add(currentClonedNode);
                                resultArr[i] = currentClonedNode;
                                dictionary[currentClonedNode.TotalNodeCount] = nodeSet;
                            }
                        }
                    }
                }
                return resultArr;
            }
            return new CloningServiceTest.Node[0];
        }

        private T TypeClone<T>(T source)
        {
            var type = typeof(T);
            var typeName = AnalyzeType(type);
            if (typeName == AnalyzeType(typeof(CloningServiceTest.Simple)))
            {
                var convertedSource = (CloningServiceTest.Simple)(Object)source;
                return (T)CloneSimple(convertedSource);
            }
            else if (typeName == AnalyzeType(typeof(CloningServiceTest.Simple2)))
            {
                var convertedSource = (CloningServiceTest.Simple2)(Object)source;
                return (T)CloneSimple2(convertedSource);
            }
            else if (typeName == AnalyzeType(typeof(CloningServiceTest.SimpleStruct)))
            {
                var convertedSource = (CloningServiceTest.SimpleStruct)(Object)source;
                return (T)CloneSimpleStruct(convertedSource);
            }
            else if (typeName == AnalyzeType(typeof(CloningServiceTest.Node)))
            {
                var convertedSource = (CloningServiceTest.Node)(Object)source;
                return (T)GraphClone(convertedSource);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        private object CloneSimpleStruct(CloningServiceTest.SimpleStruct convertedSource)
        {
            var i = DeepCopy(convertedSource.I);
            var s = DeepCopy(convertedSource.S);
            var simple = new CloningServiceTest.SimpleStruct()
            {
                I = i,
                S = s
            };
            return simple;
        }

        private object CloneSimple2(CloningServiceTest.Simple2 convertedSource)
        {
            var i = DeepCopy(convertedSource.I);
            var s = DeepCopy(convertedSource.S);
            var d = DeepCopy(convertedSource.D);
            var ss = (CloningServiceTest.SimpleStruct)CloneSimpleStruct(convertedSource.SS);
            var simple = new CloningServiceTest.Simple2()
            {
                I = i,
                S = s,
                D = d,
                SS = ss
            };
            return simple;
        }

        private object CloneSimple(CloningServiceTest.Simple convertedSource)
        {
            var i = DeepCopy(convertedSource.I);
            var s = DeepCopy(convertedSource.S);
            var shallow = convertedSource.Shallow;
            var simple = new CloningServiceTest.Simple()
            {
                I = i,
                S = s,
                Shallow = shallow
            };
            return simple;
        }

        private String AnalyzeType(Type type)
        {
            var tUnderlyingSystem = type.UnderlyingSystemType;
            return tUnderlyingSystem.Name;
        }

        public T DeepCopy<T>(T realObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, realObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static object GraphClone(CloningServiceTest.Node seed)
        {
            if (seed == null) return null;
            LinkedList<CloningServiceTest.Node> nodes = GraphCloneHelper(seed);
            Dictionary<CloningServiceTest.Node, CloningServiceTest.Node> map = new Dictionary<CloningServiceTest.Node, CloningServiceTest.Node>();

            foreach (CloningServiceTest.Node node in nodes)
            {
                map.Add(node, new CloningServiceTest.Node());
            }

            foreach (CloningServiceTest.Node node in nodes)
            {
                CloningServiceTest.Node copied = map[node];
                if (node.Left != null)
                    copied.Left = map[node.Left];

                if (node.Right != null)
                    copied.Right = map[node.Right];
            }

            return map[seed];
        }

        // get all nodes in original graph
        public static LinkedList<CloningServiceTest.Node> GraphCloneHelper(CloningServiceTest.Node seed)
        {
            HashSet<CloningServiceTest.Node> vis = new HashSet<CloningServiceTest.Node>();
            Queue<CloningServiceTest.Node> queue = new Queue<CloningServiceTest.Node>();
            queue.Enqueue(seed);
            vis.Add(seed);

            while (queue.Count != 0)
            {
                CloningServiceTest.Node node = queue.Dequeue();
                if (!vis.Contains(node.Left))
                {
                    vis.Add(node.Left);
                    queue.Enqueue(node.Left);
                }

                if (!vis.Contains(node.Right))
                {
                    vis.Add(node.Right);
                    queue.Enqueue(node.Right);
                }
            }

            return new LinkedList<CloningServiceTest.Node>(vis);
        }
    }
}
