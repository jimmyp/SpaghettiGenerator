using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaghettiGenerator
{
    class Program
    {
        static readonly Random random = new Random();

        static readonly List<string> ClassNames = new List<string>()
        {
            "Factory", "Bean", "Wrapper", "Visitor", "Model", "Singleton",
            "AThing", "Configuration", "SomethingElse", "Error", "Property", "Value",
            "Identifier", "Something", "Authentication"
        };

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

        private static readonly IList<Class> Classes = ClassNames.Select(x => new Class(x)).ToList();

        static void Main(string[] args)
        {

            foreach (var method in Methods)
            {
                for (var i = 0; i < random.Next(0, 6); i++)
                {
                    method.Calls(RandomMethod());
                }

                method.BelongsTo(RandomClass());
            }

            var sb = new StringBuilder();
            foreach (var @class in Classes)
            {
                @class.SerialiseTo(x => sb.AppendLine(x));
            }
            Console.Write(sb.ToString());

            Console.ReadLine();
        }

        private static AThing RandomMethod()
        {
            return Random(Methods);
        }

        private static Class RandomClass()
        {
            return Random(Classes);
        }

        private static T Random<T>(IEnumerable<T> ts)
        {
            var enumerable = ts as T[] ?? ts.ToArray();
            var index = random.Next(0, enumerable.Count()- 1);
            return enumerable.ElementAt(index);
        }
    }

    public class Class
    {
        private readonly IList<AThing> _methods = new List<AThing>();
        private readonly string _name;

        public Class(string name)
        {
            _name = name;
        }

        public void Add(AThing aThing)
        {
            _methods.Add(aThing);
        }

        public void SerialiseTo(Action<string> append)
        {
            append($"public class {_name} {{");

            appendInitialisedFields(append);

            foreach (var method in _methods)
            {
                method.SerialiseDefinitionTo(append);
            }
            append($"}}");

        }

        private void appendInitialisedFields(Action<string> append)
        {
            var classesUsed = _methods.SelectMany(m => m.CalledMethods()).Select(m => m.OwningClass()).Distinct(new ClassComparer());
            foreach (var @class in classesUsed)
            {
                @class.appendInitialisedField(append);
            }
        }

        private void appendInitialisedField(Action<string> append)
        {
            append($"{_name} {FieldName()} = new {_name}();");
        }

        public string GetName()
        {
            return _name;
        }

        public string FieldName()
        {
            return $"_{_name.ToLower()}";
        }
    }

    public class ClassComparer : IEqualityComparer<Class>
    {
        public bool Equals(Class x, Class y)
        {
            if (x.GetName() == y.GetName())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Class obj)
        {
            return obj.GetName().GetHashCode();
        }
    }

    public  class AThing
    {
        private readonly IList<AThing> _methodsToCall = new List<AThing>();
        private readonly string _name;
        private Class _owningClass;

        public AThing(string name)
        {
            _name = name;
        }

        internal void Calls(AThing aThingToCall)
        {
            _methodsToCall.Add(aThingToCall);
        }

        public void BelongsTo(Class owningClass)
        {
            if(_owningClass != null) throw new Exception();
            _owningClass = owningClass;
            owningClass.Add(this);
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
            append($"{_owningClass.FieldName()}.{_name}();");
        }

        public Class OwningClass()
        {
            return _owningClass;
        }

        public IEnumerable<AThing> CalledMethods()
        {
            return _methodsToCall;
        }
    }
}



