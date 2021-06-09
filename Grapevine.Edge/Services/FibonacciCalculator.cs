using System.Collections.Generic;

namespace Grapevine.Edge.Services
{
    public class FibonacciCalculator : IFibonacciCalculator
    {
        private Dictionary<int, int> cache = new();

        public int Calculate(int x) {
            if (cache.ContainsKey(x)) {
                return cache[x];
            }

            cache[x] = x < 2 ? x : Calculate(x: x - 1) + Calculate(x: x - 2);
            return cache[x];
        } 
    }
}