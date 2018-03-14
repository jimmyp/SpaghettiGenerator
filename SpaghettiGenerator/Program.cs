using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaghettiGenerator
{
    class Program
    {
        static readonly Random random = new Random();

        static readonly List<string> MethodNames = new List<string>()
        {
            "Composite", "Invalid", "Supported", "Focus", "Traversal", "Abstract",
            "Transformer", "Common", "Concrete", "Autowire", "Simple", "Aware",
            "Aspect", "Principal", "Driven", "Interruptible", "Batch",
            "Prepared", "Statement", "Remote", "Stateless", "Session",
            "Transaction", "Transactional", "Based", "Meta", "Data", "Jms",
            "Readable", "Literal", "Reflective", "Scope", "Multipart", "Xml",
            "Generic", "Interface", "Advisable", "Observable", "Identifiable",
            "Iterable", "Distributed", "Notification", "Failure", "Type",
            "Http", "Jdbc"
        };

        private static readonly IList<AThing> Methods = MethodNames.Select(x => new AThing(x)).ToList();

        static void Main(string[] args)
        {

            foreach (var method in Methods)
            {
                for (var i = 0; i < random.Next(0, 6); i++)
                {
                    method.Calls(RandomMethod());
                }
            }

            var sb = new StringBuilder();

            sb.AppendLine("public class God {");

            foreach (var method in Methods)
            {
                method.SerialiseDefinitionTo(x => sb.AppendLine(x));
            }

            sb.AppendLine("}");

            Console.Write(sb.ToString());

            Console.ReadLine();
        }

        private static AThing RandomMethod()
        {
            return Random(Methods);
        }

        private static T Random<T>(IEnumerable<T> ts)
        {
            var enumerable = ts as T[] ?? ts.ToArray();
            var index = random.Next(0, enumerable.Count()- 1);
            return enumerable.ElementAt(index);
        }
    }
    
    public  class AThing
    {
        private readonly IList<AThing> _methodsToCall = new List<AThing>();
        private readonly string _name;

        public AThing(string name)
        {
            _name = name;
        }

        internal void Calls(AThing aThingToCall)
        {
            _methodsToCall.Add(aThingToCall);
        }

        public void SerialiseDefinitionTo(Action<string> append)
        {
            append($"public void {_name}() {{");
            foreach (var method in _methodsToCall)
            {
                method.SerlialiseCallTo(append);
            }
            append($"}}");
        }

        private void SerlialiseCallTo(Action<string> append)
        {
            append($"{_name}();");
        }

        public IEnumerable<AThing> CalledMethods()
        {
            return _methodsToCall;
        }
    }
}



