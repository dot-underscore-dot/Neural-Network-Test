using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNN
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    static class ListExtensions
    {
        public static Dictionary<Node,double> ActivateFunctions(this List<Node> Nodes, List<Connection> inputConnections)
        {
            List<double> ToReturnList = new List<double>();
            for (int i = 0; i < Nodes.Count; i++) //Loop Through 
            {
                List<Connection> connections = inputConnections.Where((conn) => conn.OutputNodeID == i).ToList();
                double value = 0;
                foreach (var connection in connections)
                {
                    value += connection.value * connection.weight;
                }

                double Result = Nodes[i].ActivationFunction(value);
                ToReturnList.Add(Result);
            }
            var ToReturn = Nodes.Zip(ToReturnList, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            return ToReturn;
        }
        public static List<int> FindAllIndexesOf<T>(this List<T> Self, Predicate<T> match)
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
    class Network
    {
        List<Node> InputNodes = new List<Node>();
        List<Layer> HiddenLayers = new List<Layer>();
        List<Node> OutputNodes = new List<Node>();
        public Network(int InputNodes,int HiddenLayers,int OutputNodes)
        {

        }
        void Run()
        {

        }

    }

    class Layer
    {

        public Layer(int InputNodeCount, int OutputNodeCount)
        {

        }
        public List<Connection> ActivateLayerFunctions(List<Connection> inputConnections)
        {
            var Values = Nodes.ActivateFunctions(inputConnections).Select(x => x.Value).ToList();
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
                List<Connection> connections = inputConnections.Where((conn) => conn.OutputNodeID == i).ToList();
                double value = 0;
                foreach(var connection in connections)
                {
                    value += connection.value * connection.weight; 
                }
                
                double Result = Nodes[i].ActivationFunction(value);
            List<int> ConnIndexes = outputConnections.FindAllIndexesOf((conn) => conn.InputNodeID == i);
            foreach (var Index in ConnIndexes)
            {
                outputConnections[Index].value = Result;
            }
            }*/
        #endregion
        List<Node> Nodes;
        List<Connection> outputConnections;
    }
    class Connection
    {
        public int InputNodeID;
        public double value;
        public double weight;
        public int OutputNodeID;
    }
    class Node
    {
        public int NodeID; //aka if it's the first one it's 1 if it's the second one it's 2 etc
        public double importantnumberthingie = 1;
        public double bias = 0;
        public double ActivationFunction(double input)
        {
            double bInput = input - bias;
            double bwInput = importantnumberthingie * bInput;
            double bwEnput = Math.Exp(bwInput);
            return bwEnput / ++bwEnput;
        }
    }
}
