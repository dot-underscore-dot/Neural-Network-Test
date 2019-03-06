using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNN
{
    public class Program
    {
        static void Main(string[] args)
        {
            var iNodes = int.Parse(Input("Enter Number of Input Nodes:"));
            var hNodes = int.Parse(Input("Enter Number of Hidden Nodes:"));
            var oNodes = int.Parse(Input("Enter Number of Output Nodes:"));
            var hLayers = int.Parse(Input("Enter Number of Hidden Layers:"));
            Network Net = new Network(iNodes, hNodes, oNodes, hLayers);
            List<decimal> Inputs = new List<decimal>();
            while (true) 
            {
                for(int i = 0; i < iNodes; i++)
                {
                    Inputs.Add(decimal.Parse(Input($"Enter Value {i + 1} Here:")));
                }
                List<decimal> Results = Net.Run(Inputs.ToArray());
                for(int i = 0; i < Results.Count; i++)
                {
                    Console.WriteLine($"Output {i + 1}: {Results[i]}");
                }
                Inputs.Clear();
            }
        }
        //static decimal
        static string Input(string InputText)
        {
            Console.Write(InputText);
            return Console.ReadLine();
        }
    }

    public static class ListExtensions
    {
        public static List<decimal> ActivateFunctions(this List<Node> Nodes, List<Connection> inputConnections)
        {
            List<decimal> ToReturn = new List<decimal>();
            for (int i = 0; i < Nodes.Count; i++) //Loop Through 
            {
                List<Connection> connections = inputConnections.Where((conn) => conn.OutputNodeID == i).ToList();
                decimal value = 0;
                foreach (var connection in connections)
                {
                    value += connection.value * connection.weight;
                }

                decimal Result = Nodes[i].ActivationFunction(value);
                ToReturn.Add(Result);
            }
            //var ToReturn = Nodes.Zip(ToReturnpublic List, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            return ToReturn;
        }
        public static  List<int> FindAllIndexesOf<T>(this List<T> Self, Predicate<T> match)
        {
            List<int> ToReturn = new List<int>();
            bool Continue = true;
            int LastIndex = 0;
            while (Continue)
            {
                int Value = Self.FindIndex(LastIndex, match);
                if (Value == -1)
                    Continue = false;
                else
                {
                    ToReturn.Add(Value);
                    LastIndex = Value + 1;
                }
            }
            return ToReturn;
        }
    }
    public class Network
    {
        public int INodeCount;
        public InputLayer InputLayer;
        public List<Layer> HiddenLayers;
        public OutputLayer OutputLayer;
        public Network(int INodeCount, int HNodeCount, int ONodeCount, int HLayerCount = 1)
        {

            HiddenLayers = new List<Layer>();
            this.INodeCount = INodeCount;
            InputLayer = new InputLayer(INodeCount,HNodeCount);
            for(int i = 0; i < HLayerCount; i++)
            {
                int oCount = HNodeCount;
                if (i == HLayerCount - 1)
                    oCount = ONodeCount;
                HiddenLayers.Add(new Layer(HNodeCount,oCount));
            }
            OutputLayer = new OutputLayer(ONodeCount);
            
        }
        public List<decimal> Run(params decimal[] Input)
        {
            if (Input.Length != INodeCount)
                throw new ArgumentException($"Number Of Inputs Should Match Input Node Count (Node Count:{INodeCount}, Inputs Provided:{Input.Length}) ");
            List<decimal> ToReturn = new List<decimal>();
            InputLayer.SetValues(Input);
            List<Connection> RecentConnection = InputLayer.outputConnections;
            foreach(var Layer in  HiddenLayers)
            {
                RecentConnection = Layer.ActivateLayerFunctions(RecentConnection);
            }
            //RecentConnection.ForEach((x) => Console.WriteLine(x.value));
            //Console.WriteLine(true);
            ToReturn = OutputLayer.ActivateLayerFunction(RecentConnection);
            //ToReturn.ForEach((x) => Console.WriteLine(x));
            return ToReturn;
        }
    }
    
    public class InputLayer
    {
        //public List<Node> Nodes;
        public int INodeCount;
        public List<Connection> outputConnections;
        public InputLayer(int INodeCount,int OutputNodeCount)
        {
            this.INodeCount = INodeCount;
            //Nodes = new List<Node>();
            outputConnections = new List<Connection>();
            Random r = new Random();
            //for (int i = 0; i < INodeCount; i++)
            //{
                //Nodes.Add(new Node(i));//, r.Nextdecimal() * 100, r.Nextdecimal() * 100
            //}
            outputConnections = Connection.GenerateConnections(INodeCount, OutputNodeCount);
        }
        public void SetValues(decimal[] Inputs)
        {
            if (Inputs.Length != INodeCount)
                throw new ArgumentException($"Number Of Inputs Should Match Input Node Count (Node Count:{INodeCount}, Inputs Provided:{Inputs.Length}) ");
            foreach(var Input in Inputs)
            {
                int OutputNodeCount = outputConnections.Count / INodeCount;
                int Index = 0;
                for(int i = 0;i < INodeCount; i++)
                {
                    for(int o = 0;o < OutputNodeCount; o++)
                    {
                        outputConnections[Index].value = Inputs[i];
                        Index++;
                    }
                }
            }
        }
    }
    public class OutputLayer
    {
        public List<Node> Nodes;
        public OutputLayer(int ONodeCount)
        {
            Nodes = new List<Node>();
            Random r = new Random();
            for (int i = 0; i < ONodeCount; i++)
            {
                Nodes.Add(new Node(i, (decimal)r.NextDouble() * 10, (decimal)r.NextDouble() * 10));
            }
        }
        public List<decimal> ActivateLayerFunction(List<Connection> inputConnections)
        {
            return Nodes.ActivateFunctions(inputConnections);//.Select(x => x.Value).Topublic List();
        }
    }
    public class Layer
    {

        public Layer(int HiddenNodeCount, int OutputNodeCount)
        {
            Nodes = new List<Node>();
            outputConnections = new List<Connection>();
            Random r = new Random();
            for (int i = 0; i < HiddenNodeCount; i++)
            {
                Nodes.Add(new Node(i, (decimal)r.NextDouble() * 10, (decimal)r.NextDouble() * 10));
            }
            outputConnections = Connection.GenerateConnections(HiddenNodeCount, OutputNodeCount);
        }
        public List<Connection> ActivateLayerFunctions(List<Connection> inputConnections)
        {
            var Values = Nodes.ActivateFunctions(inputConnections);//.Select(x => x.Value).Topublic List();
            for (int i = 0; i < Nodes.Count; i++) //Loop Through 
            {
                List<int> ConnIndexes = outputConnections.FindAllIndexesOf((conn) => conn.InputNodeID == i);
                foreach (var Index in ConnIndexes)
                {
                    outputConnections[Index].value = Values[i];
                }
            }
            return outputConnections;
        }
        #region OldIgnore
        /*for (int i = 0; i < Nodes.Count; i++) //Loop Through 
            {
                public List<Connection> connections = inputConnections.Where((conn) => conn.OutputNodeID == i).Topublic List();
                decimal value = 0;
                foreach(var connection in connections)
                {
                    value += connection.value * connection.weight; 
                }
                
                decimal Result = Nodes[i].ActivationFunction(value);
            public List<int> ConnIndexes = outputConnections.FindAllIndexesOf((conn) => conn.InputNodeID == i);
            foreach (var Index in ConnIndexes)
            {
                outputConnections[Index].value = Result;
            }
            }*/
        #endregion
        public List<Node> Nodes;
        public List<Connection> outputConnections;
    }
    public class Connection
    {
        
        public int InputNodeID;
        public decimal value;
        public decimal weight;
        public int OutputNodeID;
        public static List<Connection> GenerateConnections(int InputNodeCount, int OutputNodeCount)
        {
            List<Connection> ToReturn = new List<Connection>();
            for (int i = 0; i < InputNodeCount; i++)
            {
                for (int o = 0; o < OutputNodeCount; o++)
                {
                    ToReturn.Add(new Connection(i, o));
                }
            }
            return ToReturn;
        }
        public Connection(int InputNodeID, int OutputNodeID, decimal weight = 1)
        {
            this.InputNodeID = InputNodeID;
            this.OutputNodeID = OutputNodeID;
            this.weight = weight;
        }
    }
    public class Node
    {
        public int NodeID; //aka if it's the first one it's 1 if it's the second one it's 2 etc
        public decimal importantnumberthingie = 1;
        public decimal bias = 0;
        public Node(int NodeID)
        {
            this.NodeID = NodeID;
        }
        public Node(int NodeID, decimal value, decimal bias = 0)
        {
            this.NodeID = NodeID;
            importantnumberthingie = value;
            this.bias = bias;
        }
        public decimal ActivationFunction(decimal input)
        {
            decimal bInput = input - bias;
            decimal bwInput = importantnumberthingie * bInput;
            return (decimal)(1.0 / (1.0 + Math.Pow(Math.E, (double)-bwInput)));
            /*double bwEnput = Math.Exp((double)bwInput);
            var ToReturn = (bwEnput / ++bwEnput);
            return (decimal)ToReturn;*/
        }
    }
}
