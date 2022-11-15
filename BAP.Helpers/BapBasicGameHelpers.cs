using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAP.Types;

namespace BAP.Helpers
{
    public class BapBasicGameHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodes">List of Nodes</param>
        /// <param name="currentNode">Try and avoid the current node</param>
        /// <param name="maxretries">Max retries is how hard you want to avoid using the same nodeId. 0 means avoid it for sure.</param>
        /// <returns></returns>
        /// 
        public static int GetRandomInt(int min, int max, int intToAvoid = -1, int maxRetries = 0)
        {
            int newInt = 0;
            bool intFound = false;
            int maxAttempts = 0;
            if (intToAvoid == -1 || maxRetries == 0)
            {
                return Random.Shared.Next(min, max);
            }
            while (intFound == false && maxAttempts < maxRetries)
            {
                newInt = Random.Shared.Next(min, max);
                if (newInt != intToAvoid)
                {
                    intFound = true;
                }
                else
                {
                    maxAttempts++;
                }
            }
            return newInt;

        }

        public static ushort GetRandomUshort(int min, int max)
        {
            if (max > 255)
            {
                max = 255;
            }
            return (ushort)Random.Shared.Next(min, max);
        }

        public static string GetRandomNodeId(List<string> nodes, string currentNode, int maxretries = 0)
        {
            string nextNode = "";
            bool nodeFound = false;
            int maxAttempts = maxretries == 0 ? 5 : maxretries;
            while (nodeFound == false && maxAttempts != 0)
            {
                nextNode = GetRandomNodeId(nodes);
                if (nextNode != currentNode)
                {
                    nodeFound = true;
                }
                else
                {
                    maxAttempts--;
                }
            }
            if (maxretries == 0 && nodeFound == false)
            {
                bool goUp = Random.Shared.Next(2) == 1;
                if (goUp)
                {
                    int currentNodeIndex = nodes.IndexOf(currentNode);
                    int nextNodeIndex = currentNodeIndex == nodes.Count ? 0 : currentNodeIndex + 1;
                    nextNode = nodes[nextNodeIndex];
                }
                else
                {
                    int currentNodeIndex = nodes.IndexOf(currentNode);
                    int nextNodeIndex = currentNodeIndex == 0 ? nodes.Count - 1 : currentNodeIndex - 1;
                    nextNode = nodes[nextNodeIndex];
                }
            }

            return nextNode;
        }

        public static string GetRandomNodeId(List<string> nodes)
        {
            try
            {
                int nodesCount = nodes?.Count ?? 0;
                if (nodesCount == 0)
                {
                    return "";
                }
                if (nodesCount == 1)
                {
                    return nodes?[0] ?? "";
                }
                int nextNodeNumber = Random.Shared.Next(0, nodesCount);
                return nodes?[nextNodeNumber] ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered an error getting the next nodeId");
                Console.WriteLine(ex.Message);
                return nodes.FirstOrDefault() ?? "";
            }

        }

        public static Dictionary<int, Patterns> GetNumberDictionary()
        {
            Dictionary<int, Patterns> dictionary = new()
            {
                { 0, Patterns.Number0 },
                { 1, Patterns.Number1 },
                { 2, Patterns.Number2 },
                { 3, Patterns.Number3 },
                { 4, Patterns.Number4 },
                { 5, Patterns.Number5 },
                { 6, Patterns.Number6 },
                { 7, Patterns.Number7 },
                { 8, Patterns.Number8 },
                { 9, Patterns.Number9 }
            };
            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propability">Pool of possibilities. The percentage is 1/your number. So 2 is 50/50 10 is 1/10th of the time</param>
        /// <returns></returns>
        public static bool ShouldWePerformARandomAction(int propability)
        {
            //I don't know why I picked 2 but picking 0 every time just seemed strange. Logically I don't think it matters;
            return Random.Shared.Next(0, propability) == (propability > 2 ? 2 : 0);
        }
    }
}
