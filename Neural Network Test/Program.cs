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
            List<double> Inputs = new List<double>();
            while (true) 
            {
                for(int i = 0; i < iNodes; i++)
                {
                    Inputs.Add(double.Parse(Input($"Enter Value {i + 1} Here:")));
                }
                List<double> Results = Net.Run(Inputs.ToArray());
                for(int i = 0; i < Results.Count; i++)
                {
                    Console.WriteLine($"Output {i + 1}: {Results[i]}");
                }
            }
        }
        static string Input(string InputText)
        {
            Console.Write(InputText);
            return Console.ReadLine();
        }
    }

    public static class ListExtensions
    {
        public static List<double> ActivateFunctions(this List<Node> Nodes, List<Connection> inputConnections)
        {
            List<double> ToReturn = new List<double>();
            for (int i = 0; i < Nodes.Count; i++) //Loop Through 
            {
                List<Connection> connections = inputConnections.Where((conn) => conn.OutputNodeID == i).ToList();
                double value = 0;
                foreach (var connection in connections)
                {
                    value += connection.value * connection.weight;
                }

                double Result = Nodes[i].ActivationFunction(value);
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
        public List<double> Run(params double[] Input)
        {
            if (Input.Length != INodeCount)
                throw new ArgumentException($"Number Of Inputs Should Match Input Node Count (Node Count:{INodeCount}, Inputs Provided:{Input.Length}) ");
            List<double> ToReturn = new List<double>();
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
                //Nodes.Add(new Node(i));//, r.NextDouble() * 100, r.NextDouble() * 100
            //}
            outputConnections = Connection.GenerateConnections(INodeCount, OutputNodeCount);
        }
        public void SetValues(double[] Inputs)
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
                Nodes.Add(new Node(i, r.NextDouble() * 100, r.NextDouble() * 10));
            }
        }
        public List<double> ActivateLayerFunction(List<Connection> inputConnections)
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
                Nodes.Add(new Node(i, r.NextDouble() * 100, r.NextDouble() * 10));
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
                double value = 0;
                foreach(var connection in connections)
                {
                    value += connection.value * connection.weight; 
                }
                
                double Result = Nodes[i].ActivationFunction(value);
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
        public double value;
        public double weight;
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
        public Connection(int InputNodeID, int OutputNodeID, double weight = 1)
        {
            this.InputNodeID = InputNodeID;
            this.OutputNodeID = OutputNodeID;
            this.weight = weight;
        }
    }
    public class Node
    {
        public int NodeID; //aka if it's the first one it's 1 if it's the second one it's 2 etc
        public double importantnumberthingie = 1;
        public double bias = 0;
        public Node(int NodeID)
        {
            this.NodeID = NodeID;
        }
        public Node(int NodeID, double value, double bias = 0)
        {
            this.NodeID = NodeID;
            importantnumberthingie = value;
            this.bias = bias;
        }
        public double ActivationFunction(double input)
        {
            double bInput = input - bias;
            double bwInput = importantnumberthingie * bInput;
            double bwEnput = Math.Exp(bwInput);
            return bwEnput / ++bwEnput;
        }
    }
}
